using System.Text;
using Among.Switch.Buffers;
using Among.Switch.Util;

namespace Among.Switch.Bflyt.Sections;

[LayoutSection("lyt1")]
public class GeneralLayoutSection : ILayoutSection {
    public string SectionName { get; set; }
    public bool Centered { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float UnknownParts1 { get; set; }
    public float UnknownParts2 { get; set; }
    public string LayoutName { get; set; }

    public void Load(ref SpanBuffer slice) {
        Centered = slice.ReadU8() != 0;
        slice.Offset += 3;
        Width = slice.ReadF32();
        Height = slice.ReadF32();
        UnknownParts1 = slice.ReadF32();
        UnknownParts2 = slice.ReadF32();
        LayoutName = slice.ReadStringNull();
    }

    public SpanBuffer Save(bool bigEndian) {
        SpanBuffer buffer = new SpanBuffer(new byte[(20 + Encoding.UTF8.GetByteCount(LayoutName) + 1).AlignInt(0b11)], bigEndian);
        buffer.WriteU8((byte) (Centered ? 1 : 0));
        buffer.WriteRepeatedU8(0, 3);
        buffer.WriteF32(Width);
        buffer.WriteF32(Height);
        buffer.WriteF32(UnknownParts1);
        buffer.WriteF32(UnknownParts2);
        buffer.WriteStringNull(LayoutName);
        return buffer;
    }
}