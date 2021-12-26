namespace Among.Switch.Buffers;

public interface IReadableStructure {
    int Size { get; }
    void Load(SpanBuffer slice);
    void Save(SpanBuffer slice);
}
