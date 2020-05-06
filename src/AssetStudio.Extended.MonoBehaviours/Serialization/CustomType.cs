using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization {
    internal sealed class CustomType : IReadOnlyDictionary<string, object> {

        public CustomType([NotNull] string typeName, [NotNull] IReadOnlyDictionary<string, object> variables) {
            TypeName = typeName;
            Variables = variables;
        }

        [NotNull]
        public string TypeName { get; }

        [NotNull]
        public IReadOnlyDictionary<string, object> Variables {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() {
            return Variables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)Variables).GetEnumerator();
        }

        int IReadOnlyCollection<KeyValuePair<string, object>>.Count => Variables.Count;

        bool IReadOnlyDictionary<string, object>.ContainsKey([NotNull] string key) {
            return Variables.ContainsKey(key);
        }

        bool IReadOnlyDictionary<string, object>.TryGetValue(string key, out object value) {
            return Variables.TryGetValue(key, out value);
        }

        object IReadOnlyDictionary<string, object>.this[string key] => Variables[key];

        IEnumerable<string> IReadOnlyDictionary<string, object>.Keys => Variables.Keys;

        IEnumerable<object> IReadOnlyDictionary<string, object>.Values => Variables.Values;

    }
}
