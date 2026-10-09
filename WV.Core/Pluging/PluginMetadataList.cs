using WV.Interfaces;

namespace WV.Core.Pluging
{
    public class PluginMetadataList : WVList<IPluginMetadata>, IPluginMetadataList
    {
        internal PluginMetadataList(List<IPluginMetadata> items) : base(items)
        {
        }

        public int SingletonCount => _items.Count(r => r.Singleton);
    }
}