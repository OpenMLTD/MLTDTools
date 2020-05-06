using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using AssetStudio.Extended.MonoBehaviours.Extensions;
using AssetStudio.Extended.MonoBehaviours.Serialization.DefaultConverters;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;
using JetBrains.Annotations;
using PropOrField = AssetStudio.Extended.MonoBehaviours.Utilities.MemberSetter;

namespace AssetStudio.Extended.MonoBehaviours.Serialization {
    public sealed class ScriptableObjectSerializer {

        public ScriptableObjectSerializer() {
            _createdTypeConverters = new Dictionary<Type, ISimpleTypeConverter>(10);
            _createdSetters = new Dictionary<(Type, string, ScriptableObjectPropertyAttribute), PropOrField>();

            // In old versions(?) Unity serializes booleans as bytes
            WithConverter<ByteToBooleanConverter>();
        }

        public ScriptableObjectSerializer WithConverter<T>()
            where T : ISimpleTypeConverter {
            return WithConverter(typeof(T));
        }

        public ScriptableObjectSerializer WithConverter([NotNull] Type converterType) {
            if (!converterType.ImplementsInterface(typeof(ISimpleTypeConverter))) {
                throw new InvalidCastException("The converter does not implement " + nameof(ISimpleTypeConverter) + ".");
            }

            var converter = Activator.CreateInstance(converterType, true);

            return WithConverter((ISimpleTypeConverter)converter);
        }

        public ScriptableObjectSerializer WithConverter([NotNull] ISimpleTypeConverter converter) {
            var converterType = converter.GetType();
            _createdTypeConverters[converterType] = converter;
            return this;
        }

        [CanBeNull]
        public T Deserialize<T>([NotNull] MonoBehaviour monoBehavior) where T : new() {
            var ret = Deserialize(monoBehavior, typeof(T));
            return (T)ret;
        }

        [CanBeNull]
        public object Deserialize([NotNull] MonoBehaviour monoBehavior, [NotNull] Type type) {
            if (monoBehavior == null) {
                throw new ArgumentNullException(nameof(monoBehavior));
            }

            var typeNodes = monoBehavior.serializedType.m_Nodes;

            if (typeNodes == null) {
                return CreateDefaultOf(type);
            } else {
                var structure = StructureReader.ReadMembers(typeNodes, monoBehavior.reader);

                Debug.Assert(structure.Count == 1);

                var structureValues = structure.Values.ToArray();
                var rootObject = structureValues[0];

                Debug.Assert(rootObject != null);
                Debug.Assert(rootObject is CustomType);

                var obj = DeserializeObject(type, (CustomType)rootObject, 0);

                return obj;
            }
        }

        [NotNull]
        private object DeserializeObject([NotNull] Type containerType, [NotNull] CustomType structure, int level) {
            var options = containerType.GetCustomAttribute<ScriptableObjectAttribute>() ?? DefaultClassOptions;

            PropertyInfo[] properties;
            FieldInfo[] fields;

            var allProperties = containerType.GetProperties(InternalBindings);
            var allFields = containerType.GetFields(InternalBindings).Where(field => {
                var compilerGenerated = field.GetCustomAttribute<CompilerGeneratedAttribute>();
                // Filter out compiler generated fields.
                return compilerGenerated == null;
            });

            switch (options.PopulationStrategy) {
                case PopulationStrategy.OptIn:
                    properties = allProperties.Where(prop => prop.GetCustomAttribute<ScriptableObjectPropertyAttribute>() != null).ToArray();
                    fields = allFields.Where(field => field.GetCustomAttribute<ScriptableObjectPropertyAttribute>() != null).ToArray();
                    break;
                case PopulationStrategy.OptOut:
                    properties = allProperties.Where(prop => prop.GetCustomAttribute<ScriptableObjectIgnoreAttribute>() == null).ToArray();
                    fields = allFields.Where(field => field.GetCustomAttribute<ScriptableObjectIgnoreAttribute>() == null).ToArray();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(options.PopulationStrategy), options.PopulationStrategy, null);
            }

