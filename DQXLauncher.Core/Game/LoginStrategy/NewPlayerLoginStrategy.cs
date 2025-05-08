using System.Diagnostics.Contracts;
using DQXLauncher.Core.Utils.WebClient;

namespace DQXLauncher.Core.Game.LoginStrategy;

public class NewPlayerLoginStrategy : LoginStrategy, ILoginStepHandler<UsernamePasswordAction>
{
    private WebForm? _loginForm;
    private Type? _expectedActionType;
    public string? Username { get; private set; }

    public virtual async Task<LoginStep> Start()
    {
        try
        {
            _loginForm = await GetLoginForm(new Dictionary<string, string>
            {
                { "dqxmode", "1" }
            });
        }
        catch (Exception)
        {
            // @TODO: Log the error
            return new DisplayError("Failed to load login form", new RestartStrategy());
        }

        _expectedActionType = typeof(UsernamePasswordAction);
        return new AskUsernamePassword();
    }

    public override async Task<LoginStep> Restart()
    {
        return await Start();
    }

    public virtual async Task<LoginStep> Step(UsernamePasswordAction action)
    {
        Contract.Assert(_expectedActionType == typeof(UsernamePasswordAction));
        Contract.Assert(_loginForm is not null);

        var web = await GetWebClient();

        Username = action.Username;
        _loginForm.Fields["sqexid"] = action.Username;
        _loginForm.Fields["password"] = action.Password;

        var response = await LoginResponse.FromHttpResponse(await web.SendFormAsync(_loginForm));

        if (response.ErrorMessage is not null)
        {
            _loginForm = response.Form;
            return new DisplayError(response.ErrorMessage, new AskUsernamePassword(action.Username, action.Password));
        }

        if (response.SessionId is null)
        {
            _loginForm = response.Form;
            return new DisplayError("Login failed", new AskUsernamePassword(action.Username, action.Password));
        }

        return new LoginCompleted(response.SessionId) { Token = response.Token };
    }
}