using System.Xml.Linq;
using DQXLauncher.Core.Game.ConfigFile;

namespace DQXLauncher.Core.Tests.Game.ConfigFile;

public class PlayerListTests
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

        using var playerList = await PlayerList.OpenAsync();

        Assert.NotNull(playerList.Document);
        Assert.NotNull(playerList.Players);
        Assert.Empty(playerList.Players);
    }

    [Fact]
    public async Task AddPlayer_SavedPlayer_PersistsToFile()
    {
        using var tempDir = new TempDirectory();
        Core.Game.ConfigFile.ConfigFile.RootDirectory = tempDir.Path;

        using var playerList = await PlayerList.OpenAsync();
        var newPlayerElement = new XElement("Player");
        var savedPlayer = new SavedPlayer(newPlayerElement)
        {
            Number = 1,
            Token = "test-token"
        };

        playerList.Players.Add(savedPlayer);
        playerList.Document?.Root?.Element("PlayerList")?.Add(newPlayerElement);
        await playerList.Save();

        await playerList.Load();
        Assert.Single(playerList.Players);
        var reloadedPlayer = Assert.IsType<SavedPlayer>(playerList.Players[0]);
        Assert.Equal(1, reloadedPlayer.Number);
        Assert.Equal("test-token", reloadedPlayer.Token);
    }

    [Fact]
    public async Task AddPlayer_TrialPlayer_PersistsToFile()
    {
        using var tempDir = new TempDirectory();
        Core.Game.ConfigFile.ConfigFile.RootDirectory = tempDir.Path;

        using var playerList = await PlayerList.OpenAsync();
        var newTrialElement = new XElement("TrialInfo");
        var trialPlayer = new TrialPlayer(newTrialElement)
        {
            Id = "trial-id",
            Token = "trial-token",
            Code = "trial-code"
        };

        playerList.Players.Add(trialPlayer);
        playerList.Document?.Root?.Element("PlayerList")?.Add(newTrialElement);
        await playerList.Save();
        
        await playerList.Load();
        Assert.Single(playerList.Players);
        var reloadedTrialPlayer = Assert.IsType<TrialPlayer>(playerList.Players[0]);
        Assert.Equal("trial-id", reloadedTrialPlayer.Id);
        Assert.Equal("trial-token", reloadedTrialPlayer.Token);
        Assert.Equal("trial-code", reloadedTrialPlayer.Code);
    }

    [Fact]
    public async Task Filename_HasExpectedValue()
    {
        using var tempDir = new TempDirectory();
        Core.Game.ConfigFile.ConfigFile.RootDirectory = tempDir.Path;

        using var playerList = await PlayerList.OpenAsync();
        var expectedFilename = Path.Combine(tempDir.Path, "cxjYxsgheGzie!iyx");
        Assert.Equal(expectedFilename, playerList.Filename);
    }

    [Fact]
    public async Task PlayerList_AddPlayer_NotifiesCollectionChanged()
    {
        using var tempDir = new TempDirectory();
        Core.Game.ConfigFile.ConfigFile.RootDirectory = tempDir.Path;

        using var playerList = await PlayerList.OpenAsync();
        bool collectionChanged = false;

        playerList.Players.CollectionChanged += (_, _) => collectionChanged = true;

        var newPlayer = new SavedPlayer
        {
            Number = 1,
            Token = "test-token"
        };

        playerList.Players.Add(newPlayer);

        Assert.True(collectionChanged);
    }

    [Fact]
    public async Task PlayerList_RemovePlayer_NotifiesCollectionChanged()
    {
        using var tempDir = new TempDirectory();
        Core.Game.ConfigFile.ConfigFile.RootDirectory = tempDir.Path;

        using var playerList = await PlayerList.OpenAsync();
        var newPlayer = new SavedPlayer
        {
            Number = 1,
            Token = "test-token"
        };

        playerList.Players.Add(newPlayer);

        bool collectionChanged = false;
        playerList.Players.CollectionChanged += (_, _) => collectionChanged = true;

        playerList.Players.Remove(newPlayer);

        Assert.True(collectionChanged);
    }

    [Fact]
    public void Player_PropertyChange_NotifiesPropertyChanged()
    {
        var player = new SavedPlayer
        {
            Number = 1,
            Token = "test-token"
        };

        bool propertyChanged = false;
        player.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SavedPlayer.Token))
            {
                propertyChanged = true;
            }
        };

        player.Token = "new-token";

        Assert.True(propertyChanged);
    }
}