            var naming = options.NamingConventionType != null ? (INamingConvention)Activator.CreateInstance(options.NamingConventionType, true) : null;

            var obj = Activator.CreateInstance(containerType, true);

            ApplyObjectMembers(obj, containerType, structure, options, properties, fields, naming, level);

            return obj;
        }

        [CanBeNull]
        private object DeserializeValue([CanBeNull] object value, [CanBeNull] Type typeHint, int level) {
            object rawValue;

            if (value is CustomType ct) {
                Debug.Assert(typeHint != null);
                rawValue = DeserializeObject(typeHint, ct, level + 1);
            } else if (value is IDictionary<object, object> dict) {
                Debug.Assert(typeHint != null);
                rawValue = DeserializeDictionary(dict, typeHint, level);
            } else if (value is object[] arr) {
                Debug.Assert(typeHint != null);

                if (typeHint.IsArray) {
                    rawValue = DeserializeArray(arr, typeHint, level);
                } else {
                    rawValue = DeserializeCollection(arr, typeHint, level);
                }
            } else if (value is RawData data) {
                rawValue = data.Data;
            } else {
                // Primitive values
                rawValue = value;
            }

            return rawValue;
        }

        [NotNull]
        private Array DeserializeArray([NotNull, ItemCanBeNull] object[] array, [NotNull] Type arrayType, int level) {
            var elementType = arrayType.GetElementType();

            Debug.Assert(elementType != null);

            var arrayLength = array.Length;
            var result = Array.CreateInstance(elementType, arrayLength);

            for (var i = 0; i < arrayLength; i += 1) {
                var element = DeserializeValue(array[i], elementType, level + 1);
                var convertedValue = TryConvertValueType(NotNullTypeOf(element), elementType, element, null);
                result.SetValue(convertedValue, i);
            }

            return result;
        }

        [NotNull]
        private object DeserializeCollection([NotNull, ItemCanBeNull] object[] array, [NotNull] Type typeHint, int level) {
            var collectionInterfaceType = typeHint.GetGenericInterfaceImplementation(typeof(ICollection<>));

            if (collectionInterfaceType == null) {
                throw new ArgumentException($"{typeHint.Name} does not implement ICollection<T>.");
            }

            var genericTypes = collectionInterfaceType.GetGenericArguments();
            var elementType = genericTypes[0];

            MethodInfo addMethod;

            try {
                addMethod = typeHint.GetMethod("Add", InternalBindings);
            } catch (AmbiguousMatchException ex) {
                throw new SerializationException("Cannot find a proper Add() method.", ex);
            }

            if (addMethod == null) {
                throw new SerializationException("No Add() method found.");
            }

            var result = Activator.CreateInstance(typeHint, true);

            foreach (var item in array) {
                var element = DeserializeValue(item, elementType, level + 1);
                var convertedValue = TryConvertValueType(NotNullTypeOf(element), elementType, element, null);
                addMethod.Invoke(element, new[] { convertedValue });
            }

            return result;
        }

        [NotNull]
        private object DeserializeDictionary([NotNull] IDictionary<object, object> dictionary, [NotNull] Type typeHint, int level) {
            var collectionInterfaceType = typeHint.GetGenericInterfaceImplementation(typeof(IDictionary<,>));

            if (collectionInterfaceType == null) {
                throw new ArgumentException($"{typeHint.Name} does not implement IDictionary<TKey, TValue>.");
            }

            var genericTypes = collectionInterfaceType.GetGenericArguments();
            var keyType = genericTypes[0];
            var valueType = genericTypes[1];

            MethodInfo addMethod;

            try {
                addMethod = typeHint.GetMethod("Add", InternalBindings);
            } catch (AmbiguousMatchException ex) {
                throw new SerializationException("Cannot find a proper Add() method.", ex);
            }

