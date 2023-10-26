using System;

namespace CrawlerHTML.CustomDataStructures;

public class CustomQueue<T>
{
    private const int InitialCapacity = 4;
    private const int FirstElementIndex = 0;
    private T[] items;

    public CustomQueue()
    {
        items = new T[InitialCapacity];
    }
    public int Count { get; private set; }

    public void Enqueue(T item)
    {
        if (items.Length == Count)
        {
            Resize();
        }

        items[Count] = item;

        Count++;
    }

    public T Dequeue()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("The queue is empty");
        }

        T removedItem = items[FirstElementIndex];

        ShiftLeft();

        // if needed Shrink

        Count--;

        return removedItem;
    }

    public T Peek()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("CustomQueue is empty");
        }

        return items[FirstElementIndex];
    }

    public void Clear()
    {
        items = new T[InitialCapacity];

        Count = 0;
    }

    public void ForEach(Action<T> action)
    {
        for (int i = 0; i < Count; i++)
        {
            T currentItem = items[i];

            action(currentItem);
        }
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

    private void ShiftLeft()
    {
        for (int i = FirstElementIndex; i < Count; i++)
        {
            items[i] = items[i + 1];
        }
    }
}