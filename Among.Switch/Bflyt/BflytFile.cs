using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Among.Switch.Bflyt.Sections;
using Among.Switch.Buffers;
using Among.Switch.Util;

namespace Among.Switch.Bflyt; 

public class BflytFile {
    private const string FlytMagic = "FLYT";
    private const ushort HeaderSize = 0x14;

    private static readonly Dictionary<string, Type> SectionTypes = Assembly
        .GetExecutingAssembly()
        .GetTypes()
        .Select(x => (attr: x.GetCustomAttribute<LayoutSectionAttribute>(), type: x))
        .Where(x => x.attr != null)
        .ToDictionary(x => x.attr.AsciiName, x => x.type);

    public uint Version { get; set; }
    public bool BigEndian { get; set; }
    public List<ILayoutSection> Sections { get; } = new List<ILayoutSection>();
    public SpanBuffer Save() {
        SpanBuffer buffer = new SpanBuffer(new byte[HeaderSize], BigEndian);
        buffer.WriteString(FlytMagic);
        buffer.WriteBom();
        buffer.WriteU16(HeaderSize);
        buffer.WriteU32(Version);
        Bookmark fileSize = buffer.BookmarkLocation(4);
        Console.WriteLine(Sections.Count);
        buffer.WriteU16((ushort) Sections.Count);
        foreach (ILayoutSection section in Sections) {
            SpanBuffer sectionHeader = new SpanBuffer(new byte[8], BigEndian);
            sectionHeader.WriteString(section.SectionName);
            Bookmark sectionLen = sectionHeader.BookmarkLocation(4);
            SpanBuffer sectionBuffer = section.Save(BigEndian);
            sectionLen.Toggle(ref sectionHeader);
            sectionHeader.WriteU32((uint) (sectionBuffer.Size + 8));
            buffer += sectionHeader;
            buffer += sectionBuffer;
        }
        fileSize.Jump(ref buffer);
        buffer.WriteI32(buffer.Size);
        return buffer;
    }
    public static BflytFile Load(Span<byte> data) {
        SpanBuffer buffer = new SpanBuffer(data);
        BflytFile file = new BflytFile();
        if (buffer.ReadString(4) != FlytMagic)
            throw new Exception("Buffer is not a valid BFLYT file");
        buffer.SetBomFe();
        file.BigEndian = buffer.BigEndian;

        Console.WriteLine($"header size = {buffer.ReadU16()}");

        file.Version = buffer.ReadU32();
        buffer.ReadU32(); // file size
        ushort sectionCount = buffer.ReadU16();
        buffer.Offset += 2; // struct padding

        for (int i = 0; i < sectionCount; i++) {
            string sectionMagic = buffer.ReadString(4);
            int sectionSize = (int) (buffer.ReadU32() - 8);
            Console.WriteLine($"At {sectionMagic} {SectionTypes.ContainsKey(sectionMagic)}");
            SpanBuffer sectionSlice = new SpanBuffer(buffer.ReadBytes(sectionSize), buffer.BigEndian);
            if (SectionTypes.TryGetValue(sectionMagic, out Type sectionType)) {
                ILayoutSection layoutSection = (ILayoutSection) Activator.CreateInstance(sectionType);
                layoutSection.SectionName = sectionMagic;
                layoutSection.Load(ref sectionSlice);
                file.Sections.Add(layoutSection);
            } else {
                UnknownSection uls = new UnknownSection(sectionMagic);
                uls.Load(ref sectionSlice);
                file.Sections.Add(uls);
            }
        }

        return file;
    }
}