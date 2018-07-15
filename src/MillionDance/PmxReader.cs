using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using MillionDance.Entities.Pmx;
using MillionDance.Entities.Pmx.Extensions;
using MillionDance.Extensions;
using OpenTK;

namespace MillionDance {
    // https://github.com/anydream/Pmx2Fbx/blob/master/PmxLib/PmxReader.h
    // https://github.com/benikabocha/saba/tree/master/src/Saba/Model/MMD
    // https://gist.github.com/felixjones/f8a06bd48f9da9a4539f
    // https://github.com/oguna/MMDFormats/blob/master/MikuMikuFormats/Pmx.cpp
    internal sealed class PmxReader {

        // ReSharper disable once NotNullMemberIsNotInitialized
        private PmxReader([NotNull] BinaryReader binaryReader) {
            _reader = binaryReader;
        }

        public static PmxModel ReadModel([NotNull] Stream stream) {
            PmxModel model;

            using (var binaryReader = new BinaryReader(stream)) {
                var pmxReader = new PmxReader(binaryReader);

                pmxReader.ReadHeader();
                pmxReader.ReadElementFormats();

                model = pmxReader.ReadPmxModel();
            }

            return model;
        }

        #region File states
        private PmxFormatVersion MajorVersion { get; set; }

        private float DetailedVersion { get; set; }

        private PmxStringEncoding StringEncoding { get; set; }

        private int UvaCount { get; set; }

        private int VertexElementSize { get; set; }

        private int BoneElementSize { get; set; }

        private int MorphElementSize { get; set; }

        private int MaterialElementSize { get; set; }

        private int RigidBodyElementSize { get; set; }

        private int TexElementSize { get; set; }

        [NotNull]
        private IReadOnlyDictionary<int, string> TextureNameMap { get; set; }

        [NotNull]
        private IReadOnlyDictionary<string, int> TextureIndexLookup { get; set; }
        #endregion

