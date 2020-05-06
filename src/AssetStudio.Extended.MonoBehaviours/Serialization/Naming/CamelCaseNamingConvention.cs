namespace AssetStudio.Extended.MonoBehaviours.Serialization.Naming {
    public sealed class CamelCaseNamingConvention : INamingConvention {

        private CamelCaseNamingConvention() {
        }

        public string GetCorrected(string input) {
            if (string.IsNullOrEmpty(input)) {
                return string.Empty;
            }

            if (!char.IsUpper(input, 0)) {
                return input;
            }

            return char.ToLowerInvariant(input[0]) + input.Substring(1);
        }

    }
}
