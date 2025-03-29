using System.Text;
using DQXLauncher.Core.Utils;

namespace DQXLauncher.Core.StreamObfuscator;

public class PlayerListObfuscator(Stream baseStream, string username)
    : XorObfuscator(baseStream, GenerateXorKeyFromUsername(username))
{
    // Player lists are XORed with a key derived from the username (it's just crc32)
    private static byte[] GenerateXorKeyFromUsername(string username)
    {
        if (username is null) throw new ArgumentNullException(nameof(username));
        return Crc32.Compute(Encoding.ASCII.GetBytes($"{username}\0"));
    }
}