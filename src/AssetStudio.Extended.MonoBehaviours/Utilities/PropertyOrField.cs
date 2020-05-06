using System;
using System.Reflection;
using AssetStudio.Extended.MonoBehaviours.Serialization;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Utilities {
    internal struct PropertyOrField {

        internal PropertyOrField([NotNull] FieldInfo field, [CanBeNull] ScriptableObjectPropertyAttribute attribute) {
            Field = field;
            Property = null;
            IsValid = true;
            Attribute = attribute;
        }

        internal PropertyOrField([NotNull] PropertyInfo property, [CanBeNull] ScriptableObjectPropertyAttribute attribute) {
            Field = null;
            Property = property;
            IsValid = true;
            Attribute = attribute;
        }

        internal void SetValue([CanBeNull] object @this, [CanBeNull] object value) {
            if (!IsValid) {
                throw new InvalidOperationException();
            }

            Property?.SetValue(@this, value);
            Field?.SetValue(@this, value);
        }

        [NotNull]
        internal Type GetValueType() {
            if (!IsValid) {
                throw new InvalidOperationException();
            }

            if (Property != null) {
                return Property.PropertyType;
            }

            if (Field != null) {
                return Field.FieldType;
            }

            throw new InvalidOperationException();
        }

        internal bool IsValid { get; }

        [CanBeNull]
        internal FieldInfo Field { get; }

        [CanBeNull]
        internal PropertyInfo Property { get; }

        [CanBeNull]
        internal ScriptableObjectPropertyAttribute Attribute { get; }

        internal static readonly PropertyOrField Null = new PropertyOrField();

    }
}