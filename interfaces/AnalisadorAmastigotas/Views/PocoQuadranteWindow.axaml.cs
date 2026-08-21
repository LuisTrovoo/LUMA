using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia;
using AnalisadorAmastigotas.Models;

namespace AnalisadorAmastigotas.Views;

public partial class PocoQuadranteWindow : Window
{
    private string? pocoSelecionado;
    private string? quadranteSelecionado;

    private int laminaSelecionada;


    // ============================================================
    // CORES PADRÃO L.U.M.A.
    // ============================================================

    private static readonly Color Roxo =
        Color.FromRgb(112, 69, 204);

    private static readonly Color RoxoEscuro =
        Color.FromRgb(81, 49, 143);

    private static readonly Color Rosa =
        Color.FromRgb(216, 61, 148);

    private static readonly Color FundoConcluido =
        Color.FromRgb(255, 255, 255);

    private static readonly Color FundoBloqueado =
        Color.FromRgb(217, 214, 220);

    private static readonly Color CinzaIcone =
        Color.FromRgb(201, 197, 204);

    private static readonly Color TextoBloqueado =
        Color.FromRgb(119, 119, 119);


    // ============================================================
    // CONSTRUTOR PADRÃO
    // ============================================================

    public PocoQuadranteWindow()
    {
        InitializeComponent();
    }


    // ============================================================
    // CONSTRUTOR DA CONSULTA
    // ============================================================

    public PocoQuadranteWindow(
        string doenca,
        string linhagem,
        int lamina)
    {
        InitializeComponent();

        laminaSelecionada = lamina;

        // --------------------------------------------------------
        // GABARITO
        // --------------------------------------------------------

        DoencaTextBlock.Text = doenca;
        LinhagemTextBlock.Text = linhagem;
        LaminaTextBlock.Text = $"Lâmina {lamina}";

        PocoTextBlock.Text = "Selecione o poço";
        QuadranteTextBlock.Text = "Bloqueado";

        // --------------------------------------------------------
        // LIMPA OS DADOS DA ETAPA ANTERIOR
        // --------------------------------------------------------

        ConsultaAtual.Dados.Poco = null;
        ConsultaAtual.Dados.Quadrante = null;

        // --------------------------------------------------------
        // CONFIGURA INTERFACE
        // --------------------------------------------------------

        ConfigurarPocos();
        ConfigurarQuadrantes();

        AtualizarGabarito(1);

        AvancarButton.IsEnabled = false;
    }


    // ============================================================
    // GRADIENTE L.U.M.A.
    // ============================================================

    private static LinearGradientBrush CriarGradienteLuma()
    {
        return new LinearGradientBrush
        {
            StartPoint = new RelativePoint(
                0,
                0,
                RelativeUnit.Relative),

            EndPoint = new RelativePoint(
                1,
                0,
                RelativeUnit.Relative),

            GradientStops =
            {
                new GradientStop(Roxo, 0),
                new GradientStop(Rosa, 1)
            }
        };
    }


    // ============================================================
    // CONFIGURA OS POÇOS
    // ============================================================

    private void ConfigurarPocos()
    {
        Poco1Button.IsVisible = false;
        Poco2Button.IsVisible = false;
        Poco3Button.IsVisible = false;
        Poco4Button.IsVisible = false;
        Poco5Button.IsVisible = false;

        if (laminaSelecionada >= 1)
            Poco1Button.IsVisible = true;

        if (laminaSelecionada >= 2)
            Poco2Button.IsVisible = true;

        if (laminaSelecionada >= 3)
            Poco3Button.IsVisible = true;

        if (laminaSelecionada >= 4)
            Poco4Button.IsVisible = true;

        if (laminaSelecionada >= 5)
            Poco5Button.IsVisible = true;

        Poco1Button.IsEnabled = laminaSelecionada >= 1;
        Poco2Button.IsEnabled = laminaSelecionada >= 2;
        Poco3Button.IsEnabled = laminaSelecionada >= 3;
        Poco4Button.IsEnabled = laminaSelecionada >= 4;
        Poco5Button.IsEnabled = laminaSelecionada >= 5;

        LimparSelecaoPocos();
    }


