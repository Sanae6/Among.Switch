namespace Among.Switch.Byml.Nodes;

public struct IntNode : INode {
    public int Value;

    public IntNode(int value) {
        Value = value;
    }
}