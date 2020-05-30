using System.Collections.Generic;
using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public abstract class PrettyAvatar {

        private protected PrettyAvatar() {
        }

        [NotNull]
        public abstract string Name { get; }

        [NotNull]
        public abstract PrettySkeleton AvatarSkeleton { get; }

        [NotNull]
        public abstract PrettySkeletonPose AvatarSkeletonPose { get; }

        [NotNull]
        public abstract IReadOnlyDictionary<uint, string> BoneNamesMap { get; }

    }
}
