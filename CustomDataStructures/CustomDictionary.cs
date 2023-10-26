using System;
using CrawlerHTML.CustomDataStructures;
using System.Collections.Generic;

public class CustomDictionary<TKey, TValue>
{
    private CustomList<KeyValuePair<TKey, TValue>> items = new();

    public void Add(TKey key, TValue value)
    {
        if (!ContainsKey(key))
        {
            items.Add(new KeyValuePair<TKey, TValue>(key, value));
        }
        else
        {
            throw new ArgumentException("An item with the same key already exists.");
        }
    }

    public bool ContainsKey(TKey key)
    {
        foreach (var item in items)
        {
            if (EqualityComparer<TKey>.Default.Equals(item.Key, key))
            {
                return true;
            }
        }
        return false;
    }

    public bool Remove(TKey key)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (EqualityComparer<TKey>.Default.Equals(items[i].Key, key))
            {
                items.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        foreach (var item in items)
        {
            if (EqualityComparer<TKey>.Default.Equals(item.Key, key))
            {
                value = item.Value;
                return true;
            }
        }
        value = default(TValue);
        return false;
    }

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }
            throw new KeyNotFoundException($"Key not found: {key}");
        }
        set
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (EqualityComparer<TKey>.Default.Equals(items[i].Key, key))
                {
                    items[i] = new KeyValuePair<TKey, TValue>(key, value);
                    return;
                }
            }
            Add(key, value);
        }
    }

    public int Count
    {
        get { return items.Count; }
    }

    public IEnumerable<TKey> Keys
    {
        get
        {
            foreach (var item in items)
            {
                yield return item.Key;
            }
        }
    }

    public IEnumerable<TValue> Values
    {
        get
        {
            foreach (var item in items)
            {
                yield return item.Value;
            }
        }
    }
}