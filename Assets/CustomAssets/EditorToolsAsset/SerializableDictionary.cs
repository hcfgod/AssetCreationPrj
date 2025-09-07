using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomAssets.EditorTools
{
    /// <summary>
    /// Base class for serializable dictionaries in Unity.
    /// Provides a way to serialize key-value pairs that can be edited in the Inspector.
    /// </summary>
    /// <typeparam name="TKey">The type of the dictionary keys.</typeparam>
    /// <typeparam name="TValue">The type of the dictionary values.</typeparam>
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();
        
        [SerializeField]
        private List<TValue> values = new List<TValue>();

        /// <summary>
        /// Initializes a new instance of the SerializableDictionary class.
        /// </summary>
        public SerializableDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SerializableDictionary class with the specified capacity.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the dictionary can contain.</param>
        public SerializableDictionary(int capacity) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SerializableDictionary class with elements from the specified dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary whose elements are copied to the new dictionary.</param>
        public SerializableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
        }

        /// <summary>
        /// Called before Unity serializes the object.
        /// Converts the dictionary to lists for serialization.
        /// </summary>
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (var kvp in this)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        /// <summary>
        /// Called after Unity deserializes the object.
        /// Converts the lists back to a dictionary.
        /// </summary>
        public void OnAfterDeserialize()
        {
            Clear();

            if (keys.Count != values.Count)
            {
                Debug.LogError($"SerializableDictionary: Key count ({keys.Count}) does not match value count ({values.Count}). " +
                             "This usually indicates a serialization error.");
                return;
            }

            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i] != null && !ContainsKey(keys[i]))
                {
                    Add(keys[i], values[i]);
                }
            }
        }

        /// <summary>
        /// Adds a key-value pair to the dictionary.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        public new void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                Debug.LogWarning($"SerializableDictionary: Key '{key}' already exists. Use indexer to update value.");
                return;
            }
            
            base.Add(key, value);
        }

        /// <summary>
        /// Removes the value with the specified key from the dictionary.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>True if the element is successfully removed; otherwise, false.</returns>
        public new bool Remove(TKey key)
        {
            return base.Remove(key);
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value associated with the specified key.</returns>
        public new TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out TValue value))
                {
                    return value;
                }
                return default(TValue);
            }
            set
            {
                base[key] = value;
            }
        }

        /// <summary>
        /// Determines whether the dictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns>True if the dictionary contains an element with the specified key; otherwise, false.</returns>
        public new bool ContainsKey(TKey key)
        {
            return base.ContainsKey(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, 
        /// if the key is found; otherwise, the default value for the type of the value parameter.</param>
        /// <returns>True if the dictionary contains an element with the specified key; otherwise, false.</returns>
        public new bool TryGetValue(TKey key, out TValue value)
        {
            return base.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets the number of key-value pairs contained in the dictionary.
        /// </summary>
        public new int Count => base.Count;

        /// <summary>
        /// Gets a collection containing the keys in the dictionary.
        /// </summary>
        public new Dictionary<TKey, TValue>.KeyCollection Keys => base.Keys;

        /// <summary>
        /// Gets a collection containing the values in the dictionary.
        /// </summary>
        public new Dictionary<TKey, TValue>.ValueCollection Values => base.Values;

        /// <summary>
        /// Removes all keys and values from the dictionary.
        /// </summary>
        public new void Clear()
        {
            base.Clear();
        }

        /// <summary>
        /// Returns a string representation of the dictionary.
        /// </summary>
        /// <returns>A string representation of the dictionary.</returns>
        public override string ToString()
        {
            return $"SerializableDictionary<{typeof(TKey).Name}, {typeof(TValue).Name}>: {Count} entries";
        }
    }
}
