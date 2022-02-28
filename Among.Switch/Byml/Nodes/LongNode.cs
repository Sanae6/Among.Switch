namespace Among.Switch.Byml.Nodes;

public struct LongNode : INode {
    public long Value { get; set; }

    public LongNode(long value) {
        Value = value;
    }
}
