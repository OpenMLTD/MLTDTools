using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels.Utilities {
    public static class TransformHierarchiesExtensions {

        static TransformHierarchiesExtensions() {
            MltdRootObjects = new HashSet<string> {
                "",
                "POSITION",
                "MODEL_00",
                "obj_body_GP",
                "shape_GP",
                "KUBI",
                "obj_head_GP",
            };
        }

        [NotNull, ItemNotNull]
        public static TransformObject[] GetOrderedObjectArray([NotNull] this TransformHierarchies transformHierarchies) {
            var result = new List<TransformObject>();
            var stack = new Stack<TransformObject>();

            foreach (var rootObject in transformHierarchies.Roots) {
                stack.Push(rootObject);
            }

            // DFS. We need to maintain the relative order of the hierarchy, i.e. parent is always before children.
            while (stack.Count > 0) {
                var obj = stack.Pop();
                var children = obj.Children;

                if (children.Length > 0) {
                    for (var j = children.Length - 1; j >= 0; j -= 1) {
                        stack.Push(children[j]);
                    }
                }

                result.Add(obj);
            }

            return result.ToArray();
        }

        [NotNull]
        public static TransformHierarchies PreserveMltdSpecificOnly([NotNull] this TransformHierarchies transformHierarchies) {
            var newRootList = new List<TransformObject>();

            foreach (var rootObject in transformHierarchies.Roots) {
                var children = rootObject.Children;
                Debug.Assert(children.Length > 0, nameof(children) + "." + nameof(children.Length) + " > 0");

                foreach (var level1Child in children) {
                    if (!MltdRootObjects.Contains(level1Child.Name)) {
                        continue;
                    }

                    newRootList.Add(level1Child);
                }
            }

            var newRootArray = new TransformObject[newRootList.Count];

            for (var i = 0; i < newRootArray.Length; i += 1) {
                newRootArray[i] = CloneAsNewRoot(newRootList[i]);
            }

            return TransformHierarchies.FromObjects(newRootArray);
        }

        [NotNull]
        private static TransformObject CloneAsNewRoot([NotNull] TransformObject transformObject) {
            var obj = new TransformObject(transformObject.Name, transformObject.Transform.Clone());
            var childList = obj.ChildList;
            Debug.Assert(childList != null, nameof(childList) + " != null");

            foreach (var child in transformObject.Children) {
                var clone = CloneAsNewRoot(child);
                clone.Parent = obj;
                childList.Add(clone);
            }

            obj.Seal();

            return obj;
        }

        private static readonly HashSet<string> MltdRootObjects;

    }
}
