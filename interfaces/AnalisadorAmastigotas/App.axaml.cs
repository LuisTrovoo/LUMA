using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AnalisadorAmastigotas.Services;
using AnalisadorAmastigotas.ViewModels;
using AnalisadorAmastigotas.Views;

namespace AnalisadorAmastigotas;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(),
            };

            desktop.Exit += OnApplicationExit;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnApplicationExit(
        object? sender,
        ControlledApplicationLifetimeExitEventArgs e)
    {
        LimpezaService.LimparConsulta();
    }
}