using Among.Switch.Buffers;

namespace Among.Switch.Bflyt.Sections;

[LayoutSection("lyt1")]
public class GeneralLayoutSection : ILayoutSection {
    public int Size => 0;
    public bool Centered { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float UnknownParts1 { get; set; }
    public float UnknownParts2 { get; set; }
    public string LayoutName { get; set; }

    public void Load(SpanBuffer slice) {
        Centered = slice.ReadU8() != 0;
        slice.Offset += 3;
        Width = slice.ReadF32();
        Height = slice.ReadF32();
        UnknownParts1 = slice.ReadF32();
        UnknownParts2 = slice.ReadF32();
        LayoutName = slice.ReadStringNull();
    }

    public void Save(SpanBuffer slice) {
        slice.WriteU8((byte) (Centered ? 1 : 0));
        slice.WriteRepeatedU8(0, 3);
        slice.WriteF32(Width);
        slice.WriteF32(Height);
        slice.WriteF32(UnknownParts1);
        slice.WriteF32(UnknownParts2);
        slice.WriteStringNull(LayoutName);
    }
}