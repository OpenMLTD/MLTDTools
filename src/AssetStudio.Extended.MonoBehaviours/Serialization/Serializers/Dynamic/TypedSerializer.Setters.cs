using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers.Dynamic {
    partial class TypedSerializer {

        [NotNull]
        private MemberSetter FindSetterByName([NotNull] string name, [CanBeNull] INamingConvention naming) {
            var properties = _properties;

            Debug.Assert(properties != null, nameof(properties) + " != null");

            var fields = _fields;

            Debug.Assert(fields != null, nameof(fields) + " != null");

            MemberSetter result;

            foreach (var prop in properties) {
                ScriptableObjectPropertyAttribute sopa;

                if (_propertyAttributeCache.ContainsKey(prop)) {
                    sopa = _propertyAttributeCache[prop];
                } else {
                    sopa = prop.GetCustomAttribute<ScriptableObjectPropertyAttribute>();
                    _propertyAttributeCache[prop] = sopa;
                }

                var propName = !string.IsNullOrEmpty(sopa?.Name) ? sopa.Name : (naming != null ? naming.GetCorrected(prop.Name) : prop.Name);

                if (propName == name) {
                    if (_createdSetters.ContainsKey(propName)) {
                        result = _createdSetters[propName];
                    } else {
                        result = new MemberSetter(prop, sopa);
                        _createdSetters[propName] = result;
                    }

                    return result;
                }
            }

            foreach (var field in fields) {
                ScriptableObjectPropertyAttribute sopa;

                if (_fieldAttributeCache.ContainsKey(field)) {
                    sopa = _fieldAttributeCache[field];
                } else {
                    sopa = field.GetCustomAttribute<ScriptableObjectPropertyAttribute>();
                    _fieldAttributeCache[field] = sopa;
                }

                var fieldName = !string.IsNullOrEmpty(sopa?.Name) ? sopa.Name : (naming != null ? naming.GetCorrected(field.Name) : field.Name);

                if (fieldName == name) {
                    if (_createdSetters.ContainsKey(fieldName)) {
                        result = _createdSetters[fieldName];
                    } else {
                        result = new MemberSetter(field, sopa);
                        _createdSetters[fieldName] = result;
                    }

                    return result;
                }
            }

            return MemberSetter.Null;
        }

        private void SetValue([NotNull] MemberSetter setter, [CanBeNull] object obj, [CanBeNull] object value, [NotNull] Type serializedValueType) {
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

                    setter.SetValueDirect(obj, enumValue);

                    return;
                }
            }

            var converterType = setter.Attribute?.ConverterType;
            var convertedValue = Context.Converters.TryConvertTypeOfValue(serializedValueType, acceptedType, value, converterType);

            setter.SetValueDirect(obj, convertedValue);
        }

        [NotNull]
        private readonly Dictionary<string, MemberSetter> _createdSetters;

    }
}