        private PmxModel ReadPmxModel() {
            var model = new PmxModel();

            model.Name = ReadString() ?? string.Empty;
            model.NameEnglish = ReadString() ?? string.Empty;
            model.Comment = ReadString() ?? string.Empty;
            model.CommentEnglish = ReadString() ?? string.Empty;

            ReadVertexInfo();
            ReadFaceInfo();
            ReadTextureInfo();
            ReadMaterialInfo();
            ReadBoneInfo();
            ReadMorphInfo();
            ReadNodeInfo();
            ReadRigidBodyInfo();
            ReadJointInfo();
            ReadSoftBodyInfo();

            return model;

            void ReadVertexInfo() {
                var vertexCount = _reader.ReadInt32();
                var vertices = new PmxVertex[vertexCount];

                for (var i = 0; i < vertexCount; ++i) {
                    vertices[i] = ReadPmxVertex();
                }

                model.Vertices = vertices;
            }

            void ReadFaceInfo() {
                var faceCount = _reader.ReadInt32();
                var faceIndices = new int[faceCount];

                for (var i = 0; i < faceCount; ++i) {
                    faceIndices[i] = _reader.ReadVarLenIntAsInt32(VertexElementSize, true);
                }

                model.FaceTriangles = faceIndices;
            }

            void ReadTextureInfo() {
                var textureCount = _reader.ReadInt32();
                var textureNameMap = new Dictionary<int, string>();
                var textureIndexLookup = new Dictionary<string, int>();

                for (var i = 0; i < textureCount; ++i) {
                    var textureName = ReadString() ?? string.Empty;
                    textureNameMap[i] = textureName;
                    textureIndexLookup[textureName] = i;
                }

                textureNameMap[-1] = string.Empty;

                TextureNameMap = textureNameMap;
                TextureIndexLookup = textureIndexLookup;
            }

            void ReadMaterialInfo() {
                var materialCount = _reader.ReadInt32();
                var materials = new PmxMaterial[materialCount];

                for (var i = 0; i < materialCount; ++i) {
                    materials[i] = ReadPmxMaterial();
                }

                model.Materials = materials;
            }

            void ReadBoneInfo() {
                var boneCount = _reader.ReadInt32();
                var bones = new PmxBone[boneCount];

                for (var i = 0; i < boneCount; ++i) {
                    bones[i] = ReadPmxBone();
                    bones[i].BoneIndex = i;
                }

                model.Bones = bones;
                model.BonesDictionary = bones.ToDictionary(bone => bone.Name);

                var rootBoneIndexList = new List<int>();

                for (var i = 0; i < bones.Length; ++i) {
                    var bone = bones[i];

                    if (bone.ParentIndex < 0) {
                        rootBoneIndexList.Add(i);
                    } else {
                        bone.Parent = bones[bone.ParentIndex];
                    }

                    if (bone.AppendParentIndex >= 0) {
                        bone.AppendParent = bones[bone.AppendParentIndex];
                    }

                    if (bone.ExternalParentIndex >= 0) {
                        bone.ExternalParent = bones[bone.ExternalParentIndex];
                    }

                    if (bone.HasFlag(BoneFlags.IK)) {
                        var ik = bone.IK;

                        Debug.Assert(ik != null, nameof(ik) + " != null");

                        ik.TargetBone = bones[ik.TargetBoneIndex];

                        foreach (var link in ik.Links) {
                            if (link.BoneIndex >= 0) {
                                link.Bone = bones[link.BoneIndex];
                            }
                        }
                    }
                }

                model.RootBoneIndices = rootBoneIndexList.ToArray();

                foreach (var bone in bones) {
                    bone.SetToBindingPose();
                }
            }

            void ReadMorphInfo() {
                var morphCount = _reader.ReadInt32();
                var morphs = new PmxMorph[morphCount];

                for (var i = 0; i < morphCount; ++i) {
                    morphs[i] = ReadPmxMorph();
                }

                model.Morphs = morphs;
            }

            void ReadNodeInfo() {
                var nodeCount = _reader.ReadInt32();
                var nodes = new PmxNode[nodeCount];

                for (var i = 0; i < nodeCount; ++i) {
                    var node = ReadPmxNode();

                    nodes[i] = node;

                    if (node.IsSystemNode) {
                        if (node.Name == "Root") {
                            model.RootNodeIndex = i;
                        } else if (node.Name == "表情") {
                            model.FacialExpressionNodeIndex = i;
                        }
                    }
                }

                model.Nodes = nodes;
            }

            void ReadRigidBodyInfo() {
                var bodyCount = _reader.ReadInt32();
                var bodies = new PmxRigidBody[bodyCount];

                for (var i = 0; i < bodyCount; ++i) {
                    bodies[i] = ReadPmxRigidBody();
                }

                model.RigidBodies = bodies;
            }

            void ReadJointInfo() {
                var jointCount = _reader.ReadInt32();
                var joints = new PmxJoint[jointCount];

                for (var i = 0; i < jointCount; ++i) {
                    joints[i] = ReadPmxJoint();
                }

                model.Joints = joints;
            }

            void ReadSoftBodyInfo() {
                if (DetailedVersion < 2.1f) {
                    return;
                }

                var bodyCount = _reader.ReadInt32();
                var bodies = new PmxSoftBody[bodyCount];

                for (var i = 0; i < bodyCount; ++i) {
                    bodies[i] = ReadPmxSoftBody();
                }

                model.SoftBodies = bodies;
            }
        }

