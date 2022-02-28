namespace Among.Switch.Byml.Nodes;

public struct StringNode : INode {
    public string Value { get; set; }

    public StringNode(string value) {
        Value = value;
    }
}
