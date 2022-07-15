using System;
using Among.Switch.Buffers;

namespace Among.Switch.Bflyt.Sections; 

public class UnknownSection : ILayoutSection {
    public string SectionName { get; set; }
    public byte[] Data;

    public UnknownSection(string sectionMagic) {
        SectionName = sectionMagic;
    }

    public void Load(SpanBuffer slice) {
        Data = slice.Buffer.ToArray();
    }

    public SpanBuffer Save(bool bigEndian) {
        return new SpanBuffer(Data);
    }
}