using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SimplEnteiner.Core.Lifecycle;

namespace SimplEnteiner.Core.RepositoryService
{
    internal class RepositoryService : IRepositoryService
    {
        private readonly Dictionary<Type, object> _values;
        private readonly ICleanupService _cleanupService;

        public RepositoryService(ICleanupService cleanupService)
        {
            _cleanupService = cleanupService;
        }

        public object this[Type key]
        {
            get => ((IDictionary<Type, object>)_values)[key];
            set
            {
                ((IDictionary<Type, object>)_values)[key] = value;
            }
        }
        public object this[object key]
        {
            get => ((IDictionary)_values)[key];
            set => ((IDictionary)_values)[key] = value;
        }

        public int Count => ((ICollection<KeyValuePair<Type, object>>)_values).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<Type, object>>)_values).IsReadOnly;

        public ICollection<Type> Keys => ((IDictionary<Type, object>)_values).Keys;

        public ICollection<object> Values => ((IDictionary<Type, object>)_values).Values;

        public bool IsSynchronized => ((ICollection)_values).IsSynchronized;

        public object SyncRoot => ((ICollection)_values).SyncRoot;

        public bool IsFixedSize => ((IDictionary)_values).IsFixedSize;

        IEnumerable<Type> IReadOnlyDictionary<Type, object>.Keys => ((IReadOnlyDictionary<Type, object>)_values).Keys;

        ICollection IDictionary.Keys => ((IDictionary)_values).Keys;

        IEnumerable<object> IReadOnlyDictionary<Type, object>.Values => ((IReadOnlyDictionary<Type, object>)_values).Values;

        ICollection IDictionary.Values => ((IDictionary)_values).Values;

        public void Add(KeyValuePair<Type, object> item)
        {
            ((ICollection<KeyValuePair<Type, object>>)_values).Add(item);
        }

        public void Add(Type key, object value)
        {
            ((IDictionary<Type, object>)_values).Add(key, value);
        }

        public void Add(object key, object value)
        {
            ((IDictionary)_values).Add(key, value);
        }

        public void Clear()
        {
            ((ICollection<KeyValuePair<Type, object>>)_values).Clear();
        }

        public bool Contains(KeyValuePair<Type, object> item)
        {
            return ((ICollection<KeyValuePair<Type, object>>)_values).Contains(item);
        }

        public bool Contains(object key)
        {
            return ((IDictionary)_values).Contains(key);
        }

        public bool ContainsKey(Type key)
        {
            return ((IDictionary<Type, object>)_values).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<Type, object>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<Type, object>>)_values).CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_values).CopyTo(array, index);
        }

        public IEnumerator<KeyValuePair<Type, object>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<Type, object>>)_values).GetEnumerator();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ((ISerializable)_values).GetObjectData(info, context);
        }

        public void OnDeserialization(object sender)
        {
            ((IDeserializationCallback)_values).OnDeserialization(sender);
        }

        public bool Remove(KeyValuePair<Type, object> item)
        {
            return ((ICollection<KeyValuePair<Type, object>>)_values).Remove(item);
        }

        public bool Remove(Type key)
        {
            return ((IDictionary<Type, object>)_values).Remove(key);
        }

        public void Remove(object key)
        {
            ((IDictionary)_values).Remove(key);
        }

        public bool TryGetValue(Type key, out object value)
        {
            return ((IDictionary<Type, object>)_values).TryGetValue(key, out value);
        }

        public void AddIfDisposable(object instance, Action<object> onRelease = null)
        {
            _cleanupService.AddIfDisposable(instance, onRelease);
        }

        public void Dispose()
        {
            _cleanupService.Dispose();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_values).GetEnumerator();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)_values).GetEnumerator();
        }
    }
}
