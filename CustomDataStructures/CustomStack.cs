using System;

namespace CrawlerHTML.CustomDataStructures;

public class CustomStack<T>
{
    private const int InitialCapacity = 4;
    private T[] items;

    public CustomStack()
    {
        items = new T[InitialCapacity];
    }

    public int Count { get; private set; }

    public void Push(T item)
    {
        if (items.Length == Count)
        {
            Resize();
        }

        items[Count] = item;

        Count++;
    }

    public T Pop()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("CustomStack is empty");
        }

        T removedItem = items[Count - 1];

        items[Count - 1] = default!;

        Count--;

        //TODO Shrink if needed

        return removedItem;
    }

    public T Peek()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("CustomStack is empty");
        }

        return items[Count - 1];
    }

    public void ForEach(Action<T> action)
    {
        for (int i = 0; i < Count; i++)
        {
            T currentItem = items[i];

            action(currentItem);
        }

        //Reversed if preferred
        //for (int i = Count - 1; i >= 0; i--)
        //{
        //    int currentItem = items[i];

        //    action(currentItem);
        //}
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
}