using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DQXLauncher.Core.Models;
using DQXLauncher.Windows.Services;
using Microsoft.UI.Xaml.Controls;

namespace DQXLauncher.Windows.ViewModels;

public class PlayerListItem
{
    public virtual Symbol Icon => Symbol.Help;
    public virtual string Text => null!;
}

public class SavedPlayerItem : PlayerListItem
{
    public required SavedPlayer<PlayerCredential> Player { get; init; }
    public override Symbol Icon => Symbol.Contact;
    public override string Text => Player.Name ?? $"Player {Player.Number}";
}

public class GuestPlayerItem : PlayerListItem
{
    public override Symbol Icon => Symbol.AddFriend;
    public override string Text => "New Player";
}

public class TrialPlayerItem : PlayerListItem
{
    public required TrialPlayer Player { get; init; }
    public override Symbol Icon => Symbol.Emoji2;
    public override string Text => Player.Token;
}

public partial class PlayerListViewModel : ObservableObject
{
    private PlayerList<PlayerCredential>? _playerList;

    private PlayerList<PlayerCredential>? PlayerList
    {
        get => _playerList;
        set => SetProperty(ref _playerList, value);
    }

    public ObservableCollection<PlayerListItem> List { get; } = new();

    [RelayCommand]
    public async Task LoadAsync()
    {
        PlayerList = await PlayerList<PlayerCredential>.LoadAsync();
        RebuildDisplayPlayers();
    }

    private void RebuildDisplayPlayers()
    {
        List.Clear();

        if (PlayerList is null) return;

        foreach (var savedPlayer in PlayerList.Players) List.Add(new SavedPlayerItem { Player = savedPlayer });

        if (PlayerList.Players.Count < 4) List.Add(new GuestPlayerItem());
    }
}