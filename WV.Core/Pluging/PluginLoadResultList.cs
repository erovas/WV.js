using System.Runtime.CompilerServices;
using WV.Interfaces;

namespace WV.Core.Pluging
{
    public sealed class PluginLoadResultList : IPluginLoadResultList
    {
        private readonly List<IPluginLoadResult> _items;

        internal PluginLoadResultList(List<IPluginLoadResult> items)
        {
            _items = items;
        }

        [IndexerName("Items")]
        public IPluginLoadResult this[int index]
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

        public bool Any => _items.Count > 0;

        public int SuccessCount => _items.Count(r => r.Success);
        
        public int FailureCount => _items.Count(r => !r.Success);

        public bool AnySuccess => _items.Any(r => r.Success);
        
        public bool AnyFailure => _items.Any(r => !r.Success);
        
        public bool AllSuccess => _items.Count > 0 && _items.All(r => r.Success);
        
        public bool AllFailed => _items.Count > 0 && _items.All(r => !r.Success);

        public IPluginLoadResult? First => _items.Count > 0 ? _items[0] : null;

        public IPluginLoadResult? Last => _items.Count > 0 ? _items[_items.Count - 1] : null;

        public IPluginLoadResult? Get(int index)
        {
            return index >= 0 && index < _items.Count ? _items[index] : null;
        }

        public IPluginLoadResultList Failures()
        {
            return new PluginLoadResultList(_items.Where(r => !r.Success).ToList());
        }

        public IPluginLoadResultList Successes()
        {
            return new PluginLoadResultList(_items.Where(r => r.Success).ToList());
        }
    }
}