using System.Collections.Generic;
using Among.Switch.Buffers;

namespace Among.Switch.Bflyt.Sections; 

[LayoutSection("txl1")]
public class TextureLayoutSection : ILayoutSection {
    public int Size => 0;
    public List<string> Textures { get; } = new List<string>();
    public void Load(SpanBuffer slice) {
        ushort textureCount = slice.ReadU16();
        slice.Offset += 2;
        Bookmark baseOffset = slice.BookmarkLocation(0);
        for (int i = 0; i < textureCount; i++) {
            baseOffset.Jump(ref slice);
            slice.Offset += sizeof(uint) * i;
            uint offset = slice.ReadU32();
            Bookmark strOffset = baseOffset + offset;
            strOffset.Jump(ref slice);
            Textures.Add(slice.ReadStringNull());
        }
    }

    public void Save(SpanBuffer slice) {
        throw new System.NotImplementedException();
    }
}