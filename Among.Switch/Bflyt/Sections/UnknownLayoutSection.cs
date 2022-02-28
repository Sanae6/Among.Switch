using System;
using Among.Switch.Buffers;

namespace Among.Switch.Bflyt.Sections; 

public class UnknownLayoutSection : ILayoutSection {
    public string SectionName { get; }
    public int Size => Data.Length;
    public byte[] Data;

    public UnknownLayoutSection(string sectionMagic) {
        SectionName = sectionMagic;
    }

    public void Load(SpanBuffer slice) {
        Data = slice.Buffer.ToArray();
    }

    public void Save(SpanBuffer slice) {
        Data.CopyTo(slice.Buffer);
    }
}