using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DQXLauncher.Core.Game.LoginStrategy;
using DQXLauncher.Core.Models;
using DQXLauncher.Windows.Services;
using Microsoft.UI.Xaml.Controls;

namespace DQXLauncher.Windows.ViewModels;

public abstract class PlayerListItem
{
    public virtual Symbol Icon => Symbol.Help;
    public virtual string Text => null!;
    public abstract Task<LoginStrategy> GetLoginStrategy();
}

public class SavedPlayerItem : PlayerListItem
{
    public required SavedPlayer<PlayerCredential> Player { get; init; }
    public override Symbol Icon => Symbol.Contact;
    public override string Text => Player.Name ?? $"Player {Player.Number}";

    public override async Task<LoginStrategy> GetLoginStrategy()
    {
        return await Player.GetLoginStrategy();
    }
}

public class NewPlayerItem : PlayerListItem
{
    public override Symbol Icon => Symbol.AddFriend;
    public override string Text => "New Player";

    public override Task<LoginStrategy> GetLoginStrategy()
    {
        return Task.FromResult(new NewPlayerLoginStrategy() as LoginStrategy);
    }
}

/*
 @TODO: Implement EasyPlayLoginStrategy
public class TrialPlayerItem : PlayerListItem
{
    public required TrialPlayer Player { get; init; }
    public override Symbol Icon => Symbol.Emoji2;
    public override string Text => Player.Token;
    public override Task<LoginStrategy> GetLoginStrategy() => Task.FromResult(new EasyPlayLoginStrategy(Player.Token) as LoginStrategy);
}
*/

public partial class PlayerListViewModel(PlayerList<PlayerCredential> playerList) : ObservableObject
{
    private PlayerList<PlayerCredential> PlayerList { get; } = playerList;
    public ObservableCollection<PlayerListItem> List { get; } = new();

    [RelayCommand]
    public async Task LoadAsync()
    {
        await PlayerList.LoadAsync();
        RebuildDisplayPlayers();
    }

    public void AddPlayer(string token, string? name)
    {
        var player = PlayerList.Add(token);
        if (name is not null) player.Name = name;

        List.Add(new SavedPlayerItem { Player = player });
    }

    public async Task SaveAsync()
    {
        await PlayerList.SaveAsync();
    }

    private void RebuildDisplayPlayers()
    {
        List.Clear();

        foreach (var savedPlayer in PlayerList.Players) List.Add(new SavedPlayerItem { Player = savedPlayer });

        if (PlayerList.Players.Count < 4) List.Add(new NewPlayerItem());
    }
}