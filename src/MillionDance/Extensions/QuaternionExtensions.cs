namespace MillionDance.Extensions {
    internal static class QuaternionExtensions {

        public static OpenTK.Quaternion ToOpenTK(this UnityStudio.Unity.Quaternion q) {
            return new OpenTK.Quaternion(q.X, q.Y, q.Z, q.W);
        }

    }
}
