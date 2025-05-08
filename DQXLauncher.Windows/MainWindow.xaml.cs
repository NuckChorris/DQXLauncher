using System.Collections.Generic;
using Windows.Gaming.Input;
using DQXLauncher.Windows.Utils;
using Microsoft.UI.Windowing;
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
            SetTitleBar(TitleBar);
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        }

        private void Gamepad_GamepadAdded(object _, Gamepad gamepad)
        {
            if (!_gamepads.Contains(gamepad)) _gamepads.Add(gamepad);
        }

        private void Gamepad_GamepadRemoved(object _, Gamepad gamepad)
        {
            if (_gamepads.Contains(gamepad)) _gamepads.Remove(gamepad);
        }
    }
}