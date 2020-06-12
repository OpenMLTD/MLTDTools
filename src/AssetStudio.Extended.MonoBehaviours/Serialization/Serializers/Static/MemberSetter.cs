using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers.Static {
    /// <summary>
    /// Accelerated member setter, faster than <see cref="FieldInfo.SetValue(object, object)"/> and <see cref="PropertyInfo.SetValue(object, object)"/>.
    /// However there is compilation cost, so caching should be used.
    /// </summary>
    internal sealed class MemberSetter {

        private MemberSetter() {
            IsValid = false;
            // ReSharper disable once AssignNullToNotNullAttribute
            _valueSetter = null;
        }

        public MemberSetter([NotNull] PropertyInfo property, [CanBeNull] ScriptableObjectPropertyAttribute attribute) {
            _property = property;
            _field = null;
            Attribute = attribute;
            IsValid = true;

            _valueSetter = CompilePropertySetter(property);
        }

        public MemberSetter([NotNull] FieldInfo field, [CanBeNull] ScriptableObjectPropertyAttribute attribute) {
            _property = null;
            _field = field;
            Attribute = attribute;
            IsValid = true;

            _valueSetter = CompileFieldSetter(field);
        }

        public bool IsValid { get; }

        public void SetValueDirect([CanBeNull] object obj, [CanBeNull] object value) {
            if (!IsValid) {
                throw new InvalidOperationException();
            }

            if (_valueSetter == null) {
                throw new InvalidOperationException("Value setter is not ready.");
            }

            _valueSetter.Invoke(obj, value);
        }

        [NotNull]
        public Type GetValueType() {
            if (!IsValid) {
                throw new InvalidOperationException();
            }

            if (_property != null) {
                return _property.PropertyType;
            }

            if (_field != null) {
                return _field.FieldType;
            }

            throw new InvalidOperationException();
        }

        [NotNull]
        private static Action<object, object> CompileFieldSetter([NotNull] FieldInfo field) {
            var instanceParam = Expression.Parameter(typeof(object), "instance");
            var valueParam = Expression.Parameter(typeof(object), "value");

            Debug.Assert(field.DeclaringType != null);

            var convertedInstance = Expression.Convert(instanceParam, field.DeclaringType);
            var convertedValue = Expression.Convert(valueParam, field.FieldType);

            var fieldAccess = Expression.Field(convertedInstance, field);
            var assign = Expression.Assign(fieldAccess, convertedValue);

            var lambda = Expression.Lambda<Action<object, object>>(assign, instanceParam, valueParam);
            var del = lambda.Compile();

            return del;
        }

        [NotNull]
        private static Action<object, object> CompilePropertySetter([NotNull] PropertyInfo property) {
            var instanceParam = Expression.Parameter(typeof(object), "instance");
            var valueParam = Expression.Parameter(typeof(object), "value");

            Debug.Assert(property.DeclaringType != null);

            var convertedInstance = Expression.Convert(instanceParam, property.DeclaringType);
            var convertedValue = Expression.Convert(valueParam, property.PropertyType);

            // Including private setters
            var setterCall = Expression.Call(convertedInstance, property.SetMethod, convertedValue);

            var lambda = Expression.Lambda<Action<object, object>>(setterCall, instanceParam, valueParam);
            var del = lambda.Compile();

            return del;
        }

        [CanBeNull]
        public ScriptableObjectPropertyAttribute Attribute { get; }

        [NotNull]
        public static readonly MemberSetter Null = new MemberSetter();

        [NotNull]
        private readonly Action<object, object> _valueSetter;

        [CanBeNull]
        private readonly PropertyInfo _property;

        [CanBeNull]
        private readonly FieldInfo _field;

    }
}
