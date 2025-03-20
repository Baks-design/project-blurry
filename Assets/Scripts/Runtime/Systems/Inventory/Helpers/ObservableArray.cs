using System;
using System.Collections.Generic;

namespace Assets.Scripts.Runtime.Systems.Inventory.Helpers
{
    [Serializable]
    public class ObservableArray<T> : IObservableArray<T>
    {
        public T[] items;

        public int Count
        {
            get
            {
                var count = 0;
                for (var i = 0; i < items.Length; i++)
                    if (items[i] != null)
                        count++;
                return count;
            }
        }

        public int Length => items.Length;

        public T this[int index] => items[index];

        public event Action<T[]> AnyValueChanged = delegate { };

        public ObservableArray(int size = 20, IList<T> initialList = null)
        {
            items = new T[size];
            if (initialList != null)
            {
                // Copy initialList to items without LINQ
                var copyLength = Math.Min(initialList.Count, size);
                for (var i = 0; i < copyLength; i++)
                    items[i] = initialList[i];

                Invoke();
            }
        }

        void Invoke() => AnyValueChanged.Invoke(items);

        public void Swap(int index1, int index2)
        {
            (items[index1], items[index2]) = (items[index2], items[index1]);
            Invoke();
        }

        public void Clear()
        {
            items = new T[items.Length];
            Invoke();
        }

        public bool TryAdd(T item)
        {
            for (var i = 0; i < items.Length; i++)
                if (TryAddAt(i, item)) return true;
            return false;
        }

        public bool TryAddAt(int index, T item)
        {
            if (index < 0 || index >= items.Length) return false;

            if (items[index] != null) return false;

            items[index] = item;
            Invoke();
            return true;
        }

        public bool TryRemove(T item)
        {
            for (var i = 0; i < items.Length; i++)
                if (EqualityComparer<T>.Default.Equals(items[i], item) && TryRemoveAt(i)) return true;
            return false;
        }

        public bool TryRemoveAt(int index)
        {
            if (index < 0 || index >= items.Length) return false;

            if (items[index] == null) return false;

            items[index] = default;
            Invoke();
            return true;
        }
    }
}