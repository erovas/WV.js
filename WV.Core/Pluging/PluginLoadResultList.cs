using WV.Interfaces;

namespace WV.Core.Pluging
{
    public sealed class PluginLoadResultList : WVList<IPluginLoadResult>, IPluginLoadResultList
    {

        internal PluginLoadResultList(List<IPluginLoadResult> items) : base(items)
        {       
        }

        public int SuccessCount => _items.Count(r => r.Success);
        
        public int FailureCount => _items.Count(r => !r.Success);

        public bool AnySuccess => _items.Any(r => r.Success);
        
        public bool AnyFailure => _items.Any(r => !r.Success);
        
        public bool AllSuccess => _items.Count > 0 && _items.All(r => r.Success);
        
        public bool AllFailed => _items.Count > 0 && _items.All(r => !r.Success);

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