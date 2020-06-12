using System;
using System.Collections.Generic;
using System.IO;
using AssetStudio.Extended.MonoBehaviours.Serialization.Serialized;
using JetBrains.Annotations;

namespace AssetStudio.Extended.MonoBehaviours.Serialization {
    internal static class StructureReader {

        [NotNull]
        public static KeyValuePair<string, object>[] ReadMembers([NotNull, ItemNotNull] IReadOnlyList<TypeTreeNode> typeNodes, [NotNull] ObjectReader reader) {
            reader.Reset();

            var typeNodeCount = typeNodes.Count;

            var set = new HashSet<string>();
            var result = new List<KeyValuePair<string, object>>();

            for (var i = 0; i < typeNodeCount; i += 1) {
                var (name, value) = ReadMemberValue(typeNodes, ref i, reader);

                if (set.Contains(name)) {
                    throw new ArgumentException($"Object name already exists: '{name}'.", nameof(name));
                }

                set.Add(name);
                result.Add(new KeyValuePair<string, object>(name, value));
            }

            return result.ToArray();
        }

        private static (string Name, object Value) ReadMemberValue([NotNull, ItemNotNull] IReadOnlyList<TypeTreeNode> typeNodes, ref int nodeIndex, [NotNull] BinaryReader reader) {
            var typeNode = typeNodes[nodeIndex];

            object value;

            var align = (typeNode.m_MetaFlag & 0x4000) != 0;

            switch (typeNode.m_Type) {
                case "SInt8":
                    value = reader.ReadSByte();
                    break;
                case "UInt8":
                case "char":
                    value = reader.ReadByte();
                    break;
                case "short":
                case "SInt16":
                    value = reader.ReadInt16();
                    break;
                case "UInt16":
                case "unsigned short":
                    value = reader.ReadUInt16();
                    break;
                case "int":
                case "SInt32":
                    value = reader.ReadInt32();
                    break;
                case "UInt32":
                case "unsigned int":
                case "Type*":
                    value = reader.ReadUInt32();
                    break;
                case "long long":
                case "SInt64":
                    value = reader.ReadInt64();
                    break;
                case "UInt64":
                case "unsigned long long":
                case "FileSize":
                    value = reader.ReadUInt64();
                    break;
                case "float":
                    value = reader.ReadSingle();
                    break;
                case "double":
                    value = reader.ReadDouble();
                    break;
                case "bool":
                    value = reader.ReadBoolean();
                    break;
                case "string":
                    value = reader.ReadAlignedString();
                    nodeIndex += 3;
                    break;
                case "map": {
                    // IDictionary<TKey, TValue>
                    if ((typeNodes[nodeIndex + 1].m_MetaFlag & 0x4000) != 0) {
                        align = true;
                    }

                    var count = reader.ReadInt32();
                    var map = GetChildTypeNodes(typeNodes, nodeIndex);

                    nodeIndex += map.Count - 1;

                    var first = GetChildTypeNodes(map, 4);
                    var second = GetChildTypeNodes(map, 4 + first.Count);

                    var dict = new Dictionary<object, object>();
                    Type keyType = null, valueType = null;

                    for (var j = 0; j < count; j += 1) {
                        var tmp1 = 0;
                        var (_, keyObject) = ReadMemberValue(first, ref tmp1, reader);
                        var tmp2 = 0;
                        var (_, valueObject) = ReadMemberValue(second, ref tmp2, reader);

                        if (keyType == null && !ReferenceEquals(keyObject, null)) {
                            keyType = SerializingHelper.NonNullTypeOf(keyObject);
                        }

                        if (valueType == null && !ReferenceEquals(valueObject, null)) {
                            valueType = SerializingHelper.NonNullTypeOf(valueObject);
                        }

                        dict.Add(keyObject, valueObject);
                    }

                    if (keyType == null) {
                        keyType = typeof(object);
                    }

                    if (valueType == null) {
                        valueType = typeof(object);
                    }

                    value = new ObjectDictionary(dict, keyType, valueType);

                    break;
                }
                case "TypelessData": {
                    // Raw byte array
                    var size = reader.ReadInt32();
                    var data = reader.ReadBytes(size);

                    nodeIndex += 2;

                    value = new RawData(data);

                    break;
                }
                default: {
                    if (nodeIndex < typeNodes.Count - 1 && typeNodes[nodeIndex + 1].m_Type == "Array") {
                        // Array
                        if ((typeNodes[nodeIndex + 1].m_MetaFlag & 0x4000) != 0) {
                            align = true;
                        }

                        var count = reader.ReadUInt32();
                        var vector = GetChildTypeNodes(typeNodes, nodeIndex);

                        nodeIndex += vector.Count - 1;

                        var array = new object[count];
                        Type elementType = null;

                        for (var j = 0; j < count; j += 1) {
                            var tmp = 3;
                            var (_, item) = ReadMemberValue(vector, ref tmp, reader);

                            if (elementType == null && !ReferenceEquals(item, null)) {
                                elementType = SerializingHelper.NonNullTypeOf(item);
                            }

                            array[j] = item;
                        }

                        if (elementType == null) {
                            elementType = typeof(object);
                        }

                        value = new ObjectArray(array, elementType);
                    } else {
                        // Object
                        var classTree = GetChildTypeNodes(typeNodes, nodeIndex);
                        var classTreeCount = classTree.Count;

                        nodeIndex += classTreeCount - 1;

                        var variables = new Dictionary<string, object>();

                        for (var j = 1; j < classTreeCount; j += 1) {
                            var (memberName, memberValue) = ReadMemberValue(classTree, ref j, reader);
                            variables.Add(memberName, memberValue);
                        }

                        value = new CustomType(typeNode.m_Type, variables);
                    }

                    break;
                }
            }

            if (align) {
                reader.AlignStream();
            }

            return (typeNode.m_Name, value);
        }


        [NotNull, ItemNotNull]
        private static List<TypeTreeNode> GetChildTypeNodes([NotNull, ItemNotNull] IReadOnlyList<TypeTreeNode> typeNodes, int index) {
            var startItem = typeNodes[index];

            var result = new List<TypeTreeNode> {
                startItem
            };

            var level = startItem.m_Level;

            for (var i = index + 1; i < typeNodes.Count; i++) {
                var node = typeNodes[i];
                var nodeLevel = node.m_Level;

                if (nodeLevel <= level) {
                    return result;
                }

                result.Add(node);
            }

            return result;
        }

    }
}
