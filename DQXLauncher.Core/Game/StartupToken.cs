using System.Runtime.InteropServices;

namespace DQXLauncher.Core.Game;

/// <summary>
/// Generates a startup token for launching DQX executables.
/// </summary>
/// <remarks>
/// Most DQX executables (DQXGame.exe, DQXLauncher.exe, etc.) require a startup token to be passed as a command line
/// argument, which is generated from the "boot timer" (how long since the system was booted in milliseconds))
/// </remarks>
public static class StartupToken
{
    private static readonly char[] SqEx = "SqEx".ToCharArray();
        
    [DllImport("winmm.dll", EntryPoint="timeGetTime")]
    private static extern uint GetTime();

    public static string GetStartupToken()
    {
        // The official version of this function actually uses an MT RNG seeded from the Windows "true" RNG to generate
        // these 4 chars, but because that makes it *actually random*, they can't check it and we just stuff 0000 in.
        var baseString = "0000" + (GetTime() >>> 1);
        
        return new string(baseString
            .ToCharArray()
            .Select((person, index) => (char)(person ^ SqEx[index & 3]))
            .ToArray());
    }
}