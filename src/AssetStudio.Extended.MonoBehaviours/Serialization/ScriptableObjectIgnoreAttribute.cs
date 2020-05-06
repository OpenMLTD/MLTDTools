using System;

namespace AssetStudio.Extended.MonoBehaviours.Serialization {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ScriptableObjectIgnoreAttribute : Attribute {
    }
}
