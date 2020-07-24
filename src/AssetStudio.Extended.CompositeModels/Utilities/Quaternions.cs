namespace AssetStudio.Extended.CompositeModels.Utilities {
    public static class Quaternions {

        static Quaternions() {
            Identity = new Quaternion(0, 0, 0, 1);
        }

        public static readonly Quaternion Identity;

    }
}
