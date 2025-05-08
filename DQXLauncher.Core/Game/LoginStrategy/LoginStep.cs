﻿namespace DQXLauncher.Core.Game.LoginStrategy;

public abstract record LoginStep;

public record RestartStrategy : LoginStep;

public record AskUsernamePassword(string? Username = null, string? Password = null) : LoginStep;

public record AskPassword(string Username, string? Password = null) : LoginStep;

public record AskOtp(string Username) : LoginStep;

public record AskEasyPlay : LoginStep;

public record DisplayError(string Message, LoginStep Continue) : LoginStep;

public record LoginCompleted(string SessionId) : LoginStep
{
    public string? Token { get; init; }
}