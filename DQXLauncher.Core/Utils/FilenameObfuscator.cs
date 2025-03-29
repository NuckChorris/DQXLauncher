using System.Text;

namespace DQXLauncher.Core.Utils;

public static class FilenameObfuscator
{
    private static readonly string DigitMap = "&@#+(_-)]$";
    private static readonly int[] UpperMap =
    [
        0x03, 0x05, 0x14, 0x17, 0x08, 0x18, 0x06, 0x07,
        0x01, 0x0C, 0x02, 0x09, 0x0A, 0x0C, 0x19, 0x0D,
        0x04, 0x0F, 0x15, 0x0E, 0x10, 0x00, 0x11, 0x0B,
        0x16, 0x13
    ];

    private static readonly int[] LowerMap =
    [
        0x12, 0x15, 0x04, 0x17, 0x0B, 0x19, 0x0D, 0x0E,
        0x03, 0x11, 0x0C, 0x10, 0x14, 0x05, 0x07, 0x0F,
        0x08, 0x06, 0x01, 0x09, 0x02, 0x0A, 0x16, 0x13,
        0x00, 0x18
    ];

    public static string Obfuscate(string input, int checksumSeed = 0)
    {
        int checksum = checksumSeed & 0xFF;
        string basename = Path.GetFileName(input);
        List<char> result = [];

        foreach (char ch in basename)
        {
            char c = ch;

            if (ch is >= '0' and <= '9')
            {
                int idx = (checksum + (ch - '0')) % 10;
                c = DigitMap[idx];
            }
            else if (ch is >= 'A' and <= 'Z')
            {
                int idx = (checksum + (ch - 'A')) % 26;
                c = (char)('A' + UpperMap[idx]);
            }
            else if (ch is >= 'a' and <= 'z')
            {
                int idx = (checksum + (ch - 'a')) % 26;
                c = (char)('a' + LowerMap[idx]);
            }
            else if (ch == '.')
            {
                c = '!';
            }
            else if (ch == '*')
            {
                c = '~';
            }

            result.Add(c);
            checksum = (checksum + ch) & 0xFF;
        }

        return new string(result.ToArray());
    }
    public static string Deobfuscate(string obfuscated, int checksumSeed = 0)
    {
        StringBuilder result = new StringBuilder();
        int checksum = checksumSeed & 0xFF;

        foreach (char ch in obfuscated)
        {
            char orig = ch;

            if (DigitMap.Contains(ch))
            {
                int idx = DigitMap.IndexOf(ch);
                int val = (idx - checksum) % 10;
                if (val < 0) val += 10;
                orig = (char)('0' + val);
            }
            else if (ch is >= 'A' and <= 'Z')
            {
                int mapped = ch - 'A';
                int idx = Array.IndexOf(UpperMap, mapped);
                if (idx != -1)
                {
                    int val = (idx - checksum) % 26;
                    if (val < 0) val += 26;
                    orig = (char)('A' + val);
                }
            }
            else if (ch is >= 'a' and <= 'z')
            {
                int mapped = ch - 'a';
                int idx = Array.IndexOf(LowerMap, mapped);
                if (idx != -1)
                {
                    int val = (idx - checksum) % 26;
                    if (val < 0) val += 26;
                    orig = (char)('a' + val);
                }
            }
            else if (ch == '!')
            {
                orig = '.';
            }
            else if (ch == '~')
            {
                orig = '*';
            }

            result.Append(orig);
            checksum = (checksum + orig) & 0xFF;
        }

        return result.ToString();
    }
}
