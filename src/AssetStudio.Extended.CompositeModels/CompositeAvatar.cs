using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AssetStudio.Extended.CompositeModels.Utilities;
using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class CompositeAvatar : PrettyAvatar {

        private CompositeAvatar([NotNull] PrettyAvatar[] avatars) {
            ComponentAvatarCount = avatars.Length;

            {
                var avatarNames = new List<string>();

                foreach (var a in avatars) {
                    if (a is CompositeAvatar ca) {
                        avatarNames.AddRange(ca.Names);
                    } else {
                        avatarNames.Add(a.Name);
                    }
                }

                Names = avatarNames.ToArray();

                var sb = new StringBuilder();

                sb.Append("Composite avatar of:");

                foreach (var name in avatarNames) {
                    sb.AppendLine();
                    sb.Append("\t");
                    sb.Append(name);
                }

                Name = sb.ToString();
            }

            AvatarSkeleton = CombineSkeletons(avatars.Select(av => av.AvatarSkeleton));
            AvatarSkeletonPose = CombineSkeletonPoses(avatars.Select(av => av.AvatarSkeletonPose), avatars.Select(av => av.AvatarSkeleton));

            {
                var boneNamesMap = new Dictionary<uint, string>();

                foreach (var avatar in avatars) {
                    foreach (var kv in avatar.BoneNamesMap) {
                        if (!boneNamesMap.ContainsKey(kv.Key)) {
                            boneNamesMap.Add(kv.Key, kv.Value);
                        }
                    }
                }

                BoneNamesMap = boneNamesMap;
            }
        }

        [NotNull, ItemNotNull]
        public string[] Names { get; }

        public override string Name { get; }

        public override PrettySkeleton AvatarSkeleton { get; }

        public override PrettySkeletonPose AvatarSkeletonPose { get; }

        public override IReadOnlyDictionary<uint, string> BoneNamesMap { get; }

        public int ComponentAvatarCount { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static CompositeAvatar FromAvatars([NotNull, ItemNotNull] params PrettyAvatar[] avatars) {
            return new CompositeAvatar(avatars);
        }

        [NotNull]
        private static PrettySkeleton CombineSkeletons([NotNull, ItemNotNull] IEnumerable<PrettySkeleton> skeletons) {
            var nodeIDList = new List<uint>();
            var nodeList = new List<SkeletonNode>();

            var nodeStart = 0;
            var counter = 0;

            foreach (var skeleton in skeletons) {
                Debug.Assert(skeleton.NodeIDs.Length == skeleton.Nodes.Length);

                var nodeCount = skeleton.NodeIDs.Length;

                foreach (var id in skeleton.NodeIDs) {
                    if (id == 0) {
                        if (nodeIDList.Count == 0) {
                            nodeIDList.Add(id);
                        }
                    } else {
                        nodeIDList.Add(id);
                    }
                }

                var isFirstNode = true;

                foreach (var node in skeleton.Nodes) {
                    if (isFirstNode) {
                        if (nodeList.Count > 0) {
                            isFirstNode = false;
                            continue;
                        }
                    }

                    int parentIndex;

                    if (counter == 0) {
                        parentIndex = node.ParentIndex;
                    } else {
                        if (node.ParentIndex <= 0) {
                            parentIndex = node.ParentIndex;
                        } else {
                            parentIndex = node.ParentIndex + nodeStart - 1;
                        }
                    }

                    var n = new SkeletonNode(parentIndex, node.AxisIndex);

                    nodeList.Add(n);

                    isFirstNode = false;
                }

                if (counter == 0) {
                    nodeStart += nodeCount;
                } else {
                    // From the second time, "" (empty name) bone is filtered out because it will duplicate.
                    nodeStart += nodeCount - 1;
                }

                ++counter;
            }

            var result = new PrettySkeleton(nodeList.ToArray(), nodeIDList.ToArray());

            return result;
        }

        [NotNull]
        private static PrettySkeletonPose CombineSkeletonPoses([NotNull, ItemNotNull] IEnumerable<PrettySkeletonPose> poses, [NotNull, ItemNotNull] IEnumerable<PrettySkeleton> skeletons) {
            var transformList = new List<RawTransform>();

            foreach (var (pose, skeleton) in EnumerableUtils.Zip(poses, skeletons)) {
                var transforms = pose.Transforms;
                var ids = skeleton.NodeIDs;

                Debug.Assert(transforms.Length == ids.Length);

                var len = transforms.Length;

                for (var i = 0; i < len; ++i) {
                    if (ids[i] == 0) {
                        if (transformList.Count == 0) {
                            transformList.Add(transforms[i]);
                        }
                    } else {
                        transformList.Add(transforms[i]);
                    }
                }
            }

            var result = new PrettySkeletonPose(transformList.ToArray());

            return result;
        }

    }
}
