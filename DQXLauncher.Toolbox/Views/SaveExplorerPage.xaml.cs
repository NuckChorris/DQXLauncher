using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using DQXLauncher.Toolbox.Services;
using DQXLauncher.Toolbox.ViewModels;
using Windows.UI.Popups;
using Microsoft.UI.Xaml;

namespace DQXLauncher.Toolbox.Views;

public sealed partial class SaveExplorerPage
{
    public SaveExplorerPage()
    {
        this.InitializeComponent();
    }
    
    private ObservableCollection<SaveExplorerItem> Items { get; set; } =
        [new SaveExplorerItem { Path = Settings.Instance.SaveFolderPath }];

    private async void OnExportDeobfuscatedClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is SaveExplorerItem item)
        {

            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeChoices.Add("All Files", new List<string> { "." });
            if (item.DeobfuscatedName != null) picker.SuggestedFileName = item.DeobfuscatedName;

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.AppWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                File.Copy(item.Path, file.Path, overwrite: true);
                var dialog = new MessageDialog($"Deobfuscated file exported to: {file.Path}");
                await dialog.ShowAsync();
            }
        }
    }
}
