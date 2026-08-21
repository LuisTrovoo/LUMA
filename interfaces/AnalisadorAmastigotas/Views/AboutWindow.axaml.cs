using Avalonia.Controls;

namespace AnalisadorAmastigotas.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
    }

    private void Voltar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
} 