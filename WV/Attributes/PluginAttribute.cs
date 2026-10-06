namespace WV.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class PluginAttribute : Attribute
    {
        /// <summary>
        /// Nombre lógico del plugin.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Tipo concreto que hereda de Plugin.
        /// </summary>
        public required string Type { get; set; }

        /// <summary>
        /// Indica si el plugin va a ser de instancia Singleton.
        /// </summary>
        public bool Singleton { get; set; }

        /// <summary>
        /// Versión. 
        /// </summary>
        public required string Version { get; set; }

        /// <summary>
        /// Autor o empresa.
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// Descripción corta, apta para UI.
        /// </summary>
        public string? Description { get; set; }

    }
}