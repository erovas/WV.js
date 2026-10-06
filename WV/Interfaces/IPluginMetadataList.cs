namespace WV.Interfaces
{
    public interface IPluginMetadataList
    {
        IPluginMetadata this[int index] { get; }

        int Length { get; }

        int Count { get; }

        bool IsEmpty { get; }

        int SingletonCount { get; }

        IPluginMetadata? First { get; }

        IPluginMetadata? Last { get; }

        IPluginMetadata? Get(int index);
    }
}