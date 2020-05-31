using System;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using LruCacheNet;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Naming {
    public sealed class CamelCaseNamingConvention : INamingConvention {

        static CamelCaseNamingConvention() {
            LocalNameCache = new ThreadLocal<LruCache<string, string>>(CreateNameCache);
            LocalStringBuilder = new ThreadLocal<StringBuilder>(CreateStringBuilder);
        }

        private CamelCaseNamingConvention() {
        }

        public string GetCorrected(string input) {
            if (string.IsNullOrEmpty(input)) {
                return string.Empty;
            }

            if (char.IsWhiteSpace(input, 0)) {
                throw new ArgumentException("The string cannot start with whitespaces.", nameof(input));
            }

            if (!char.IsUpper(input, 0)) {
                return input;
            }

            var nameCache = LocalNameCache.Value;

            if (nameCache.ContainsKey(input)) {
                return nameCache[input];
            }

            var sb = LocalStringBuilder.Value;

            sb.Clear();
            sb.Append(char.ToLowerInvariant(input[0]));

            if (input.Length > 1) {
                sb.Append(input, 1, input.Length - 1);
            }

            var result = sb.ToString();

            nameCache.AddOrUpdate(input, result);

            return result;
        }

        [NotNull]
        private static LruCache<string, string> CreateNameCache() {
            return new LruCache<string, string>(CacheSize);
        }

        [NotNull]
        private static StringBuilder CreateStringBuilder() {
            return new StringBuilder();
        }

        // Warning: static variable that cannot be collected
        [NotNull]
        private static readonly ThreadLocal<LruCache<string, string>> LocalNameCache;

        // Warning: static variable that cannot be collected
        [NotNull]
        private static readonly ThreadLocal<StringBuilder> LocalStringBuilder;

        // 200 items should be enough for (scrobj + imo.unity3d)
        private const int CacheSize = 200;

    }
}
