using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using AssetStudio.Extended.MonoBehaviours.Extensions;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Managing {
    // TODO: use this as a template to create runtime serializer classes
    internal sealed partial class TypedSerializerBase {

        public TypedSerializerBase([NotNull] SerializerManager manager, [NotNull] Type containerType) {
            Manager = manager;
            ContainerType = containerType;

            _propertyAttributeCache = new Dictionary<PropertyInfo, ScriptableObjectPropertyAttribute>();
            _fieldAttributeCache = new Dictionary<FieldInfo, ScriptableObjectPropertyAttribute>();

            _createdSetters = new Dictionary<string, MemberSetter>();
        }

        [NotNull]
        public SerializerManager Manager { get; }

        [NotNull]
        public Type ContainerType { get; }

        [NotNull]
        public object DeserializeObject([NotNull] CustomType structure) {
            return DeserializeObject(structure, 0);
        }

        [NotNull]
        public object DeserializeObject([NotNull] CustomType structure, int level) {
            var containerType = ContainerType;

            var options = containerType.GetCustomAttribute<ScriptableObjectAttribute>() ?? ScriptableObjectAttribute.Default;

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

            _properties = properties;
            _fields = fields;

            var naming = options.NamingConventionType != null ? (INamingConvention)Activator.CreateInstance(options.NamingConventionType, true) : null;

            var obj = Activator.CreateInstance(containerType, true);

            ApplyObjectMembers(obj, structure, options, naming, level);

            return obj;
        }

        [CanBeNull]
        private object DeserializeValue([CanBeNull] object value, [CanBeNull] Type typeHint, int level) {
            object rawValue;

            if (value is CustomType ct) {
                Debug.Assert(typeHint != null);
                var serializer = Manager.GetSerializerOf(typeHint);
                rawValue = serializer.DeserializeObject(ct, level + 1);
            } else if (value is IDictionary<object, object> dict) {
                Debug.Assert(typeHint != null);
                rawValue = DeserializeDictionary(dict, typeHint, level);
            } else if (value is object[] arr) {
                Debug.Assert(typeHint != null);

                if (typeHint.IsArray) {
                    var rank = typeHint.GetArrayRank();

                    Debug.Assert(rank > 0, nameof(rank) + " > 0");

                    if (rank != 1) {
                        throw new NotSupportedException("Multi-rank arrays are not supported yet.");
                    }

                    rawValue = DeserializeRank1Array(arr, typeHint, level);
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
        private Array DeserializeRank1Array([NotNull, ItemCanBeNull] object[] array, [NotNull] Type arrayType, int level) {
            var elementType = arrayType.GetElementType();

            Debug.Assert(elementType != null);

            var arrayLength = array.Length;
            var result = Array.CreateInstance(elementType, arrayLength);

            var manager = Manager;

            for (var i = 0; i < arrayLength; i += 1) {
                var element = DeserializeValue(array[i], elementType, level + 1);
                var convertedValue = manager.TryConvertValueType(SerializingHelper.NonNullTypeOf(element), elementType, element, null);
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

            var manager = Manager;

            foreach (var item in array) {
                var element = DeserializeValue(item, elementType, level + 1);
                var convertedValue = manager.TryConvertValueType(SerializingHelper.NonNullTypeOf(element), elementType, element, null);
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

            var manager = Manager;

            foreach (var kv in dictionary) {
                var key = DeserializeValue(kv.Key, keyType, level + 1);
                var convertedKey = manager.TryConvertValueType(SerializingHelper.NonNullTypeOf(key), keyType, key, null);

                var value = DeserializeValue(kv.Value, valueType, level + 1);
                var convertedValue = manager.TryConvertValueType(SerializingHelper.NonNullTypeOf(value), valueType, value, null);

                addMethod.Invoke(result, new[] { convertedKey, convertedValue });
            }

            return result;
        }

        private void ApplyObjectMembers([NotNull] object obj, [NotNull] IReadOnlyDictionary<string, object> container, [NotNull] ScriptableObjectAttribute options, [CanBeNull] INamingConvention naming, int level) {
            foreach (var kv in container) {
                if (level == 0 && FilteredNames.Contains(kv.Key)) {
                    continue;
                }

                var setter = FindSetterByName(kv.Key, naming);

                if (!setter.IsValid) {
                    if (options.ThrowOnUnmatched) {
                        throw new SerializationException();
                    } else {
                        continue;
                    }
                }

                var acceptedType = setter.GetValueType();
                var rawValue = DeserializeValue(kv.Value, acceptedType, level);
                var rawValueType = SerializingHelper.NonNullTypeOf(rawValue);

                SetValue(setter, obj, rawValue, rawValueType);
            }
        }

        private const BindingFlags InternalBindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        [NotNull]
        private readonly Dictionary<PropertyInfo, ScriptableObjectPropertyAttribute> _propertyAttributeCache;

        [NotNull]
        private readonly Dictionary<FieldInfo, ScriptableObjectPropertyAttribute> _fieldAttributeCache;

        [CanBeNull, ItemNotNull]
        private PropertyInfo[] _properties;

        [CanBeNull, ItemNotNull]
        private FieldInfo[] _fields;

        [NotNull, ItemNotNull]
        private static readonly string[] FilteredNames = {
            "m_GameObject",
            "m_Enabled",
            "m_Script",
            "m_Name"
        };

    }
}
