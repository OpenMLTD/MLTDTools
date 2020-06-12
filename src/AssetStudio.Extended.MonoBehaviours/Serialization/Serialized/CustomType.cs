using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serialized {
    public sealed class CustomType : IReadOnlyDictionary<string, object> {

        internal CustomType([NotNull] string typeName, [NotNull] Dictionary<string, object> variables) {
            TypeName = typeName;
            MutableVariables = variables;
        }

        [NotNull]
        public string TypeName { get; }

        [NotNull]
        public IReadOnlyDictionary<string, object> Variables {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MutableVariables;
        }

        // Should keep immutable
        [NotNull]
        internal Dictionary<string, object> MutableVariables {
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() {
            return MutableVariables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)MutableVariables).GetEnumerator();
        }

        int IReadOnlyCollection<KeyValuePair<string, object>>.Count => MutableVariables.Count;

        bool IReadOnlyDictionary<string, object>.ContainsKey([NotNull] string key) {
            return MutableVariables.ContainsKey(key);
        }

        bool IReadOnlyDictionary<string, object>.TryGetValue(string key, out object value) {
            return MutableVariables.TryGetValue(key, out value);
        }

        object IReadOnlyDictionary<string, object>.this[string key] => MutableVariables[key];

        IEnumerable<string> IReadOnlyDictionary<string, object>.Keys => MutableVariables.Keys;

        IEnumerable<object> IReadOnlyDictionary<string, object>.Values => MutableVariables.Values;

    }
}
