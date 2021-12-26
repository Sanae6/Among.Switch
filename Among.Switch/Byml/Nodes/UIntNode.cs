namespace Among.Switch.Byml.Nodes;

public struct UIntNode : INode {
    public uint Value;

    public UIntNode(uint value) {
        Value = value;
    }
}