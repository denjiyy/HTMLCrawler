using System;
using System.Collections;

namespace CrawlerHTML.CustomDataStructures;

public class CustomList<T> : IEnumerable<T>
{
    private const int InitialCapacity = 2;

    private T[] items;

    public CustomList()
    {
        items = new T[InitialCapacity];
    }

    public CustomList(IEnumerable<T> collection)
    {
        items = new T[InitialCapacity];
        Count = 0;

        foreach (T item in collection)
        {
            Add(item);
        }
    }

    public int Count { get; private set; }

    public T this[int index]
    {
        get
        {
            ThrowExceptionIfIndexOutOfRange(index);

            return items[index];
        }
        set
        {
            ThrowExceptionIfIndexOutOfRange(index);

            items[index] = value;
        }
    }

    public void Add(T item)
    {
        if (items.Length == Count)
        {
            Resize();
        }

        items[Count] = item;

        Count++;
    }

    public void AddRange(T[] items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    public T RemoveAt(int index)
    {
        ThrowExceptionIfIndexOutOfRange(index);

        T removedItem = items[index];

        //items[index] = default; // only for debugging

        ShiftLeft(index);

        Count--;

        if (Count <= items.Length / 4)
        {
            Shrink();
        }

        return removedItem;
    }

    public void InsertAt(int index, T item)
    {
        ThrowExceptionIfIndexOutOfRange(index);

        if (items.Length == Count)
        {
            Resize();
        }

        ShiftRight(index);

        items[index] = item;

        Count++;
    }

    public bool Contains(T item)
    {
        for (int i = 0; i < Count; i++)
        {
            if (items[i]!.Equals(item))
            {
                return true;
            }
        }

        return false;
    }

    public void Swap(int firstIndex, int secondIndex)
    {
        ThrowExceptionIfIndexOutOfRange(firstIndex);
        ThrowExceptionIfIndexOutOfRange(secondIndex);

        (items[secondIndex], items[firstIndex]) = (items[firstIndex], items[secondIndex]);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return items[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void Resize()
    {
        T[] copy = new T[items.Length * 2];

        for (int i = 0; i < Count; i++)
        {
            copy[i] = items[i];
        }

        items = copy;
    }

    private void Shrink()
    {
        T[] copy = new T[items.Length / 2];

        for (int i = 0; i < Count; i++)
        {
            copy[i] = items[i];
        }

        items = copy;
    }

    private void ShiftLeft(int index)
    {
        for (int i = index; i < Count - 1; i++)
        {
            items[i] = items[i + 1];
        }
    }

    private void ShiftRight(int index)
    {
        for (int i = Count - 1; i >= index; i--)
        {
            items[i + 1] = items[i];
        }
    }

    public T[] ToArray()
    {
        T[] arr = new T[Count];

        for (int i = 0; i < Count; i++)
        {
            arr[i] = items[i];
        }

        return arr;
    }

    private void ThrowExceptionIfIndexOutOfRange(int index)
    {
        if (index < 0 || index >= Count)
        {
            throw new IndexOutOfRangeException();
        }
    }
}