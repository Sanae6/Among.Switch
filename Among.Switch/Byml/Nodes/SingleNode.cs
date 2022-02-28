namespace Among.Switch.Byml.Nodes;

public struct SingleNode : INode {
    public float Value { get; set; }

    public SingleNode(float value) {
        Value = value;
    }
}