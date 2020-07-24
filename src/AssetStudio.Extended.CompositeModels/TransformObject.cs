using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    [DebuggerDisplay(nameof(TransformObject) + " (Name: {Name}, Full Name: {GetFullName()})")]
    public sealed class TransformObject {

        internal TransformObject([NotNull] string name, [NotNull] RawTransform transform) {
            Name = name;
            Transform = transform;
            Children = Array.Empty<TransformObject>();
            ChildList = new List<TransformObject>();
        }

        [NotNull]
        public string Name { get; }

        [NotNull]
        public string GetFullName() {
            if (_fullName != null) {
                return _fullName;
            }

            var sb = new StringBuilder();

            sb.Append(Name);

            var node = Parent;

            while (node != null) {
                sb.Insert(0, "/");
                sb.Insert(0, node.Name);

                node = node.Parent;
            }

            _fullName = sb.ToString();

            return _fullName;
        }

        [NotNull]
        public RawTransform Transform { get; }

        [CanBeNull]
        public TransformObject Parent { get; internal set; }

        [NotNull, ItemNotNull]
        public TransformObject[] Children { get; private set; }

        [CanBeNull, ItemNotNull]
        internal List<TransformObject> ChildList { get; set; }

        internal void Seal() {
            Debug.Assert(ChildList != null, nameof(ChildList) + " != null");
            Children = ChildList.ToArray();
            ChildList.Clear();
            ChildList = null;
        }

        [CanBeNull]
        private string _fullName;

    }
}
