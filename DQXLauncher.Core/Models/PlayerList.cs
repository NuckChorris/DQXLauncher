﻿using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using DQXLauncher.Core.Game.ConfigFile;
using DQXLauncher.Core.Game.LoginStrategy;
using DQXLauncher.Core.Services;

namespace DQXLauncher.Core.Models;

/// <summary>
///     A player registered in Dragon Quest X. Backed by records in three different places: our own PlayerList.json, the
///     official dqxPlayerList.xml, and an implementation of IPlayerCredential. Modifies the records in-place.
/// </summary>
/// <param name="json">The PlayerList.json record</param>
/// <param name="xml">The dqxPlayerList.xml record</param>
/// <param name="credential">The IPlayerCredential record</param>
/// <typeparam name="TCredential">An OS-specific implementation of IPlayerCredential</typeparam>
public class SavedPlayer<TCredential>(
    PlayerListJson.SavedPlayer json,
    PlayerListXml.SavedPlayer xml,
    TCredential credential)
    where TCredential : class, IPlayerCredential<TCredential>, new()
{
    private readonly TCredential _credential = credential;
    private readonly PlayerListXml.SavedPlayer _xml = xml;
    private readonly PlayerListJson.SavedPlayer _json = json;

    /// <summary>
    ///     Create a new player with the given token. It is generally recommended to use
    ///     <see cref="PlayerList{TCredential}.Add(string)" /> instead, since it handles number allocation.
    /// </summary>
    /// <param name="token">The token to assign</param>
    public SavedPlayer(string token) : this(new PlayerListJson.SavedPlayer { Token = token },
        new PlayerListXml.SavedPlayer { Token = token }, new TCredential { Token = token })
    {
    }

    /// <summary>
    ///     The number assigned to this player.
    /// </summary>
    /// <remarks>
    ///     This is the number used in the official launcher to identify the player, and it is stored in the XML file. It
    ///     seems to be one-indexed, and does not align with the folder used for the save file, which is zero-indexed.
    ///     Honestly, it might be entirely for displaying "Player 1"?
    /// </remarks>
    public int? Number
    {
        get => _json.Number;
        set
        {
            if (value is null) return;
            _xml.Number = (int)value;
            _json.Number = (int)value;
        }
    }

    /// <summary>
    ///     The name (sqexid) of the Player.
    /// </summary>
    /// <remarks>
    ///     This is not stored by the official launcher, but we store it in <see cref="PlayerListJson">PlayerList.json</see>
    ///     to display in our UI. It is not used for anything else.
    /// </remarks>
    public string? Name
    {
        get => _json.Name;
        set => _json.Name = value;
    }

    /// <summary>
    ///     The token identifying the player.
    /// </summary>
    /// <remarks>
    ///     This is the same token used in the official launcher, and is stored in both the XML and JSON files. It is a
    ///     base64 string which seem to uniquely identify the user by sqexid.
    /// </remarks>
    public string Token => _json.Token;

    /// <summary>
    ///     The player's password, if saved.
    /// </summary>
    /// <remarks>
    ///     This is stored in the <see cref="IPlayerCredential{TSelf}" /> implementation, and is not stored in the XML or
    ///     JSON files.
    /// </remarks>
    public string? Password
    {
        get => _credential.Password;
        set => _credential.Password = value;
    }

    /// <summary>
    ///     The TOTP key for the player, if saved.
    /// </summary>
    /// <remarks>
    ///     This is a base32 string which is used to generate a TOTP code for the player. It is stored in the
    ///     <see cref="IPlayerCredential{TSelf}" /> implementation, and is not stored in the XML or JSON files.
    /// </remarks>
    public string? TotpKey
    {
        get => _credential.TotpKey;
        set => _credential.TotpKey = value;
    }

    public async Task<SavedPlayerLoginStrategy> GetLoginStrategy()
    {
        var strategy = new SavedPlayerLoginStrategy();
        await strategy.Step(Token);
        return strategy;
    }
}

/// <summary>
///     A player using "Easy Play"
/// </summary>
/// <remarks>
///     There's only ever one of these, and it's stored in the XML file with the TrialInfo tag. We don't really know how
///     all of these work, and we haven't implemented it yet.
/// </remarks>
public class TrialPlayer
{
    /// <summary>
    ///     This appears to be a Device ID, since the request to /create/token has this as the x-cis-device-id header.
    ///     Unfortunately it doesn't appear to be the computer ID used in the User-Agent.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///     This is the `device_token` we get from /api/device/create/token
    /// </summary>
    public required string Token { get; init; }

    /// <summary>
    ///     This is the `supportid` we get from submitting the login form.
    /// </summary>
    public required string Code { get; init; }
}

/// <summary>
///     This class represents our list of registered players in Dragon Quest X.
/// </summary>
/// <remarks>
///     This provides a superset of the data stored in <see cref="PlayerListXml">dqxPlayerList.xml</see>, with additional
///     properties such as the username, password, OTP key, etc. As such, we store this data in our own
///     <see cref="PlayerListJson">PlayerList.json</see> and an OS-specific implementation of
///     <see cref="IPlayerCredential{TSelf}" /> which is used to store the password and TOTP key securely. This class
///     coordinates the three, when you save it you save all three. When loading, the XML file is used as the source of
///     truth, and the JSON and PlayerCredential are updated to match. This means removing a player from the XML (using the
///     official launcher) will also remove it from the JSON on next startup.
/// </remarks>
public class PlayerList<TCredential> where TCredential : class, IPlayerCredential<TCredential>, new()
{
    /// <summary>
    ///     The official launcher only supports 4 players. This exception is thrown when you try to add a 5th player.
    /// </summary>
    /// <remarks>
    ///     Ideally we should catch it in the UI and prevent the user from adding a 5th player, so this is a last-ditch
    ///     protection.
    /// </remarks>
    public class PlayerLimitReached() : Exception("Player limit reached");

