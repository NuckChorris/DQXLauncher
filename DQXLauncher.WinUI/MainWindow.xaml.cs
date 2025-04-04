using System;
using System.Diagnostics;
using DQXLauncher.Core.Game.LoginStrategy;
using DQXLauncher.Pages;
using Microsoft.Graphics.Display;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIEx.Messaging;
using WinUIEx;
using System.Runtime.InteropServices;
using WinRT.Interop;
using DQXLauncher.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.UI.Xaml.Navigation;

namespace DQXLauncher
{
    public sealed partial class MainWindow : WinUIEx.WindowEx
    {

        public double TitleBarHeight => AppWindow.TitleBar.Height;
        public double TitleBarRightInset => AppWindow.TitleBar.RightInset;
        public double TitleBarLeftInset => AppWindow.TitleBar.LeftInset;

        public MainWindow()
        {
            this.InitializeComponent();
            this.SetIsResizable(false);
            this.SetIsMaximizable(false);
            // Workaround for microsoft/microsoft-ui-xaml#9427
            NativeWindowHelper.ForceDisableMaximize(this);
            this.SetTitleBar(TitleBar);
            this.ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            AppFrame.Navigate(typeof(HomePage));
        }

        private void ResizeToFit()
        {
            var scale = AppGrid.XamlRoot.RasterizationScale;

            var width = (int)Math.Ceiling(AppGrid.DesiredSize.Width * scale);
            var height = (int)Math.Ceiling(AppGrid.DesiredSize.Height * scale);

            AppWindow.ResizeClient(new(width, height));
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is FrameworkElement frame)
            {
                ResizeToFit();
            }
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
