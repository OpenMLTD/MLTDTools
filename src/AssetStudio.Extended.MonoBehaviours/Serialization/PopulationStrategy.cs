namespace AssetStudio.Extended.MonoBehaviours.Serialization {
    public enum PopulationStrategy {

        /// <summary>
        /// Populate only properties or fields with <see cref="ScriptableObjectPropertyAttribute"/> attribute.
        /// </summary>
        OptIn,

        /// <summary>
        /// Populate every property or field, unless it has marked with a <see cref="ScriptableObjectIgnoreAttribute"/> attribute.
        /// </summary>
        OptOut

    }
}
