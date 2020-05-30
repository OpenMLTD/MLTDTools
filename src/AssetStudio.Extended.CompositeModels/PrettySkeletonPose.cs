using System.Diagnostics;
using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class PrettySkeletonPose {

        internal PrettySkeletonPose([NotNull, ItemNotNull] RawTransform[] transforms) {
            Transforms = transforms;
        }

        internal PrettySkeletonPose([NotNull] SkeletonPose pose) {
            var transformCount = pose.m_X.Length;
            var transforms = new RawTransform[transformCount];

            for (var i = 0; i < transformCount; i += 1) {
                var t = new RawTransform(pose.m_X[i]);
                transforms[i] = t;
            }

            Transforms = transforms;
        }

        [NotNull, ItemNotNull]
        public RawTransform[] Transforms {
            [DebuggerStepThrough]
            get;
        }

    }
}
