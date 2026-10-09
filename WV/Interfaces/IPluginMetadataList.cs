namespace WV.Interfaces
{
    public interface IPluginMetadataList
    {
        IPluginMetadata this[int index] { get; }

        int Length { get; }

        int Count { get; }

        bool IsEmpty { get; }

        bool Any { get; }

        IPluginMetadata? First { get; }

        IPluginMetadata? Last { get; }

        IPluginMetadata? Get(int index);

        int SingletonCount { get; }
    }
}