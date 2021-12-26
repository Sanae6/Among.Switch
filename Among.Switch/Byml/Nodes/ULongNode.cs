namespace Among.Switch.Byml.Nodes;

public struct ULongNode : INode {
    public ulong Value;

    public ULongNode(ulong value) {
        Value = value;
    }
}
