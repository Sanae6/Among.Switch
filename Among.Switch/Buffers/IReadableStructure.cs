namespace Among.Switch.Buffers;

public interface IReadableStructure {
    void Load(ref SpanBuffer slice);
    SpanBuffer Save(bool bigEndian);
}
