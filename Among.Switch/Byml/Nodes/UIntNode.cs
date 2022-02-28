namespace Among.Switch.Byml.Nodes;

public struct UIntNode : INode {
    public uint Value { get; set; }

    public UIntNode(uint value) {
        Value = value;
    }
}