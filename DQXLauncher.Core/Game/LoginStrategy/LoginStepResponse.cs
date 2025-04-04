namespace DQXLauncher.Core.Game.LoginStrategy;

public abstract record LoginStepAction;
public record UsernamePasswordAction(string Username, string Password) : LoginStepAction;
public record PasswordAction(string Password) : LoginStepAction;
public record AskOtpResponse(string Otp) : LoginStepAction;
public record AskEasyPlayResponse() : LoginStepAction;