    // ============================================================
    // CONFIGURA OS QUADRANTES
    // ============================================================

    private void ConfigurarQuadrantes()
    {
        Quadrante1Button.IsEnabled = false;
        Quadrante2Button.IsEnabled = false;
        Quadrante3Button.IsEnabled = false;
        Quadrante4Button.IsEnabled = false;

        LimparSelecaoQuadrantes();

        QuadranteTextBlock.Text = "Bloqueado";
    }


    // ============================================================
    // LIMPA SELEÇÃO DOS POÇOS
    // ============================================================

    private void LimparSelecaoPocos()
    {
        Poco1Button.Classes.Remove("selected");
        Poco2Button.Classes.Remove("selected");
        Poco3Button.Classes.Remove("selected");
        Poco4Button.Classes.Remove("selected");
        Poco5Button.Classes.Remove("selected");
    }


    // ============================================================
    // LIMPA SELEÇÃO DOS QUADRANTES
    // ============================================================

    private void LimparSelecaoQuadrantes()
    {
        Quadrante1Button.Classes.Remove("selected");
        Quadrante2Button.Classes.Remove("selected");
        Quadrante3Button.Classes.Remove("selected");
        Quadrante4Button.Classes.Remove("selected");
    }


    // ============================================================
    // POÇO 1
    // ============================================================

    private void Poco1_Click(
        object? sender,
        RoutedEventArgs e)
    {
        SelecionarPoco(1);
    }


    // ============================================================
    // POÇO 2
    // ============================================================

    private void Poco2_Click(
        object? sender,
        RoutedEventArgs e)
    {
        SelecionarPoco(2);
    }


    // ============================================================
    // POÇO 3
    // ============================================================

    private void Poco3_Click(
        object? sender,
        RoutedEventArgs e)
    {
        SelecionarPoco(3);
    }


    // ============================================================
    // POÇO 4
    // ============================================================

    private void Poco4_Click(
        object? sender,
        RoutedEventArgs e)
    {
        SelecionarPoco(4);
    }


    // ============================================================
    // POÇO 5
    // ============================================================

    private void Poco5_Click(
        object? sender,
        RoutedEventArgs e)
    {
        SelecionarPoco(5);
    }


    // ============================================================
    // PROCESSA SELEÇÃO DO POÇO
    // ============================================================

    private void SelecionarPoco(int numero)
    {
        if (numero < 1 || numero > laminaSelecionada)
            return;

        pocoSelecionado = $"Poço {numero}";

        ConsultaAtual.Dados.Poco = numero;

        PocoTextBlock.Text = pocoSelecionado;

        LimparSelecaoPocos();

        Button? botaoSelecionado = numero switch
        {
            1 => Poco1Button,
            2 => Poco2Button,
            3 => Poco3Button,
            4 => Poco4Button,
            5 => Poco5Button,
            _ => null
        };

        botaoSelecionado?.Classes.Add("selected");

        // --------------------------------------------------------
        // Trocar o poço invalida o quadrante anterior
        // --------------------------------------------------------

        quadranteSelecionado = null;

        ConsultaAtual.Dados.Quadrante = null;

        LimparSelecaoQuadrantes();

        Quadrante1Button.IsEnabled = true;
        Quadrante2Button.IsEnabled = true;
        Quadrante3Button.IsEnabled = true;
        Quadrante4Button.IsEnabled = true;

        QuadranteTextBlock.Text = "Selecione um quadrante";

        AvancarButton.IsEnabled = false;

        // --------------------------------------------------------
        // POÇO continua sendo a etapa ativa
        // até o usuário escolher um quadrante.
        // --------------------------------------------------------

        AtualizarGabarito(1);

        VerificarPreenchimento();
    }


    // ============================================================
    // QUADRANTE 1
    // ============================================================

