using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using AssetStudio.Extended.MonoBehaviours.Extensions;
using AssetStudio.Extended.MonoBehaviours.Serialization.Naming;
using AssetStudio.Extended.MonoBehaviours.Serialization.Serialized;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers.Dynamic {
    // TODO: use this as a template to create runtime serializer classes
    internal sealed partial class TypedSerializer : TypedSerializerBase {

        public TypedSerializer([NotNull] DynamicSerializationContext context, [NotNull] Type containerType) {
            Context = context;
            ContainerType = containerType;

            _propertyAttributeCache = new Dictionary<PropertyInfo, ScriptableObjectPropertyAttribute>();
            _fieldAttributeCache = new Dictionary<FieldInfo, ScriptableObjectPropertyAttribute>();

            _createdSetters = new Dictionary<string, MemberSetter>();
        }

        [NotNull]
        public DynamicSerializationContext Context { get; }

        [NotNull]
        public Type ContainerType { get; }

        protected override object DeserializeObject(CustomType structure, int level) {
            InitializeOnContainerType();

            // Attention! Fields of the created object are not initialized because we don't call its constructor.
            var obj = FormatterServices.GetSafeUninitializedObject(ContainerType);

            Debug.Assert(_options != null, nameof(_options) + " != null");

            obj = ApplyObjectMembers(obj, structure.MutableVariables, _options, _naming, level);

            return obj;
        }

        private void InitializeOnContainerType() {
            if (_isInitialized) {
                return;
            }

            var containerType = ContainerType;

            var options = containerType.GetCustomAttribute<ScriptableObjectAttribute>() ?? ScriptableObjectAttribute.Default;

            PropertyInfo[] properties;
            FieldInfo[] fields;

            var allProperties = containerType.GetProperties(InternalBindings);
            // Filter out compiler generated fields.
            var allFields = containerType.GetFields(InternalBindings).WhereToArray(field => !Attribute.IsDefined(field, typeof(CompilerGeneratedAttribute)));

            switch (options.PopulationStrategy) {
                case PopulationStrategy.OptIn:
                    properties = allProperties.WhereToArray(prop => Attribute.IsDefined(prop, typeof(ScriptableObjectPropertyAttribute)));
                    fields = allFields.WhereToArray(field => Attribute.IsDefined(field, typeof(ScriptableObjectPropertyAttribute)));
                    break;
                case PopulationStrategy.OptOut:
                    properties = allProperties.WhereToArray(prop => !Attribute.IsDefined(prop, typeof(ScriptableObjectIgnoreAttribute)));
                    fields = allFields.WhereToArray(field => !Attribute.IsDefined(field, typeof(ScriptableObjectIgnoreAttribute)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(options.PopulationStrategy), options.PopulationStrategy, null);
            }

            _properties = properties;
            _fields = fields;
            _options = options;
            _naming = options.NamingConventionType != null ? (INamingConvention)Context.Activator.CreateInstance(options.NamingConventionType, true) : null;

            _isInitialized = true;
        }

        [CanBeNull]
        private object DeserializeValue([CanBeNull] object value, [CanBeNull] Type typeHint, int level) {
            object rawValue;

            if (value is CustomType ct) {
                Debug.Assert(typeHint != null);
                var serializer = Context.Serializers.GetSerializerOf(typeHint);
                rawValue = serializer.DeserializeObject(ct, level + 1);
            } else if (value is ObjectDictionary dict) {
                Debug.Assert(typeHint != null);
                rawValue = DeserializeDictionary(dict, typeHint, level);
            } else if (value is ObjectArray arr) {
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
        private Array DeserializeRank1Array([NotNull] ObjectArray arr, [NotNull] Type arrayType, int level) {
            var elementType = arrayType.GetElementType();

            Debug.Assert(elementType != null);

            var array = arr.Array;
            var arrayLength = array.Length;
            var result = Array.CreateInstance(elementType, arrayLength);

            var converters = Context.Converters;

            for (var i = 0; i < arrayLength; i += 1) {
                var element = DeserializeValue(array[i], elementType, level + 1);
                var convertedValue = converters.TryConvertTypeOfValue(SerializingHelper.NonNullTypeOf(element), elementType, element, null);
                result.SetValue(convertedValue, i);
            }

            return result;
        }

        [NotNull]
        private object DeserializeCollection([NotNull] ObjectArray array, [NotNull] Type typeHint, int level) {
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

            var result = Context.Activator.CreateInstance(typeHint, true);

            var converters = Context.Converters;

            foreach (var item in array.Array) {
                var element = DeserializeValue(item, elementType, level + 1);
                var convertedValue = converters.TryConvertTypeOfValue(SerializingHelper.NonNullTypeOf(element), elementType, element, null);
                addMethod.Invoke(element, new[] { convertedValue });
            }

            return result;
        }

        [NotNull]
        private object DeserializeDictionary([NotNull] ObjectDictionary dictionary, [NotNull] Type typeHint, int level) {
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

            var result = Context.Activator.CreateInstance(typeHint, true);

            var converters = Context.Converters;

            foreach (var kv in dictionary.Dictionary) {
                var key = DeserializeValue(kv.Key, keyType, level + 1);
                var convertedKey = converters.TryConvertTypeOfValue(SerializingHelper.NonNullTypeOf(key), keyType, key, null);

                var value = DeserializeValue(kv.Value, valueType, level + 1);
                var convertedValue = converters.TryConvertTypeOfValue(SerializingHelper.NonNullTypeOf(value), valueType, value, null);

                addMethod.Invoke(result, new[] { convertedKey, convertedValue });
            }

            return result;
        }

        [CanBeNull]
        private object ApplyObjectMembers([CanBeNull] object obj, [NotNull] Dictionary<string, object> container, [NotNull] ScriptableObjectAttribute options, [CanBeNull] INamingConvention naming, int level) {
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

                obj = SetValue(setter, obj, rawValue, rawValueType);
            }

            return obj;
        }

        private const BindingFlags InternalBindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        [NotNull]
        private readonly Dictionary<PropertyInfo, ScriptableObjectPropertyAttribute> _propertyAttributeCache;

        [NotNull]
        private readonly Dictionary<FieldInfo, ScriptableObjectPropertyAttribute> _fieldAttributeCache;

        private bool _isInitialized;

        [CanBeNull, ItemNotNull]
        private PropertyInfo[] _properties;

        [CanBeNull, ItemNotNull]
        private FieldInfo[] _fields;

        [CanBeNull]
        private ScriptableObjectAttribute _options;

        [CanBeNull]
        private INamingConvention _naming;

        [NotNull, ItemNotNull]
        private static readonly string[] FilteredNames = {
            "m_GameObject",
            "m_Enabled",
            "m_Script",
            "m_Name"
        };

    }
}
