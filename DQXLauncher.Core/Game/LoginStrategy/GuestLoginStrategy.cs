using System.Diagnostics.Contracts;
using DQXLauncher.Core.Utils.WebClient;

namespace DQXLauncher.Core.Game.LoginStrategy;

public class GuestLoginStrategy : LoginStrategy
{
    private WebForm? _loginForm;
    private Type? _expectedActionType;

    public async Task<LoginStep> Step()
    {
        // Load the login form
        _loginForm = await GetLoginForm(new Dictionary<string, string>
        {
            { "dqxmode", "3" } // Guest mode
        });
        
        // TODO: Handle the case when the login form fails

        _expectedActionType = typeof(UsernamePasswordAction);
        return new AskUsernamePassword();
    }

    public async Task<LoginStep> Step(UsernamePasswordAction action)
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
            return new DisplayError(response.ErrorMessage, new AskUsernamePassword());
        }
        
        return new LoginCompleted();

        // TODO: handle 2FA
    }

}