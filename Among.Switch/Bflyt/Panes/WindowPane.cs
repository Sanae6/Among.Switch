using Among.Switch.Buffers;

namespace Among.Switch.Bflyt.Panes; 

[LayoutSection("wnd1")]
public class WindowPane : Pane {

    public override void Load(ref SpanBuffer slice) {
        
    }
    public override SpanBuffer Save(bool bigEndian) {
        return new SpanBuffer();
    }
}