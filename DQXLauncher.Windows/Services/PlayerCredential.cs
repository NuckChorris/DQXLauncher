﻿using System;
using System.Collections.Immutable;
using System.Linq;
using Windows.Security.Credentials;
using DQXLauncher.Core.Models;

namespace DQXLauncher.Windows.Services;

public class PlayerCredential : IPlayerCredential<PlayerCredential>
{
    private static readonly string TotpResource = "DQXLauncher:TOTP";
    private static readonly string PasswordResource = "DQXLauncher:Password";
    private static readonly PasswordVault Vault = new();

    public string Token { get; init; }
    public string? Password { get; set; }
    public string? TotpKey { get; set; }

    private PasswordCredential? PasswordCred
    {
        get
        {
            try
            {
                return Vault.Retrieve(PasswordResource, Token);
            }
            catch (Exception)
            {
                return null;
            }
        }
        set
        {
            if (PasswordCred is not null) Vault.Remove(PasswordCred);
            Vault.Add(value);
        }
    }

    private PasswordCredential? TotpCred
    {
        get
        {
            try
            {
                return Vault.Retrieve(TotpResource, Token);
            }
            catch (Exception)
            {
                return null;
            }
        }
        set
        {
            if (TotpCred is not null) Vault.Remove(TotpCred);
            Vault.Add(value);
        }
    }

    public static PlayerCredential Load(string token)
    {
        var instance = new PlayerCredential { Token = token };
        instance._Load();
        return instance;
    }

    private void _Load()
    {
        PasswordCred?.RetrievePassword();
        TotpCred?.RetrievePassword();

        Password = PasswordCred?.Password;
        TotpKey = TotpCred?.Password;
    }

    public void Save()
    {
        PasswordCred = new PasswordCredential(PasswordResource, Token, Password);
        TotpCred = new PasswordCredential(TotpResource, Token, TotpKey);
    }

    public void Remove()
    {
        if (PasswordCred is not null) Vault.Remove(PasswordCred);
        if (TotpCred is not null) Vault.Remove(TotpCred);
    }

    public static ImmutableList<PlayerCredential> GetAll()
    {
        try
        {
            return Vault.FindAllByResource(PasswordResource)
                .Select(cred => new PlayerCredential { Token = cred.UserName }).ToImmutableList();
        }
        catch (Exception)
        {
            return ImmutableList<PlayerCredential>.Empty;
        }
    }
}