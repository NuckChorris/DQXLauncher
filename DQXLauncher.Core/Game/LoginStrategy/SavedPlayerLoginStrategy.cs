using System.Diagnostics.Contracts;
using DQXLauncher.Core.Utils.WebClient;

namespace DQXLauncher.Core.Game.LoginStrategy;

public class SavedPlayerLoginStrategy : LoginStrategy, ILoginStepHandler<PasswordAction>
{
    private WebForm? _loginForm;
    private Type? _expectedActionType;
    private string? _token;

    public virtual async Task<LoginStep> Start(string token)
    {
        _token = token;

        try
        {
            _loginForm = await GetLoginForm(new Dictionary<string, string>
            {
                { "dqxmode", "2" },
                { "id", token }
            });
        }
        catch (Exception)
        {
            // @TODO: Log the error
            return new DisplayError("Failed to load login form", new RestartStrategy());
        }

        var username = _loginForm.Fields["sqexid"];

        _expectedActionType = typeof(PasswordAction);
        return new AskPassword(username);
    }

    public override async Task<LoginStep> Restart()
    {
        if (_token is null) throw new InvalidOperationException("Token is null");

        return await Start(_token);
    }

    public virtual async Task<LoginStep> Step(PasswordAction action)
    {
        Contract.Assert(_expectedActionType == typeof(PasswordAction));
        Contract.Assert(_loginForm is not null);

        var web = await GetWebClient();

        _loginForm.Fields["password"] = action.Password;

        var response = await LoginResponse.FromHttpResponse(await web.SendFormAsync(_loginForm));

        if (response.ErrorMessage is not null && response.Form is not null)
        {
            _loginForm = response.Form;
            return new DisplayError(response.ErrorMessage, new AskPassword(
                _loginForm.Fields["sqexid"],
                action.Password));
        }

        if (response.SessionId is null && response.Form is not null)
        {
            _loginForm = response.Form;
            return new DisplayError("Login failed", new AskPassword(
                _loginForm.Fields["sqexid"],
                action.Password));
        }

        if (response.SessionId is null) return new DisplayError("Login failed", new RestartStrategy());

        return new LoginCompleted(response.SessionId);
    }
}