namespace Among.Switch.Byml; 

public enum NodeTypes : byte {
    StringIndex = 0xA0,
    BinaryData = 0xA1,
    Array = 0xC0,
    Dictionary = 0xC1,
    StringTable = 0xC2,
    Bool = 0xD0,
    Int = 0xD1,
    Single = 0xD2,
    UInt = 0xD3,
    Long = 0xD4,
    ULong = 0xD5,
    Double = 0xD6,
    Null = 0xFF
}