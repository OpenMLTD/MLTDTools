using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class MeshWrapper : PrettyMesh {

        public MeshWrapper([NotNull] IReadOnlyList<SerializedFile> assetFiles, [NotNull] Mesh mesh, bool flipTexture, bool applyToon) {
            Name = mesh.m_Name;

            {
                var subMeshCount = mesh.m_SubMeshes.Length;
                var subMeshes = new SubMesh[subMeshCount];
                uint meshIndexStart = 0;

                for (var i = 0; i < mesh.m_SubMeshes.Length; i += 1) {
                    var subMesh = mesh.m_SubMeshes[i];
                    var mat = FindMaterialInfo(assetFiles, mesh, i, flipTexture, applyToon);
                    var sm = new SubMesh(meshIndexStart, subMesh, mat);
                    subMeshes[i] = sm;
                    meshIndexStart += subMesh.indexCount;
                }

                SubMeshes = subMeshes;
            }

            Indices = mesh.m_Indices.ToArray();

            {
                const int bonePerVertex = 4;

                var skinCount = mesh.m_Skin.Length;
                var skins = new BoneInfluence[skinCount][];

                for (var i = 0; i < skinCount; i += 1) {
                    var s = new BoneInfluence[bonePerVertex];

                    skins[i] = s;

                    var w = mesh.m_Skin[i];

                    for (var j = 0; j < bonePerVertex; j += 1) {
                        BoneInfluence influence;

                        if (w != null) {
                            influence = new BoneInfluence(w, j);
                        } else {
                            influence = null;
                        }

                        s[j] = influence;
                    }
                }

                Skin = skins;
            }

            BindPose = (Matrix4x4[])mesh.m_BindPose.Clone();
            VertexCount = mesh.m_VertexCount;

            Vertices = ReadVector3Array(mesh.m_Vertices, mesh.m_VertexCount);
            Normals = ReadVector3Array(mesh.m_Normals);

            if (mesh.m_Colors != null) {
                Colors = ReadVector4Array(mesh.m_Colors);
            }

            UV1 = ReadVector2Array(mesh.m_UV0);

            if (mesh.m_Tangents != null) {
                Tangents = ReadVector3Array(mesh.m_Tangents);
            }

            BoneNameHashes = (uint[])mesh.m_BoneNameHashes.Clone();

            if (mesh.m_Shapes != null) {
                Shape = new BlendShapeData(mesh.m_Shapes);
            }
        }

        public override string Name { get; }

        public override SubMesh[] SubMeshes { get; }

        public override uint[] Indices { get; }

        public override BoneInfluence[][] Skin { get; }

        public override Matrix4x4[] BindPose { get; }

        public override int VertexCount { get; }

        public override Vector3[] Vertices { get; }

        public override Vector3[] Normals { get; }

        public override Vector4[] Colors { get; }

        public override Vector2[] UV1 { get; }

        public override Vector3[] Tangents { get; }

        public override uint[] BoneNameHashes { get; }

        public override BlendShapeData Shape { get; }

        [NotNull]
        private static Vector2[] ReadVector2Array([NotNull] float[] array) {
            return ReadVector2Array(array, array.Length / 2);
        }

        [NotNull]
        private static Vector2[] ReadVector2Array([NotNull] float[] array, int count) {
            count = Math.Max(Math.Min(count, array.Length / 2), 0);

            var result = new Vector2[count];

            for (var i = 0; i < count; i += 1) {
                var index = i * 2;
                var x = array[index];
                var y = array[index + 1];

                result[i] = new Vector2(x, y);
            }

            return result;
        }

        [NotNull]
        private static Vector3[] ReadVector3Array([NotNull] float[] array) {
            return ReadVector3Array(array, array.Length / 3);
        }

        [NotNull]
        private static Vector3[] ReadVector3Array([NotNull] float[] array, int count) {
            count = Math.Max(Math.Min(count, array.Length / 3), 0);

            var result = new Vector3[count];

            for (var i = 0; i < count; i += 1) {
                var index = i * 3;
                var x = array[index];
                var y = array[index + 1];
                var z = array[index + 2];

                result[i] = new Vector3(x, y, z);
            }

            return result;
        }

        [NotNull]
        private static TexturedMaterial FindMaterialInfo([NotNull] IReadOnlyList<SerializedFile> assetFiles, [NotNull] Mesh mesh, int meshIndex, bool flipTexture, bool applyToon) {
            SkinnedMeshRenderer meshRenderer = null;

            foreach (var assetFile in assetFiles) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.SkinnedMeshRenderer) {
                        continue;
                    }

                    var renderer = obj as SkinnedMeshRenderer;

                    Debug.Assert(renderer != null);

                    if (renderer.m_Mesh.m_PathID == mesh.m_PathID) {
                        meshRenderer = renderer;
                        break;
                    }
                }
            }

            if (meshRenderer == null) {
                throw new KeyNotFoundException($"Found no SkinnedMeshRenderer associated with this mesh ({mesh.m_Name}).");
            }

            Debug.Assert(meshRenderer.m_Materials != null);

            if (meshRenderer.m_Materials.Length <= meshIndex) {
                throw new FormatException("No corresponding material is associated with this SkinnedMeshRenderer.");
            }

            var materialPtr = meshRenderer.m_Materials[meshIndex];
            Material material = null;

            foreach (var assetFile in assetFiles) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.Material) {
                        continue;
                    }

                    var mat = obj as Material;

                    Debug.Assert(mat != null);

                    if (mat.m_PathID == materialPtr.m_PathID) {
                        material = mat;
                        break;
                    }
                }
            }

            if (material == null) {
                throw new KeyNotFoundException("Main material is not found by path ID.");
            }

            Debug.Assert(material.m_SavedProperties != null);
            Debug.Assert(material.m_SavedProperties.m_TexEnvs != null);
            Debug.Assert(material.m_SavedProperties.m_TexEnvs.Length > 0);

            var kvPairs = material.m_SavedProperties.m_TexEnvs;
            UnityTexEnv mainTexEnv = null;
            UnityTexEnv subTexEnv = null;

            foreach (var kv in kvPairs) {
                if (kv.Key.Equals("_MainTex", StringComparison.Ordinal)) {
                    mainTexEnv = kv.Value;
                } else if (kv.Key.Equals("_SubTex", StringComparison.Ordinal)) {
                    subTexEnv = kv.Value;
                }
            }

            if (mainTexEnv == null) {
                throw new KeyNotFoundException("Main texture is missing.");
            }

            Texture2D mainTexture = null;

            foreach (var assetFile in assetFiles) {
                foreach (var obj in assetFile.Objects) {
                    if (obj.type != ClassIDType.Texture2D) {
                        continue;
                    }

                    var tex = obj as Texture2D;

                    Debug.Assert(tex != null);

                    if (tex.m_PathID == mainTexEnv.m_Texture.m_PathID) {
                        mainTexture = tex;
                        break;
                    }
                }
            }

            if (mainTexture == null) {
                throw new KeyNotFoundException("Main texture is not found by path ID.");
            }

            Texture2D subTexture = null;

            if (subTexEnv != null) {
                foreach (var assetFile in assetFiles) {
                    foreach (var obj in assetFile.Objects) {
                        if (obj.type != ClassIDType.Texture2D) {
                            continue;
                        }

                        var tex = obj as Texture2D;

                        Debug.Assert(tex != null);

                        if (tex.m_PathID == subTexEnv.m_Texture.m_PathID) {
                            subTexture = tex;
                            break;
                        }
                    }
                }

                if (subTexture == null) {
                    throw new KeyNotFoundException("Sub texture is not found by path ID.");
                }
            }

            return new TexturedMaterial(material.m_Name, mainTexture, subTexture, flipTexture, applyToon);
        }

        [NotNull]
        private static Vector4[] ReadVector4Array([NotNull] float[] array) {
            return ReadVector4Array(array, array.Length / 4);
        }

        [NotNull]
        private static Vector4[] ReadVector4Array([NotNull] float[] array, int count) {
            count = Math.Max(Math.Min(count, array.Length / 4), 0);

            var result = new Vector4[count];

            for (var i = 0; i < count; i += 1) {
                var index = i * 4;
                var x = array[index];
                var y = array[index + 1];
                var z = array[index + 2];
                var w = array[index + 3];

                result[i] = new Vector4(x, y, z, w);
            }

            return result;
        }

    }
}
