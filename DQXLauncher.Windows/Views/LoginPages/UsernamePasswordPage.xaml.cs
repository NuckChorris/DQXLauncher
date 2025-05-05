using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DQXLauncher.Core.Game.LoginStrategy;
using DQXLauncher.Windows.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DQXLauncher.Windows.Views.LoginPages;

public sealed partial class UsernamePasswordPage : Page
{
    public LoginFrameViewModel ViewModel { get; set; }

    public UsernamePasswordPage()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<LoginFrameViewModel>();
    }

    [RelayCommand]
    private async Task SubmitLogin()
    {
        var action = new UsernamePasswordAction(Username.Text, Password.Password);
        ViewModel.Forward(await ViewModel.Strategy.Step(action));
    }
}