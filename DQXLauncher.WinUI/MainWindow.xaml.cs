using System;
using System.Diagnostics;
using DQXLauncher.Core.Game.LoginStrategy;
using DQXLauncher.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DQXLauncher
{
    public sealed partial class MainWindow : WinUIEx.WindowEx
    {
 
        public MainWindow()
        {
            this.InitializeComponent();
            this.SetTitleBar(AppTitleBar);
            this.ExtendsContentIntoTitleBar = true;
            AppFrame.Navigate(typeof(HomePage));
        }

        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
            var login = new GuestLoginStrategy();
            //var result = await login.Login(Username.Text, Password.Text);
            Debug.WriteLine("result");
            var dialog = new ContentDialog
            {
                Content = "result",
                PrimaryButtonText = "OK"
            };
            await dialog.ShowAsync();

            // Process process = new Process();
            // process.StartInfo.FileName = "C:\\Program Files (x86)\\SquareEnix\\DRAGON QUEST X\\Boot\\DQXLauncher.exe";
            // process.StartInfo.Arguments = "-StartupToken=" + this.GetStartupToken();
            // Submit.Content = (GetTime() >>> 1).ToString();
            // process.Start();
        }
    }
}
