namespace Among.Switch.Byml.Nodes;

public struct BoolNode : INode {
    public bool Value { get; set; }

    public BoolNode(bool value) {
        Value = value;
    }
}