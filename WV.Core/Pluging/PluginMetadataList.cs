using WV.Interfaces;

namespace WV.Core.Pluging
{
    public class PluginMetadataList : IPluginMetadataList
    {
        private readonly List<IPluginMetadata> _items;

        internal PluginMetadataList(List<IPluginMetadata> items)
        {
            _items = items;
        }

        public IPluginMetadata this[int index]
        {
            get
            {
                if (index < 0 || index >= _items.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _items[index];
            }
        }

        public int Length => _items.Count;

        public int Count => _items.Count;

        public bool IsEmpty => _items.Count == 0;

        public int SingletonCount => _items.Count(r => r.Singleton);

        public IPluginMetadata? First => _items.Count > 0 ? _items[0] : null;

        public IPluginMetadata? Last => _items.Count > 0 ? _items[_items.Count - 1] : null;

        public IPluginMetadata? Get(int index)
        {
            return index >= 0 && index < _items.Count ? _items[index] : null;
        }
    }
}