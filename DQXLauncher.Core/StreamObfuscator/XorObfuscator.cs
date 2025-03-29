namespace DQXLauncher.Core.StreamObfuscator;

public class XorObfuscator : Stream
{
    private readonly Stream _baseStream;
    private readonly byte[] _key;
    private long _position;

    public XorObfuscator(Stream baseStream, byte[] key)
    {
        if (key == null || key.Length == 0)
            throw new ArgumentException("Key must be a non-empty byte array.", nameof(key));

        _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
        _key = key;
        _position = 0;
    }

    public override bool CanRead => _baseStream.CanRead;
    public override bool CanSeek => _baseStream.CanSeek;
    public override bool CanWrite => _baseStream.CanWrite;
    public override long Length => _baseStream.Length;

    public override long Position 
    { 
        get => _baseStream.Position;
        set
        {
            _baseStream.Position = value;
            _position = value;
        }
    }

    public override void Flush() => _baseStream.Flush();

    public override int Read(byte[] buffer, int offset, int count)
    {
        int read = _baseStream.Read(buffer, offset, count);
        for (int i = 0; i < read; i++)
        {
            byte keyByte = _key[(_position + i) % _key.Length];
            byte current = buffer[offset + i];

            if (current != 0x00 && current != keyByte)
            {
                buffer[offset + i] = (byte)(current ^ keyByte);
            }
        }
        _position += read;
        return read;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        byte[] tempBuffer = new byte[count];
        for (int i = 0; i < count; i++)
        {
            byte keyByte = _key[(_position + i) % _key.Length];
            byte current = buffer[offset + i];

            tempBuffer[i] = (current != 0x00 && current != keyByte)
                ? (byte)(current ^ keyByte)
                : current;
        }
        _baseStream.Write(tempBuffer, 0, count);
        _position += count;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        long newPos = _baseStream.Seek(offset, origin);
        _position = newPos;
        return newPos;
    }

    public override void SetLength(long value) => _baseStream.SetLength(value);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _baseStream.Dispose();
        }
        base.Dispose(disposing);
    }
}
