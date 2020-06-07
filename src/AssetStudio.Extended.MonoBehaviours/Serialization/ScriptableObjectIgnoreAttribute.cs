using System;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.Itself)]
    public sealed class ScriptableObjectIgnoreAttribute : Attribute {

    }
}
