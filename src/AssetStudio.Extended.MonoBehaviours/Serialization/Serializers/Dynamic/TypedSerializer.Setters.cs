using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers.Dynamic {
    partial class TypedSerializer {

        [NotNull]
        private MemberSetter FindSetterByName([NotNull] string serializedName, [CanBeNull] INamingConvention naming) {
            if (_createdSetters.ContainsKey(serializedName)) {
                return _createdSetters[serializedName];
            }

            var properties = _properties;

            Debug.Assert(properties != null, nameof(properties) + " != null");

            var fields = _fields;

            Debug.Assert(fields != null, nameof(fields) + " != null");

            MemberSetter result = null;
            ScriptableObjectPropertyAttribute sopa;

            foreach (var prop in properties) {
                if (_propertyAttributeCache.ContainsKey(prop)) {
                    sopa = _propertyAttributeCache[prop];
                } else {
                    sopa = prop.GetCustomAttribute<ScriptableObjectPropertyAttribute>();
                    _propertyAttributeCache[prop] = sopa;
                }

                var propName = GetCorrectedMemberName(sopa, naming, prop.Name);

                if (string.Equals(propName, serializedName, StringComparison.Ordinal)) {
                    result = new MemberSetter(prop, sopa);
                    break;
                }
            }

            if (result == null) {
                foreach (var field in fields) {
                    if (_fieldAttributeCache.ContainsKey(field)) {
                        sopa = _fieldAttributeCache[field];
                    } else {
                        sopa = field.GetCustomAttribute<ScriptableObjectPropertyAttribute>();
                        _fieldAttributeCache[field] = sopa;
                    }

                    var fieldName = GetCorrectedMemberName(sopa, naming, field.Name);

                    if (string.Equals(fieldName, serializedName, StringComparison.Ordinal)) {
                        result = new MemberSetter(field, sopa);
                        break;
                    }
                }
            }

            if (result == null) {
                result = MemberSetter.Null;
            }

            _createdSetters[serializedName] = result;

            return result;
        }

        [CanBeNull]
        private object SetValue([NotNull] MemberSetter setter, [CanBeNull] object obj, [CanBeNull] object value, [NotNull] Type serializedValueType) {
            var acceptedType = setter.GetValueType();

            if (ReferenceEquals(value, null)) {
                if (acceptedType.IsValueType) {
                    throw new InvalidCastException("Cannot cast 'null' to a value type.");
                }
            }

            // Treat enum as inline, special case of integers
            if (acceptedType.IsEnum) {
                if (ReferenceEquals(value, null)) {
                    throw new ApplicationException("Not possible.");
                }

                if (SerializingHelper.IsNumericType(serializedValueType)) {
                    var enumValue = Enum.ToObject(acceptedType, value);

                    return setter.SetValueDirect(obj, enumValue);
                }
            }

            var converterType = setter.Attribute?.ConverterType;
            var convertedValue = Context.Converters.TryConvertTypeOfValue(serializedValueType, acceptedType, value, converterType);

            return setter.SetValueDirect(obj, convertedValue);
        }

        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetCorrectedMemberName([CanBeNull] ScriptableObjectPropertyAttribute sopa, [CanBeNull] INamingConvention naming, [NotNull] string fallback) {
            return !string.IsNullOrEmpty(sopa?.Name) ? sopa.Name : (naming != null ? naming.GetCorrected(fallback) : fallback);
        }

        [NotNull]
        private readonly Dictionary<string, MemberSetter> _createdSetters;

    }
}
