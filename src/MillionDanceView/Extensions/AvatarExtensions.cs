using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityStudio.UnityEngine.Animation;

namespace MillionDanceView.Extensions {
    internal static class AvatarExtensions {

        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FindBonePathByNameHash([NotNull] this Avatar avatar, uint hash) {
            avatar.BoneNamesMap.TryGetValue(hash, out var result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindBoneIndexByNameHash([NotNull] this Avatar avatar, uint hash) {
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
