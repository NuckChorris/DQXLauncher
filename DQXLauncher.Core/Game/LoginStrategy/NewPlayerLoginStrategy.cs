using System.Diagnostics.Contracts;
using DQXLauncher.Core.Utils.WebClient;

namespace DQXLauncher.Core.Game.LoginStrategy;

public class NewPlayerLoginStrategy : LoginStrategy, ILoginStepHandler<UsernamePasswordAction>
{
    private WebForm? _loginForm;
    private Type? _expectedActionType;

    public virtual async Task<LoginStep> Step()
    {
        Contract.Assert(_expectedActionType == null);

        _loginForm = await GetLoginForm(new Dictionary<string, string>
        {
            { "dqxmode", "1" }
        });


        _expectedActionType = typeof(UsernamePasswordAction);
        return new AskUsernamePassword();
    }

    public virtual async Task<LoginStep> Step(UsernamePasswordAction action)
    {
        Contract.Assert(_expectedActionType == typeof(UsernamePasswordAction));
        Contract.Assert(_loginForm is not null);

        var web = await GetWebClient();

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