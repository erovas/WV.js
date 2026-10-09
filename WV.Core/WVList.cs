using WV.Interfaces;

namespace WV.Core
{
    public abstract class WVList<T> : IWVList<T>
    {
        protected readonly List<T> _items;

        internal WVList(List<T> items)
        {
            _items = items;
        }

        public T this[int index]
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

        public T? First => _items.Count > 0 ? _items[0] : default;

        public T? Last => _items.Count > 0 ? _items[_items.Count - 1] : default;

        public T? Get(int index)
        {
            return index >= 0 && index < _items.Count ? _items[index] : default;
        }
    }
}