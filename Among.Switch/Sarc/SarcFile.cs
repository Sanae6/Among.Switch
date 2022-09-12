using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Among.Switch.Buffers;
using Among.Switch.Util;

namespace Among.Switch;

public class SarcFile {
    private const string SarcMagic = "SARC";
    private const string SfatMagic = "SFAT";
    private const string SfntMagic = "SFNT";
    private const ushort SarcHeaderSize = 0x14;
    private const ushort SfatHeaderSize = 0xC;
    private const ushort SfntHeaderSize = 0x8;

    public uint HashKey { get; }
    public Dictionary<string, byte[]> Files { get; } = new Dictionary<string, byte[]>();

    public SpanBuffer Save(bool compress, bool bigEndian = false) {
        SpanBuffer buffer = new SpanBuffer(new byte[SarcHeaderSize + SfatHeaderSize + Files.Count * FatNode.NodeSize], bigEndian);
        buffer.WriteString(SarcMagic);
        buffer.WriteU16(SarcHeaderSize);
        buffer.WriteBom();
        Bookmark fileSizeBookmark = buffer.BookmarkLocation(4);
        buffer.WriteU16(0x0100);
        buffer.Offset += 2;
        
        //sfat
        buffer.WriteString(SfatMagic);
        buffer.WriteU16(SfatHeaderSize);
        buffer.WriteU16((ushort) Files.Count);
        buffer.WriteU32(HashKey);
        
        

        return buffer;
    }

    public static SarcFile Load(Span<byte> data) {
        SpanBuffer buffer = new SpanBuffer(data, true);
        SarcFile sarc = new SarcFile();
        bool yaz = false;
        if (buffer.ReadString(4) != SarcMagic) {
            buffer = YazUtility.Decompress(data);
            File.WriteAllBytes("lol.sarc", buffer.Buffer.ToArray());
            buffer.BigEndian = true;
            if (buffer.ReadString(4) != SarcMagic) throw new Exception("Buffer is not valid (optionally Yaz0 compressed) SARC data");
            yaz = true;
        }

        Bookmark headerStart = buffer.BookmarkLocation(sizeof(ushort));
        buffer.SetBomFe();

        headerStart.Toggle(ref buffer);
        ushort headerSize = buffer.ReadU16();
        if (headerSize != SarcHeaderSize)
            throw new Exception($"SARC header length was not 0x{SarcHeaderSize:X}, got 0x{headerSize:X} (yaz0 archive: {yaz})");
        headerStart.Toggle(ref buffer);
        _ = buffer.ReadU32();
        Bookmark dataStart = new Bookmark((int) buffer.ReadU32());
        _ = buffer.ReadU32();
        if (buffer.ReadString(4) != SfatMagic) throw new Exception("Could not find SFAT magic in SARC data");
        Dictionary<uint, FatNode> hashMap = new Dictionary<uint, FatNode>();
        if (buffer.ReadU16() != SfatHeaderSize)
            throw new Exception($"SFAT header length was not 0x{SfatHeaderSize:X}");
        int nodeCount = buffer.ReadU16();
        uint hashKey = buffer.ReadU32();
        for (int i = 0; i < nodeCount; i++) {
            FatNode node = buffer.ReadStruct<FatNode>();
            hashMap[node.NameHash] = node;
        }

        if (buffer.ReadString(4) != SfntMagic) throw new Exception("Could not find SFNT magic in SARC data");
        if (buffer.ReadU16() != SfntHeaderSize)
            throw new Exception($"SFNT header length was not 0x{SfntHeaderSize:X}");
        buffer.Offset += 2;

        for (int i = 0; i < nodeCount; i++) {
            string str = buffer.ReadStringNull();
            FatNode node = hashMap[Hash(str, hashKey)];
            sarc.Files[str] = buffer[(int) (dataStart + node.Start)..(int) (dataStart + node.End)].ToArray();
            buffer.Align4();
        }

        return sarc;
    }

    public static uint Hash(string name, uint key) {
        return name.Aggregate<char, uint>(0, (current, t) => t + current * key);
    }

    private struct FatNode : IReadableStructure {
        public uint NameHash;
        public uint FileAttributes;
        public uint Start;
        public uint End;

        public const int NodeSize = 0x10;
        public void Load(ref SpanBuffer slice) {
            NameHash = slice.ReadU32();
            FileAttributes = slice.ReadU32();
            Start = slice.ReadU32();
            End = slice.ReadU32();
        }

        public SpanBuffer Save(bool bigEndian) {
            SpanBuffer buffer = new SpanBuffer(new byte[16], bigEndian);
            buffer.WriteU32(NameHash);
            buffer.WriteU32(FileAttributes);
            buffer.WriteU32(Start);
            buffer.WriteU32(End);
            return buffer;
        }
    }
}