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
        Color.FromRgb(100, 72, 153);

    private static readonly Color RoxoEscuro =
        Color.FromRgb(82, 59, 126);

    private static readonly Color TextoNormal =
        Color.FromRgb(110, 102, 122);

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

        DoencaTextBlock.Text =
            doenca;

        LinhagemTextBlock.Text =
            linhagem;

        LaminaTextBlock.Text =
            $"Lâmina {lamina}";


        PocoTextBlock.Text =
            "Selecione o poço";

        QuadranteTextBlock.Text =
            "Bloqueado";


        // --------------------------------------------------------
        // LIMPA DADOS ANTERIORES
        // --------------------------------------------------------

        ConsultaAtual.Dados.Poco =
            null;

        ConsultaAtual.Dados.Quadrante =
            null;


        // --------------------------------------------------------
        // CONFIGURA INTERFACE
        // --------------------------------------------------------

        ConfigurarPocos();

        ConfigurarQuadrantes();

        AtualizarGabarito(1);

        AvancarButton.IsEnabled =
            false;
    }


    // ============================================================
    // CONFIGURA LÂMINA E BOTÕES DOS POÇOS
    // ============================================================

    private void ConfigurarPocos()
    {
        // --------------------------------------------------------
        // ESCONDE TODAS AS IMAGENS
        // --------------------------------------------------------

        Lamina1VisualImage.IsVisible =
            false;

        Lamina2VisualImage.IsVisible =
            false;

        Lamina3VisualImage.IsVisible =
            false;

        Lamina4VisualImage.IsVisible =
            false;

        Lamina5VisualImage.IsVisible =
            false;


        // --------------------------------------------------------
        // ESCONDE TODOS OS BOTÕES
        // --------------------------------------------------------

        Poco1Button.IsVisible =
            false;

        Poco2Button.IsVisible =
            false;

        Poco3Button.IsVisible =
            false;

        Poco4Button.IsVisible =
            false;

        Poco5Button.IsVisible =
            false;


        Poco1Button.IsEnabled =
            false;

        Poco2Button.IsEnabled =
            false;

        Poco3Button.IsEnabled =
            false;

        Poco4Button.IsEnabled =
            false;

        Poco5Button.IsEnabled =
            false;


        // --------------------------------------------------------
        // CONFIGURA CONFORME A LÂMINA
        // --------------------------------------------------------

        switch (laminaSelecionada)
        {
            // ====================================================
            // LÂMINA 1
            //
            //                IMAGEM    [1]
            // ====================================================

            case 1:

                Lamina1VisualImage.IsVisible =
                    true;


                ConfigurarBotaoPoco(
                    Poco1Button,
                    coluna: 0,
                    linha: 1);

                break;


            // ====================================================
            // LÂMINA 2
            //
            //                          [1]
            //
            //                IMAGEM
            //
            //                          [2]
            // ====================================================

            case 2:

                Lamina2VisualImage.IsVisible =
                    true;


                ConfigurarBotaoPoco(
                    Poco1Button,
                    coluna: 0,
                    linha: 0);


                ConfigurarBotaoPoco(
                    Poco2Button,
                    coluna: 0,
                    linha: 2);

                break;


            // ====================================================
            // LÂMINA 3
            //
            //                          [1]
            //
            //                IMAGEM    [2]
            //
            //                          [3]
            // ====================================================

            case 3:

                Lamina3VisualImage.IsVisible =
                    true;


                ConfigurarBotaoPoco(
                    Poco1Button,
                    coluna: 0,
                    linha: 0);


                ConfigurarBotaoPoco(
                    Poco2Button,
                    coluna: 0,
                    linha: 1);


                ConfigurarBotaoPoco(
                    Poco3Button,
                    coluna: 0,
                    linha: 2);

                break;


            // ====================================================
            // LÂMINA 4
            //
            // [1]                        [4]
            //
            // [2]         IMAGEM
            //
            // [3]
            // ====================================================

            case 4:

                Lamina4VisualImage.IsVisible =
                    true;


                ConfigurarBotaoPoco(
                    Poco1Button,
                    coluna: 0,
                    linha: 0);


                ConfigurarBotaoPoco(
                    Poco2Button,
                    coluna: 0,
                    linha: 1);


                ConfigurarBotaoPoco(
                    Poco3Button,
                    coluna: 0,
                    linha: 2);


                ConfigurarBotaoPoco(
                    Poco4Button,
                    coluna: 2,
                    linha: 1);

                break;


            // ====================================================
            // LÂMINA 5
            //
            // [1]                        [4]
            //
            // [2]         IMAGEM
            //
            // [3]                        [5]
            // ====================================================

            case 5:

                Lamina5VisualImage.IsVisible =
                    true;


                ConfigurarBotaoPoco(
                    Poco1Button,
                    coluna: 0,
                    linha: 0);


                ConfigurarBotaoPoco(
                    Poco2Button,
                    coluna: 0,
                    linha: 1);


                ConfigurarBotaoPoco(
                    Poco3Button,
                    coluna: 0,
                    linha: 2);


                ConfigurarBotaoPoco(
                    Poco4Button,
                    coluna: 2,
                    linha: 0);


                ConfigurarBotaoPoco(
                    Poco5Button,
                    coluna: 2,
                    linha: 2);

                break;
        }


        LimparSelecaoPocos();
    }


    // ============================================================
    // CONFIGURA BOTÃO DO POÇO NO GRID
    // ============================================================

    private static void ConfigurarBotaoPoco(
        Button botao,
        int coluna,
        int linha)
    {
        botao.IsVisible =
            true;

        botao.IsEnabled =
            true;


        Grid.SetColumn(
            botao,
            coluna);


        Grid.SetRow(
            botao,
            linha);
    }


    // ============================================================
    // CONFIGURA QUADRANTES
    // ============================================================

    private void ConfigurarQuadrantes()
    {
        Quadrante1Button.IsEnabled =
            false;

        Quadrante2Button.IsEnabled =
            false;

        Quadrante3Button.IsEnabled =
            false;

        Quadrante4Button.IsEnabled =
            false;


        LimparSelecaoQuadrantes();


        QuadranteTextBlock.Text =
            "Bloqueado";


        QuadranteBloqueioOverlay.IsVisible =
            true;
    }


    // ============================================================
    // LIMPA SELEÇÃO DOS POÇOS
    // ============================================================

    private void LimparSelecaoPocos()
    {
        Poco1Button.Classes.Remove(
            "selected");

        Poco2Button.Classes.Remove(
            "selected");

        Poco3Button.Classes.Remove(
            "selected");

        Poco4Button.Classes.Remove(
            "selected");

        Poco5Button.Classes.Remove(
            "selected");
    }


    // ============================================================
    // LIMPA SELEÇÃO DOS QUADRANTES
    // ============================================================

    private void LimparSelecaoQuadrantes()
    {
        Quadrante1Button.Classes.Remove(
            "selected");

        Quadrante2Button.Classes.Remove(
            "selected");

        Quadrante3Button.Classes.Remove(
            "selected");

        Quadrante4Button.Classes.Remove(
            "selected");
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
    // SELECIONA POÇO
    // ============================================================

    private void SelecionarPoco(
        int numero)
    {
        if (numero < 1 ||
            numero > laminaSelecionada)
        {
            return;
        }


        pocoSelecionado =
            $"Poço {numero}";


        ConsultaAtual.Dados.Poco =
            numero;


        // --------------------------------------------------------
        // AGORA APARECE NA BARRA:
        //
        // POÇO
        // Poço 1
        // --------------------------------------------------------

        PocoTextBlock.Text =
            pocoSelecionado;


        LimparSelecaoPocos();


        Button? botaoSelecionado =
            numero switch
            {
                1 => Poco1Button,

                2 => Poco2Button,

                3 => Poco3Button,

                4 => Poco4Button,

                5 => Poco5Button,

                _ => null
            };


        botaoSelecionado?
            .Classes
            .Add("selected");


        // --------------------------------------------------------
        // TROCAR POÇO INVALIDA QUADRANTE ANTERIOR
        // --------------------------------------------------------

        quadranteSelecionado =
            null;


        ConsultaAtual.Dados.Quadrante =
            null;


        LimparSelecaoQuadrantes();


        // --------------------------------------------------------
        // LIBERA QUADRANTES
        // --------------------------------------------------------

        Quadrante1Button.IsEnabled =
            true;

        Quadrante2Button.IsEnabled =
            true;

        Quadrante3Button.IsEnabled =
            true;

        Quadrante4Button.IsEnabled =
            true;


        QuadranteTextBlock.Text =
            "Selecione um quadrante";


        // --------------------------------------------------------
        // REMOVE CINZA
        // --------------------------------------------------------

        QuadranteBloqueioOverlay.IsVisible =
            false;


        AvancarButton.IsEnabled =
            false;


        // --------------------------------------------------------
        // POÇO AGORA ESTÁ CONCLUÍDO.
        // QUADRANTE É A ETAPA ATIVA.
        // --------------------------------------------------------

        AtualizarGabarito(2);


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
    // SELECIONA QUADRANTE
    // ============================================================

    private void SelecionarQuadrante(
        int numero)
    {
        if (string.IsNullOrEmpty(
                pocoSelecionado))
        {
            return;
        }


        if (numero < 1 ||
            numero > 4)
        {
            return;
        }


        quadranteSelecionado =
            $"Quadrante {numero}";


        ConsultaAtual.Dados.Quadrante =
            numero;


        QuadranteTextBlock.Text =
            quadranteSelecionado;


        LimparSelecaoQuadrantes();


        Button? botaoSelecionado =
            numero switch
            {
                1 => Quadrante1Button,

                2 => Quadrante2Button,

                3 => Quadrante3Button,

                4 => Quadrante4Button,

                _ => null
            };


        botaoSelecionado?
            .Classes
            .Add("selected");


        AtualizarGabarito(2);


        VerificarPreenchimento();
    }


    // ============================================================
    // ATUALIZA GABARITO
    //
    // 1 = POÇO ATIVO
    // 2 = QUADRANTE ATIVO
    // ============================================================

    private void AtualizarGabarito(
        int etapa)
    {
        // --------------------------------------------------------
        // DOENÇA
        // --------------------------------------------------------

        AplicarEtapaConcluida(
            DoencaEtapaBorder,
            DoencaIconBorder,
            DoencaIconTextBlock,
            DoencaTituloTextBlock,
            DoencaTextBlock);


        // --------------------------------------------------------
        // LINHAGEM
        // --------------------------------------------------------

        AplicarEtapaConcluida(
            LinhagemEtapaBorder,
            LinhagemIconBorder,
            LinhagemIconTextBlock,
            LinhagemTituloTextBlock,
            LinhagemTextBlock);


        // --------------------------------------------------------
        // LÂMINA
        // --------------------------------------------------------

        AplicarEtapaConcluida(
            LaminaEtapaBorder,
            LaminaIconBorder,
            LaminaIconTextBlock,
            LaminaTituloTextBlock,
            LaminaTextBlock);


        // --------------------------------------------------------
        // POÇO
        // --------------------------------------------------------

        if (etapa == 1)
        {
            AplicarEtapaAtiva(
                PocoEtapaBorder,
                PocoIconBorder,
                PocoIconTextBlock,
                PocoTituloTextBlock,
                PocoTextBlock);
        }
        else
        {
            AplicarEtapaConcluida(
                PocoEtapaBorder,
                PocoIconBorder,
                PocoIconTextBlock,
                PocoTituloTextBlock,
                PocoTextBlock);
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
                QuadranteTituloTextBlock,
                QuadranteTextBlock);
        }
        else
        {
            AplicarEtapaBloqueada(
                QuadranteEtapaBorder,
                QuadranteIconBorder,
                QuadranteIconTextBlock,
                QuadranteTituloTextBlock,
                QuadranteTextBlock);
        }
    }


    // ============================================================
    // ETAPA ATIVA
    // ============================================================

    private static void AplicarEtapaAtiva(
        Border etapa,
        Border icone,
        TextBlock iconeTexto,
        TextBlock titulo,
        TextBlock valor)
    {
        etapa.Classes.Remove(
            "bloqueada");


        etapa.Classes.Remove(
            "ativa");


        etapa.Classes.Add(
            "ativa");


        etapa.BorderThickness =
            new Thickness(0);


        icone.Background =
            Brushes.White;


        iconeTexto.Foreground =
            new SolidColorBrush(
                Roxo);


        titulo.Foreground =
            Brushes.White;


        // texto abaixo do título

        valor.Foreground =
            new SolidColorBrush(
                Color.FromRgb(
                    255,
                    247,
                    252));
    }


    // ============================================================
    // ETAPA CONCLUÍDA
    // ============================================================

    private static void AplicarEtapaConcluida(
        Border etapa,
        Border icone,
        TextBlock iconeTexto,
        TextBlock titulo,
        TextBlock valor)
    {
        etapa.Classes.Remove(
            "ativa");


        etapa.Classes.Remove(
            "bloqueada");


        etapa.Classes.Add(
            "etapa");


        etapa.Background =
            new SolidColorBrush(
                FundoConcluido);


        etapa.BorderBrush =
            new SolidColorBrush(
                Color.FromRgb(
                    232,
                    225,
                    241));


        etapa.BorderThickness =
            new Thickness(1);


        icone.Background =
            new SolidColorBrush(
                Roxo);


        iconeTexto.Text =
            "✓";


        iconeTexto.Foreground =
            Brushes.White;


        titulo.Foreground =
            new SolidColorBrush(
                RoxoEscuro);


        // IMPORTANTE:
        // garante que "Poço 1" apareça no fundo branco

        valor.Foreground =
            new SolidColorBrush(
                TextoNormal);
    }


    // ============================================================
    // ETAPA BLOQUEADA
    // ============================================================

    private static void AplicarEtapaBloqueada(
        Border etapa,
        Border icone,
        TextBlock iconeTexto,
        TextBlock titulo,
        TextBlock valor)
    {
        etapa.Classes.Remove(
            "ativa");


        etapa.Classes.Remove(
            "bloqueada");


        etapa.Classes.Add(
            "bloqueada");


        etapa.Background =
            new SolidColorBrush(
                FundoBloqueado);


        etapa.BorderBrush =
            new SolidColorBrush(
                FundoBloqueado);


        etapa.BorderThickness =
            new Thickness(1);


        icone.Background =
            new SolidColorBrush(
                CinzaIcone);


        iconeTexto.Foreground =
            new SolidColorBrush(
                TextoBloqueado);


        titulo.Foreground =
            new SolidColorBrush(
                TextoBloqueado);


        valor.Foreground =
            new SolidColorBrush(
                TextoBloqueado);
    }


    // ============================================================
    // VERIFICA PREENCHIMENTO
    // ============================================================

    private void VerificarPreenchimento()
    {
        bool preenchido =
            !string.IsNullOrEmpty(
                pocoSelecionado)
            &&
            !string.IsNullOrEmpty(
                quadranteSelecionado);


        AvancarButton.IsEnabled =
            preenchido;
    }


    // ============================================================
    // AVANÇAR
    // ============================================================

    private async void Avancar_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(
                pocoSelecionado)
            ||
            string.IsNullOrEmpty(
                quadranteSelecionado))
        {
            return;
        }


        var confirmacao =
            new ConfirmacaoWindow();


        await confirmacao.ShowDialog(this);


        if (!confirmacao.Confirmado)
        {
            return;
        }


        // --------------------------------------------------------
        // GARANTE POÇO
        // --------------------------------------------------------

        if (pocoSelecionado.StartsWith(
                "Poço "))
        {
            string numeroPoco =
                pocoSelecionado.Replace(
                    "Poço ",
                    "");


            if (int.TryParse(
                    numeroPoco,
                    out int poco))
            {
                ConsultaAtual.Dados.Poco =
                    poco;
            }
        }


        // --------------------------------------------------------
        // GARANTE QUADRANTE
        // --------------------------------------------------------

        if (quadranteSelecionado.StartsWith(
                "Quadrante "))
        {
            string numeroQuadrante =
                quadranteSelecionado.Replace(
                    "Quadrante ",
                    "");


            if (int.TryParse(
                    numeroQuadrante,
                    out int quadrante))
            {
                ConsultaAtual.Dados.Quadrante =
                    quadrante;
            }
        }


        // --------------------------------------------------------
        // ABRE ANÁLISE
        // --------------------------------------------------------

        var analiseImagemWindow =
            new AnaliseImagemWindow();


        analiseImagemWindow.Show();


        Close();
    }
}