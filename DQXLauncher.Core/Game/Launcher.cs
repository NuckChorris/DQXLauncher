using System.Text;
using System.Security.Cryptography;

namespace DQXLauncher.Core.Game;

public class Launcher
{
    public string EncodeSessionId(string sid)
    {
        if (!IsValidHex(sid)) throw new ArgumentException("Input must be a 56-character hex string.");

        string timeStr = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 60).ToString();

        string input = $"DQUEST10{sid}";
        byte[] md5 = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes($"{timeStr}DraqonQuestX"));

        byte[] output = new byte[64];
        // Encoding loop
        for (int i = 0; i < 64; i++)
        {
            int ecx = md5[i % 16];
            int eax = i < input.Length ? input[i] : 0;
            ecx -= 48;
            eax += ecx;
            eax %= 78;
            eax += 48;
            output[i] = (byte)eax;
        }

        return Encoding.UTF8.GetString(output, 0, 64);
    }

    private bool IsValidHex(string str)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(str, "^[0-9a-fA-F]{56}$");
    }
}