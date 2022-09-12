using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Among.Switch.Buffers;
using Among.Switch.Util;

namespace Among.Switch.Bflyt.Sections; 

[LayoutSection("fnl1")]
public class FontRefSection : ILayoutSection {
    public string SectionName { get; set; }
    public List<string> Fonts = new List<string>();
    public void Load(ref SpanBuffer slice) {
        ushort textureCount = slice.ReadU16();
        slice.Offset += 2;
        Bookmark baseOffset = slice.BookmarkLocation(0);
        for (int i = 0; i < textureCount; i++) {
            baseOffset.Jump(ref slice);
            slice.Offset += sizeof(uint) * i;
            uint offset = slice.ReadU32();
            Bookmark strOffset = baseOffset + offset;
            strOffset.Jump(ref slice);
            Fonts.Add(slice.ReadStringNull());
        }
    }
    public SpanBuffer Save(bool bigEndian) {
        int len = Fonts.Select(x => (Encoding.UTF8.GetByteCount(x) + 1).AlignInt(0b11)).Sum().AlignInt(0b11);
        SpanBuffer spanBuffer = new SpanBuffer(new byte[4 + Fonts.Count * 4 + len], bigEndian);
        spanBuffer.WriteU16((ushort) Fonts.Count);
        spanBuffer.Offset += 2;
        Bookmark currentOffset = spanBuffer.GetBookmark(SeekOrigin.Current, Fonts.Count * 4);
        foreach (string texture in Fonts) {
            currentOffset.Toggle(ref spanBuffer);
            int offset = spanBuffer.Offset;
            spanBuffer.WriteStringNull(texture);
            spanBuffer.Offset = offset + (Encoding.UTF8.GetByteCount(texture) + 1).AlignInt(0b11);
            int end = spanBuffer.Offset;
            currentOffset.Toggle(ref spanBuffer);
            currentOffset.Offset = end;
            spanBuffer.WriteI32(offset - 4);
        }
        return spanBuffer;
    }
}