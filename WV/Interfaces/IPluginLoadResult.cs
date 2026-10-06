namespace WV.Interfaces
{
    public interface IPluginLoadResult
    {
        /// <summary>
        /// Ruta completa del archivo del plugin.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Nombre del plugin, si se pudo leer. Null si falló antes de identificarlo.
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// True si el plugin se cargó correctamente.
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// Mensaje de error si Success == false. Null si Success == true.
        /// </summary>
        string? Error { get; }
    }
}