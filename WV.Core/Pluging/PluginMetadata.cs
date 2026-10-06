using WV.Interfaces;

namespace WV.Core.Pluging
{
    public sealed class PluginMetadata : IPluginMetadata
    {
        public string Path { get; internal set; }

        public string Name { get; internal set; }

        public string Type { get; internal set; }

        public string Version { get; internal set; }

        public bool Singleton { get; internal set; }

        public string? Author { get; internal set; }

        public string? Description { get; internal set; }

        internal PluginMetadata()
        {
            Path = string.Empty; 
            Name = string.Empty; 
            Type = string.Empty;
            Version = string.Empty;
        }

        internal PluginMetadata(string path, string name, string entryType, string version, bool singleton = false, string? author = null, string? description = null)
        {
            Path = path;
            Name = name;
            Type = entryType;
            Version = version;
            Singleton = singleton;
            Author = author;
            Description = description;
        }
    }
}