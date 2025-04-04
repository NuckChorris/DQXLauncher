namespace DQXLauncher.Core.Game.LoginStrategy;

public abstract record LoginStep;
public record AskUsernamePassword : LoginStep;
public record AskPassword : LoginStep;
public record AskOtp : LoginStep;
public record AskEasyPlay : LoginStep;

public partial record DisplayError(string Message, LoginStep? Then) : LoginStep;

public record LoginCompleted : LoginStep;