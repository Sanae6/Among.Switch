namespace Among.Switch.Byml.Nodes;

public struct StringNode : INode {
    public string Value;

    public StringNode(string value) {
        Value = value;
    }
}
