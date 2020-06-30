using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Serializers.Dynamic {
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

        // We have to return the modified object.
        // If `obj` is a boxed value type, only calling setters/setting fields will not reflect the changes
        // on the original (which is constructed by activator). So we have to unbox, modify, and then box again.
        // There will be a little efficiency loss, but it's still better than naively using PropertyInfo and
        // FieldInfo. For reference types there is no such a problem.
        [CanBeNull]
        public object SetValueDirect([CanBeNull] object obj, [CanBeNull] object value) {
            if (!IsValid) {
                throw new InvalidOperationException();
            }

            if (_valueSetter == null) {
                throw new InvalidOperationException("Value setter is not ready.");
            }

            if (ReferenceEquals(obj, null)) {
                return null;
            } else {
                return _valueSetter.Invoke(obj, value);
            }
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
        private static Func<object, object, object> CompileFieldSetter([NotNull] FieldInfo field) {
            var instanceParam = Expression.Parameter(typeof(object), "instance");
            var valueParam = Expression.Parameter(typeof(object), "value");

            Debug.Assert(field.DeclaringType != null);

            var variable = Expression.Variable(field.DeclaringType, "this");
            var convertedInstance = Expression.Convert(instanceParam, field.DeclaringType);
            var assignThis = Expression.Assign(variable, convertedInstance);

            var convertedValue = Expression.Convert(valueParam, field.FieldType);
            var fieldAccess = Expression.Field(variable, field);
            var assignValue = Expression.Assign(fieldAccess, convertedValue);

            var label = Expression.Label(typeof(object));
            var convertedReturn = Expression.Convert(variable, typeof(object));
            var ret = Expression.Return(label, convertedReturn, typeof(object));

            var block = Expression.Block(new[] { variable }, assignThis, assignValue, ret, Expression.Label(label, Expression.Default(typeof(object))));

            var lambda = Expression.Lambda<Func<object, object, object>>(block, instanceParam, valueParam);
            var del = lambda.Compile();

            return del;
        }

        [NotNull]
        private static Func<object, object, object> CompilePropertySetter([NotNull] PropertyInfo property) {
            var instanceParam = Expression.Parameter(typeof(object), "instance");
            var valueParam = Expression.Parameter(typeof(object), "value");

            Debug.Assert(property.DeclaringType != null);

            var variable = Expression.Variable(property.DeclaringType, "this");
            var convertedInstance = Expression.Convert(instanceParam, property.DeclaringType);
            var assignThis = Expression.Assign(variable, convertedInstance);

            var convertedValue = Expression.Convert(valueParam, property.PropertyType);
            var setterCall = Expression.Call(variable, property.SetMethod, convertedValue);

            var label = Expression.Label(typeof(object));
            var convertedReturn = Expression.Convert(convertedInstance, typeof(object));
            var ret = Expression.Return(label, convertedReturn, typeof(object));

            var block = Expression.Block(new[] { variable }, assignThis, setterCall, ret, Expression.Label(label, Expression.Default(typeof(object))));

            var lambda = Expression.Lambda<Func<object, object, object>>(block, instanceParam, valueParam);
            var del = lambda.Compile();

            return del;
        }

        [CanBeNull]
        public ScriptableObjectPropertyAttribute Attribute { get; }

        [NotNull]
        public static readonly MemberSetter Null = new MemberSetter();

        [NotNull]
        private readonly Func<object, object, object> _valueSetter;

        [CanBeNull]
        private readonly PropertyInfo _property;

        [CanBeNull]
        private readonly FieldInfo _field;

    }
}
