namespace Among.Switch.Byml.Nodes;

public struct DoubleNode : INode {
    public double Value;

    public DoubleNode(double value) {
        Value = value;
    }
}