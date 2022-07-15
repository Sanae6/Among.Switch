using System;
using System.Numerics;
using Among.Switch.Buffers;

namespace Among.Switch.Bflyt.Panes;

public abstract class Pane : ILayoutSection {
    public string SectionName { get; set; }

    public VisibilityFlags Visibility;
    public OriginFlags Origin;
    public byte Alpha;
    public PanelFlags Flags;
    public string Name;
    public string UserInfo;
    public Vector3 Translation;
    public Vector3 Rotation;
    public Vector2 Scale;
    public Vector2 Size;
    public virtual void Load(SpanBuffer slice) {
        Visibility = (VisibilityFlags) slice.ReadU8();
        Origin = (OriginFlags) slice.ReadU8();
        Alpha = slice.ReadU8();
        Flags = (PanelFlags) slice.ReadU8();
    }

    public virtual SpanBuffer Save(bool bigEndian) {
        SpanBuffer buffer = new SpanBuffer(new byte[0x44], bigEndian);
        buffer.WriteU8((byte) Visibility);
        buffer.WriteU8((byte) Origin);
        buffer.WriteU8(Alpha);
        buffer.WriteU8((byte) Flags);
        buffer.WriteStringNull(Name);
        buffer.Offset = 0x1C;
        buffer.WriteStringNull(UserInfo);
        buffer.Offset = 0x24;
        buffer.Write(Translation);
        buffer.Write(Rotation);
        buffer.Write(Scale);
        buffer.Write(Size);
        return buffer;
    }

    [Flags]
    public enum VisibilityFlags : byte {
        Visible = 1 << 0,
        InfluenceAlpha = 1 << 1,
        LocationAdjust = 1 << 2,
        PaneHidden = 1 << 7
    }

    [Flags]
    public enum OriginFlags : byte {
        Visible = 1 << 0,
        InfluenceAlpha = 1 << 1,
        LocationAdjust = 1 << 2,
        PaneHidden = 1 << 7
    }

    [Flags]
    public enum PanelFlags : byte {
        IgnoreMagnificationParts = 1 << 0,
        PartsMagnifyAdjustToPartsBound = 1 << 1,
        ExternalUserDataAnims = 1 << 2,
        ViewerInvisible = 1 << 3,
        IsConstantBufferReadySelf = 1 << 4
    }
}