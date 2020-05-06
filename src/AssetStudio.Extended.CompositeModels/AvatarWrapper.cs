using System.Collections.Generic;
using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class AvatarWrapper : PrettyAvatar {

        public AvatarWrapper([NotNull] Avatar avatar) {
            Name = avatar.m_Name;
            AvatarSkeleton = new Skeleton(avatar.m_Avatar.m_AvatarSkeleton);
            AvatarSkeletonPose = new SkeletonPose(avatar.m_Avatar.m_AvatarSkeletonPose);

            {
                var map = new Dictionary<uint, string>();
                var tos = avatar.m_TOS;

                foreach (var kv in tos) {
                    map.Add(kv.Key, kv.Value);
                }

                BoneNamesMap = map;
            }
        }

        public override string Name { get; }

        public override Skeleton AvatarSkeleton { get; }

        public override SkeletonPose AvatarSkeletonPose { get; }

        public override IReadOnlyDictionary<uint, string> BoneNamesMap { get; }

    }
}
