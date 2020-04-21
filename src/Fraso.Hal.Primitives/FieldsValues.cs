using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fraso.Hal.Primitives
{
    public sealed class FieldsValues
        : IDictionary<string, object>
    {
        private IDictionary<string, object> InnerDictionary { get; }

        #region Ctors
        public FieldsValues()
        {
            InnerDictionary = 
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public FieldsValues(IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            InnerDictionary = 
                new Dictionary<string, object>(
                    keyValuePairs.Count(), 
                    StringComparer.OrdinalIgnoreCase);

            foreach (var kvp in keyValuePairs)
                InnerDictionary.Add(kvp);
        }
        #endregion // Ctors

        public object this[string key]
        {
            get => InnerDictionary[key];
            set => InnerDictionary[key] = value;
        }

        public ICollection<string> Keys 
            => InnerDictionary.Keys;

        public ICollection<object> Values 
            => InnerDictionary.Values;

        public int Count 
            => InnerDictionary.Count;

        public bool IsReadOnly 
            => false;

        public void Add(string key, object value) 
            => InnerDictionary.Add(key, value);

        public void Add(KeyValuePair<string, object> item) 
            => InnerDictionary.Add(item.Key, item.Value);

        public void Clear() => InnerDictionary.Clear();

        public bool Contains(KeyValuePair<string, object> item) 
            => InnerDictionary.Contains(item);

        public bool ContainsKey(string key) 
            => InnerDictionary.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) 
            => InnerDictionary.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() 
            => InnerDictionary.GetEnumerator();

        public bool Remove(string key) 
            => InnerDictionary.Remove(key);

        public bool Remove(KeyValuePair<string, object> item) 
            => InnerDictionary.Remove(item);

        public bool TryGetValue(string key, out object value) 
            => InnerDictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() 
            => InnerDictionary.GetEnumerator();
    }
}
