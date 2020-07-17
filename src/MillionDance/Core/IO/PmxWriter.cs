using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Pmx;
using OpenMLTD.MillionDance.Entities.Pmx.Extensions;
using OpenMLTD.MillionDance.Extensions;
using OpenTK;

namespace OpenMLTD.MillionDance.Core.IO {
    internal sealed class PmxWriter : DisposableBase {

        public PmxWriter([NotNull] Stream stream) {
            _writer = new BinaryWriter(stream);
        }

        public void Write([NotNull] PmxModel model) {
            EnsureNotDisposed();

            WriteHeader();
            WriteElementFormats(model);

            WritePmxModel(model);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                _writer.Dispose();
            }

            base.Dispose(disposing);
        }

        private PmxFormatVersion MajorVersion { get; } = PmxFormatVersion.Version2;

        private float DetailedVersion { get; } = 2.0f;

        private PmxStringEncoding StringEncoding { get; } = PmxStringEncoding.Utf16;

        private int VertexElementSize { get; } = 4;

        private int BoneElementSize { get; } = 4;

        private int MorphElementSize { get; } = 4;

        private int MaterialElementSize { get; } = 4;

        private int RigidBodyElementSize { get; } = 4;

        private int TexElementSize { get; } = 4;

        private void WritePmxModel([NotNull] PmxModel model) {
            var textureNames = BuildTextureNameMap(model);

            WriteString(model.Name);
            WriteString(model.NameEnglish);
            WriteString(model.Comment);
            WriteString(model.CommentEnglish);

            WriteVertexInfo(model);
            WriteFaceInfo(model);
            WriteTextureInfo(textureNames);
            WriteMaterialInfo(model, textureNames);
            WriteBoneInfo(model);
            WriteMorphInfo(model);
            WriteNodeInfo(model);
            WriteRigidBodyInfo(model);
            WriteJointInfo(model);
            WriteSoftBodyInfo(model);
        }

        private void WriteVertexInfo([NotNull] PmxModel model) {
            _writer.Write(model.Vertices.Length);

            foreach (var vertex in model.Vertices) {
                WritePmxVertex(model, vertex);
            }
        }

        private void WriteFaceInfo([NotNull] PmxModel model) {
            _writer.Write(model.FaceTriangles.Length);

            foreach (var v in model.FaceTriangles) {
                _writer.WriteInt32AsVarLenInt(v, VertexElementSize, true);
            }
        }

        private void WriteTextureInfo([NotNull, ItemNotNull] string[] textureNames) {
            _writer.Write(textureNames.Length);

            foreach (var textureName in textureNames) {
                WriteString(textureName);
            }
        }

        private void WriteMaterialInfo([NotNull] PmxModel model, [NotNull, ItemNotNull] string[] textureNames) {
            _writer.Write(model.Materials.Length);

            foreach (var material in model.Materials) {
                WritePmxMaterial(material, textureNames);
            }
        }

        private void WriteBoneInfo([NotNull] PmxModel model) {
            _writer.Write(model.Bones.Length);

            foreach (var bone in model.Bones) {
                WritePmxBone(bone);
            }
        }

        private void WriteMorphInfo([NotNull] PmxModel model) {
            _writer.Write(model.Morphs.Length);

            foreach (var morph in model.Morphs) {
                WritePmxMorph(morph);
            }
        }

        private void WriteNodeInfo([NotNull] PmxModel model) {
            _writer.Write(model.Nodes.Length);

            foreach (var node in model.Nodes) {
                WritePmxNode(node);
            }
        }

        private void WriteRigidBodyInfo([NotNull] PmxModel model) {
            _writer.Write(model.RigidBodies.Length);

            foreach (var body in model.RigidBodies) {
                WritePmxRigidBody(body);
            }
        }

        private void WriteJointInfo([NotNull] PmxModel model) {
            _writer.Write(model.Joints.Length);

            foreach (var joint in model.Joints) {
                WritePmxJoint(joint);
            }
        }

