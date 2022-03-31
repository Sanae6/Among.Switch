using System;
using System.Buffers;
using System.Diagnostics;
using Among.Switch.Buffers;

namespace Among.Switch.Util;

// http://wiki.tockdom.com/wiki/Yaz0_(File_Format)
public static class YazUtility {
    public const uint YazMagic = 0x59617A30;

    public static SpanBuffer Decompress(Span<byte> srcData) {
        SpanBuffer src = new SpanBuffer(srcData, true);

        if (src.ReadU32() != YazMagic) throw new Exception("Buffer is not a Yaz0 compressed archive");

        int size = (int) src.ReadU32();
        src.Offset = 0x10;
        Span<byte> destData = MemoryPool<byte>.Shared.Rent(size).Memory.Span[..size];
        SpanBuffer dest = new SpanBuffer(destData, true);

        {
            byte groupHead = 0;
            int groupHeadLen = 0;
            while (src.HasLeft && dest.HasLeft) {
                if (groupHeadLen == 0) {
                    groupHead = src.ReadU8();
                    groupHeadLen = 8;
                }

                groupHeadLen--;

                if ((groupHead & 0x80) != 0) {
                    dest.WriteU8(src.ReadU8());
                } else {
                    byte b1 = src.ReadU8();
                    byte b2 = src.ReadU8();
                    int offset = (((b1 & 0xF) << 8) | b2) + 1;
                    int len = b1 >> 4;
                    if (len == 0) {
                        len = src.ReadU8() + 0x12;
                    } else {
                        len = (len & 0xF) + 2;
                    }

                    Debug.Assert(len is >= 3 and <= 0x111);
                    if (offset < 0 || dest.Offset + len > dest.Size)
                        throw new Exception("Corrupted data!\n");

                    for (int n = 0; n < len; ++n) {
                        int copyOffset = dest.Offset - offset;
                        if (copyOffset < 0)
                            copyOffset = 0;

                        dest.WriteU8(destData[copyOffset]);
                    }
                }

                groupHead <<= 1;
            }
        }
        dest.Offset = 0;
        dest.BigEndian = false;
        return dest;
    }

    public static Span<byte> Compress(Span<byte> srcData) {
        SpanBuffer srcBuffer = new SpanBuffer(srcData, true);
        SpanBuffer destBuffer = new SpanBuffer(new byte[0x10 + srcData.Length + srcData.Length], true);
        destBuffer.WriteU32(YazMagic);
        destBuffer.WriteU32((uint) srcBuffer.Size);
        destBuffer.Offset += 8;

        for (int i = 0; i < srcBuffer.Size; i++) {
            destBuffer.WriteU8(0x80);
            destBuffer.WriteU8(srcBuffer.ReadU8());
        }

        return destBuffer.Buffer;
    }
}