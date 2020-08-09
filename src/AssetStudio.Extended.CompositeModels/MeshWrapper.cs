using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class MeshWrapper : PrettyMesh {

        public MeshWrapper([NotNull] SerializedObjectsLookup serializedObjectsLookup, [NotNull] Mesh mesh, [NotNull] TexturedMaterialExtraProperties extraMaterialProperties) {
            Name = mesh.m_Name;

            {
                var subMeshCount = mesh.m_SubMeshes.Length;
                var subMeshes = new PrettySubMesh[subMeshCount];
                uint meshIndexStart = 0;

                for (var i = 0; i < mesh.m_SubMeshes.Length; i += 1) {
                    var subMesh = mesh.m_SubMeshes[i];
                    var material = FindMaterialInfo(serializedObjectsLookup, mesh, i, extraMaterialProperties);
                    var sm = new PrettySubMesh(meshIndexStart, subMesh, material);
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

            if (mesh.m_UV1 != null) {
                Debug.Assert(mesh.m_UV0.Length == mesh.m_UV1.Length, "mesh.m_UV0.Length == mesh.m_UV1.Length");
            }

            UV2 = ReadNullableVector2Array(mesh.m_UV1, mesh.m_UV0.Length / 2);

            if (mesh.m_Tangents != null) {
                Tangents = ReadVector3Array(mesh.m_Tangents);
            }

            BoneNameHashes = (uint[])mesh.m_BoneNameHashes.Clone();

            if (mesh.m_Shapes != null) {
                Shape = new BlendShapeData(mesh.m_Shapes);
            }
        }

        public override string Name { get; }

        public override PrettySubMesh[] SubMeshes { get; }

        public override uint[] Indices { get; }

        public override BoneInfluence[][] Skin { get; }

        public override Matrix4x4[] BindPose { get; }

        public override int VertexCount { get; }

        public override Vector3[] Vertices { get; }

        public override Vector3[] Normals { get; }

        public override Vector4[] Colors { get; }

        public override Vector2[] UV1 { get; }

        public override Vector2?[] UV2 { get; }

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

        [NotNull, ItemCanBeNull]
        private static Vector2?[] ReadNullableVector2Array([CanBeNull] float[] array, int count) {
            if (array != null) {
                count = Math.Max(Math.Min(count, array.Length / 2), 0);
            } else {
                count = Math.Max(count, 0);
            }

            var result = new Vector2?[count];

            if (array != null) {
                for (var i = 0; i < count; i += 1) {
                    var index = i * 2;
                    var x = array[index];
                    var y = array[index + 1];

                    result[i] = new Vector2(x, y);
                }
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
        private static TexturedMaterial FindMaterialInfo([NotNull] SerializedObjectsLookup serializedObjectsLookup, [NotNull] Mesh mesh, int meshIndex, [NotNull] TexturedMaterialExtraProperties extraProperties) {
            var meshRenderer = serializedObjectsLookup.Find<SkinnedMeshRenderer>(renderer => renderer.m_Mesh.m_PathID == mesh.m_PathID);

            if (meshRenderer == null) {
                throw new KeyNotFoundException($"Found no SkinnedMeshRenderer associated with this mesh ({mesh.m_Name}).");
            }

            Debug.Assert(meshRenderer.m_Materials != null);

            if (meshRenderer.m_Materials.Length <= meshIndex) {
                throw new FormatException("No corresponding material is associated with this SkinnedMeshRenderer.");
            }

            var materialPtr = meshRenderer.m_Materials[meshIndex];
            serializedObjectsLookup.TryGet(materialPtr, out var material);

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

                if (mainTexEnv != null && subTexEnv != null) {
                    break;
                }
            }

            var hasMainTexture = mainTexEnv != null;
            var hasSubTexture = subTexEnv != null;

            Texture2D mainTexture = null;

            if (hasMainTexture) {
                serializedObjectsLookup.TryGet(mainTexEnv.m_Texture, out mainTexture);

                if (mainTexture == null) {
                    var pptrStr = mainTexEnv.m_Texture.GetDebugDescription();
                    Trace.WriteLine($"Warning: Main texture is not found by path ID, use default \"white\" instead. {pptrStr}");
                }
            }

            Texture2D subTexture = null;

            if (hasSubTexture) {
                serializedObjectsLookup.TryGet(subTexEnv.m_Texture, out subTexture);

                if (subTexture == null) {
                    var pptrStr = subTexEnv.m_Texture.GetDebugDescription();
                    Trace.WriteLine($"Warning: Sub texture is not found by path ID, use default \"white\" instead. {pptrStr}");
                }
            }

            return new TexturedMaterial(material.m_Name, hasMainTexture, mainTexture, hasSubTexture, subTexture, extraProperties);
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
