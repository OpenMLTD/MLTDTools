using System.Runtime.CompilerServices;
using AssetStudio.Extended.CompositeModels;
using JetBrains.Annotations;

namespace OpenMLTD.MillionDance.Viewer.Extensions {
    internal static class AvatarExtensions {

        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FindBonePathByNameHash([NotNull] this PrettyAvatar avatar, uint hash) {
            avatar.BoneNamesMap.TryGetValue(hash, out var result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindBoneIndexByNameHash([NotNull] this PrettyAvatar avatar, uint hash) {
            var list = avatar.AvatarSkeleton.NodeIDs;

            for (var i = 0; i < list.Length; ++i) {
                if (list[i] == hash) {
                    return i;
                }
            }

            return -1;
        }

    }
}
