namespace WV.Interfaces
{
    public interface IContextMenuItemList
    {
        IContextMenuItem this[int index] { get; }

        int Length { get; }

        int Count { get; }

        bool IsEmpty { get; }

        bool Any { get; }

        IContextMenuItem? First { get; }

        IContextMenuItem? Last { get; }

        IContextMenuItem? Get(int index);
    }
}