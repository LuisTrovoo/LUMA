using Avalonia.Controls;
using Avalonia.Interactivity;
using AnalisadorAmastigotas.Models;
using AnalisadorAmastigotas.Services;
using System;
using System.Linq;
using Avalonia.Platform.Storage;

namespace AnalisadorAmastigotas.Views;

public partial class ResultadoWindow : Window
{
    private readonly ResultadoGabarito resultado;


    public ResultadoWindow(
        ResultadoGabarito resultado)
    {
        InitializeComponent();

        this.resultado =
            resultado;

        CarregarResultado();
    }


    // ============================================================
    // CARREGA RESULTADO
    // ============================================================

    private void CarregarResultado()
    {
        // --------------------------------------------------------
        // IDENTIFICAÇÃO
        // --------------------------------------------------------

        DoencaTextBlock.Text =
            resultado.Doenca ?? "—";

        LinhagemTextBlock.Text =
            resultado.Linhagem ?? "—";

        LaminaTextBlock.Text =
            resultado.Lamina.HasValue
                ? $"Slide {resultado.Lamina}"
                : "—";

        PocoTextBlock.Text =
            resultado.Poco.HasValue
                ? $"Well {resultado.Poco}"
                : "—";


        // --------------------------------------------------------
        // RESUMO
        // --------------------------------------------------------

        QuadrantesTextBlock.Text =
            $"{resultado.QuadrantesAnalisados} of 4 quadrants were analyzed.\n"
            + "The final slide result corresponds to the average "
            + "of the quadrants that were effectively analyzed.";


        // --------------------------------------------------------
        // QUADRANTES
        // --------------------------------------------------------

        CarregarQuadrante(
            1,
            Quadrante1TituloTextBlock,
            Quadrante1ResultadoTextBlock);

        CarregarQuadrante(
            2,
            Quadrante2TituloTextBlock,
            Quadrante2ResultadoTextBlock);

        CarregarQuadrante(
            3,
            Quadrante3TituloTextBlock,
            Quadrante3ResultadoTextBlock);

        CarregarQuadrante(
            4,
            Quadrante4TituloTextBlock,
            Quadrante4ResultadoTextBlock);


        // --------------------------------------------------------
        // RESULTADO FINAL
        // --------------------------------------------------------

        NumeroCelulasTextBlock.Text =
            resultado.NumeroCelulas
                .ToString("F2");

        NumeroCelulasGrandesTextBlock.Text =
            resultado.NumeroCelulasGrandes
                .ToString("F2");

        NumeroCelulasPequenasTextBlock.Text =
            resultado.NumeroCelulasPequenas
                .ToString("F2");

        NumeroInfectadasTextBlock.Text =
            resultado.NumeroInfectadas
                .ToString("F2");
    }


    // ============================================================
    // CARREGA UM QUADRANTE
    // ============================================================

    private void CarregarQuadrante(
        int numero,
        TextBlock titulo,
        TextBlock resultadoTextBlock)
    {
        var quadrante =
            resultado.Quadrantes
                .FirstOrDefault(q =>
                    q.Quadrante == numero);


        titulo.Text =
            $"QUADRANT {numero}";


        if (quadrante == null ||
            !quadrante.Analisado)
        {
            resultadoTextBlock.Text =
                "Not analyzed.";

            return;
        }


        resultadoTextBlock.Text =
            $"Detected cells: {quadrante.NumeroCelulas:F2}\n"
            + $"Large cells: {quadrante.NumeroCelulasGrandes:F2}\n"
            + $"Small cells: {quadrante.NumeroCelulasPequenas:F2}\n"
            + $"Infected cells: {quadrante.NumeroInfectadas:F2}";
    }



    // ============================================================
    // VOLTAR AO MENU
    // ============================================================

    private void VoltarMenu_Click(
        object? sender,
        RoutedEventArgs e)
    {
        Close();

        var mainWindow =
            new MainWindow();

        mainWindow.Show();
    }


    private void MenuPrincipal_Click(
        object? sender,
        RoutedEventArgs e)
    {
        LimpezaService.LimparConsulta();

        var mainWindow =
            new MainWindow();

        mainWindow.Show();

        Close();
    }


    // ============================================================
    // NOVA ANÁLISE
    // ============================================================

    private void NovaAnalise_Click(
        object? sender,
        RoutedEventArgs e)
    {
        LimpezaService.LimparConsulta();

        var consultaWindow =
            new ConsultaWindow();

        consultaWindow.Show();

        Close();
    }


    // ============================================================
    // SALVAR PDF
    // ============================================================

    private async void SalvarPdf_Click(
        object? sender,
        RoutedEventArgs e)
    {
        var arquivo =
            await StorageProvider.SaveFilePickerAsync(
                new FilePickerSaveOptions
                {
                    Title = "Save report as PDF",
                    SuggestedFileName =
                        $"LUMA_Analysis_Result_Slide_{resultado.Lamina ?? 0}.pdf",
                    FileTypeChoices =
                    [
                        new FilePickerFileType("PDF Document")
                        {
                            Patterns =
                            [
                                "*.pdf"
                            ]
                        }
                    ]
                });


        if (arquivo == null)
            return;


        string? caminho =
            arquivo.TryGetLocalPath();


        if (string.IsNullOrWhiteSpace(caminho))
            return;


        try
        {
            PdfService.Gerar(
                resultado,
                caminho);

            await MessageBoxService.Mostrar(
                this,
                "PDF saved successfully.",
                "L.U.M.A.");
        }
        catch (Exception ex)
        {
            await MessageBoxService.Mostrar(
                this,
                "The PDF could not be generated.\n\n"
                + ex.Message,
                "Error");
        }
    }


    // ============================================================
    // FECHAR
    // ============================================================

    private void Fechar_Click(
        object? sender,
        RoutedEventArgs e)
    {
        Close();
    }
}