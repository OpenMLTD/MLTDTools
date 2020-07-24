using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace AssetStudio.Extended.CompositeModels {
    public sealed class TransformHierarchies {

        private TransformHierarchies([NotNull, ItemNotNull] params TransformObject[] roots) {
            Roots = roots;
        }

        [NotNull, ItemNotNull]
        public TransformObject[] Roots { get; }

        [NotNull]
        public static TransformHierarchies Combine([NotNull, ItemNotNull] params TransformHierarchies[] hierarchiesArray) {
            if (hierarchiesArray.Length == 0) {
                throw new ArgumentException("There must be at least one hierarchy to combine.", nameof(hierarchiesArray));
            }

            var roots = new List<TransformObject>();

            foreach (var transformHierarchy in hierarchiesArray) {
                roots.AddRange(transformHierarchy.Roots);
            }

            return new TransformHierarchies(roots.ToArray());
        }

        [NotNull]
        public static TransformHierarchies FromObjects([NotNull, ItemNotNull] params TransformObject[] roots) {
            return new TransformHierarchies(roots);
        }

        [NotNull]
        public static TransformHierarchies FromAssets([NotNull] AssetsManager manager) {
            var idObjectMap = new List<GameObject>();
            var lookup = SerializedObjectsLookup.Create(manager);

            foreach (var serializedFile in manager.assetsFileList) {
                foreach (var o in serializedFile.Objects) {
                    if (o.type != ClassIDType.GameObject) {
                        continue;
                    }

                    var go = o as GameObject;
                    Debug.Assert(go != null, nameof(go) + " != null");

                    idObjectMap.Add(go);
                }
            }

            var objObjMap = new Dictionary<long, TransformObject>();

            foreach (var gameObject in idObjectMap) {
                BuildHierarchy(gameObject, lookup, objObjMap);
            }

            var roots = new List<TransformObject>();

            foreach (var transformObject in objObjMap.Values) {
                transformObject.Seal();

                if (transformObject.Parent == null) {
                    roots.Add(transformObject);
                }
            }

            var result = new TransformHierarchies(roots.ToArray());

            return result;
        }

        private static void BuildHierarchy([NotNull] GameObject root, [NotNull] SerializedObjectsLookup serializedObjectsLookup, [NotNull] Dictionary<long, TransformObject> objObjMap) {
            if (objObjMap.ContainsKey(root.m_PathID)) {
                return;
            }

            var transform = new RawTransform(root.m_Transform);
            var transformObject = new TransformObject(root.m_Name, transform);

            objObjMap.Add(root.m_PathID, transformObject);

            {
                var childTransforms = root.m_Transform.m_Children;

                if (childTransforms.Length > 0) {
                    var childList = transformObject.ChildList;
                    Debug.Assert(childList != null, nameof(childList) + " != null");

                    foreach (var childTransform in childTransforms) {
                        Debug.Assert(childTransform != null, nameof(childTransform) + " != null");

                        var childGameObject = serializedObjectsLookup.Find<GameObject>(t => t.m_Transform.m_PathID == childTransform.m_PathID);
                        Debug.Assert(childGameObject != null, nameof(childGameObject) + " != null");

                        BuildHierarchy(childGameObject, serializedObjectsLookup, objObjMap);

                        childList.Add(objObjMap[childGameObject.m_PathID]);
                    }
                }
            }

            {
                var parentTransform = root.m_Transform.m_Father;

                if (!parentTransform.IsNull) {
                    var parentGameObject = serializedObjectsLookup.Find<GameObject>(t => t.m_Transform.m_PathID == parentTransform.m_PathID);
                    Debug.Assert(parentGameObject != null, nameof(parentGameObject) + " != null");

                    BuildHierarchy(parentGameObject, serializedObjectsLookup, objObjMap);

                    transformObject.Parent = objObjMap[parentGameObject.m_PathID];
                }
            }
        }

    }
}
