using System.Collections.Immutable;
using OtpNet;

namespace DQXLauncher.Core.Models;

public interface IPlayerCredential<TSelf> where TSelf : IPlayerCredential<TSelf>
{
    public string Token { get; init; }
    public string? Password { get; set; }
    public string? TotpKey { get; set; }

    public static abstract TSelf Load(string token);
    public void Save();
    public void Remove();
    public static abstract ImmutableList<TSelf> GetAll();

    public string ComputeTotp()
    {
        var totp = new Totp(Base32Encoding.ToBytes(TotpKey));
        return totp.ComputeTotp();
    }
}