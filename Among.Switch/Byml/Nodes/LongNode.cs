namespace Among.Switch.Byml.Nodes;

public struct LongNode : INode {
    public long Value;

    public LongNode(long value) {
        Value = value;
    }
}
