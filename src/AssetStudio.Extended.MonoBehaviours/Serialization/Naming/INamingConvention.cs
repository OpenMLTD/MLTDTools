using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization.Naming {
    public interface INamingConvention {

        string GetCorrected([CanBeNull] string input);

    }
}