            if (addMethod == null) {
                throw new SerializationException("No Add() method found.");
            }

            var result = Activator.CreateInstance(typeHint, true);

            foreach (var kv in dictionary) {
                var key = DeserializeValue(kv.Key, keyType, level + 1);
                var convertedKey = TryConvertValueType(NotNullTypeOf(key), keyType, key, null);

                var value = DeserializeValue(kv.Value, valueType, level + 1);
                var convertedValue = TryConvertValueType(NotNullTypeOf(value), valueType, value, null);

                addMethod.Invoke(result, new[] { convertedKey, convertedValue });
            }

            return result;
        }

        private void ApplyObjectMembers([NotNull] object obj, [NotNull] Type containerType, [NotNull] IReadOnlyDictionary<string, object> container, [NotNull] ScriptableObjectAttribute options, [NotNull, ItemNotNull] PropertyInfo[] properties, [NotNull, ItemNotNull] FieldInfo[] fields, [CanBeNull] INamingConvention naming, int level) {
            foreach (var kv in container) {
                if (level == 0 && FilteredNames.Contains(kv.Key)) {
                    continue;
                }

                var setter = FindSetterByName(containerType, properties, fields, kv.Key, naming);

                if (!setter.IsValid) {
                    if (options.ThrowOnUnmatched) {
                        throw new SerializationException();
                    } else {
                        continue;
                    }
                }

                var acceptedType = setter.GetValueType();
                var rawValue = DeserializeValue(kv.Value, acceptedType, level);
                var rawValueType = NotNullTypeOf(rawValue);

                SetValue(setter, obj, rawValue, rawValueType);
            }
        }

        [NotNull]
        private PropOrField FindSetterByName([NotNull] Type objectType, [NotNull, ItemNotNull] PropertyInfo[] properties, [NotNull, ItemNotNull] FieldInfo[] fields, [NotNull] string name, [CanBeNull] INamingConvention naming) {
            PropOrField result;

            foreach (var prop in properties) {
                var mbp = prop.GetCustomAttribute<ScriptableObjectPropertyAttribute>();
                var propName = !string.IsNullOrEmpty(mbp?.Name) ? mbp.Name : (naming != null ? naming.GetCorrected(prop.Name) : prop.Name);

                if (propName == name) {
                    var key = (objectType, propName, mbp);

                    if (_createdSetters.ContainsKey(key)) {
                        result = _createdSetters[key];
                    } else {
                        result = new PropOrField(prop, mbp);
                        _createdSetters[key] = result;
                    }

                    return result;
                }
            }

            foreach (var field in fields) {
                var mbp = field.GetCustomAttribute<ScriptableObjectPropertyAttribute>();
                var fieldName = !string.IsNullOrEmpty(mbp?.Name) ? mbp.Name : (naming != null ? naming.GetCorrected(field.Name) : field.Name);

                if (fieldName == name) {
                    var key = (objectType, fieldName, mbp);

                    if (_createdSetters.ContainsKey(key)) {
                        result = _createdSetters[key];
                    } else {
                        result = new PropOrField(field, mbp);
                        _createdSetters[key] = result;
                    }

                    return result;
                }
            }

            return PropOrField.Null;
        }

        private void SetValue([NotNull] PropOrField setter, [CanBeNull] object obj, [CanBeNull] object value, [NotNull] Type serializedValueType) {
            var acceptedType = setter.GetValueType();

            if (ReferenceEquals(value, null)) {
                if (acceptedType.IsValueType) {
                    throw new InvalidCastException("Cannot cast 'null' to a value type.");
                }
            }

            if (acceptedType.IsEnum) {
                if (ReferenceEquals(value, null)) {
                    throw new ApplicationException("Not possible.");
                }

                if (serializedValueType == typeof(byte) || serializedValueType == typeof(sbyte) ||
                    serializedValueType == typeof(ushort) || serializedValueType == typeof(short) ||
                    serializedValueType == typeof(uint) || serializedValueType == typeof(int) ||
                    serializedValueType == typeof(ulong) || serializedValueType == typeof(long)) {
                    var enumValue = Enum.ToObject(acceptedType, value);

                    setter.SetValueDirect(obj, enumValue);

                    return;
                }
            }

            var converterType = setter.Attribute?.ConverterType;
            var convertedValue = TryConvertValueType(serializedValueType, acceptedType, value, converterType);

            setter.SetValueDirect(obj, convertedValue);
        }

