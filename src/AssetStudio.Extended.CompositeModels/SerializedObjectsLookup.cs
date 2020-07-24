using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class SerializedObjectsLookup {

        private SerializedObjectsLookup() {
            _lookup = new Dictionary<long, Object>();
        }

        public bool Contains<T>([NotNull] PPtr<T> ptr)
            where T : Object {
            var b = Contains(ptr.m_PathID);

            if (!b) {
                return false;
            }

            var obj = Get(ptr.m_PathID);

            return obj is T;
        }

        public bool Contains(long pathId) {
            return _lookup.ContainsKey(pathId);
        }

        public bool TryGet(long pathId, [CanBeNull] out Object result) {
            if (!Contains(pathId)) {
                result = null;
                return false;
            } else {
                result = _lookup[pathId];
                return true;
            }
        }

        public bool TryGet<T>([NotNull] PPtr<T> ptr, [CanBeNull] out T result)
            where T : Object {
            return TryGet<T, T>(ptr, out result);
        }

        public bool TryGet<TQuery, TResult>([NotNull] PPtr<TQuery> ptr, [CanBeNull] out TResult result)
            where TQuery : Object
            where TResult : TQuery {
            var b = TryGet(ptr.m_PathID, out var obj);

            if (!b) {
                result = null;
                return false;
            }

            if (!(obj is TResult t)) {
                result = null;
                return false;
            }

            result = t;
            return true;
        }

        [NotNull]
        public Object Get(long pathId) {
            return _lookup[pathId];
        }

        [NotNull]
        public T Get<T>(long pathId)
            where T : Object {
            return (T)Get(pathId);
        }

        [NotNull]
        public T Get<T>([NotNull] PPtr<T> ptr)
            where T : Object {
            return Get<T, T>(ptr);
        }

        [NotNull]
        public TResult Get<TQuery, TResult>([NotNull] PPtr<TQuery> ptr)
            where TQuery : Object
            where TResult : TQuery {
            var q = Get<TQuery>(ptr.m_PathID);

            if (q is TResult r) {
                return r;
            }

            var tq = typeof(TQuery);
            var tr = typeof(TResult);

            throw new InvalidCastException($"Found specified object with query type but it is not of result type. (query={tq.FullName}, desired result={tr.FullName})");
        }

        [CanBeNull]
        public Object Find([NotNull] Predicate<Object> predicate) {
            foreach (var value in _lookup.Values) {
                if (predicate(value)) {
                    return value;
                }
            }

            return null;
        }

        [CanBeNull]
        public T Find<T>([NotNull] Predicate<T> predicate)
            where T : Object {
            return Find<T, T>(predicate);
        }

        [CanBeNull]
        public TResult Find<TQuery, TResult>([NotNull] Predicate<TQuery> predicate)
            where TQuery : Object
            where TResult : TQuery {
            foreach (var value in _lookup.Values) {
                if (value is TResult t && predicate(t)) {
                    return t;
                }
            }

            return null;
        }

        [NotNull]
        public static SerializedObjectsLookup Create([NotNull] AssetsManager manager) {
            var result = new SerializedObjectsLookup();
            var dict = result._lookup;

            foreach (var serializedFile in manager.assetsFileList) {
                foreach (var o in serializedFile.Objects) {
                    dict.Add(o.m_PathID, o);
                }
            }

            return result;
        }

        [NotNull]
        private readonly Dictionary<long, Object> _lookup;

    }
}
