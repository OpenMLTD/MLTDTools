using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Extensions;
using OpenMLTD.MillionDance.Utilities;
using OpenTK;

namespace OpenMLTD.MillionDance.Entities.Internal {
    internal sealed class BoneNode {

        public BoneNode([CanBeNull] BoneNode parent, int index, [NotNull] string path, Vector3 initialPosition, Quaternion initialRotation) {
            Parent = parent;
            Path = path;
            Index = index;

            parent?.AddChild(this);

            initialPosition = initialPosition.FixUnityToOpenTK();
            initialRotation = initialRotation.FixUnityToOpenTK();

            InitialPosition = initialPosition;
            InitialRotation = initialRotation;
            CurrentPosition = initialPosition;
            LocalPosition = initialPosition;
            LocalRotation = initialRotation;

            if (path.Contains(BoneLookup.BoneNamePart_BodyScale)) {
                Name = path.Replace(BoneLookup.BoneNamePart_BodyScale, string.Empty);
            } else {
                Name = path;
            }
        }

        [CanBeNull]
        public BoneNode Parent { get; internal set; }

        /// <summary>
        /// Index in bone list.
        /// </summary>
        public int Index { get; }

        [NotNull]
        public string Path { get; }

        [NotNull]
        public string Name { get; }

        public int Level { get; internal set; }

        // Do not modify this list directly
        // TODO: use something like "internal list" for this property
        [NotNull, ItemNotNull]
        public List<BoneNode> Children => _children;

        public bool IsDirty { get; private set; } = true;

        public Vector3 CurrentPosition { get; private set; }

        public Vector3 LocalPosition {
            get => _localPosition;
            set {
                _localPosition = value;
                SetDirty();
            }
        }

        public Quaternion LocalRotation {
            get => _localRotation;
            set {
                _localRotation = value;
                SetDirty();
            }
        }

        public Matrix4 LocalMatrix => _localMatrix;

        public Matrix4 WorldMatrix => _worldMatrix;

        public Matrix4 SkinMatrix => _skinMatrix;

        public Vector3 InitialPosition { get; internal set; }

        public Quaternion InitialRotation { get; internal set; }

        public override string ToString() {
            var parent = Parent;

            if (parent != null) {
                return $"Bone \"{Path}\" (Parent: \"{parent.Path}\")";
            } else {
                return $"Bone \"{Path}\"";
            }
        }

        internal Vector3 InitialPositionWorld { get; set; }

        internal Matrix4 BindingPoseInverse {
            get {
                Debug.Assert(_bindingPoseInverse != null, nameof(_bindingPoseInverse) + " != null");
                return _bindingPoseInverse.Value;
            }
        }

        internal Matrix4 BindingPose {
            get {
                Debug.Assert(_bindingPose != null, nameof(_bindingPose) + " != null");
                return _bindingPose.Value;
            }
        }

        internal void AddChild([NotNull] BoneNode node) {
            if (!_children.Contains(node)) {
                _children.Add(node);
            }
        }

        internal bool RemoveChild([NotNull] BoneNode node) {
            return _children.Remove(node);
        }

        internal void Initialize(bool forced = false) {
            if (!forced && _isInitialized) {
                return;
            }

            var parent = Parent;

            parent?.Initialize();

            var t = InitialPosition;
            var q = InitialRotation;

            _localMatrix = CreateTransformMatrix(t, q);

            if (forced || _bindingPoseInverse == null) {
                _worldMatrix = ComputeWorldMatrix();
                _bindingPose = _worldMatrix;
                _bindingPoseInverse = _worldMatrix.Inverted();
            } else {
                throw new InvalidOperationException();
            }

            var posWorld = Vector3.TransformPosition(Vector3.Zero, _worldMatrix);

            InitialPositionWorld = posWorld;

            CurrentPosition = InitialPositionWorld;

            _isInitialized = true;
        }

        internal void UpdateTransform() {
            if (!IsDirty) {
                return;
            }

            IsDirty = false;

            var parent = Parent;

            parent?.UpdateTransform();

            var t = LocalPosition;
            var q = LocalRotation;

            _localMatrix = CreateTransformMatrix(t, q);
            _worldMatrix = ComputeWorldMatrix();
            _skinMatrix = BindingPoseInverse * _worldMatrix;

            CurrentPosition = Vector3.TransformPosition(InitialPositionWorld, _skinMatrix);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Matrix4 CreateTransformMatrix(Vector3 translation, Quaternion rotation) {
            var rotMat = Matrix4.CreateFromQuaternion(rotation);
            var transMat = Matrix4.CreateTranslation(translation);

            return rotMat * transMat;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Matrix4 ComputeWorldMatrix() {
            var parent = Parent;

            if (parent != null) {
                return _localMatrix * parent.WorldMatrix;
            } else {
                return _localMatrix;
            }
        }

        private void SetDirty() {
            IsDirty = true;

            foreach (var child in Children) {
                child.SetDirty();
            }
        }

        [NotNull, ItemNotNull]
        private readonly List<BoneNode> _children = new List<BoneNode>();

        [CanBeNull]
        private Matrix4? _bindingPoseInverse;

        [CanBeNull]
        private Matrix4? _bindingPose;

        private Vector3 _localPosition = Vector3.Zero;

        private Quaternion _localRotation = Quaternion.Identity;

        private bool _isInitialized;

        private Matrix4 _localMatrix;

        private Matrix4 _worldMatrix;

        private Matrix4 _skinMatrix;

    }
}