    private PlayerListXml? _xml;
    private PlayerListJson? _json;
    private ImmutableList<TCredential>? _credentials;

    public ObservableCollection<SavedPlayer<TCredential>> Players { get; private set; }

    /// <summary>
    ///     Add a new player to the list by their token. This will add the player to the XML, JSON, and credential store but
    ///     NOT save them to disk until you call <see cref="SaveAsync" />.
    /// </summary>
    /// <param name="token">The sqex token for a saved player</param>
    /// <returns>The SavedPlayer that was created</returns>
    public SavedPlayer<TCredential> Add(string token)
    {
        var xml = new PlayerListXml.SavedPlayer { Token = token, Number = GetAvailableNumber() };
        var json = new PlayerListJson.SavedPlayer { Token = token, Number = GetAvailableNumber() };
        var credential = new TCredential { Token = token };

        var player = new SavedPlayer<TCredential>(json, xml, credential);
        Add(player);
        return player;
    }

    /// <summary>
    ///     Add a SavedPlayer to the list. This will add the player to the XML, JSON, and credential store but NOT save them
    ///     until you call <see cref="SaveAsync" />.
    /// </summary>
    /// <param name="player">The SavedPlayer to add</param>
    public void Add(SavedPlayer<TCredential> player)
    {
        Players.Add(player);
    }

    /// <summary>
    ///     Remove a player from the list by their token. This will remove the player from the XML, JSON, and credential
    ///     store but NOT save them to disk until you call <see cref="SaveAsync" />.
    /// </summary>
    /// <param name="token">The token identifying the player to remove</param>
    public void Remove(string token)
    {
        var player = Players.ToList().Find(p => p.Token == token);
        if (player is null) return;

        Remove(player);
    }

    /// <summary>
    ///     Remove a SavedPlayer from the list. This will remove the player from the XML, JSON, and credential store but NOT
    ///     save them to disk until you call <see cref="SaveAsync" />.
    /// </summary>
    /// <param name="player">The player to remove</param>
    public void Remove(SavedPlayer<TCredential> player)
    {
        Players.Remove(player);
    }

    /// <summary>
    ///     Save the PlayerList to disk. This will save the XML, JSON, and credential store.
    /// </summary>
    /// <remarks>
    ///     When saving the JSON, if any players don't have a Name yet, their name will be loaded by making an HTTP request
    ///     to the login form with their token. It is recommended to set the name if you have it, to avoid hammering their
    ///     servers.
    /// </remarks>
    public async Task SaveAsync()
    {
        Contract.Assert(_xml is not null);
        Contract.Assert(_json is not null);

        await Task.WhenAll(_xml.SaveAsync(), _json.SaveAsync());
    }

    /// <summary>
    ///     Load the PlayerList from disk. This will load the XML, JSON, and credential store, sync it all to the XML, and
    ///     provide the PlayerList in response.
    /// </summary>
    /// <returns>The loaded PlayerList instance</returns>
    public static async Task<PlayerList<TCredential>> LoadAsync()
    {
        var instance = new PlayerList<TCredential>();
        await instance._LoadAsync();
        await instance.SyncFromXml();
        instance.Players = new ObservableCollection<SavedPlayer<TCredential>>(
            instance._json!.Players.Values.Select(jsonPlayer =>
            {
                var xmlPlayer = instance._xml!.Players[jsonPlayer.Token];
                var credential = TCredential.Load(jsonPlayer.Token);

                return new SavedPlayer<TCredential>(jsonPlayer, xmlPlayer, credential);
            }));
        await instance.SaveAsync();
        return instance;
    }

    private int GetAvailableNumber()
    {
        if (Players.Count >= 4) throw new PlayerLimitReached();

        var numberSet = new SortedSet<int> { 1, 2, 3, 4 };
        foreach (var player in Players)
            if (player.Number is not null)
                numberSet.Remove((int)player.Number);

        return numberSet.First();
    }

    private async Task _LoadAsync()
    {
        _xml = await PlayerListXml.LoadAsync();
        _json = await PlayerListJson.LoadAsync();
        _credentials = TCredential.GetAll();
    }

    private async Task SyncFromXml()
    {
        Contract.Assert(_xml is not null);
        Contract.Assert(_json is not null);
        Contract.Assert(_credentials is not null);

        // Delete credentials for any players removed from the XML file
        var xmlTokens = _xml.Players.Keys.ToHashSet();
        var credTokens = _credentials.Select(cr => cr.Token).ToHashSet();
        foreach (var token in credTokens.Except(xmlTokens))
        {
            var cred = new TCredential { Token = token };
            cred.Remove();
        }

        // Recreate JSON player list from XML file
        _json.Players = _xml.Players.Values.Select(xmlPlayer =>
        {
            _json.Players.TryGetValue(xmlPlayer.Token, out var match);

            if (match is not null) return match;

            return new PlayerListJson.SavedPlayer
            {
                Number = xmlPlayer.Number,
                Token = xmlPlayer.Token
            };
        }).ToDictionary(p => p.Token);
        _json.Trial = _xml.Trial is null
            ? null
            : new PlayerListJson.TrialPlayer
            {
                Token = _xml.Trial.Token,
                Id = _xml.Trial.Id,
                Code = _xml.Trial.Code
            };
        await _json.SaveAsync();
    }
}