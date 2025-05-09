using System.Collections.Immutable;
using OtpNet;

namespace DQXLauncher.Core.Models;

public interface IPlayerCredential
{
    public string Token { get; init; }
    public string? Password { get; set; }
    public string? TotpKey { get; set; }

    public void Save();
    public void Remove();

    public string ComputeTotp()
    {
        var totp = new Totp(Base32Encoding.ToBytes(TotpKey));
        return totp.ComputeTotp();
    }
}

public interface IPlayerCredential<TSelf> : IPlayerCredential where TSelf : IPlayerCredential<TSelf>
{
    public static abstract TSelf Load(string token);
    public static abstract ImmutableList<TSelf> GetAll();
}