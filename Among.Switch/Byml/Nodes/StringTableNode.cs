using System.Collections;
using System.Collections.Generic;

namespace Among.Switch.Byml.Nodes;

public class StringTableNode : INode, IContainerNode<string> {
    public List<string> Children { get; } = new List<string>();
    public IEnumerator<string> GetEnumerator() => Children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
