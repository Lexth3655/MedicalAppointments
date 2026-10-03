namespace NugetPackage_Rest.Configs;

/// <summary>Configuración enlazada desde la sección RestSettings.</summary>
public class RequestSettings
{
    /// <summary>Registra método, URL y contenido de cada request. Mantener desactivado en producción si hay datos sensibles.</summary>
    public bool EnableRequestLogs { get; set; }
}
