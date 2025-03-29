namespace DQXLauncher.Core.Utils;

public static class Crc32
{
    private static readonly uint[] Table = GenerateTable();

    // Standard CRC32 polynomial
    private const uint Polynomial = 0x04C11DB7;

    public static byte[] Compute(byte[] data)
    {
        uint crc = 0; // Initial value is 0

        foreach (byte b in data)
        {
            uint index = ((crc >> 24) ^ b) & 0xFF;
            crc = (crc << 8) ^ Table[index];
        }

        // Convert the resulting uint to a byte array
        return
        [
            (byte)(crc & 0xFF),
            (byte)((crc >> 8) & 0xFF),
            (byte)((crc >> 16) & 0xFF),
            (byte)((crc >> 24) & 0xFF)
        ];
    }

    private static uint[] GenerateTable()
    {
        var table = new uint[256];
        for (uint i = 0; i < 256; i++)
        {
            uint crc = i << 24;
            for (int j = 0; j < 8; j++)
            {
                crc = (crc & 0x80000000) != 0
                    ? (crc << 1) ^ Polynomial
                    : (crc << 1);
            }

            table[i] = crc;
        }

        return table;
    }
}