        [CanBeNull]
        private object TryConvertValueType([NotNull] Type serializedValueType, [NotNull] Type acceptedType, [CanBeNull] object value, [CanBeNull] Type converterTypeHint) {
            if (acceptedType == serializedValueType) {
                return value;
            } else {
                var converter = FindConverter(serializedValueType, acceptedType, converterTypeHint);

                if (converter != null) {
                    var convertedValue = ConvertValue(converter, serializedValueType, acceptedType, value);

                    return convertedValue;
                } else {
                    return value;
                }
            }
        }

        [CanBeNull]
        private static object ConvertValue([NotNull] ISimpleTypeConverter converter, [NotNull] Type serializedValueType, [NotNull] Type acceptedType, [CanBeNull] object value) {
            if (!converter.CanConvertFrom(serializedValueType) || !converter.CanConvertTo(acceptedType)) {
                throw new InvalidCastException($"Serialized type {serializedValueType} cannot be converted to {acceptedType}.");
            }

            return converter.ConvertTo(value, serializedValueType, acceptedType);
        }

        [CanBeNull]
        private ISimpleTypeConverter FindConverter([NotNull] Type sourceType, [NotNull] Type destinationType, [CanBeNull] Type converterTypeHint) {
            ISimpleTypeConverter converter;

            foreach (var kv in _createdTypeConverters) {
                converter = kv.Value;

                if (converter.CanConvertFrom(sourceType) && converter.CanConvertTo(destinationType)) {
                    return converter;
                }
            }

            if (converterTypeHint == null) {
                return null;
            }

            converter = GetConverter(converterTypeHint);

            if (!(converter.CanConvertFrom(sourceType) && converter.CanConvertTo(destinationType))) {
                return null;
            }

            return converter;
        }

        [NotNull]
        private ISimpleTypeConverter GetConverter([NotNull] Type converterType) {
            if (!converterType.ImplementsInterface(typeof(ISimpleTypeConverter))) {
                throw new ArgumentException("Converter does not implement " + nameof(ISimpleTypeConverter) + ".");
            }

            ISimpleTypeConverter converter;

            if (_createdTypeConverters.ContainsKey(converterType)) {
                converter = _createdTypeConverters[converterType];
            } else {
                converter = (ISimpleTypeConverter)Activator.CreateInstance(converterType, true);
                _createdTypeConverters[converterType] = converter;
            }

            return converter;
        }

        [CanBeNull]
        private static object CreateDefaultOf([NotNull] Type type) {
            if (type.IsValueType) {
                return Activator.CreateInstance(type);
            } else {
                return null;
            }
        }

        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Type NotNullTypeOf([CanBeNull] object obj) {
            return obj?.GetType() ?? typeof(object);
        }

        private const BindingFlags InternalBindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        [NotNull]
        private static readonly ScriptableObjectAttribute DefaultClassOptions = new ScriptableObjectAttribute();

        [NotNull]
        private readonly Dictionary<Type, ISimpleTypeConverter> _createdTypeConverters;

        [NotNull]
        private readonly Dictionary<(Type, string, ScriptableObjectPropertyAttribute), PropOrField> _createdSetters;

        [NotNull, ItemNotNull]
        private static readonly string[] FilteredNames = {
            "m_GameObject",
            "m_Enabled",
            "m_Script",
            "m_Name"
        };

    }
}
