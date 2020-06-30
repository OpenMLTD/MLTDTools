using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Utilities {
    internal class TimedList<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : IComparable<TKey> {

        public TimedList() {
            _list = new List<KeyValuePair<TKey, TValue>>();
        }

        public void AddOrUpdate([NotNull] TKey key, [CanBeNull] TValue value) {
            var pair = new KeyValuePair<TKey, TValue>(key, value);

            var itemIndex = _list.BinarySearch(pair, KeyComparer.Instance.Value);

            if (itemIndex >= 0) {
                _list[itemIndex] = pair;
            } else {
                itemIndex = ~itemIndex;

                if (itemIndex == _list.Count) {
                    _list.Add(pair);
                } else {
                    _list.Insert(itemIndex, pair);
                }
            }
        }

        public bool TryGetCurrentValue([NotNull] TKey key, [CanBeNull] out TValue value) {
            if (_list.Count == 0) {
                value = default;
                return false;
            }

            var kv = new KeyValuePair<TKey, TValue>(key, default);
            var index = _list.BinarySearch(kv, KeyComparer.Instance.Value);

            if (index >= 0) {
                value = _list[index].Value;
                return true;
            } else {
                index = ~index;

                if (index == 0) {
                    value = default;
                    return false;
                } else {
                    value = _list[index - 1].Value;
                    return true;
                }
            }
        }

        public int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _list.Count;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        [NotNull]
        private readonly List<KeyValuePair<TKey, TValue>> _list;

        private sealed class KeyComparer : IComparer<KeyValuePair<TKey, TValue>> {

            public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y) {
                return x.Key.CompareTo(y.Key);
            }

            [NotNull]
            private static KeyComparer Create() {
                return new KeyComparer();
            }

            [NotNull, ItemNotNull]
            public static readonly Lazy<KeyComparer> Instance = new Lazy<KeyComparer>(Create);

        }

    }
}
