using System;
using System.Collections.Generic;

namespace CrawlerHTML;
class CustomHashSet<T>
{
    private CustomHashMap<T, bool> customHashMap;

    public CustomHashSet()
    {
        customHashMap = new CustomHashMap<T, bool>();
    }

    public void Add(T value)
    {
        customHashMap.Put(value, true);
    }

    public bool Contains(T obj)
    {
        return customHashMap.Contains(obj);
    }

    public void Display()
    {
        customHashMap.DisplaySet();
    }

    public bool Remove(T obj)
    {
        return customHashMap.Remove(obj);
    }
}

class CustomHashMap<TKey, TValue>
{
    private Entry<TKey, TValue>[] table;
    private int size;
    private int capacity = 16;
    private float loadFactor = 0.75f;

    public CustomHashMap()
    {
        table = new Entry<TKey, TValue>[capacity];
    }

    public void Put(TKey newKey, TValue data)
    {
        if (newKey == null)
            return;

        // Check load factor and resize if necessary
        if ((float)size / capacity > loadFactor)
        {
            Resize();
        }

        int hash = Hash(newKey);
        Entry<TKey, TValue> newEntry = new(newKey, data);

        if (table[hash] == null)
        {
            table[hash] = newEntry;
            size++;
        }
        else
        {
            Entry<TKey, TValue> current = table[hash];
            Entry<TKey, TValue> previous = null;

            while (current != null)
            {
                if (current.key.Equals(newKey))
                {
                    current.value = data;
                    return;
                }
                previous = current;
                current = current.Next;
            }

            previous.Next = newEntry;
            size++;
        }
    }

    public TValue Get(TKey key)
    {
        int hash = Hash(key);
        Entry<TKey, TValue> current = table[hash];

        while (current != null)
        {
            if (current.key.Equals(key))
                return current.value;
            current = current.Next;
        }

        return default;
    }

    public bool Remove(TKey deleteKey)
    {
        int hash = Hash(deleteKey);
        Entry<TKey, TValue> current = table[hash];
        Entry<TKey, TValue> previous = null;

        while (current != null)
        {
            if (current.key.Equals(deleteKey))
            {
                if (previous == null)
                {
                    table[hash] = current.Next;
                }
                else
                {
                    previous.Next = current.Next;
                }
                size--;
                return true;
            }
            previous = current;
            current = current.Next;
        }
        return false;
    }

    public bool Contains(TKey key)
    {
        int hash = Hash(key);
        Entry<TKey, TValue> current = table[hash];

        while (current != null)
        {
            if (current.key.Equals(key))
                return true;
            current = current.Next;
        }

        return false;
    }

    public void DisplaySet()
    {
        for (int i = 0; i < capacity; i++)
        {
            Entry<TKey, TValue> entry = table[i];
            while (entry != null)
            {
                Console.Write(entry.key + " ");
                entry = entry.Next;
            }
        }
    }

    private void Resize()
    {
        capacity *= 2;
        Entry<TKey, TValue>[] newTable = new Entry<TKey, TValue>[capacity];

        foreach (var entry in table)
        {
            Entry<TKey, TValue> current = entry;

            while (current != null)
            {
                int hash = Hash(current.key);
                Entry<TKey, TValue> newEntry = new(current.key, current.value);

                if (newTable[hash] == null)
                {
                    newTable[hash] = newEntry;
                }
                else
                {
                    Entry<TKey, TValue> temp = newTable[hash];
                    while (temp.Next != null)
                    {
                        temp = temp.Next;
                    }
                    temp.Next = newEntry;
                }

                current = current.Next;
            }
        }

        table = newTable;
    }

    private int Hash(TKey key)
    {
        return (key.GetHashCode() & 0x7FFFFFFF) % capacity;
    }

    class Entry<K, V>
    {
        public K key;
        public V value;
        public Entry<K, V> Next;

        public Entry(K key, V value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
