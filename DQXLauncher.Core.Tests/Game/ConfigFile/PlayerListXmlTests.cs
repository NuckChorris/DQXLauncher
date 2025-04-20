using DQXLauncher.Core.Game.ConfigFile;

namespace DQXLauncher.Core.Tests.Game.ConfigFile;

public class PlayerListXmlTests
{
    private class TempDirectory : IDisposable
    {
        public string Path { get; }

        public TempDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
            Directory.CreateDirectory(Path);
        }

        public void Dispose()
        {
            Directory.Delete(Path, true);
        }
    }

    [Fact]
    public async Task Load_DefaultFile_CreatesDefaultStructure()
    {
        using var tempDir = new TempDirectory();
        Core.Game.ConfigFile.ConfigFile.RootDirectory = tempDir.Path;

        var playerList = await PlayerListXml.OpenAsync();

        Assert.NotNull(playerList.Document);
        Assert.NotNull(playerList.Players);
        Assert.Empty(playerList.Players);
    }

    [Fact]
    public async Task AddPlayer_SavedPlayer_PersistsToFile()
    {
        using var tempDir = new TempDirectory();
        Core.Game.ConfigFile.ConfigFile.RootDirectory = tempDir.Path;

        var playerList = await PlayerListXml.OpenAsync();
        playerList.Add(new PlayerListXml.SavedPlayer
        {
            Number = 1,
            Token = "test-token"
        });
        await playerList.Save();

        await playerList.LoadAsync();
        Assert.Single(playerList.Players);
        var reloadedPlayer = Assert.IsType<PlayerListXml.SavedPlayer>(playerList.Players[0]);
        Assert.Equal(1, reloadedPlayer.Number);
        Assert.Equal("test-token", reloadedPlayer.Token);
    }

    [Fact]
    public async Task Trial_Set_PersistsToFile()
    {
        using var tempDir = new TempDirectory();
        Core.Game.ConfigFile.ConfigFile.RootDirectory = tempDir.Path;

        var playerList = await PlayerListXml.OpenAsync();
        playerList.Trial = new PlayerListXml.TrialPlayer
        {
            Id = "trial-id",
            Token = "trial-token",
            Code = "trial-code"
        };
        await playerList.Save();

        await playerList.LoadAsync();
        var reloadedTrialPlayer = Assert.IsType<PlayerListXml.TrialPlayer>(playerList.Trial);
        Assert.Equal("trial-id", reloadedTrialPlayer.Id);
        Assert.Equal("trial-token", reloadedTrialPlayer.Token);
        Assert.Equal("trial-code", reloadedTrialPlayer.Code);
    }

    [Fact]
    public async Task Filename_HasExpectedValue()
    {
        using var tempDir = new TempDirectory();
        Core.Game.ConfigFile.ConfigFile.RootDirectory = tempDir.Path;

        var playerList = await PlayerListXml.OpenAsync();
        var expectedFilename = Path.Combine(tempDir.Path, "cxjYxsgheGzie!iyx");
        Assert.Equal(expectedFilename, playerList.Filename);
    }
}