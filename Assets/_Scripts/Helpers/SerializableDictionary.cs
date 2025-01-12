using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue>
{
    [Serializable]
    public struct KeyValuePair
    {
        public TKey Key;
        public TValue Value;

        public KeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    [SerializeField]
    private List<KeyValuePair> keyValuePairs = new();

    public Dictionary<TKey, TValue> ToDictionary()
    {
        Dictionary<TKey, TValue> dictionary = new();

        foreach (var pair in keyValuePairs)
        {
            if (!dictionary.ContainsKey(pair.Key)) dictionary[pair.Key] = pair.Value;
        }

        return dictionary;
    }

    public void FromDictionary(Dictionary<TKey, TValue> dictionary)
    {
        keyValuePairs.Clear();

        foreach (var kvp in dictionary)
        {
            keyValuePairs.Add(new KeyValuePair(kvp.Key, kvp.Value));
        }
    }
}
