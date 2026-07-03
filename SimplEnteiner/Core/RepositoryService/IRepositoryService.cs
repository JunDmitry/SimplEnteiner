using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SimplEnteiner.Core.RepositoryService
{
    public interface IRepositoryService :
        ICollection<KeyValuePair<Type, object>>,
        IEnumerable<KeyValuePair<Type, object>>,
        IEnumerable, IDictionary<Type, object>,
        IReadOnlyCollection<KeyValuePair<Type, object>>,
        IReadOnlyDictionary<Type, object>,
        ICollection,
        IDictionary,
        IDeserializationCallback,
        ISerializable,
        IDisposable
    {
        void AddIfDisposable(object instance, Action<object> onRelease = null);
    }
}