        #region Read structures
        private PmxVertex ReadPmxVertex() {
            var vertex = new PmxVertex();

            vertex.Position = _reader.ReadVector3();
            vertex.Normal = _reader.ReadVector3();
            vertex.UV = _reader.ReadVector2();

            for (var i = 0; i < UvaCount && i < PmxVertex.MaxUvaCount; ++i) {
                vertex.Uva[i] = _reader.ReadVector4();
            }

            vertex.Deformation = (Deformation)_reader.ReadByte();

            switch (vertex.Deformation) {
                case Deformation.Bdef1:
                    vertex.BoneWeights[0].BoneIndex = _reader.ReadVarLenIntAsInt32(BoneElementSize);
                    vertex.BoneWeights[0].Weight = 1;
                    break;
                case Deformation.Bdef2:
                    vertex.BoneWeights[0].BoneIndex = _reader.ReadVarLenIntAsInt32(BoneElementSize);
                    vertex.BoneWeights[1].BoneIndex = _reader.ReadVarLenIntAsInt32(BoneElementSize);
                    vertex.BoneWeights[0].Weight = _reader.ReadSingle();
                    vertex.BoneWeights[1].Weight = 1 - vertex.BoneWeights[0].Weight;
                    break;
                case Deformation.Bdef4:
                case Deformation.Qdef:
                    vertex.BoneWeights[0].BoneIndex = _reader.ReadVarLenIntAsInt32(BoneElementSize);
                    vertex.BoneWeights[1].BoneIndex = _reader.ReadVarLenIntAsInt32(BoneElementSize);
                    vertex.BoneWeights[2].BoneIndex = _reader.ReadVarLenIntAsInt32(BoneElementSize);
                    vertex.BoneWeights[3].BoneIndex = _reader.ReadVarLenIntAsInt32(BoneElementSize);
                    vertex.BoneWeights[0].Weight = _reader.ReadSingle();
                    vertex.BoneWeights[1].Weight = _reader.ReadSingle();
                    vertex.BoneWeights[2].Weight = _reader.ReadSingle();
                    vertex.BoneWeights[3].Weight = _reader.ReadSingle();
                    break;
                case Deformation.Sdef: {
                        vertex.BoneWeights[0].BoneIndex = _reader.ReadVarLenIntAsInt32(BoneElementSize);
                        vertex.BoneWeights[1].BoneIndex = _reader.ReadVarLenIntAsInt32(BoneElementSize);
                        vertex.BoneWeights[0].Weight = _reader.ReadSingle();
                        vertex.BoneWeights[1].Weight = 1 - vertex.BoneWeights[0].Weight;

                        vertex.C0 = _reader.ReadVector3();
                        vertex.R0 = _reader.ReadVector3();
                        vertex.R1 = _reader.ReadVector3();

                        var vec = vertex.R0 * vertex.BoneWeights[0].Weight + vertex.R1 * vertex.BoneWeights[1].Weight;

                        vertex.RW0 = vertex.R0 - vec;
                        vertex.RW1 = vertex.R1 - vec;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            vertex.EdgeScale = _reader.ReadSingle();

            return vertex;
        }

        private PmxMaterial ReadPmxMaterial() {
            var material = new PmxMaterial();

            material.Name = ReadString() ?? string.Empty;
            material.NameEnglish = ReadString() ?? string.Empty;

            material.Diffuse = _reader.ReadVector4();
            material.Specular = _reader.ReadVector3();
            material.SpecularPower = _reader.ReadSingle();
            material.Ambient = _reader.ReadVector3();
            material.Flags = (MaterialFlags)_reader.ReadByte();
            material.EdgeColor = _reader.ReadVector4();
            material.EdgeSize = _reader.ReadSingle();

            var texNameIndex = _reader.ReadVarLenIntAsInt32(TexElementSize);
            material.TextureFileName = TextureNameMap[texNameIndex];
            var sphereTexNameIndex = _reader.ReadVarLenIntAsInt32(TexElementSize);
            material.SphereTextureFileName = TextureNameMap[sphereTexNameIndex];
            material.SphereMode = (SphereMode)_reader.ReadByte();

            var mappedToonTexture = _reader.ReadByte() == 0;

            if (mappedToonTexture) {
                var toonTexNameIndex = _reader.ReadVarLenIntAsInt32(TexElementSize);
                material.ToonTextureFileName = TextureNameMap[toonTexNameIndex];
            } else {
                var toonTexIndex = (int)_reader.ReadByte();

                if (toonTexIndex < 0) {
                    toonTexIndex = -1;
                }

                toonTexIndex += 1;
                material.ToonTextureFileName = $"toon{toonTexIndex}.bmp";
            }

            material.MemoTextureFileName = ReadString() ?? string.Empty;
            material.AppliedFaceVertexCount = _reader.ReadInt32();

            return material;
        }

        private IKLink ReadIKLink() {
            var link = new IKLink();

            link.BoneIndex = _reader.ReadVarLenIntAsInt32(BoneElementSize);
            link.IsLimited = _reader.ReadBoolean();

            if (link.IsLimited) {
                link.LowerBound = _reader.ReadVector3();
                link.UpperBound = _reader.ReadVector3();
            }

            return link;
        }

        private PmxIK ReadPmxIK() {
            var ik = new PmxIK();

            ik.TargetBoneIndex = _reader.ReadVarLenIntAsInt32(BoneElementSize);
            ik.LoopCount = _reader.ReadInt32();
            ik.AngleLimit = _reader.ReadSingle();

            var linkCount = _reader.ReadInt32();
            var ikLinks = new IKLink[linkCount];

            for (var i = 0; i < linkCount; ++i) {
                ikLinks[i] = ReadIKLink();
            }

            ik.Links = ikLinks;

            return ik;
        }

        private PmxBone ReadPmxBone() {
            var bone = new PmxBone();

            bone.Name = ReadString() ?? string.Empty;
            bone.NameEnglish = ReadString() ?? string.Empty;
            bone.InitialPosition = _reader.ReadVector3();
            bone.CurrentPosition = bone.InitialPosition;
            bone.ParentIndex = _reader.ReadVarLenIntAsInt32(BoneElementSize);
            bone.Level = _reader.ReadInt32();
            bone.Flags = (BoneFlags)_reader.ReadUInt16();

            if (bone.HasFlag(BoneFlags.ToBone)) {
                bone.To_Bone = _reader.ReadVarLenIntAsInt32(BoneElementSize);
            } else {
                bone.To_Offset = _reader.ReadVector3();
            }

            if (bone.HasFlag(BoneFlags.AppendRotation) || bone.HasFlag(BoneFlags.AppendTranslation)) {
                bone.AppendParentIndex = _reader.ReadVarLenIntAsInt32(BoneElementSize);
                bone.AppendRatio = _reader.ReadSingle();
            }

            if (bone.HasFlag(BoneFlags.FixedAxis)) {
                bone.Axis = _reader.ReadVector3();
            }

            if (bone.HasFlag(BoneFlags.LocalFrame)) {
                var localX = _reader.ReadVector3();
                var localZ = _reader.ReadVector3();

                localX.Normalize();
                localZ.Normalize();

                var localY = Vector3.Cross(localZ, localX);

                localZ = Vector3.Cross(localX, localY);
                localY.Normalize();
                localZ.Normalize();

                bone.SetInitialRotationFromRotationAxes(localX, localY, localZ);
            } else {
                bone.InitialRotation = Quaternion.Identity;
                bone.CurrentRotation = Quaternion.Identity;
            }

            if (bone.HasFlag(BoneFlags.ExternalParent)) {
                bone.ExternalParentIndex = _reader.ReadInt32();
            }

            if (bone.HasFlag(BoneFlags.IK)) {
                bone.IK = ReadPmxIK();
            }

            return bone;
        }

        #region Morphs
        private PmxGroupMorph ReadPmxGroupMorph() {
            var morph = new PmxGroupMorph();

            morph.Index = _reader.ReadVarLenIntAsInt32(MorphElementSize);
            morph.Ratio = _reader.ReadSingle();

            return morph;
        }

        private PmxVertexMorph ReadPmxVertexMorph() {
            var morph = new PmxVertexMorph();

            morph.Index = _reader.ReadVarLenIntAsInt32(VertexElementSize, true);
            morph.Offset = _reader.ReadVector3();

            return morph;
        }

        private PmxBoneMorph ReadPmxBoneMorph() {
            var morph = new PmxBoneMorph();

            morph.Index = _reader.ReadVarLenIntAsInt32(BoneElementSize);
            morph.Translation = _reader.ReadVector3();
            morph.Rotation = _reader.ReadQuaternion();

            return morph;
        }

        private PmxUVMorph ReadPmxUVMorph() {
            var morph = new PmxUVMorph();

            morph.Index = _reader.ReadVarLenIntAsInt32(VertexElementSize, true);
            morph.Offset = _reader.ReadVector4();

            return morph;
        }

        private PmxMaterialMorph ReadPmxMaterialMorph() {
            var morph = new PmxMaterialMorph();

            morph.Index = _reader.ReadVarLenIntAsInt32(MaterialElementSize);
            morph.Op = (MorphOp)_reader.ReadByte();
            morph.Diffuse = _reader.ReadVector4();
            morph.Specular = _reader.ReadVector3();
            morph.SpecularPower = _reader.ReadSingle();
            morph.Ambient = _reader.ReadVector3();
            morph.EdgeColor = _reader.ReadVector4();
            morph.EdgeSize = _reader.ReadSingle();
            morph.Texture = _reader.ReadVector4();
            morph.Sphere = _reader.ReadVector4();
            morph.Toon = _reader.ReadVector4();

            return morph;
        }

        private PmxImpulseMorph ReadPmxImpulseMorph() {
            var morph = new PmxImpulseMorph();

            morph.Index = _reader.ReadVarLenIntAsInt32(RigidBodyElementSize);
            morph.IsLocal = _reader.ReadBoolean();
            morph.Velocity = _reader.ReadVector3();
            morph.Torque = _reader.ReadVector3();

            return morph;
        }

        private PmxMorph ReadPmxMorph() {
            var morph = new PmxMorph();

            morph.Name = ReadString() ?? string.Empty;
            morph.NameEnglish = ReadString() ?? string.Empty;
            morph.Panel = _reader.ReadSByte(); // TODO: Signed? Unsigned?
            morph.OffsetKind = (MorphOffsetKind)_reader.ReadByte(); // TODO: Signed? Unsigned?

            var morphCount = _reader.ReadInt32();
            var morphs = new List<PmxBaseMorph>();

            for (var i = 0; i < morphCount; ++i) {
                PmxBaseMorph baseMorph;

                switch (morph.OffsetKind) {
                    case MorphOffsetKind.Group:
                    case MorphOffsetKind.Flip:
                        baseMorph = ReadPmxGroupMorph();
                        break;
                    case MorphOffsetKind.Vertex:
                        baseMorph = ReadPmxVertexMorph();
                        break;
                    case MorphOffsetKind.Bone:
                        baseMorph = ReadPmxBoneMorph();
                        break;
                    case MorphOffsetKind.UV:
                    case MorphOffsetKind.Uva1:
                    case MorphOffsetKind.Uva2:
                    case MorphOffsetKind.Uva3:
                    case MorphOffsetKind.Uva4:
                        baseMorph = ReadPmxUVMorph();
                        break;
                    case MorphOffsetKind.Material:
                        baseMorph = ReadPmxMaterialMorph();
                        break;
                    case MorphOffsetKind.Impulse:
                        baseMorph = ReadPmxImpulseMorph();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                morphs.Add(baseMorph);
            }

            morph.Offsets = morphs.ToArray();

            return morph;
        }
        #endregion

        private NodeElement ReadNoteElement() {
            var nodeElement = new NodeElement();

            nodeElement.ElementType = (ElementType)_reader.ReadByte();

            switch (nodeElement.ElementType) {
                case ElementType.Bone:
                    nodeElement.Index = _reader.ReadVarLenIntAsInt32(BoneElementSize);
                    break;
                case ElementType.Morph:
                    nodeElement.Index = _reader.ReadVarLenIntAsInt32(MorphElementSize);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return nodeElement;
        }

        private PmxNode ReadPmxNode() {
            var node = new PmxNode();

            node.Name = ReadString() ?? string.Empty;
            node.NameEnglish = ReadString() ?? string.Empty;
            node.IsSystemNode = _reader.ReadBoolean();

            var elementCount = _reader.ReadInt32();
            var elements = new NodeElement[elementCount];

            for (var i = 0; i < elementCount; ++i) {
                elements[i] = ReadNoteElement();
            }

            node.Elements = elements;

            return node;
        }

        private PmxRigidBody ReadPmxRigidBody() {
            var body = new PmxRigidBody();

            body.Name = ReadString() ?? string.Empty;
            body.NameEnglish = ReadString() ?? string.Empty;

            body.BoneIndex = _reader.ReadVarLenIntAsInt32(BoneElementSize);
            body.GroupIndex = _reader.ReadByte(); // TODO: Signed? Unsigned?

            var bits = _reader.ReadUInt16();
            var passGroup = PmxBodyPassGroup.FromFlagBits(bits);
            body.PassGroup = passGroup;

            body.BoundingBoxKind = (BoundingBoxKind)_reader.ReadByte();
            body.BoundingBoxSize = _reader.ReadVector3();
            body.Position = _reader.ReadVector3();
            body.RotationAngles = _reader.ReadVector3();
            body.Mass = _reader.ReadSingle();
            body.PositionDamping = _reader.ReadSingle();
            body.RotationDamping = _reader.ReadSingle();
            body.Restitution = _reader.ReadSingle();
            body.Friction = _reader.ReadSingle();
            body.KineticMode = (KineticMode)_reader.ReadByte();

            return body;
        }

        private PmxJoint ReadPmxJoint() {
            var joint = new PmxJoint();

            joint.Name = ReadString() ?? string.Empty;
            joint.NameEnglish = ReadString() ?? string.Empty;

            joint.Kind = (JointKind)_reader.ReadByte();
            joint.BodyIndex1 = _reader.ReadVarLenIntAsInt32(RigidBodyElementSize);
            joint.BodyIndex2 = _reader.ReadVarLenIntAsInt32(RigidBodyElementSize);
            joint.Position = _reader.ReadVector3();
            joint.RotationAngles = _reader.ReadVector3();
            joint.LimitMoveLower = _reader.ReadVector3();
            joint.LimitMoveUpper = _reader.ReadVector3();
            joint.LimitAngleLower = _reader.ReadVector3();
            joint.LimitAngleUpper = _reader.ReadVector3();
            joint.SpConst_Move = _reader.ReadVector3();
            joint.SpConst_Rotate = _reader.ReadVector3();

            return joint;
        }

        private BodyAnchor ReadBodyAnchor() {
            var anchor = new BodyAnchor();

            anchor.BodyIndex = _reader.ReadVarLenIntAsInt32(RigidBodyElementSize);
            anchor.VertexIndex = _reader.ReadVarLenIntAsInt32(VertexElementSize, true);
            anchor.IsNear = _reader.ReadBoolean();

            return anchor;
        }

        private VertexPin ReadVertexPin() {
            var pin = new VertexPin();

            pin.VertexIndex = _reader.ReadVarLenIntAsInt32(VertexElementSize, true);

            return pin;
        }

        private PmxSoftBody ReadPmxSoftBody() {
            var body = new PmxSoftBody();

            body.Name = ReadString() ?? string.Empty;
            body.NameEnglish = ReadString() ?? string.Empty;

            body.Shape = (SoftBodyShape)_reader.ReadByte(); // TODO: Signed? Unsigned?
            body.MaterialIndex = _reader.ReadVarLenIntAsInt32(MaterialElementSize);
            body.GroupIndex = _reader.ReadByte(); // TODO: Signed? Unsigned?

            var bits = _reader.ReadUInt16();
            var passGroup = PmxBodyPassGroup.FromFlagBits(bits);
            body.PassGroup = passGroup;

            body.Flags = (SoftBodyFlags)_reader.ReadByte();
            body.BendingLinkDistance = _reader.ReadInt32();
            body.ClusterCount = _reader.ReadInt32();
            body.TotalMass = _reader.ReadSingle();
            body.Margin = _reader.ReadSingle();

            var config = body.Config;
            config.AeroModel = _reader.ReadInt32();
            config.VCF = _reader.ReadSingle();
            config.DP = _reader.ReadSingle();
            config.DG = _reader.ReadSingle();
            config.LF = _reader.ReadSingle();
            config.PR = _reader.ReadSingle();
            config.VC = _reader.ReadSingle();
            config.DF = _reader.ReadSingle();
            config.MT = _reader.ReadSingle();
            config.CHR = _reader.ReadSingle();
            config.KHR = _reader.ReadSingle();
            config.SHR = _reader.ReadSingle();
            config.AHR = _reader.ReadSingle();
            config.SRHR_CL = _reader.ReadSingle();
            config.SKHR_CL = _reader.ReadSingle();
            config.SSHR_CL = _reader.ReadSingle();
            config.SR_SPLT_CL = _reader.ReadSingle();
            config.SK_SPLT_CL = _reader.ReadSingle();
            config.SS_SPLT_CL = _reader.ReadSingle();
            config.V_IT = _reader.ReadInt32();
            config.P_IT = _reader.ReadInt32();
            config.D_IT = _reader.ReadInt32();
            config.C_IT = _reader.ReadInt32();

            var matCfg = body.MaterialConfig;
            matCfg.LST = _reader.ReadSingle();
            matCfg.AST = _reader.ReadSingle();
            matCfg.VST = _reader.ReadSingle();

            var bodyAnchorCount = _reader.ReadInt32();
            var bodyAnchors = new BodyAnchor[bodyAnchorCount];

            for (var i = 0; i < bodyAnchorCount; ++i) {
                bodyAnchors[i] = ReadBodyAnchor();
            }

            body.BodyAnchors = bodyAnchors.Distinct().ToArray();

            var vertexPinCount = _reader.ReadInt32();
            var vertexPins = new VertexPin[vertexPinCount];

            for (var i = 0; i < vertexPinCount; ++i) {
                vertexPins[i] = ReadVertexPin();
            }

            body.VertexPins = vertexPins.Distinct().ToArray();

            return body;
        }
        #endregion

        #region File structures
        private void ReadHeader() {
            var magicBytes = _reader.ReadBytes(4);

            if (magicBytes.ElementEquals(PmxSignatureV1)) {
                MajorVersion = PmxFormatVersion.Version1;
                DetailedVersion = _reader.ReadSingle();
            } else if (magicBytes.ElementEquals(PmxSignatureV2)) {
                MajorVersion = PmxFormatVersion.Version2;
                DetailedVersion = _reader.ReadSingle();

                if (DetailedVersion > 2.1f) {
                    throw new FormatException("PMX versions above 2.1 are not supported.");
                }
            } else {
                throw new FormatException("PMX header magic bytes not match.");
            }
        }

        private void ReadElementFormats() {
            var elementSizeEntryCount = _reader.ReadByte(); // TODO: Signed? Unsigned?
            var elementSizes = _reader.ReadBytes(elementSizeEntryCount);

            switch (MajorVersion) {
                case PmxFormatVersion.Version1:
                    VertexElementSize = elementSizes[0];
                    BoneElementSize = elementSizes[1];
                    MorphElementSize = elementSizes[2];
                    MaterialElementSize = elementSizes[3];
                    RigidBodyElementSize = elementSizes[4];
                    break;
                case PmxFormatVersion.Version2:
                    StringEncoding = (PmxStringEncoding)elementSizes[0];
                    UvaCount = elementSizes[1];
                    VertexElementSize = elementSizes[2];
                    TexElementSize = elementSizes[3];
                    MaterialElementSize = elementSizes[4];
                    BoneElementSize = elementSizes[5];
                    MorphElementSize = elementSizes[6];
                    RigidBodyElementSize = elementSizes[7];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

        #region Reading helpers
        [CanBeNull]
        private string ReadString() {
            switch (MajorVersion) {
                case PmxFormatVersion.Version1:
                    return ReadStringV1();
                case PmxFormatVersion.Version2:
                    switch (StringEncoding) {
                        case PmxStringEncoding.Utf16:
                            return ReadStringV2();
                        case PmxStringEncoding.Utf8:
                            return ReadStringV1();
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CanBeNull]
        private string ReadStringV1() {
            var count = _reader.ReadInt32();

            if (count > 0) {
                var bytes = _reader.ReadBytes(count);
                return Encoding.UTF8.GetString(bytes);
            } else {
                return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CanBeNull]
        private string ReadStringV2() {
            var count = _reader.ReadInt32();

            if (count > 0) {
                var bytes = _reader.ReadBytes(count);
                return Encoding.Unicode.GetString(bytes);
            } else {
                return null;
            }
        }
        #endregion

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

        private readonly BinaryReader _reader;

    }
}

