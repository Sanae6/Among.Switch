using System;
using Among.Switch.Buffers;

namespace Among.Switch.Util; 

public static class BomSetter {
    public static void SetBom(this ref SpanBuffer buffer) =>
        buffer.BigEndian = buffer.ReadU16() switch {
            0xFEFF => false,
            0xFFFE => true,
            _ => throw new Exception("Endianness BOM was not valid!")
        };
}