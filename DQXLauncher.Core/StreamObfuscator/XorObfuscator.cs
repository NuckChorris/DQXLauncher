namespace DQXLauncher.Core.StreamObfuscator;

public class XorObfuscator : StreamObfuscator
{
    private readonly byte[] _key;

    public XorObfuscator (Stream baseStream, byte[] key) : base(baseStream)
    {
        if (key == null || key.Length == 0)
            throw new ArgumentException("Key must be a non-empty byte array.", nameof(key));

        _key = key;
    }
    
    public override int Read(byte[] buffer, int offset, int count)
    {
        int read = BaseStream.Read(buffer, offset, count);
        for (int i = 0; i < read; i++)
        {
            byte keyByte = _key[(ProcessedPosition + i) % _key.Length];
            byte current = buffer[offset + i];

            if (current != 0x00 && current != keyByte)
            {
                buffer[offset + i] = (byte)(current ^ keyByte);
            }
        }
        ProcessedPosition += read;
        return read;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        byte[] tempBuffer = new byte[count];
        for (int i = 0; i < count; i++)
        {
            byte keyByte = _key[(ProcessedPosition + i) % _key.Length];
            byte current = buffer[offset + i];

            tempBuffer[i] = (current != 0x00 && current != keyByte)
                ? (byte)(current ^ keyByte)
                : current;
        }
        BaseStream.Write(tempBuffer, 0, count);
        ProcessedPosition += count;
    }
}