        private void WriteSoftBodyInfo([NotNull] PmxModel model) {
            if (DetailedVersion < 2.1f) {
                return;
            }

            if (model.SoftBodies != null) {
                _writer.Write(model.SoftBodies.Length);

                foreach (var body in model.SoftBodies) {
                    WritePmxSoftBody(body);
                }
            } else {
                _writer.Write(0);
            }
        }

        private void WritePmxVertex([NotNull] PmxModel model, [NotNull] PmxVertex vertex) {
            _writer.Write(vertex.Position);
            _writer.Write(vertex.Normal);
            _writer.Write(vertex.UV);

            for (var i = 0; i < model.UvaCount && i < PmxVertex.MaxUvaCount; ++i) {
                _writer.Write(vertex.Uva[i]);
            }

            _writer.Write((byte)vertex.Deformation);

            switch (vertex.Deformation) {
                case Deformation.Bdef1:
                    _writer.WriteInt32AsVarLenInt(vertex.BoneWeights[0].BoneIndex, BoneElementSize);
                    break;
                case Deformation.Bdef2:
                    _writer.WriteInt32AsVarLenInt(vertex.BoneWeights[0].BoneIndex, BoneElementSize);
                    _writer.WriteInt32AsVarLenInt(vertex.BoneWeights[1].BoneIndex, BoneElementSize);
                    _writer.Write(vertex.BoneWeights[0].Weight);
                    break;
                case Deformation.Bdef4:
                case Deformation.Qdef:
                    _writer.WriteInt32AsVarLenInt(vertex.BoneWeights[0].BoneIndex, BoneElementSize);
                    _writer.WriteInt32AsVarLenInt(vertex.BoneWeights[1].BoneIndex, BoneElementSize);
                    _writer.WriteInt32AsVarLenInt(vertex.BoneWeights[2].BoneIndex, BoneElementSize);
                    _writer.WriteInt32AsVarLenInt(vertex.BoneWeights[3].BoneIndex, BoneElementSize);
                    _writer.Write(vertex.BoneWeights[0].Weight);
                    _writer.Write(vertex.BoneWeights[1].Weight);
                    _writer.Write(vertex.BoneWeights[2].Weight);
                    _writer.Write(vertex.BoneWeights[3].Weight);
                    break;
                case Deformation.Sdef: {
                    _writer.WriteInt32AsVarLenInt(vertex.BoneWeights[0].BoneIndex, BoneElementSize);
                    _writer.WriteInt32AsVarLenInt(vertex.BoneWeights[1].BoneIndex, BoneElementSize);
                    _writer.Write(vertex.BoneWeights[0].Weight);

                    _writer.Write(vertex.C0);
                    _writer.Write(vertex.R0);
                    _writer.Write(vertex.R1);

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _writer.Write(vertex.EdgeScale);
        }

        private void WritePmxMaterial([NotNull] PmxMaterial material, [NotNull, ItemNotNull] string[] textureNames) {
            WriteString(material.Name);
            WriteString(material.NameEnglish);

            _writer.Write(material.Diffuse);
            _writer.Write(material.Specular);
            _writer.Write(material.SpecularPower);
            _writer.Write(material.Ambient);
            _writer.Write((byte)material.Flags);
            _writer.Write(material.EdgeColor);
            _writer.Write(material.EdgeSize);

            var texNameIndex = GetTextureIndex(textureNames, material.TextureFileName);
            _writer.WriteInt32AsVarLenInt(texNameIndex, TexElementSize);
            var sphereTexNameIndex = GetTextureIndex(textureNames, material.SphereTextureFileName);
            _writer.WriteInt32AsVarLenInt(sphereTexNameIndex, TexElementSize);
            _writer.Write((byte)material.SphereTextureMode);

            var mappedToonTexture = !IsNormalToonTexture(material.ToonTextureFileName, out var toon);

            _writer.Write(!mappedToonTexture);

            if (mappedToonTexture) {
                var toonTexNameIndex = GetTextureIndex(textureNames, material.ToonTextureFileName);
                _writer.WriteInt32AsVarLenInt(toonTexNameIndex, TexElementSize);
            } else {
                _writer.Write((byte)toon);
            }

            WriteString(material.MemoTextureFileName);
            _writer.Write(material.AppliedFaceVertexCount);
        }

        private void WritePmxBone([NotNull] PmxBone bone) {
            WriteString(bone.Name);
            WriteString(bone.NameEnglish);

            _writer.Write(bone.InitialPosition);
            _writer.WriteInt32AsVarLenInt(bone.ParentIndex, BoneElementSize);
            _writer.Write(bone.Level);
            _writer.Write((ushort)bone.Flags);

            if (bone.HasFlag(BoneFlags.ToBone)) {
                _writer.WriteInt32AsVarLenInt(bone.To_Bone, BoneElementSize);
            } else {
                _writer.Write(bone.To_Offset);
            }

            if (bone.HasFlag(BoneFlags.AppendRotation) || bone.HasFlag(BoneFlags.AppendTranslation)) {
                _writer.WriteInt32AsVarLenInt(bone.AppendParentIndex, BoneElementSize);
                _writer.Write(bone.AppendRatio);
            }

            if (bone.HasFlag(BoneFlags.FixedAxis)) {
                _writer.Write(bone.Axis);
            }

            if (bone.HasFlag(BoneFlags.LocalFrame)) {
                var rotation = bone.InitialRotation;
                var mat = Matrix4.CreateFromQuaternion(rotation);

                var localX = mat.Row0.Xyz;
                var localZ = mat.Row2.Xyz;

                localX.Normalize();
                localZ.Normalize();

                _writer.Write(localX);
                _writer.Write(localZ);
            }

            if (bone.HasFlag(BoneFlags.ExternalParent)) {
                _writer.Write(bone.ExternalParentIndex);
            }

            if (bone.HasFlag(BoneFlags.IK) && bone.IK != null) {
                WritePmxIK(bone.IK);
            }
        }

        // ReSharper disable once InconsistentNaming
        private void WritePmxIK([NotNull] PmxIK ik) {
            _writer.WriteInt32AsVarLenInt(ik.TargetBoneIndex, BoneElementSize);
            _writer.Write(ik.LoopCount);
            _writer.Write(ik.AngleLimit);

            _writer.Write(ik.Links.Length);

            foreach (var link in ik.Links) {
                WriteIKLink(link);
            }
        }

        // ReSharper disable once InconsistentNaming
        private void WriteIKLink([NotNull] IKLink link) {
            _writer.WriteInt32AsVarLenInt(link.BoneIndex, BoneElementSize);
            _writer.Write(link.IsLimited);

            if (link.IsLimited) {
                _writer.Write(link.LowerBound);
                _writer.Write(link.UpperBound);
            }
        }

        private void WritePmxMorph([NotNull] PmxMorph morph) {
            WriteString(morph.Name);
            WriteString(morph.NameEnglish);

            _writer.Write((sbyte)morph.Panel);
            _writer.Write((byte)morph.OffsetKind);

            _writer.Write(morph.Offsets.Length);

            foreach (var subMorph in morph.Offsets) {
                switch (subMorph) {
                    case PmxGroupMorph m0:
                        WritePmxGroupMorph(m0);
                        break;
                    case PmxVertexMorph m1:
                        WritePmxVertexMorph(m1);
                        break;
                    case PmxBoneMorph m2:
                        WritePmxBoneMorph(m2);
                        break;
                    case PmxUVMorph m3:
                        WritePmxUVMorph(m3);
                        break;
                    case PmxMaterialMorph m4:
                        WritePmxMaterialMorph(m4);
                        break;
                    case PmxImpulseMorph m5:
                        WritePmxImpulseMorph(m5);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void WritePmxGroupMorph([NotNull] PmxGroupMorph morph) {
            _writer.WriteInt32AsVarLenInt(morph.Index, MorphElementSize);
            _writer.Write(morph.Ratio);
        }

        private void WritePmxVertexMorph([NotNull] PmxVertexMorph morph) {
            _writer.WriteInt32AsVarLenInt(morph.Index, MorphElementSize);
            _writer.Write(morph.Offset);
        }

        private void WritePmxBoneMorph([NotNull] PmxBoneMorph morph) {
            _writer.WriteInt32AsVarLenInt(morph.Index, MorphElementSize);
            _writer.Write(morph.Translation);
            _writer.Write(morph.Rotation);
        }

        // ReSharper disable once InconsistentNaming
        private void WritePmxUVMorph([NotNull] PmxUVMorph morph) {
            _writer.WriteInt32AsVarLenInt(morph.Index, MorphElementSize);
            _writer.Write(morph.Offset);
        }

        private void WritePmxMaterialMorph([NotNull] PmxMaterialMorph morph) {
            _writer.WriteInt32AsVarLenInt(morph.Index, MorphElementSize);
            _writer.Write((byte)morph.Op);
            _writer.Write(morph.Diffuse);
            _writer.Write(morph.Specular);
            _writer.Write(morph.SpecularPower);
            _writer.Write(morph.Ambient);
            _writer.Write(morph.EdgeColor);
            _writer.Write(morph.EdgeSize);
            _writer.Write(morph.Texture);
            _writer.Write(morph.Sphere);
            _writer.Write(morph.Toon);
        }

        private void WritePmxImpulseMorph([NotNull] PmxImpulseMorph morph) {
            _writer.WriteInt32AsVarLenInt(morph.Index, MorphElementSize);
            _writer.Write(morph.IsLocal);
            _writer.Write(morph.Velocity);
            _writer.Write(morph.Torque);
        }

        private void WritePmxNode([NotNull] PmxNode node) {
            WriteString(node.Name);
            WriteString(node.NameEnglish);

            _writer.Write(node.IsSystemNode);

            _writer.Write(node.Elements.Length);

            foreach (var element in node.Elements) {
                WriteNodeElement(element);
            }
        }

        private void WriteNodeElement([NotNull] NodeElement element) {
            _writer.Write((byte)element.ElementType);

            switch (element.ElementType) {
                case ElementType.Bone:
                    _writer.WriteInt32AsVarLenInt(element.Index, BoneElementSize);
                    break;
                case ElementType.Morph:
                    _writer.WriteInt32AsVarLenInt(element.Index, MorphElementSize);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void WritePmxRigidBody([NotNull] PmxRigidBody body) {
            WriteString(body.Name);
            WriteString(body.NameEnglish);

            _writer.WriteInt32AsVarLenInt(body.BoneIndex, BoneElementSize);
            _writer.Write((byte)body.GroupIndex);

            _writer.Write(body.PassGroup.ToFlagBits());

            _writer.Write((byte)body.BoundingBoxKind);
            _writer.Write(body.BoundingBoxSize);
            _writer.Write(body.Position);
            _writer.Write(body.RotationAngles);
            _writer.Write(body.Mass);
            _writer.Write(body.PositionDamping);
            _writer.Write(body.RotationDamping);
            _writer.Write(body.Restitution);
            _writer.Write(body.Friction);
            _writer.Write((byte)body.KineticMode);
        }

        private void WritePmxJoint([NotNull] PmxJoint joint) {
            WriteString(joint.Name);
            WriteString(joint.NameEnglish);

            _writer.Write((byte)joint.Kind);
            _writer.WriteInt32AsVarLenInt(joint.BodyIndex1, RigidBodyElementSize);
            _writer.WriteInt32AsVarLenInt(joint.BodyIndex2, RigidBodyElementSize);
            _writer.Write(joint.Position);
            _writer.Write(joint.Rotation);
            _writer.Write(joint.LowerTranslationLimit);
            _writer.Write(joint.UpperTranslationLimit);
            _writer.Write(joint.LowerRotationLimit);
            _writer.Write(joint.UpperRotationLimit);
            _writer.Write(joint.TranslationSpringConstants);
            _writer.Write(joint.RotationSpringConstants);
        }

        private void WritePmxSoftBody([NotNull] PmxSoftBody body) {
            WriteString(body.Name);
            WriteString(body.NameEnglish);

            _writer.Write((byte)body.Shape);
            _writer.WriteInt32AsVarLenInt(body.MaterialIndex, MaterialElementSize);
            _writer.Write((byte)body.GroupIndex);

            _writer.Write(body.PassGroup.ToFlagBits());

            _writer.Write((byte)body.Flags);
            _writer.Write(body.BendingLinkDistance);
            _writer.Write(body.ClusterCount);
            _writer.Write(body.TotalMass);
            _writer.Write(body.Margin);

            var config = body.Config;

            _writer.Write(config.AeroModel);
            _writer.Write(config.VCF);
            _writer.Write(config.DP);
            _writer.Write(config.DG);
            _writer.Write(config.LF);
            _writer.Write(config.PR);
            _writer.Write(config.VC);
            _writer.Write(config.DF);
            _writer.Write(config.MT);
            _writer.Write(config.CHR);
            _writer.Write(config.KHR);
            _writer.Write(config.SHR);
            _writer.Write(config.AHR);
            _writer.Write(config.SRHR_CL);
            _writer.Write(config.SKHR_CL);
            _writer.Write(config.SSHR_CL);
            _writer.Write(config.SR_SPLT_CL);
            _writer.Write(config.SK_SPLT_CL);
            _writer.Write(config.SS_SPLT_CL);
            _writer.Write(config.V_IT);
            _writer.Write(config.P_IT);
            _writer.Write(config.D_IT);
            _writer.Write(config.C_IT);

            var matCfg = body.MaterialConfig;

            _writer.Write(matCfg.LST);
            _writer.Write(matCfg.AST);
            _writer.Write(matCfg.VST);

            _writer.Write(body.BodyAnchors.Length);

            foreach (var anchor in body.BodyAnchors) {
                WriteBodyAnchor(anchor);
            }

            _writer.Write(body.VertexPins.Length);

            foreach (var pin in body.VertexPins) {
                WriteVertexPin(pin);
            }
        }

        private void WriteBodyAnchor([NotNull] BodyAnchor anchor) {
            _writer.WriteInt32AsVarLenInt(anchor.BodyIndex, RigidBodyElementSize);
            _writer.WriteInt32AsVarLenInt(anchor.VertexIndex, VertexElementSize, true);
            _writer.Write(anchor.IsNear);
        }

        private void WriteVertexPin([NotNull] VertexPin pin) {
            _writer.WriteInt32AsVarLenInt(pin.VertexIndex, VertexElementSize, true);
        }

        private void WriteHeader() {
            byte[] magicBytes;

            switch (MajorVersion) {
                case PmxFormatVersion.Version1:
                    magicBytes = PmxSignatureV1;
                    break;
                case PmxFormatVersion.Version2:
                    magicBytes = PmxSignatureV2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _writer.Write(magicBytes);
            _writer.Write(DetailedVersion);
        }

        private void WriteElementFormats([NotNull] PmxModel model) {
            byte[] elementSizes;
            byte elementSizeEntryCount;

            switch (MajorVersion) {
                case PmxFormatVersion.Version1:
                    elementSizeEntryCount = 5;
                    elementSizes = new byte[elementSizeEntryCount];
                    elementSizes[0] = (byte)VertexElementSize;
                    elementSizes[1] = (byte)BoneElementSize;
                    elementSizes[2] = (byte)MorphElementSize;
                    elementSizes[3] = (byte)MaterialElementSize;
                    elementSizes[4] = (byte)RigidBodyElementSize;
                    break;
                case PmxFormatVersion.Version2:
                    elementSizeEntryCount = 8;
                    elementSizes = new byte[elementSizeEntryCount];
                    elementSizes[0] = (byte)StringEncoding;
                    elementSizes[1] = (byte)model.UvaCount;
                    elementSizes[2] = (byte)VertexElementSize;
                    elementSizes[3] = (byte)TexElementSize;
                    elementSizes[4] = (byte)MaterialElementSize;
                    elementSizes[5] = (byte)BoneElementSize;
                    elementSizes[6] = (byte)MorphElementSize;
                    elementSizes[7] = (byte)RigidBodyElementSize;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _writer.Write(elementSizeEntryCount);
            _writer.Write(elementSizes);
        }

        private void WriteString([CanBeNull] string str) {
            if (string.IsNullOrEmpty(str)) {
                _writer.Write(0);
                return;
            }

            Encoding enc;

            switch (MajorVersion) {
                case PmxFormatVersion.Version1:
                    enc = Utf8NoBom;
                    break;
                case PmxFormatVersion.Version2:
                    switch (StringEncoding) {
                        case PmxStringEncoding.Utf16:
                            enc = Utf16NoBom;
                            break;
                        case PmxStringEncoding.Utf8:
                            enc = Utf8NoBom;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var bytes = enc.GetBytes(str);

            _writer.Write(bytes.Length);
            _writer.Write(bytes);
        }

        [NotNull, ItemNotNull]
        private string[] BuildTextureNameMap([NotNull] PmxModel model) {
            var materials = model.Materials;

            var nameList = new List<string>();

            foreach (var material in materials) {
                if (!string.IsNullOrEmpty(material.TextureFileName)) {
                    nameList.Add(material.TextureFileName);
                }

                if (!string.IsNullOrEmpty(material.SphereTextureFileName)) {
                    nameList.Add(material.SphereTextureFileName);
                }

                if (!string.IsNullOrEmpty(material.ToonTextureFileName) && !ToonNameRegex.IsMatch(material.ToonTextureFileName)) {
                    nameList.Add(material.ToonTextureFileName);
                }
            }

            return nameList.Distinct().ToArray();
        }

        private static bool IsNormalToonTexture(string name, out int toonIndex) {
            if (string.IsNullOrEmpty(name)) {
                // Maps to "no texture" (empty string)
                toonIndex = -1;
                return false;
            }

            var match = ToonNameRegex.Match(name);

            if (!match.Success) {
                toonIndex = -1;
                return false;
            }

            var toonStr = match.Groups["toonIndex"].Value;
            toonIndex = Convert.ToInt32(toonStr);
            // From 1-based to 0-based
            toonIndex -= 1;

            return toonIndex > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetTextureIndex([NotNull, ItemNotNull] string[] textureNames, [NotNull] string s) {
            return textureNames.IndexOf(s);
        }

        private enum PmxFormatVersion {

            Unknown = 0,

            Version1 = 1,

            Version2 = 2

        }

        private enum PmxStringEncoding {

            Utf16 = 0,

            Utf8 = 1

        }

        private static readonly byte[] PmxSignatureV1 = { 0x50, 0x6d, 0x78, 0x20 }; // "Pmx "

        private static readonly byte[] PmxSignatureV2 = { 0x50, 0x4d, 0x58, 0x20 }; // "PMX "

        private static readonly Encoding Utf16NoBom = new UnicodeEncoding(false, false);

        private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

        private static readonly Regex ToonNameRegex = new Regex(@"^toon(?<toonIndex>\d+)\.bmp$");

        [NotNull]
        private readonly BinaryWriter _writer;

    }
}
