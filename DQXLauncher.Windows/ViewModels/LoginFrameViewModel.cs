using System;
using CommunityToolkit.Mvvm.ComponentModel;
using DQXLauncher.Core.Game.LoginStrategy;

namespace DQXLauncher.Windows.ViewModels;

public partial class LoginFrameViewModel : ObservableObject
{
    public event EventHandler<StepChange>? StepChanged;

    public void Start(LoginStrategy strategy, LoginStep step)
    {
        Strategy = strategy;
        ChangeStep(step, StepChangeDirection.None);
    }

    public void Forward(LoginStep step)
    {
        ChangeStep(step);
    }

    public void Backward(LoginStep step)
    {
        ChangeStep(step, StepChangeDirection.Backward);
    }

    private void ChangeStep(LoginStep step, StepChangeDirection direction = StepChangeDirection.Forward)
    {
        Step = step;
        var change = new StepChange { Step = step, Direction = direction };
        StepChanged?.Invoke(this, change);
    }

    [ObservableProperty] public partial LoginStrategy? Strategy { get; set; }

    [ObservableProperty] public partial LoginStep? Step { get; set; }

    [ObservableProperty] public partial bool IsLoading { get; set; } = false;

    [ObservableProperty] public partial bool SaveCredentials { get; set; } = false;

    public enum StepChangeDirection
    {
        // Progressing to the next step
        Forward,

        // Going back to the previous step
        Backward,

        // No transition
        None
    }

    public class StepChange
    {
        public required LoginStep Step { get; init; }
        public StepChangeDirection Direction { get; init; } = StepChangeDirection.Forward;
    }
}