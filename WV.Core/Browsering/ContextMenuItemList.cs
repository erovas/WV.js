using WV.Interfaces;

namespace WV.Core.Browsering
{
    public sealed class ContextMenuItemList : WVList<IContextMenuItem>, IContextMenuItemList
    {
        internal ContextMenuItemList(List<IContextMenuItem> items) : base(items)
        {
        }
    }
}