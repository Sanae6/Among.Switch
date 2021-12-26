namespace Among.Switch.Byml.Nodes;

public struct SingleNode : INode {
    public float Value;

    public SingleNode(float value) {
        Value = value;
    }
}