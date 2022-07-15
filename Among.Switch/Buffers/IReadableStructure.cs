namespace Among.Switch.Buffers;

public interface IReadableStructure {
    void Load(SpanBuffer slice);
    SpanBuffer Save(bool bigEndian);
}
