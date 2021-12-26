namespace Among.Switch.Byml.Nodes;

public struct BoolNode : INode {
    public bool Value;

    public BoolNode(bool value) {
        Value = value;
    }
}