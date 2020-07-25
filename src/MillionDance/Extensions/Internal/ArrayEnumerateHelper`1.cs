using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Extensions.Internal {
    public sealed class ArrayEnumerateHelper<T> : IEnumerable<(int Index, T Item)> {

        internal ArrayEnumerateHelper([NotNull, ItemCanBeNull] T[] array) {
            _array = array;
        }

        public Enumerator GetEnumerator() {
            return new Enumerator(_array);
        }

        IEnumerator<(int Index, T Item)> IEnumerable<(int Index, T Item)>.GetEnumerator() {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        [NotNull, ItemCanBeNull]
        private readonly T[] _array;

        public struct Enumerator : IEnumerator<(int Index, T Item)> {

            internal Enumerator([NotNull, ItemCanBeNull] T[] array) {
                _index = -1;
                _array = array;
            }

            public void Dispose() {
            }

            public bool MoveNext() {
                if (_index < _array.Length - 1) {
                    _index += 1;
                    return true;
                } else {
                    return false;
                }
            }

            public void Reset() {
                _index = -1;
            }

            public (int Index, T Item) Current {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get {
                    if (_index < 0 || _index >= _array.Length) {
                        throw new IndexOutOfRangeException();
                    } else {
                        return (_index, _array[_index]);
                    }
                }
            }

            object IEnumerator.Current => Current;

            [NotNull, ItemCanBeNull]
            private readonly T[] _array;

            private int _index;

        }

    }
}
