using System.Collections;
using System.Collections.Generic;

namespace Among.Switch.Byml.Nodes;

public struct ArrayNode : INode, IContainerNode<INode> {
    public List<INode> Children = new List<INode>();

    public ArrayNode() { }

    public IEnumerator<INode> GetEnumerator() => Children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
