namespace WV.Interfaces
{
    public interface IPluginMetadata
    {
        string Path { get; }

        string Name { get; }

        string Type { get; }

        string Version { get; }

        bool Singleton { get; }

        string? Author { get; }

        string? Description { get; }
    }
}