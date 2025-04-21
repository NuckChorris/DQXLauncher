using System.Diagnostics.Contracts;
using DQXLauncher.Core.Utils.WebClient;

namespace DQXLauncher.Core.Game.LoginStrategy;

public class SavedPlayerLoginStrategy : LoginStrategy
{
    private WebForm? _loginForm;
    private Type? _expectedActionType;

    public virtual async Task<LoginStep> Step(string token)
    {
        Contract.Assert(_expectedActionType == null);

        _loginForm = await GetLoginForm(new Dictionary<string, string>
        {
            { "dqxmode", "2" },
            { "id", token }
        });

        var username = _loginForm.Fields["sqexid"];

        _expectedActionType = typeof(PasswordAction);
        return new AskPassword(username);
    }

    public virtual async Task<LoginStep> Step(PasswordAction action)
    {
        Contract.Assert(_expectedActionType == typeof(PasswordAction));
        Contract.Assert(_loginForm is not null);

        var web = await GetWebClient();

        _loginForm.Fields["password"] = action.Password;

        var response = await LoginResponse.FromHttpResponse(await web.SendFormAsync(_loginForm));

        if (response.ErrorMessage is not null)
        {
            _loginForm = response.Form;
            return new DisplayError(response.ErrorMessage, new AskPassword(response.Form.Fields["sqexid"]));
        }

        return new LoginCompleted();
    }
}