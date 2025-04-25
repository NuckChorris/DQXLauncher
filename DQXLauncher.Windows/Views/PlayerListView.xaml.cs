using System;
using DQXLauncher.Windows.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DQXLauncher.Windows.Views;

public partial class PlayerListView
{
    public event EventHandler<PlayerSelectedEventArgs>? PlayerSelected;
    public PlayerListViewModel ViewModel { get; } = new();

    public PlayerListView()
    {
        InitializeComponent();
        DataContext = ViewModel;
        ViewModel.LoadCommand.Execute(null);
    }

    public class PlayerSelectedEventArgs : EventArgs
    {
        public required PlayerListItem Item { get; set; }
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListView listView) return;
        if (listView.SelectedItem is not PlayerListItem item) return;

        PlayerSelected?.Invoke(this, new PlayerSelectedEventArgs { Item = item });
    }
}