    private void Quadrante1_Click(
        object? sender,
        RoutedEventArgs e)
    {
        SelecionarQuadrante(1);
    }


    // ============================================================
    // QUADRANTE 2
    // ============================================================

    private void Quadrante2_Click(
        object? sender,
        RoutedEventArgs e)
    {
        SelecionarQuadrante(2);
    }


    // ============================================================
    // QUADRANTE 3
    // ============================================================

    private void Quadrante3_Click(
        object? sender,
        RoutedEventArgs e)
    {
        SelecionarQuadrante(3);
    }


    // ============================================================
    // QUADRANTE 4
    // ============================================================

    private void Quadrante4_Click(
        object? sender,
        RoutedEventArgs e)
    {
        SelecionarQuadrante(4);
    }


    // ============================================================
    // PROCESSA SELEÇÃO DO QUADRANTE
    // ============================================================

    private void SelecionarQuadrante(int numero)
    {
        if (string.IsNullOrEmpty(pocoSelecionado))
            return;

        if (numero < 1 || numero > 4)
            return;

        quadranteSelecionado = $"Quadrante {numero}";

        ConsultaAtual.Dados.Quadrante = numero;

        QuadranteTextBlock.Text = quadranteSelecionado;

        LimparSelecaoQuadrantes();

        Button? botaoSelecionado = numero switch
        {
            1 => Quadrante1Button,
            2 => Quadrante2Button,
            3 => Quadrante3Button,
            4 => Quadrante4Button,
            _ => null
        };

        botaoSelecionado?.Classes.Add("selected");

        // --------------------------------------------------------
        // Agora a etapa ativa passa para QUADRANTE
        // --------------------------------------------------------

        AtualizarGabarito(2);

        VerificarPreenchimento();
    }


    // ============================================================
    // GABARITO LATERAL
    // ============================================================

    // etapa:
    //
    // 0 = estado inicial
    // 1 = POÇO ativo
    // 2 = QUADRANTE ativo
    //
    private void AtualizarGabarito(int etapa)
    {
        // --------------------------------------------------------
        // DOENÇA, LINHAGEM E LÂMINA
        // --------------------------------------------------------

        AplicarEtapaConcluida(
            DoencaEtapaBorder,
            DoencaIconBorder,
            DoencaIconTextBlock,
            DoencaTituloTextBlock);

        AplicarEtapaConcluida(
            LinhagemEtapaBorder,
            LinhagemIconBorder,
            LinhagemIconTextBlock,
            LinhagemTituloTextBlock);

        AplicarEtapaConcluida(
            LaminaEtapaBorder,
            LaminaIconBorder,
            LaminaIconTextBlock,
            LaminaTituloTextBlock);


        // --------------------------------------------------------
        // POÇO
        // --------------------------------------------------------

        if (etapa == 1)
        {
            AplicarEtapaAtiva(
                PocoEtapaBorder,
                PocoIconBorder,
                PocoIconTextBlock,
                PocoTituloTextBlock);
        }
        else
        {
            AplicarEtapaConcluida(
                PocoEtapaBorder,
                PocoIconBorder,
                PocoIconTextBlock,
                PocoTituloTextBlock);
        }


        // --------------------------------------------------------
        // QUADRANTE
        // --------------------------------------------------------

        if (etapa == 2)
        {
            AplicarEtapaAtiva(
                QuadranteEtapaBorder,
                QuadranteIconBorder,
                QuadranteIconTextBlock,
                QuadranteTituloTextBlock);
        }
        else
        {
            AplicarEtapaBloqueada(
                QuadranteEtapaBorder,
                QuadranteIconBorder,
                QuadranteIconTextBlock,
                QuadranteTituloTextBlock);
        }
    }


    // ============================================================
    // ETAPA ATIVA
    // ============================================================

