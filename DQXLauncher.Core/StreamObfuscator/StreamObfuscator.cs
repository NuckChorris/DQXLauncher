namespace DQXLauncher.Core.StreamObfuscator;

public abstract class StreamObfuscator(Stream baseStream) : Stream
{
    protected readonly Stream BaseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
    protected long ProcessedPosition;
    
    public override bool CanRead => BaseStream.CanRead;
    public override bool CanSeek => BaseStream.CanSeek;
    public override bool CanWrite => BaseStream.CanWrite;
    public override long Length => BaseStream.Length;

    public override long Position
    {
        get => BaseStream.Position;
        set
        {
            BaseStream.Position = value;
            ProcessedPosition = value;
        }
    }
    
    public override void Flush() => BaseStream.Flush();
    
    public override long Seek(long offset, SeekOrigin origin)
    {
        long newPos = BaseStream.Seek(offset, origin);
        ProcessedPosition = newPos;
        return newPos;
    }
    
    public override void SetLength(long value) => BaseStream.SetLength(value);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            BaseStream.Dispose();
        }
        base.Dispose(disposing);
    }
}