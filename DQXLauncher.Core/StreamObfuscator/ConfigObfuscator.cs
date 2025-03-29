namespace DQXLauncher.Core.StreamObfuscator;

public class ConfigObfuscator(Stream baseStream) : XorObfuscator(baseStream, [0xA7])
{
    // Common config files are just XORed with 0xA7
}