using System;
using System.Collections.Generic;
using System.Text;

namespace WV.Interfaces
{
    public interface IWVList<T>
    {
        T this[int index] { get; }

        int Length { get; }

        int Count { get; }

        bool IsEmpty { get; }

        bool Any { get; }

        T? First { get; }

        T? Last { get; }

        T? Get(int index);
    }
}