    private static void AplicarEtapaAtiva(
        Border etapa,
        Border icone,
        TextBlock iconeTexto,
        TextBlock titulo)
    {
        etapa.Classes.Remove("bloqueada");
        etapa.Classes.Remove("ativa");
        etapa.Classes.Add("ativa");

        etapa.BorderThickness =
            new Thickness(0);

        icone.Background =
            Brushes.White;

        iconeTexto.Foreground =
            new SolidColorBrush(Roxo);

        titulo.Foreground =
            Brushes.White;
    }


    // ============================================================
    // ETAPA CONCLUÍDA
    // ============================================================

    private static void AplicarEtapaConcluida(
        Border etapa,
        Border icone,
        TextBlock iconeTexto,
        TextBlock titulo)
    {
        etapa.Classes.Remove("ativa");
        etapa.Classes.Remove("bloqueada");
        etapa.Classes.Add("etapa");

        etapa.Background =
            new SolidColorBrush(FundoConcluido);

        etapa.BorderBrush =
            new SolidColorBrush(
                Color.FromRgb(232, 225, 241));

        etapa.BorderThickness =
            new Thickness(1);

        icone.Background =
            new SolidColorBrush(Roxo);

        iconeTexto.Text = "✓";

        iconeTexto.Foreground =
            Brushes.White;

        titulo.Foreground =
            new SolidColorBrush(RoxoEscuro);
    }


    // ============================================================
    // ETAPA BLOQUEADA
    // ============================================================

    private static void AplicarEtapaBloqueada(
        Border etapa,
        Border icone,
        TextBlock iconeTexto,
        TextBlock titulo)
    {
        etapa.Classes.Remove("ativa");
        etapa.Classes.Remove("bloqueada");
        etapa.Classes.Add("bloqueada");

        etapa.Background =
            new SolidColorBrush(FundoBloqueado);

        etapa.BorderBrush =
            new SolidColorBrush(FundoBloqueado);

        etapa.BorderThickness =
            new Thickness(1);

        icone.Background =
            new SolidColorBrush(CinzaIcone);

        iconeTexto.Foreground =
            new SolidColorBrush(TextoBloqueado);

        titulo.Foreground =
            new SolidColorBrush(TextoBloqueado);
    }


    // ============================================================
    // VERIFICA SE A CONSULTA ESTÁ COMPLETA
    // ============================================================

    private void VerificarPreenchimento()
    {
        bool preenchido =
            !string.IsNullOrEmpty(pocoSelecionado)
            &&
            !string.IsNullOrEmpty(quadranteSelecionado);

        AvancarButton.IsEnabled = preenchido;
    }


    // ============================================================
    // AVANÇAR
    // ============================================================

    private async void Avancar_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(pocoSelecionado)
            ||
            string.IsNullOrEmpty(quadranteSelecionado))
        {
            return;
        }

        var confirmacao =
            new ConfirmacaoWindow();

        await confirmacao.ShowDialog(this);

        if (!confirmacao.Confirmado)
            return;


        // --------------------------------------------------------
        // Garante POÇO
        // --------------------------------------------------------

        if (pocoSelecionado.StartsWith("Poço "))
        {
            string numeroPoco =
                pocoSelecionado.Replace(
                    "Poço ",
                    "");

            if (int.TryParse(
                    numeroPoco,
                    out int poco))
            {
                ConsultaAtual.Dados.Poco = poco;
            }
        }


        // --------------------------------------------------------
        // Garante QUADRANTE
        // --------------------------------------------------------

        if (quadranteSelecionado.StartsWith("Quadrante "))
        {
            string numeroQuadrante =
                quadranteSelecionado.Replace(
                    "Quadrante ",
                    "");

            if (int.TryParse(
                    numeroQuadrante,
                    out int quadrante))
            {
                ConsultaAtual.Dados.Quadrante = quadrante;
            }
        }


        // --------------------------------------------------------
        // Abre análise da imagem
        // --------------------------------------------------------

        var analiseImagemWindow =
            new AnaliseImagemWindow();

        analiseImagemWindow.Show();

        Close();
    }
}