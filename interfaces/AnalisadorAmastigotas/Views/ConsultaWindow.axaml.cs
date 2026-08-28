using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using AnalisadorAmastigotas.Models;

namespace AnalisadorAmastigotas.Views;

public partial class ConsultaWindow : Window
{
    private int? laminaSelecionada = null;

    // ============================================================
    // L.U.M.A. VISUAL IDENTITY COLORS
    // ============================================================

    private static readonly Color Roxo =
        Color.FromRgb(100, 72, 153);

    private static readonly Color RoxoEscuro =
        Color.FromRgb(82, 59, 126);

    private static readonly Color Rosa =
        Color.FromRgb(220, 57, 148);

    private static readonly Color FundoAtivo =
        Color.FromRgb(246, 238, 251);

    private static readonly Color FundoConcluido =
        Color.FromRgb(255, 255, 255);

    private static readonly Color FundoBloqueado =
        Color.FromRgb(226, 223, 232);

    private static readonly Color TextoSecundario =
        Color.FromRgb(110, 102, 122);

    private static readonly Color BordaClara =
        Color.FromRgb(232, 225, 241);

    private static readonly Color CinzaIcone =
        Color.FromRgb(210, 207, 216);


    public ConsultaWindow()
    {
        InitializeComponent();

        AtualizarEtapaLateral(1);
        AtualizarBotaoAvancar();
    }


    // ============================================================
    // GIEMSA GRADIENT
    // ============================================================

    private static LinearGradientBrush CriarGradienteGiemsa()
    {
        return new LinearGradientBrush
        {
            StartPoint =
                new RelativePoint(
                    0,
                    0,
                    RelativeUnit.Relative),

            EndPoint =
                new RelativePoint(
                    1,
                    0,
                    RelativeUnit.Relative),

            GradientStops =
            {
                new GradientStop(
                    Roxo,
                    0),

                new GradientStop(
                    Roxo,
                    1)
            }
        };
    }


    // ============================================================
    // EXPERIMENTAL CONDITION
    // ============================================================

    private void DoencaComboBox_SelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        if (DoencaComboBox.SelectedItem is not ComboBoxItem item)
            return;

        string doenca =
            item.Content?.ToString() ?? "";

        ConsultaAtual.Dados.Doenca = doenca;

        // --------------------------------------------------------
        // GUIDE
        // --------------------------------------------------------

        DoencaSelecionadaTextBlock.Text = doenca;
        DoencaSelecionadaTextBlock.Foreground =
            new SolidColorBrush(Brushes.White.Color);

        // --------------------------------------------------------
        // CLEAR STRAIN
        // --------------------------------------------------------

        LinhagemComboBox.Items.Clear();
        LinhagemComboBox.SelectedIndex = -1;

        ConsultaAtual.Dados.Linhagem = null;
        ConsultaAtual.Dados.Lamina = null;

        LinhagemSelecionadaTextBlock.Text =
            "Select a strain";

        LinhagemSelecionadaTextBlock.Foreground =
            new SolidColorBrush(TextoSecundario);

        // --------------------------------------------------------
        // RESET SLIDES
        // --------------------------------------------------------

        ResetarLaminas();

        // --------------------------------------------------------
        // STRAINS
        // --------------------------------------------------------

        if (doenca.Contains("Chagas"))
        {
            LinhagemComboBox.Items.Add("HTR-8/SVneo");
            LinhagemComboBox.Items.Add("H9c2");
            LinhagemComboBox.Items.Add("C2C12");
            LinhagemComboBox.Items.Add("Vero");
        }
        else if (doenca.Contains("Cutaneous"))
        {
            LinhagemComboBox.Items.Add("RAW 264.7");
            LinhagemComboBox.Items.Add("J774A.1");
            LinhagemComboBox.Items.Add("HaCaT");
            LinhagemComboBox.Items.Add("U937");
        }
        else if (doenca.Contains("Visceral"))
        {
            LinhagemComboBox.Items.Add("THP-1");
            LinhagemComboBox.Items.Add("DH82");
            LinhagemComboBox.Items.Add("HepG2");
            LinhagemComboBox.Items.Add("Huh-7");
        }

        LinhagemComboBox.IsEnabled = true;
        LinhagemComboBox.PlaceholderText =
            "Select a strain";

        // --------------------------------------------------------
        // CURRENT STEP
        // --------------------------------------------------------

