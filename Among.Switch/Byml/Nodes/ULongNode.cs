﻿namespace Among.Switch.Byml.Nodes;

public struct ULongNode : INode {
    public ulong Value { get; set; }

    public ULongNode(ulong value) {
        Value = value;
    }
}
