using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Among.Switch.Buffers;
using Among.Switch.Byml.Nodes;

namespace Among.Switch.Byml;

public class BymlFile {
    private const string MagicLittleEndian = "YB";
    private const string MagicBigEndian = "BY";

    public int Version;

    private BymlFile(ushort version) {
        Version = version;
    }

    public StringTableNode Hashes;
    public StringTableNode Strings;
    public INode Root;

    public static BymlFile Load(Span<byte> data) {
        SpanBuffer buffer = new SpanBuffer(data);
        buffer.BigEndian = buffer.ReadString(2) switch {
            MagicLittleEndian => false,
            MagicBigEndian => true,
            _ => throw new Exception("Magic was not BY or YB!")
        };
        BymlFile byml = new BymlFile(buffer.ReadU16());

        // if (byml.Version < 3) throw new NotSupportedException("Versions lower than 3 are currently not supported");

        Bookmark hashTable = Bookmark.At(buffer.ReadI32());
        Bookmark stringTable = Bookmark.At(buffer.ReadI32());
        Bookmark rootNode = Bookmark.At(buffer.ReadI32());

        if (!rootNode) return byml; // document is empty, return early since no more parsing needs to take place

        INode ReadNode(ref SpanBuffer buffer, NodeTypes? type = null) {
            type ??= (NodeTypes) buffer.ReadU8();
            // Debug.WriteLine($"Node {type} version {byml.Version}");
            switch (type, byml.Version) {
                case (NodeTypes.StringIndex, >= 2):
                    return new StringNode(byml.Strings.Children[buffer.ReadI32()]);
                case (NodeTypes.BinaryData, >= 4):
                    throw new NotImplementedException("Binary data nodes are not yet supported.");
                case (NodeTypes.Array, >= 2): {
                    ArrayNode node = new ArrayNode();
                    int len = buffer.ReadI24();
                    Span<NodeTypes> types = MemoryMarshal.Cast<byte, NodeTypes>(buffer.ReadBytes(len));
                    buffer.Align4();
                    for (int i = 0; i < len; i++) {
                        if (IsSpecialNode(types[i])) {
                            Bookmark temp = buffer.GetBookmark(SeekOrigin.Begin, (int) buffer.ReadU32());
                            temp.Toggle(ref buffer);
                            node.Children.Add(ReadNode(ref buffer));
                            temp.Toggle(ref buffer);
                        } else {
                            node.Children.Add(ReadNode(ref buffer, types[i]));
                        }
                    }

                    return node;
                }
                case (NodeTypes.Dictionary, >= 2): {
                    DictionaryNode node = new DictionaryNode();
                    int len = buffer.ReadI24();
                    for (int i = 0; i < len; i++) {
                        string name = byml.Hashes.Children[buffer.ReadI24()];
                        NodeTypes subType = (NodeTypes) buffer.ReadU8();
                        if (IsSpecialNode(subType)) {
                            Bookmark temp = buffer.GetBookmark(SeekOrigin.Begin, (int) buffer.ReadU32());
                            temp.Toggle(ref buffer);
                            node.Children.Add(name, ReadNode(ref buffer));
                            temp.Toggle(ref buffer);
                        } else {
                            node.Children.Add(name, ReadNode(ref buffer, subType));
                        }
                    }

                    return node;
                }
                case (NodeTypes.StringTable, >= 2): {
                    StringTableNode node = new StringTableNode();
                    Bookmark start = buffer.BookmarkLocation(0) - 1;
                    int len = buffer.ReadI24();
                    for (int i = 0; i < len; i++) {
                        Bookmark bookmark = start + buffer.ReadU32();
                        bookmark.Toggle(ref buffer);
                        node.Children.Add(buffer.ReadStringNull());
                        bookmark.Toggle(ref buffer);
                    }

                    return node;
                }
                case (NodeTypes.Bool, >= 2):
                    return new BoolNode(buffer.ReadU32() > 0);
                case (NodeTypes.Int, >= 2):
                    return new IntNode(buffer.ReadI32());
                case (NodeTypes.Single, >= 2):
                    return new SingleNode(buffer.ReadF32());
                case (NodeTypes.UInt, >= 2):
                    return new UIntNode(buffer.ReadU32());
                case (NodeTypes.Long, >= 3): {
                    Bookmark temp = buffer.GetBookmark(SeekOrigin.Begin, buffer.ReadI32());
                    temp.Toggle(ref buffer);
                    LongNode node = new LongNode(buffer.ReadI64());
                    temp.Toggle(ref buffer);
                    return node;
                }
                case (NodeTypes.ULong, >= 3): {
                    Bookmark temp = buffer.GetBookmark(SeekOrigin.Begin, buffer.ReadI32());
                    temp.Toggle(ref buffer);
                    ULongNode node = new ULongNode(buffer.ReadU64());
                    temp.Toggle(ref buffer);
                    return node;
                }
                case (NodeTypes.Double, >= 3): {
                    Bookmark temp = buffer.GetBookmark(SeekOrigin.Begin, buffer.ReadI32());
                    temp.Toggle(ref buffer);
                    DoubleNode node = new DoubleNode(buffer.ReadF64());
                    temp.Toggle(ref buffer);
                    return node;
                }
                case (NodeTypes.Null, >= 3):
                    buffer.Offset += sizeof(int);
                    return new NullNode();
                default:
                    throw new NotSupportedException($"Node type {type} not supported on version {byml.Version}");
            }
        }

        if (hashTable) {
            hashTable.Jump(ref buffer);
            byml.Hashes = (StringTableNode) ReadNode(ref buffer);
        }

        if (stringTable) {
            stringTable.Jump(ref buffer);
            byml.Strings = (StringTableNode) ReadNode(ref buffer);
        }

        rootNode.Jump(ref buffer);

        if ((NodeTypes) buffer.PeekU8() is not NodeTypes.Array and not NodeTypes.Dictionary) {
            throw new Exception("Root node is not an array or a dictionary");
        }

        byml.Root = ReadNode(ref buffer);

        return byml;
    }

    private static bool IsSpecialNode(NodeTypes type) => type is NodeTypes.Array or NodeTypes.Dictionary;
}