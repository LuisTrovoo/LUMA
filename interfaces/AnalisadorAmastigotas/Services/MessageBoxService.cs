using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;

namespace AnalisadorAmastigotas.Services;

public static class MessageBoxService
{
    public static async Task Mostrar(
        Window owner,
        string mensagem,
        string titulo)
    {
        bool ehErro =
            titulo.Equals(
                "Erro",
                StringComparison.OrdinalIgnoreCase);


        // =========================================================
        // CORES DA IDENTIDADE VISUAL DO L.U.M.A.
        // =========================================================

        var fundo =
            Color.FromRgb(248, 246, 250);

        var branco =
            Color.FromRgb(255, 255, 255);

        var roxo =
            Color.FromRgb(94, 43, 151);

        var roxoPrincipal =
            Color.FromRgb(113, 59, 208);

        var rosa =
            Color.FromRgb(216, 61, 148);

        var textoSecundario =
            Color.FromRgb(110, 102, 122);

        var borda =
            Color.FromRgb(221, 213, 229);

        var fundoIcone =
            ehErro
                ? Color.FromRgb(252, 235, 240)
                : Color.FromRgb(242, 234, 249);

        var corIcone =
            ehErro
                ? Color.FromRgb(210, 65, 105)
                : roxoPrincipal;


        // =========================================================
        // JANELA
        // =========================================================

        var janela =
            new Window
            {
                Title = titulo,

                Width = 460,
                Height = 350,

                WindowStartupLocation =
                    WindowStartupLocation.CenterOwner,

                CanResize = false,

                Background =
                    new SolidColorBrush(fundo)
            };


        // =========================================================
        // CONTAINER PRINCIPAL
        // =========================================================

        var container =
            new Border
            {
                Background =
                    new SolidColorBrush(branco),

                BorderBrush =
                    new SolidColorBrush(borda),

                BorderThickness =
                    new Thickness(1),

                CornerRadius =
                    new CornerRadius(16),

                Padding =
                    new Thickness(28)
            };


        var painel =
            new Grid
            {
                HorizontalAlignment =
                    HorizontalAlignment.Stretch,

                VerticalAlignment =
                    VerticalAlignment.Stretch
            };


        painel.RowDefinitions.Add(
            new RowDefinition
            {
                Height = GridLength.Auto
            });

        painel.RowDefinitions.Add(
            new RowDefinition
            {
                Height = new GridLength(
                    1,
                    GridUnitType.Star)
            });

        painel.RowDefinitions.Add(
            new RowDefinition
            {
                Height = GridLength.Auto
            });


        // =========================================================
        // CABEÇALHO
        // =========================================================

        var cabecalho =
            new StackPanel
            {
                HorizontalAlignment =
                    HorizontalAlignment.Center,

                Spacing = 5
            };


        var marca =
            new TextBlock
            {
                Text = "L.U.M.A.",

                FontSize = 13,

                FontWeight =
                    FontWeight.Bold,

                Foreground =
                    new SolidColorBrush(roxo),

                HorizontalAlignment =
                    HorizontalAlignment.Center
            };


        // Pequeno detalhe em degradê da identidade visual

        var linha =
            new Border
            {
                Width = 42,
                Height = 3,

                CornerRadius =
                    new CornerRadius(2),

                HorizontalAlignment =
                    HorizontalAlignment.Center,

                Background =
                    new LinearGradientBrush
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
                                roxoPrincipal,
                                0),

                            new GradientStop(
                                rosa,
                                1)
                        }
                    }
            };


        cabecalho.Children.Add(marca);
        cabecalho.Children.Add(linha);

        Grid.SetRow(
            cabecalho,
            0);

        painel.Children.Add(cabecalho);


        // =========================================================
        // CONTEÚDO CENTRAL
        // =========================================================

        var conteudo =
            new StackPanel
            {
                HorizontalAlignment =
                    HorizontalAlignment.Center,

                VerticalAlignment =
                    VerticalAlignment.Center,

                Spacing = 10
            };


        // =========================================================
        // ÍCONE DE STATUS
        // =========================================================

        var circulo =
            new Border
            {
                Width = 68,
                Height = 68,

                CornerRadius =
                    new CornerRadius(34),

                Background =
                    new SolidColorBrush(fundoIcone),

                HorizontalAlignment =
                    HorizontalAlignment.Center,

                Margin =
                    new Thickness(
                        0,
                        8,
                        0,
                        4)
            };


        var icone =
            new TextBlock
            {
                Text =
                    ehErro
                        ? "!"
                        : "✓",

                FontSize = 34,

                FontWeight =
                    FontWeight.Bold,

                Foreground =
                    new SolidColorBrush(corIcone),

                HorizontalAlignment =
                    HorizontalAlignment.Center,

                VerticalAlignment =
                    VerticalAlignment.Center,

                TextAlignment =
                    TextAlignment.Center
            };


        circulo.Child = icone;


        // =========================================================
        // TÍTULO DA MENSAGEM
        // =========================================================

        var tituloTextBlock =
            new TextBlock
            {
                Text =
                    ehErro
                        ? "NÃO FOI POSSÍVEL GERAR O PDF"
                        : "PDF SALVO COM SUCESSO!",

                FontSize = 19,

                FontWeight =
                    FontWeight.Bold,

                Foreground =
                    new SolidColorBrush(roxo),

                HorizontalAlignment =
                    HorizontalAlignment.Center,

                TextAlignment =
                    TextAlignment.Center,

                TextWrapping =
                    TextWrapping.Wrap,

                MaxWidth = 390
            };


        // =========================================================
        // MENSAGEM
        // =========================================================

        var mensagemTextBlock =
            new TextBlock
            {
                Text = mensagem,

                FontSize = 13,

                Foreground =
                    new SolidColorBrush(
                        textoSecundario),

                HorizontalAlignment =
                    HorizontalAlignment.Center,

                TextAlignment =
                    TextAlignment.Center,

                TextWrapping =
                    TextWrapping.Wrap,

                MaxWidth = 380,

                Margin =
                    new Thickness(
                        10,
                        0,
                        10,
                        4)
            };


        conteudo.Children.Add(circulo);
        conteudo.Children.Add(tituloTextBlock);
        conteudo.Children.Add(mensagemTextBlock);


        Grid.SetRow(
            conteudo,
            1);

        painel.Children.Add(conteudo);


        // =========================================================
        // RODAPÉ
        // =========================================================

        var rodape =
            new StackPanel
            {
                HorizontalAlignment =
                    HorizontalAlignment.Center,

                Spacing = 10
            };


        // =========================================================
        // BOTÃO OK
        // =========================================================

        var botao =
            new Button
            {
                Content = "OK",

                Width = 120,
                Height = 42,

                HorizontalAlignment =
                    HorizontalAlignment.Center,

                HorizontalContentAlignment =
                    HorizontalAlignment.Center,

                VerticalContentAlignment =
                    VerticalAlignment.Center,

                FontSize = 12,

                FontWeight =
                    FontWeight.Bold,

                Foreground =
                    Brushes.White,

                BorderThickness =
                    new Thickness(0),

                CornerRadius =
                    new CornerRadius(10),

                Padding =
                    new Thickness(0),

                Background =
                    new LinearGradientBrush
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
                                roxoPrincipal,
                                0),

                            new GradientStop(
                                rosa,
                                1)
                        }
                    }
            };


        botao.Click += (_, _) =>
        {
            janela.Close();
        };


        // =========================================================
        // EFEITO HOVER
        // =========================================================

        botao.PointerEntered += (_, _) =>
        {
            botao.Opacity = 0.88;
        };

        botao.PointerExited += (_, _) =>
        {
            botao.Opacity = 1.0;
        };


        rodape.Children.Add(botao);


        // =========================================================
        // DECORAÇÃO INFERIOR
        // =========================================================

        var decoracao =
            new StackPanel
            {
                Orientation =
                    Orientation.Horizontal,

                HorizontalAlignment =
                    HorizontalAlignment.Center,

                Spacing = 6,

                Opacity = 0.55
            };


        decoracao.Children.Add(
            new Ellipse
            {
                Width = 5,
                Height = 5,

                Fill =
                    new SolidColorBrush(
                        Color.FromRgb(
                            205,
                            188,
                            235))
            });


        decoracao.Children.Add(
            new Ellipse
            {
                Width = 5,
                Height = 5,

                Fill =
                    new SolidColorBrush(
                        Color.FromRgb(
                            229,
                            185,
                            219))
            });


        decoracao.Children.Add(
            new Ellipse
            {
                Width = 5,
                Height = 5,

                Fill =
                    new SolidColorBrush(
                        Color.FromRgb(
                            205,
                            188,
                            235))
            });


        decoracao.Children.Add(
            new Ellipse
            {
                Width = 5,
                Height = 5,

                Fill =
                    new SolidColorBrush(
                        Color.FromRgb(
                            229,
                            185,
                            219))
            });


        rodape.Children.Add(decoracao);


        Grid.SetRow(
            rodape,
            2);

        painel.Children.Add(rodape);


        // =========================================================
        // MONTAR A JANELA
        // =========================================================

        container.Child = painel;

        janela.Content = container;


        // =========================================================
        // MOSTRAR
        // =========================================================

        await janela.ShowDialog(owner);
    }
}