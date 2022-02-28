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
    public List<ILayoutSection> Sections { get; } = new List<ILayoutSection>();
    public static BflytFile Load(Span<byte> data) {
        SpanBuffer buffer = new SpanBuffer(data);
        BflytFile file = new BflytFile();
        if (buffer.ReadString(4) != FlytMagic)
            throw new Exception("Buffer is not a valid BFLYT file");
        buffer.SetBom();

        Console.WriteLine($"header size = {buffer.ReadU16()}");

        file.Version = buffer.ReadU32();
        buffer.ReadU32(); // file size
        ushort sectionCount = buffer.ReadU16();
        buffer.Offset += 2; // struct padding

        for (int i = 0; i < sectionCount; i++) {
            string sectionMagic = buffer.ReadString(4);
            int sectionSize = (int) (buffer.ReadU32() - 8);
            Console.WriteLine($"At {sectionMagic} {SectionTypes.ContainsKey(sectionMagic)}");
            if (SectionTypes.TryGetValue(sectionMagic, out Type sectionType)) {
                ILayoutSection layoutSection = (ILayoutSection) Activator.CreateInstance(sectionType);
                layoutSection.Load(new SpanBuffer(buffer.ReadBytes(sectionSize), buffer.BigEndian));
                file.Sections.Add(layoutSection);
            } else {
                UnknownLayoutSection uls = new UnknownLayoutSection(sectionMagic);
                uls.Load(new SpanBuffer(buffer.ReadBytes(sectionSize), buffer.BigEndian));
                file.Sections.Add(uls);
            }
        }

        return file;
    }
}