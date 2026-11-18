/*
   Datos de prueba para MedicalAgenda.
   Carga 25 filas en total (5 por tabla) y se puede volver a ejecutar sin duplicar
   los registros identificados por sus nombres/códigos/fecha y hora.
   PacienteID se conserva como referencia externa: cambia 1..5 por IDs que existan
   en la base de datos del microservicio de Pacientes cuando vayas a probar REST.
*/

USE [MedicalAgenda];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    INSERT INTO dbo.Especialidades (Nombre, Descripcion)
    SELECT source.Nombre, source.Descripcion
    FROM (VALUES
        (N'Cardiología', N'Prevención, diagnóstico y tratamiento de enfermedades cardiovasculares.'),
        (N'Pediatría', N'Atención integral de niñas, niños y adolescentes.'),
        (N'Medicina General', N'Consulta primaria, control preventivo y seguimiento de salud.'),
        (N'Dermatología', N'Atención de enfermedades de la piel, cabello y uñas.'),
        (N'Ginecología y Obstetricia', N'Salud ginecológica, control prenatal y atención materna.')
    ) AS source(Nombre, Descripcion)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.Especialidades existing WHERE existing.Nombre = source.Nombre
    );

    INSERT INTO dbo.Consultorios (Nombre, Ubicacion, EdificioNivel, Capacidad)
    SELECT source.Nombre, source.Ubicacion, source.EdificioNivel, source.Capacidad
    FROM (VALUES
        (N'Consultorio 101', N'Colonia Escalón, San Salvador', N'Torre A, nivel 1', 2),
        (N'Consultorio 102', N'Colonia Escalón, San Salvador', N'Torre A, nivel 1', 2),
        (N'Consultorio 201', N'Colonia San Benito, San Salvador', N'Torre B, nivel 2', 3),
        (N'Consultorio 202', N'Colonia San Benito, San Salvador', N'Torre B, nivel 2', 3),
        (N'Sala de evaluación 1', N'Santa Tecla, La Libertad', N'Edificio clínico, nivel 1', 4)
    ) AS source(Nombre, Ubicacion, EdificioNivel, Capacidad)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.Consultorios existing WHERE existing.Nombre = source.Nombre
    );

    INSERT INTO dbo.Medicos
        (Nombre, Apellido, CodigoMedico, EspecialidadID, Telefono, CorreoElectronico, NumeroDocumento, TipoDocumento)
    SELECT source.Nombre, source.Apellido, source.CodigoMedico, specialty.EspecialidadID,
           source.Telefono, source.CorreoElectronico, NULL, NULL
    FROM (VALUES
        (N'Ana María', N'Pineda', 'MED-001', N'Cardiología', '50322001001', 'ana.pineda@clinica-demo.sv'),
        (N'Carlos Ernesto', N'Meléndez', 'MED-002', N'Pediatría', '50322001002', 'carlos.melendez@clinica-demo.sv'),
        (N'Gabriela Isabel', N'Rivas', 'MED-003', N'Medicina General', '50322001003', 'gabriela.rivas@clinica-demo.sv'),
        (N'José Ricardo', N'Alvarado', 'MED-004', N'Dermatología', '50322001004', 'jose.alvarado@clinica-demo.sv'),
        (N'María Fernanda', N'López', 'MED-005', N'Ginecología y Obstetricia', '50322001005', 'maria.lopez@clinica-demo.sv')
    ) AS source(Nombre, Apellido, CodigoMedico, EspecialidadNombre, Telefono, CorreoElectronico)
    INNER JOIN dbo.Especialidades AS specialty ON specialty.Nombre = source.EspecialidadNombre
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.Medicos existing WHERE existing.CodigoMedico = source.CodigoMedico
    );

    DECLARE @Hoy date = CONVERT(date, GETDATE());

    INSERT INTO dbo.HorariosMedicos
        (MedicoID, ConsultorioID, FechaDisponible, HoraInicio, HoraFin, DuracionCitaMin)
    SELECT doctor.MedicoID, room.ConsultorioID, DATEADD(day, source.Dias, @Hoy),
           source.HoraInicio, source.HoraFin, 30
    FROM (VALUES
        ('MED-001', N'Consultorio 101', -2, CAST('08:00' AS time), CAST('12:00' AS time)),
        ('MED-002', N'Consultorio 102', -1, CAST('09:00' AS time), CAST('13:00' AS time)),
        ('MED-003', N'Consultorio 201',  0, CAST('08:30' AS time), CAST('12:30' AS time)),
        ('MED-004', N'Consultorio 202',  1, CAST('10:00' AS time), CAST('14:00' AS time)),
        ('MED-005', N'Sala de evaluación 1', 2, CAST('08:00' AS time), CAST('12:00' AS time))
    ) AS source(CodigoMedico, ConsultorioNombre, Dias, HoraInicio, HoraFin)
    INNER JOIN dbo.Medicos AS doctor ON doctor.CodigoMedico = source.CodigoMedico
    INNER JOIN dbo.Consultorios AS room ON room.Nombre = source.ConsultorioNombre
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.HorariosMedicos existing
        WHERE existing.MedicoID = doctor.MedicoID
          AND existing.ConsultorioID = room.ConsultorioID
          AND existing.FechaDisponible = DATEADD(day, source.Dias, @Hoy)
          AND existing.HoraInicio = source.HoraInicio
    );

    INSERT INTO dbo.Citas
        (PacienteID, MedicoID, ConsultorioID, FechaHora, MotivoConsulta, Estado, Observaciones, UsuarioReserva)
    SELECT source.PacienteID, doctor.MedicoID, room.ConsultorioID,
           DATEADD(minute, source.MinutosDesdeMedianoche,
               CONVERT(datetime2, DATEADD(day, source.Dias, @Hoy))),
           source.MotivoConsulta, source.Estado, source.Observaciones, source.UsuarioReserva
    FROM (VALUES
        (CAST(1 AS bigint), 'MED-001', N'Consultorio 101', -2, 540, N'Control de presión arterial', N'Atendida', N'Control realizado; continuar seguimiento.', N'recepcion1'),
        (CAST(2 AS bigint), 'MED-002', N'Consultorio 102', -1, 600, N'Consulta por fiebre y tos', N'noAsistio', N'Paciente no se presentó.', N'recepcion1'),
        (CAST(3 AS bigint), 'MED-003', N'Consultorio 201',  0, 570, N'Chequeo general anual', N'Confirmada', N'Primera consulta de control.', N'recepcion2'),
        (CAST(4 AS bigint), 'MED-004', N'Consultorio 202',  1, 630, N'Revisión de irritación en la piel', N'Programada', N'Presentar resultados de laboratorio si los tiene.', N'recepcion2'),
        (CAST(5 AS bigint), 'MED-005', N'Sala de evaluación 1', 2, 660, N'Consulta de control ginecológico', N'Cancelada', N'Cancelada por solicitud de la paciente.', N'recepcion3')
    ) AS source(PacienteID, CodigoMedico, ConsultorioNombre, Dias, MinutosDesdeMedianoche,
                MotivoConsulta, Estado, Observaciones, UsuarioReserva)
    INNER JOIN dbo.Medicos AS doctor ON doctor.CodigoMedico = source.CodigoMedico
    INNER JOIN dbo.Consultorios AS room ON room.Nombre = source.ConsultorioNombre
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.Citas existing
        WHERE existing.PacienteID = source.PacienteID
          AND existing.MedicoID = doctor.MedicoID
          AND existing.FechaHora = DATEADD(minute, source.MinutosDesdeMedianoche,
              CONVERT(datetime2, DATEADD(day, source.Dias, @Hoy)))
    );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;

