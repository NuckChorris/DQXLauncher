using System;
using System.Collections.Generic;
using Windows.Gaming.Input;
using DQXLauncher.Windows.Utils;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinUIEx;

namespace DQXLauncher.Windows
{
    public sealed partial class MainWindow : WindowEx
    {
        private List<Gamepad> _gamepads = new();
        public double TitleBarHeight => AppWindow.TitleBar.Height;
        public double TitleBarRightInset => AppWindow.TitleBar.RightInset;
        public double TitleBarLeftInset => AppWindow.TitleBar.LeftInset;

        public MainWindow()
        {
            this.SetIcon("Assets/icon.ico");
            this.InitializeComponent();
            this.SetIsResizable(false);
            this.SetIsMaximizable(false);
            // Workaround for microsoft/microsoft-ui-xaml#9427
            NativeWindowHelper.ForceDisableMaximize(this);
            this.SetTitleBar(TitleBar);
            this.ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
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

        private void Gamepad_GamepadAdded(object sender, Gamepad gamepad)
        {
            if (!_gamepads.Contains(gamepad)) _gamepads.Add(gamepad);
        }

        private void Gamepad_GamepadRemoved(object sender, Gamepad gamepad)
        {
            if (_gamepads.Contains(gamepad)) _gamepads.Remove(gamepad);
        }
    }
}