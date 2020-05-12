using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AssetStudio;
using AssetStudio.Extended.CompositeModels;
using AssetStudio.Extended.MonoBehaviours.Serialization;
using Imas.Data.Serialized;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Viewer.Extensions;
using OpenMLTD.MillionDance.Viewer.Internal;
using OpenMLTD.MillionDance.Viewer.ObjectGL;
using OpenTK.Graphics.OpenGL4;
using Shader = OpenMLTD.MillionDance.Viewer.ObjectGL.Shader;

namespace OpenMLTD.MillionDance.Viewer {
    internal static class ResHelper {

        [NotNull]
        public static T LoadProgram<T>([NotNull] string vertSourceFile, [NotNull] string fragSourceFile)
            where T : Program {
            var vertSource = File.ReadAllText(vertSourceFile);
            var fragSource = File.ReadAllText(fragSourceFile);

            var vert = Shader.Compile(vertSource, ShaderType.VertexShader);
            var frag = Shader.Compile(fragSource, ShaderType.FragmentShader);

            var program = Program.Link<T>(vert, frag);

            return program;
        }

        [CanBeNull]
        public static PrettyMesh LoadBodyMesh() {
            MeshWrapper result = null;

            var manager = new AssetsManager();
            manager.LoadFiles(BodyModelFilePath);

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.Mesh) {
                        continue;
                    }

                    var mesh = obj as Mesh;

                    if (mesh == null) {
                        throw new ArgumentNullException(nameof(mesh), "Body mesh is null.");
                    }

                    result = new MeshWrapper(manager.assetsFileList, mesh, true);

                    break;
                }
            }

            return result;
        }

        [CanBeNull]
        public static PrettyMesh LoadHeadMesh() {
            var meshList = new List<PrettyMesh>();

            var manager = new AssetsManager();
            manager.LoadFiles(HeadModelFilePath);

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.Mesh) {
                        continue;
                    }

                    var mesh = obj as Mesh;

                    if (mesh == null) {
                        throw new ArgumentNullException(nameof(mesh), "One of head meshes is null.");
                    }

                    var m = new MeshWrapper(manager.assetsFileList, mesh, false);
                    meshList.Add(m);
                }
            }

            var result = CompositeMesh.FromMeshes(meshList.ToArray());

            return result;
        }

        [NotNull, ItemNotNull]
        public static IReadOnlyList<BoneNode> BuildBoneHierachy([NotNull] PrettyAvatar avatar, [NotNull] PrettyMesh mesh) {
            var boneList = new List<BoneNode>();

            for (var i = 0; i < avatar.AvatarSkeleton.Nodes.Length; i++) {
                var n = avatar.AvatarSkeleton.Nodes[i];

                var parent = n.ParentIndex >= 0 ? boneList[n.ParentIndex] : null;
                var boneId = avatar.AvatarSkeleton.NodeIDs[i];
                var bonePath = avatar.BoneNamesMap[boneId];

                var boneIndex = avatar.AvatarSkeleton.NodeIDs.FindIndex(boneId);

                if (boneIndex < 0) {
                    throw new IndexOutOfRangeException();
                }

                var initialPose = avatar.AvatarSkeletonPose.Transforms[boneIndex];

                var bone = new BoneNode(parent, i, bonePath, initialPose.Translation.ToOpenTK(), initialPose.Rotation.ToOpenTK());

                boneList.Add(bone);
            }

            foreach (var bone in boneList) {
                bone.Initialize();
            }

#if DEBUG
            Debug.Print("Model bones:");

            for (var i = 0; i < boneList.Count; i++) {
                var bone = boneList[i];
                Debug.Print("[{0}]: {1}", i, bone.ToString());
            }
#endif

            return boneList;
        }

        [CanBeNull]
        public static PrettyAvatar LoadBodyAvatar() {
            AvatarWrapper result = null;

            var manager = new AssetsManager();
            manager.LoadFiles(BodyModelFilePath);

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.Avatar) {
                        continue;
                    }

                    var avatar = obj as Avatar;

                    if (avatar == null) {
                        throw new ArgumentNullException(nameof(avatar), "Body avatar is null.");
                    }

                    result = new AvatarWrapper(avatar);

                    break;
                }
            }

            return result;
        }

        [CanBeNull]
        public static PrettyAvatar LoadHeadAvatar() {
            AvatarWrapper result = null;

            var manager = new AssetsManager();
            manager.LoadFiles(HeadModelFilePath);

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.Avatar) {
                        continue;
                    }

                    var avatar = obj as Avatar;

                    if (avatar == null) {
                        throw new ArgumentNullException(nameof(avatar), "Head avatar is null.");
                    }

                    result = new AvatarWrapper(avatar);

                    break;
                }
            }

            return result;
        }

        public static (CharacterImasMotionAsset, CharacterImasMotionAsset, CharacterImasMotionAsset) LoadDance() {
            CharacterImasMotionAsset dan = null, apa = null, apg = null;

            const string danComp = DancerPosition + "_dan";
            const string apaComp = DancerPosition + "_apa";
            const string apgComp = DancerPosition + "_apg";

            var manager = new AssetsManager();
            manager.LoadFiles(DanceDataFilePath);

            var ser = new ScriptableObjectSerializer();

            foreach (var assetFile in manager.assetsFileList) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.MonoBehaviour) {
                        continue;
                    }

                    var behaviour = obj as MonoBehaviour;

                    if (behaviour == null) {
                        throw new ArgumentException("An object serialized as MonoBehaviour is actually not a MonoBehaviour.");
                    }

                    if (behaviour.m_Name.Contains(danComp)) {
                        dan = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                    } else if (behaviour.m_Name.EndsWith(apaComp)) {
                        apa = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                    } else if (behaviour.m_Name.EndsWith(apgComp)) {
                        apg = ser.Deserialize<CharacterImasMotionAsset>(behaviour);
                    }

                    if (dan != null && apa != null && apg != null) {
                        break;
                    }
                }
            }

            return (dan, apa, apg);
        }

        private const string BodyModelFilePath = "Resources/cb_ss001_015siz.unity3d";

        private const string HeadModelFilePath = "Resources/ch_ss001_015siz.unity3d";

        private const string SongResourceName = "hzwend";

        private const string DancerPosition = "01";

        private const string DanceDataFilePath = "Resources/dan_" + SongResourceName + "_" + DancerPosition + ".imo.unity3d";

    }
}