-- Resumen rápido: 5 especialidades, 5 consultorios, 5 médicos,
-- 5 horarios y 5 citas = 25 registros en total.
SELECT N'Especialidades' AS Tabla, COUNT_BIG(*) AS Registros FROM dbo.Especialidades WHERE Nombre IN
    (N'Cardiología', N'Pediatría', N'Medicina General', N'Dermatología', N'Ginecología y Obstetricia')
UNION ALL
SELECT N'Consultorios', COUNT_BIG(*) FROM dbo.Consultorios WHERE Nombre IN
    (N'Consultorio 101', N'Consultorio 102', N'Consultorio 201', N'Consultorio 202', N'Sala de evaluación 1')
UNION ALL
SELECT N'Medicos', COUNT_BIG(*) FROM dbo.Medicos WHERE CodigoMedico IN ('MED-001', 'MED-002', 'MED-003', 'MED-004', 'MED-005')
UNION ALL
SELECT N'HorariosMedicos', COUNT_BIG(*) FROM dbo.HorariosMedicos h INNER JOIN dbo.Medicos m ON m.MedicoID = h.MedicoID
WHERE m.CodigoMedico IN ('MED-001', 'MED-002', 'MED-003', 'MED-004', 'MED-005')
UNION ALL
SELECT N'Citas', COUNT_BIG(*) FROM dbo.Citas WHERE UsuarioReserva IN ('recepcion1', 'recepcion2', 'recepcion3');
