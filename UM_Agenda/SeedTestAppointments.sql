/*
  Datos de agenda de prueba: un próximo turno mensual por médico y una cita futura
  por paciente activo. Idempotente: solo agrega fechas y citas que todavía no existan.
  Usa pacientes ya registrados en MedicalPaciente y los siete médicos de MedicalAgenda.
*/
USE [MedicalAgenda];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET DATEFIRST 1;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @FechaInicio date = DATEADD(day, 1, CONVERT(date, DATEADD(hour, -6, SYSUTCDATETIME())));
    DECLARE @FechaFin date = DATEADD(day, 30, @FechaInicio);

    IF (SELECT COUNT_BIG(*) FROM MedicalPaciente.dbo.Patients WHERE Activo = 1) = 0
        THROW 51000, 'No hay pacientes activos para crear las citas de prueba.', 1;
    IF (SELECT COUNT_BIG(*) FROM dbo.Medicos WHERE Activo = 1) < 7
        THROW 51001, 'Se requieren los siete médicos activos de la demostración.', 1;
    IF (SELECT COUNT_BIG(*) FROM dbo.Consultorios WHERE Activo = 1) < 4
        THROW 51002, 'Se requieren cinco consultorios activos para distribuir los turnos.', 1;

    CREATE TABLE #WorkDays (DayNumber int NOT NULL PRIMARY KEY, WorkDate date NOT NULL UNIQUE);
    ;WITH DateSeries AS
    (
        SELECT @FechaInicio AS WorkDate
        UNION ALL
        SELECT DATEADD(day, 1, WorkDate) FROM DateSeries WHERE WorkDate < @FechaFin
    )
    INSERT INTO #WorkDays (DayNumber, WorkDate)
    SELECT ROW_NUMBER() OVER (ORDER BY WorkDate) - 1, WorkDate
    FROM DateSeries
    WHERE DATEPART(weekday, WorkDate) BETWEEN 1 AND 5
    OPTION (MAXRECURSION 40);

    CREATE TABLE #DoctorPlan
    (
        MedicoId bigint NOT NULL PRIMARY KEY,
        ConsultorioId bigint NOT NULL,
        HoraInicio time NOT NULL,
        HoraFin time NOT NULL,
        MinutosInicio int NOT NULL,
        CitasPorDia int NOT NULL
    );

    ;WITH Doctors AS
    (
        SELECT MedicoID, ROW_NUMBER() OVER (ORDER BY MedicoID) AS Position
        FROM dbo.Medicos WHERE Activo = 1
    ), Rooms AS
    (
        SELECT ConsultorioID, ROW_NUMBER() OVER (ORDER BY ConsultorioID) AS Position
        FROM dbo.Consultorios WHERE Activo = 1
    )
    INSERT INTO #DoctorPlan (MedicoId, ConsultorioId, HoraInicio, HoraFin, MinutosInicio, CitasPorDia)
    SELECT d.MedicoID, r.ConsultorioID,
           CASE WHEN d.Position <= 4 THEN CAST('08:00' AS time) ELSE CAST('16:00' AS time) END,
           CASE WHEN d.Position <= 4 THEN CAST('16:00' AS time) ELSE CAST('20:00' AS time) END,
           CASE WHEN d.Position <= 4 THEN 480 ELSE 960 END,
           CASE WHEN d.Position <= 4 THEN 16 ELSE 8 END
    FROM Doctors d
    INNER JOIN Rooms r ON r.Position = CASE WHEN d.Position <= 4 THEN d.Position ELSE d.Position - 4 END;

    INSERT INTO dbo.HorariosMedicos (MedicoID, ConsultorioID, FechaDisponible, HoraInicio, HoraFin, DuracionCitaMin)
    SELECT dp.MedicoId, dp.ConsultorioId, days.WorkDate, dp.HoraInicio, dp.HoraFin, 30
    FROM #DoctorPlan dp
    CROSS JOIN #WorkDays days
    WHERE NOT EXISTS
    (
        SELECT 1 FROM dbo.HorariosMedicos existing
        WHERE existing.MedicoID = dp.MedicoId
          AND existing.FechaDisponible = days.WorkDate
          AND existing.HoraInicio = dp.HoraInicio
          AND existing.HoraFin = dp.HoraFin
    );

    DELETE FROM dbo.Citas
    WHERE FechaHora >= CAST(@FechaInicio AS datetime2)
      AND UsuarioReserva = N'recepcion.prueba1'
      AND Observaciones = N'Cita de prueba creada para verificar la agenda del profesional.';

    ;WITH Patients AS
    (
        SELECT PacienteID, ROW_NUMBER() OVER (ORDER BY PacienteID) - 1 AS SequenceNumber
        FROM MedicalPaciente.dbo.Patients WHERE Activo = 1
    ), Doctors AS
    (
        SELECT MedicoId, ConsultorioId, MinutosInicio, CitasPorDia, ROW_NUMBER() OVER (ORDER BY MedicoId) AS Position FROM #DoctorPlan
    ), Assigned AS
    (
        SELECT p.PacienteID, d.MedicoId, d.ConsultorioId, d.MinutosInicio, d.CitasPorDia
        FROM Patients p INNER JOIN Doctors d ON d.Position = (p.SequenceNumber % 7) + 1
    ), Numbered AS
    (
        SELECT *, ROW_NUMBER() OVER (PARTITION BY MedicoId ORDER BY PacienteID) - 1 AS DoctorSequence
        FROM Assigned
    ), Assignments AS
    (
        SELECT p.PacienteID, p.MedicoId, p.ConsultorioId,
               DATEADD(minute, p.MinutosInicio + (p.DoctorSequence % p.CitasPorDia) * 30,
                   CAST(days.WorkDate AS datetime2)) AS FechaHora
        FROM Numbered p
        INNER JOIN #WorkDays days ON days.DayNumber = p.DoctorSequence / p.CitasPorDia
    )
    INSERT INTO dbo.Citas
        (PacienteID, MedicoID, ConsultorioID, FechaHora, MotivoConsulta, Estado, Observaciones, UsuarioReserva)
    SELECT a.PacienteID, a.MedicoId, a.ConsultorioId, a.FechaHora,
           N'Control médico programado', N'Programada',
           N'Cita de prueba creada para verificar la agenda del profesional.', N'recepcion.prueba1'
    FROM Assignments a
    WHERE NOT EXISTS
    (
        SELECT 1 FROM dbo.Citas existing
        WHERE existing.PacienteID = a.PacienteID AND existing.MedicoID = a.MedicoId AND existing.FechaHora = a.FechaHora
    );

    COMMIT TRANSACTION;

    SELECT N'Horarios de prueba futuros' AS Registro, COUNT_BIG(*) AS Total
    FROM dbo.HorariosMedicos h INNER JOIN #DoctorPlan p ON p.MedicoId = h.MedicoID
    WHERE h.FechaDisponible BETWEEN @FechaInicio AND @FechaFin
    UNION ALL
    SELECT N'Citas futuras asignadas', COUNT_BIG(*)
    FROM dbo.Citas c INNER JOIN #DoctorPlan p ON p.MedicoId = c.MedicoID
    WHERE c.FechaHora >= CAST(@FechaInicio AS datetime2)
      AND c.Observaciones = N'Cita de prueba creada para verificar la agenda del profesional.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
