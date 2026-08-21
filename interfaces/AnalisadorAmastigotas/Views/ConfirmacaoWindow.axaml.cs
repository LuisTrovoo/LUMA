using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AnalisadorAmastigotas.Views;

public partial class ConfirmacaoWindow : Window
{
    public bool Confirmado { get; private set; }

    public ConfirmacaoWindow()
    {
        InitializeComponent();
    }

    private void Permanecer_Click(
        object? sender,
        RoutedEventArgs e)
    {
        Confirmado = false;
        Close();
    }

    private void Avancar_Click(
        object? sender,
        RoutedEventArgs e)
    {
        Confirmado = true;
        Close();
    }
}