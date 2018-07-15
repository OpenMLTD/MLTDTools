using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using OpenTK;

namespace MillionDance.Entities.Pmx.Extensions {
    internal static class PmxBoneExtensions {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasFlag([NotNull] this PmxBone bone, BoneFlags flags) {
            return (bone.Flags & flags) != 0;
        }

        // Set to binding pose ("T" pose)
        public static void SetToBindingPose([NotNull] this PmxBone bone) {
            if (bone.IsTransformCalculated) {
                return;
            }

            var parent = bone.Parent;

            Vector3 localPosition;

            if (parent == null) {
                localPosition = bone.InitialPosition;
            } else {
                parent.SetToBindingPose();
                localPosition = bone.InitialPosition - parent.InitialPosition;
            }

            bone.LocalMatrix = CalculateTransform(localPosition, Quaternion.Identity);
            bone.WorldMatrix = bone.CalculateWorldMatrix();

            bone.IsTransformCalculated = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4 CalculateWorldMatrix([NotNull] this PmxBone bone) {
            if (bone.Parent == null) {
                return bone.LocalMatrix;
            } else {
                return bone.LocalMatrix * bone.Parent.WorldMatrix;
            }
        }

        internal static void SetInitialRotationFromRotationAxes([NotNull] this PmxBone bone, Vector3 localX, Vector3 localY, Vector3 localZ) {
            var rotationMatrix = new Matrix3(
                localX.X, localX.Y, localX.Z,
                localY.X, localY.Y, localY.Z,
                localZ.X, localZ.Y, localZ.Z);

            bone.InitialRotation = Quaternion.FromMatrix(rotationMatrix);
            bone.CurrentRotation = bone.InitialRotation;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Matrix4 CalculateTransform(Vector3 translation, Quaternion rotation) {
            var translationMatrix = Matrix4.CreateTranslation(translation);
            var rotationanMatrix = Matrix4.CreateFromQuaternion(rotation);

            return rotationanMatrix * translationMatrix;
        }

    }
}
