using Among.Switch.Buffers;

namespace Among.Switch.Bflyt.Sections; 

public class MaterialSection : ILayoutSection {
    public string SectionName { get; set; }

    public void Load(SpanBuffer slice) {
        
    }
    public SpanBuffer Save(bool bigEndian) {
        throw new System.NotImplementedException();
    }
}