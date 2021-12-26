using System.Collections;
using System.Collections.Generic;

namespace Among.Switch.Byml.Nodes;

public struct DictionaryNode : INode, IContainerNode<KeyValuePair<string, INode>> {
    public Dictionary<string, INode> Children = new Dictionary<string, INode>();
    public IEnumerator<KeyValuePair<string, INode>> GetEnumerator() => Children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
