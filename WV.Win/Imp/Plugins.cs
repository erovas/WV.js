using WV.Configs;
using WV.Core.Pluging;
using WV.Interfaces;

namespace WV.Win.Imp
{
    public sealed class Plugins : PluginsCore
    {
        public Plugins(IPluginContext context, PluginsConfig pluginsConfig) : base(context, pluginsConfig)
        {
        }

        internal void ToDefault()
        {
            ClearListeners();
            ClearEvents();

            Dispose(true);
        }
    }
}