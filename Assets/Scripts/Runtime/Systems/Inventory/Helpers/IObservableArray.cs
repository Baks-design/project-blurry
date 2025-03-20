using System;

namespace Assets.Scripts.Runtime.Systems.Inventory.Helpers
{
    public interface IObservableArray<T>
    {
        int Count { get; }
        T this[int index] { get; }

        event Action<T[]> AnyValueChanged;

        void Swap(int index1, int index2);
        void Clear();
        bool TryAdd(T item);
        bool TryAddAt(int index, T item);
        bool TryRemove(T item);
        bool TryRemoveAt(int index);
    }
}