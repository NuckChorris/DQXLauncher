using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using DQXLauncher.Core.Game;
using DQXLauncher.Core.Game.LoginStrategy;
using DQXLauncher.Core.Utils;
using Microsoft.UI.Xaml.Controls;
namespace DQXLauncher.Pages
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
        }

        private async void Submit_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var login = new GuestLoginStrategy();
            //var auth = await login.Login(Username.Text, Password.Text);
            //var sessionId = login.EncodeSessionId(auth);
            var sessionId = "string";
            var startupToken = StartupToken.GetStartupToken();
            var gamePath = Path.Combine(InstallInfo.Location, "game", "DQXGame.exe");
            Process process = new Process();
            process.StartInfo.WorkingDirectory = Path.Combine(InstallInfo.Location, "game");
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = gamePath;
            process.StartInfo.Arguments = $"-SessionID={sessionId} -StartupToken={startupToken} -PlayerNumber=0 -USE_APARTMENTTHREADED";
            process.Start();
        }

        private async void CRC_User(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var user = Encoding.ASCII.GetBytes($"{Environment.UserName}\0");
            var crc = BitConverter.ToString(Crc32.Compute(user));
            Username.Text = $"{BitConverter.ToString(user)}={crc}";
        }

    }
}