        AtualizarEtapaLateral(2);
    }


    // ============================================================
    // STRAIN
    // ============================================================

    private void LinhagemComboBox_SelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        if (LinhagemComboBox.SelectedItem is not string linhagem)
            return;

        ConsultaAtual.Dados.Linhagem = linhagem;

        LinhagemSelecionadaTextBlock.Text =
            linhagem;

        LinhagemSelecionadaTextBlock.Foreground =
            new SolidColorBrush(RoxoEscuro);

        HabilitarLaminas();

        AtualizarEtapaLateral(3);
    }


    // ============================================================
    // ENABLE SLIDES
    // ============================================================

    private void HabilitarLaminas()
    {
        Lamina1Button.IsEnabled = true;
        Lamina2Button.IsEnabled = true;
        Lamina3Button.IsEnabled = true;
        Lamina4Button.IsEnabled = true;
        Lamina5Button.IsEnabled = true;

        LaminaSelecionadaTextBlock.Text =
            "Select a slide";

        LaminaSelecionadaTextBlock.Foreground =
            new SolidColorBrush(TextoSecundario);

        RestaurarVisualLaminas();
    }


    // ============================================================
    // RESET SLIDES
    // ============================================================

    private void ResetarLaminas()
    {
        Lamina1Button.IsEnabled = false;
        Lamina2Button.IsEnabled = false;
        Lamina3Button.IsEnabled = false;
        Lamina4Button.IsEnabled = false;
        Lamina5Button.IsEnabled = false;

        laminaSelecionada = null;

        LaminaSelecionadaTextBlock.Text =
            "Locked";

        LaminaSelecionadaTextBlock.Foreground =
            new SolidColorBrush(TextoSecundario);

        RestaurarVisualLaminas();

        AvancarButton.IsEnabled = false;

        AplicarBotaoBloqueado();
    }


    // ============================================================
    // SLIDE SELECTION
    // ============================================================

    private void SelecionarLamina(int numero)
    {
        laminaSelecionada = numero;

        ConsultaAtual.Dados.Lamina = numero;

        LaminaSelecionadaTextBlock.Text =
            $"Slide {numero}";

        LaminaSelecionadaTextBlock.Foreground =
            new SolidColorBrush(RoxoEscuro);

        DestacarLaminaSelecionada(numero);

        AvancarButton.IsEnabled = true;

        AplicarBotaoAtivo();
    }


    // ============================================================
    // SLIDE CLICKS
    // ============================================================

    private void Lamina1_Click(
        object? sender,
        RoutedEventArgs e)
    {
        SelecionarLamina(1);
    }

    private void Lamina2_Click(
        object? sender,
        RoutedEventArgs e)
    {
        SelecionarLamina(2);
    }

    private void Lamina3_Click(
        object? sender,
        RoutedEventArgs e)
    {
        SelecionarLamina(3);
    }

    private void Lamina4_Click(
        object? sender,
        RoutedEventArgs e)
    {
        SelecionarLamina(4);
    }

    private void Lamina5_Click(
        object? sender,
        RoutedEventArgs e)
    {
        SelecionarLamina(5);
    }


    // ============================================================
    // SLIDE VISUAL
    // ============================================================

    private void RestaurarVisualLaminas()
    {
        AplicarVisualLamina(Lamina1Button, false);
        AplicarVisualLamina(Lamina2Button, false);
        AplicarVisualLamina(Lamina3Button, false);
        AplicarVisualLamina(Lamina4Button, false);
        AplicarVisualLamina(Lamina5Button, false);
    }


    private void DestacarLaminaSelecionada(int numero)
    {
        AplicarVisualLamina(Lamina1Button, numero == 1);
        AplicarVisualLamina(Lamina2Button, numero == 2);
        AplicarVisualLamina(Lamina3Button, numero == 3);
        AplicarVisualLamina(Lamina4Button, numero == 4);
        AplicarVisualLamina(Lamina5Button, numero == 5);
    }


    private static void AplicarVisualLamina(
        Button botao,
        bool selecionada)
    {
        if (selecionada)
        {
            botao.Background =
                new SolidColorBrush(FundoAtivo);

            botao.BorderBrush =
                new SolidColorBrush(Rosa);

            botao.BorderThickness =
                new Thickness(2);

            botao.Opacity = 1.0;
        }
        else
        {
            botao.Background =
                new SolidColorBrush(FundoConcluido);

            botao.BorderBrush =
                new SolidColorBrush(BordaClara);

            botao.BorderThickness =
                new Thickness(1);

            botao.Opacity =
                botao.IsEnabled
                    ? 1.0
                    : 0.65;
        }
    }


    // ============================================================
    // NEXT BUTTON
    // ============================================================

    private void AtualizarBotaoAvancar()
    {
        if (laminaSelecionada.HasValue)
            AplicarBotaoAtivo();
        else
            AplicarBotaoBloqueado();
    }


    private void AplicarBotaoAtivo()
    {
        AvancarButton.Background =
            CriarGradienteGiemsa();

        AvancarButton.Foreground =
            Brushes.White;

        AvancarButton.BorderThickness =
            new Thickness(0);

        AvancarButton.Opacity = 1.0;
    }


    private void AplicarBotaoBloqueado()
    {
        AvancarButton.Background =
            new SolidColorBrush(
                Color.FromRgb(217, 214, 220));

        AvancarButton.Foreground =
            new SolidColorBrush(
                Color.FromRgb(139, 135, 144));

        AvancarButton.BorderThickness =
            new Thickness(0);

        AvancarButton.Opacity = 1.0;
    }


    // ============================================================
    // SIDE GUIDE
    // ============================================================

    private void AtualizarEtapaLateral(int etapaAtual)
    {
        // --------------------------------------------------------
        // EXPERIMENTAL CONDITION
        // --------------------------------------------------------

        if (etapaAtual == 1)
        {
            AplicarEtapaAtiva(
                DoencaEtapaBorder,
                DoencaIconBorder,
                DoencaIconTextBlock,
                DoencaTituloTextBlock,
                DoencaSelecionadaTextBlock);

            AplicarEtapaBloqueada(
                LinhagemEtapaBorder,
                LinhagemIconBorder,
                LinhagemIconTextBlock,
                LinhagemTituloTextBlock,
                LinhagemSelecionadaTextBlock);

            AplicarEtapaBloqueada(
                LaminaEtapaBorder,
                LaminaIconBorder,
                LaminaIconTextBlock,
                LaminaTituloTextBlock,
                LaminaSelecionadaTextBlock);
        }

        // --------------------------------------------------------
        // STRAIN
        // --------------------------------------------------------

        else if (etapaAtual == 2)
        {
            AplicarEtapaConcluida(
                DoencaEtapaBorder,
                DoencaIconBorder,
                DoencaIconTextBlock,
                DoencaTituloTextBlock,
                DoencaSelecionadaTextBlock);

            AplicarEtapaAtiva(
                LinhagemEtapaBorder,
                LinhagemIconBorder,
                LinhagemIconTextBlock,
                LinhagemTituloTextBlock,
                LinhagemSelecionadaTextBlock);

            AplicarEtapaBloqueada(
                LaminaEtapaBorder,
                LaminaIconBorder,
                LaminaIconTextBlock,
                LaminaTituloTextBlock,
                LaminaSelecionadaTextBlock);
        }

        // --------------------------------------------------------
        // SLIDE
        // --------------------------------------------------------

        else if (etapaAtual == 3)
        {
            AplicarEtapaConcluida(
                DoencaEtapaBorder,
                DoencaIconBorder,
                DoencaIconTextBlock,
                DoencaTituloTextBlock,
                DoencaSelecionadaTextBlock);

            AplicarEtapaConcluida(
                LinhagemEtapaBorder,
                LinhagemIconBorder,
                LinhagemIconTextBlock,
                LinhagemTituloTextBlock,
                LinhagemSelecionadaTextBlock);

            AplicarEtapaAtiva(
                LaminaEtapaBorder,
                LaminaIconBorder,
                LaminaIconTextBlock,
                LaminaTituloTextBlock,
                LaminaSelecionadaTextBlock);
        }
    }


    private static void AplicarEtapaAtiva(
        Border etapa,
        Border icone,
        TextBlock iconeTexto,
        TextBlock titulo,
        TextBlock valor)
    {
        etapa.Background =
            CriarGradienteGiemsa();

        etapa.BorderBrush =
            Brushes.Transparent;

        etapa.BorderThickness =
            new Thickness(0);

        icone.Background =
            new SolidColorBrush(
                Color.FromRgb(255, 255, 255));

        iconeTexto.Text = "✓";

        iconeTexto.Foreground =
            new SolidColorBrush(Roxo);

        titulo.Foreground =
            Brushes.White;

        valor.Foreground =
            Brushes.White;
    }


    private static void AplicarEtapaConcluida(
        Border etapa,
        Border icone,
        TextBlock iconeTexto,
        TextBlock titulo,
        TextBlock valor)
    {
        etapa.Background =
            new SolidColorBrush(FundoConcluido);

        etapa.BorderBrush =
            new SolidColorBrush(BordaClara);

        etapa.BorderThickness =
            new Thickness(1);

        icone.Background =
            new SolidColorBrush(Roxo);

        iconeTexto.Text = "✓";

        iconeTexto.Foreground =
            Brushes.White;

        titulo.Foreground =
            new SolidColorBrush(RoxoEscuro);

        valor.Foreground =
            new SolidColorBrush(RoxoEscuro);
    }


    private static void AplicarEtapaBloqueada(
        Border etapa,
        Border icone,
        TextBlock iconeTexto,
        TextBlock titulo,
        TextBlock valor)
    {
        etapa.Background =
            new SolidColorBrush(FundoBloqueado);

        etapa.BorderBrush =
            Brushes.Transparent;

        etapa.BorderThickness =
            new Thickness(0);

        icone.Background =
            new SolidColorBrush(CinzaIcone);

        iconeTexto.Text = "🔒";

        iconeTexto.Foreground =
            new SolidColorBrush(TextoSecundario);

        titulo.Foreground =
            new SolidColorBrush(
                Color.FromRgb(102, 97, 111));

        valor.Foreground =
            new SolidColorBrush(
                Color.FromRgb(119, 114, 127));
    }


    // ============================================================
    // NEXT
    // ============================================================

    private void Avancar_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (laminaSelecionada == null)
            return;

        var pocoQuadranteWindow =
            new PocoQuadranteWindow(
                DoencaSelecionadaTextBlock.Text!,
                LinhagemSelecionadaTextBlock.Text!,
                laminaSelecionada.Value);

        pocoQuadranteWindow.Show();

        Close();
    }
}