using Avalonia.Controls;

namespace AnalisadorAmastigotas.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Sair_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }

    private void Opcoes_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var optionsWindow = new OptionsWindow();
        optionsWindow.Show();
    }

    private void Sobre_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var aboutWindow = new AboutWindow();
        aboutWindow.Show();
    }
    private void NovaConsulta_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var consultaWindow = new ConsultaWindow();

        consultaWindow.Show();

        Close();
    }
}