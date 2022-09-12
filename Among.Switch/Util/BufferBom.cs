using System;
using Among.Switch.Buffers;

namespace Among.Switch.Util;

public static class BufferBom {
    public static void SetBomFe(this ref SpanBuffer buffer) =>
        buffer.BigEndian = buffer.ReadU16() switch {
            0xFFFE => false,
            0xFEFF => true,
            _ => throw new Exception("Endianness BOM was not valid!")
        };
    public static void SetBomFf(this ref SpanBuffer buffer) =>
        buffer.BigEndian = buffer.ReadU16() switch {
            0xFFFE => false,
            0xFEFF => true,
            _ => throw new Exception("Endianness BOM was not valid!")
        };

    public static void WriteBom(this ref SpanBuffer buffer) =>
        buffer.WriteU16((ushort) (buffer.BigEndian ? 0xFFFE : 0xFEFF));
}