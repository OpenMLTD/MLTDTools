using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers.Common {
    internal sealed class ConstructorManager {

        public ConstructorManager() {
            _constructors = new ConditionalWeakTable<Type, ConstructorInfo>();
        }

        [NotNull]
        public object CreateInstance([NotNull] Type type, bool nonPublic) {
            ConditionalWeakTable<Type, ConstructorInfo>.CreateValueCallback creator;

            if (nonPublic) {
                creator = GetDefaultConstructorWithNonPublic;
            } else {
                creator = GetDefaultConstructorPublicOnly;
            }

            var ctor = _constructors.GetValue(type, creator);

            if (ctor == null) {
                throw new ArgumentNullException(nameof(ctor), "Cannot find default constructor.");
            }

            return ctor.Invoke(Array.Empty<object>());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CanBeNull]
        private static ConstructorInfo GetDefaultConstructorPublicOnly([NotNull] Type type) {
            return GetDefaultConstructor(type, false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CanBeNull]
        private static ConstructorInfo GetDefaultConstructorWithNonPublic([NotNull] Type type) {
            return GetDefaultConstructor(type, true);
        }

        [CanBeNull]
        private static ConstructorInfo GetDefaultConstructor([NotNull] Type type, bool nonPublic) {
            BindingFlags flags;

            if (nonPublic) {
                flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            } else {
                flags = BindingFlags.Public | BindingFlags.Instance;
            }

            var ctor = type.GetConstructor(flags, null, Type.EmptyTypes, null);

            return ctor;
        }

        [NotNull]
        private readonly ConditionalWeakTable<Type, ConstructorInfo> _constructors;

    }
}
