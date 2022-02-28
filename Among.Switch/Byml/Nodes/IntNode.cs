namespace Among.Switch.Byml.Nodes;

public struct IntNode : INode {
    public int Value { get; set; }

    public IntNode(int value) {
        Value = value;
    }
}