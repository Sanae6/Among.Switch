namespace Among.Switch.Byml.Nodes;

public struct DoubleNode : INode {
    public double Value { get; set; }

    public DoubleNode(double value) {
        Value = value;
    }
}