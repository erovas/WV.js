namespace WV.Interfaces
{
    public interface IPluginMetadataList : IWVList<IPluginMetadata>
    {
        int SingletonCount { get; }
    }
}