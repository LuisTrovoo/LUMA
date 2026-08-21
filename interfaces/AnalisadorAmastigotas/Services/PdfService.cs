using AnalisadorAmastigotas.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;

namespace AnalisadorAmastigotas.Services;

public static class PdfService
{
    // ============================================================
    // CORES DA IDENTIDADE VISUAL DO L.U.M.A.
    // ============================================================

    private const string Fundo = "#F8F6FA";
    private const string FundoCabecalho = "#EEEAF2";

    private const string Roxo = "#5E2B97";
    private const string RoxoEscuro = "#3D1E6D";
    private const string RoxoPrincipal = "#713BD0";

    private const string Rosa = "#D83D94";
    private const string RosaClaro = "#F8EAF3";

    private const string Texto = "#555555";
    private const string TextoSecundario = "#777777";

    private const string Borda = "#E2DAEB";
    private const string BordaEscura = "#D8D2DE";

    private const string Branco = "#FFFFFF";


    // ============================================================
    // GERAR PDF
    // ============================================================

    public static void Gerar(
        ResultadoGabarito resultado,
        string caminho)
    {
        QuestPDF.Settings.License =
            LicenseType.Community;


        Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);

                page.MarginHorizontal(40);
                page.MarginVertical(35);



                // =================================================
                // CABEÇALHO
                // =================================================

                page.Header()
                    .Column(header =>
                    {
                        // Faixa principal
                        header.Item()
                            .Background(FundoCabecalho)
                            .Border(1)
                            .BorderColor(Borda)
                            .Padding(20)
                            .Row(row =>
                            {
                                row.RelativeItem()
                                    .Column(column =>
                                    {
                                        column.Item()
                                            .Text("L.U.M.A.")
                                            .FontSize(28)
                                            .Bold()
                                            .FontColor(Roxo);

                                        column.Item()
                                            .PaddingTop(3)
                                            .Text("RESULTADO DA ANÁLISE")
                                            .FontSize(16)
                                            .Bold()
                                            .FontColor(RoxoEscuro);
                                    });
                            });


                        // Barra de identidade visual
                        header.Item()
                            .Height(4)
                            .Background(Rosa);
                    });


                // =================================================
                // CONTEÚDO
                // =================================================

                page.Content()
                    .PaddingTop(20)
                    .Column(column =>
                    {
                        // =================================================
                        // IDENTIFICAÇÃO
                        // =================================================

                        TituloSecao(
                            column,
                            "IDENTIFICAÇÃO DA CONSULTA");


                        column.Item()
                            .PaddingTop(8)
                            .Background(Branco)
                            .Border(1)
                            .BorderColor(Borda)
                            .Padding(14)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });


                                table.Cell()
                                    .Element(CelulaIdentificacao)
                                    .Column(cell =>
                                    {
                                        cell.Item()
                                            .Text("DOENÇA")
                                            .FontSize(9)
                                            .Bold()
                                            .FontColor(TextoSecundario);

                                        cell.Item()
                                            .PaddingTop(3)
                                            .Text(resultado.Doenca ?? "—")
                                            .FontSize(12)
                                            .FontColor(Roxo);
                                    });


                                table.Cell()
                                    .Element(CelulaIdentificacao)
                                    .Column(cell =>
                                    {
                                        cell.Item()
                                            .Text("LINHAGEM")
                                            .FontSize(9)
                                            .Bold()
                                            .FontColor(TextoSecundario);

                                        cell.Item()
                                            .PaddingTop(3)
                                            .Text(resultado.Linhagem ?? "—")
                                            .FontSize(12)
                                            .FontColor(Roxo);
                                    });


                                table.Cell()
                                    .Element(CelulaIdentificacao)
                                    .Column(cell =>
                                    {
                                        cell.Item()
                                            .Text("LÂMINA")
                                            .FontSize(9)
                                            .Bold()
                                            .FontColor(TextoSecundario);

                                        cell.Item()
                                            .PaddingTop(3)
                                            .Text(
                                                resultado.Lamina.HasValue
                                                    ? resultado.Lamina.ToString()
                                                    : "—")
                                            .FontSize(12)
                                            .FontColor(Roxo);
                                    });


                                table.Cell()
                                    .Element(CelulaIdentificacao)
                                    .Column(cell =>
                                    {
                                        cell.Item()
                                            .Text("POÇO")
                                            .FontSize(9)
                                            .Bold()
                                            .FontColor(TextoSecundario);

                                        cell.Item()
                                            .PaddingTop(3)
                                            .Text(
                                                resultado.Poco.HasValue
                                                    ? resultado.Poco.ToString()
                                                    : "—")
                                            .FontSize(12)
                                            .FontColor(Roxo);
                                    });
                            });


                        // =================================================
                        // RESUMO
                        // =================================================

                        column.Item()
                            .PaddingTop(20);

                        TituloSecao(
                            column,
                            "RESUMO DA ANÁLISE");


                        column.Item()
                            .PaddingTop(8)
                            .Background(Branco)
                            .Border(1)
                            .BorderColor(Borda)
                            .Padding(15)
                            .Column(resumo =>
                            {
                                resumo.Item()
                                    .Text(
                                        $"{resultado.QuadrantesAnalisados} de 4 quadrantes foram analisados.")
                                    .FontSize(13)
                                    .Bold()
                                    .FontColor(RoxoEscuro);

                                resumo.Item()
                                    .PaddingTop(6)
                                    .Text(
                                        "O resultado final da lâmina corresponde à média dos quadrantes efetivamente analisados.")
                                    .FontSize(11)
                                    .FontColor(Texto);
                            });


                        // =================================================
                        // ANÁLISE DOS QUADRANTES
                        // =================================================

                        column.Item()
                            .PaddingTop(20);

                        TituloSecao(
                            column,
                            "ANÁLISE DOS QUADRANTES");


                        foreach (var quadrante in resultado.Quadrantes)
                        {
                            column.Item()
                                .PaddingTop(9)
                                .Background(Branco)
                                .Border(1)
                                .BorderColor(Borda)
                                .Padding(14)
                                .Column(q =>
                                {
                                    // Cabeçalho do quadrante
                                    q.Item()
                                        .Row(row =>
                                        {
                                            row.RelativeItem()
                                                .Text(
                                                    $"QUADRANTE {quadrante.Quadrante}")
                                                .FontSize(13)
                                                .Bold()
                                                .FontColor(RoxoEscuro);


                                            row.AutoItem()
                                                .Background(
                                                    quadrante.Analisado
                                                        ? RosaClaro
                                                        : "#F1EFF2")
                                                .PaddingHorizontal(8)
                                                .PaddingVertical(4)
                                                .Text(
                                                    quadrante.Analisado
                                                        ? "ANALISADO"
                                                        : "NÃO ANALISADO")
                                                .FontSize(8)
                                                .Bold()
                                                .FontColor(
                                                    quadrante.Analisado
                                                        ? Rosa
                                                        : TextoSecundario);
                                        });


                                    if (!quadrante.Analisado)
                                    {
                                        q.Item()
                                            .PaddingTop(8)
                                            .Text("Este quadrante não foi analisado.")
                                            .FontSize(11)
                                            .FontColor(TextoSecundario);

                                        return;
                                    }


                                    // Métricas
                                    q.Item()
                                        .PaddingTop(12)
                                        .Table(table =>
                                        {
                                            table.ColumnsDefinition(columns =>
                                            {
                                                columns.RelativeColumn();
                                                columns.RelativeColumn();
                                            });


                                            table.Cell()
                                                .Element(CelulaResultado)
                                                .Column(cell =>
                                                {
                                                    cell.Item()
                                                        .Text("CÉLULAS DETECTADAS")
                                                        .FontSize(8)
                                                        .Bold()
                                                        .FontColor(TextoSecundario);

                                                    cell.Item()
                                                        .PaddingTop(3)
                                                        .Text(
                                                            $"{quadrante.NumeroCelulas:F2}")
                                                        .FontSize(15)
                                                        .Bold()
                                                        .FontColor(Roxo);
                                                });


                                            table.Cell()
                                                .Element(CelulaResultado)
                                                .Column(cell =>
                                                {
                                                    cell.Item()
                                                        .Text("CÉLULAS GRANDES")
                                                        .FontSize(8)
                                                        .Bold()
                                                        .FontColor(TextoSecundario);

                                                    cell.Item()
                                                        .PaddingTop(3)
                                                        .Text(
                                                            $"{quadrante.NumeroCelulasGrandes:F2}")
                                                        .FontSize(15)
                                                        .Bold()
                                                        .FontColor(Roxo);
                                                });


                                            table.Cell()
                                                .Element(CelulaResultado)
                                                .Column(cell =>
                                                {
                                                    cell.Item()
                                                        .Text("CÉLULAS PEQUENAS")
                                                        .FontSize(8)
                                                        .Bold()
                                                        .FontColor(TextoSecundario);

                                                    cell.Item()
                                                        .PaddingTop(3)
                                                        .Text(
                                                            $"{quadrante.NumeroCelulasPequenas:F2}")
                                                        .FontSize(15)
                                                        .Bold()
                                                        .FontColor(Roxo);
                                                });


                                            table.Cell()
                                                .Element(CelulaResultado)
                                                .Column(cell =>
                                                {
                                                    cell.Item()
                                                        .Text("CÉLULAS INFECTADAS")
                                                        .FontSize(8)
                                                        .Bold()
                                                        .FontColor(TextoSecundario);

                                                    cell.Item()
                                                        .PaddingTop(3)
                                                        .Text(
                                                            $"{quadrante.NumeroInfectadas:F2}")
                                                        .FontSize(15)
                                                        .Bold()
                                                        .FontColor(Rosa);
                                                });
                                        });
                                });
                        }


                        // =================================================
                        // RESULTADO FINAL
                        // =================================================

                        column.Item()
                            .PaddingTop(22);

                        column.Item()
                            .Background(RoxoEscuro)
                            .Padding(18)
                            .Column(final =>
                            {
                                final.Item()
                                    .Text("RESULTADO FINAL DA LÂMINA")
                                    .FontSize(17)
                                    .Bold()
                                    .FontColor(Branco);


                                final.Item()
                                    .PaddingTop(14)
                                    .Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                        });


                                        table.Cell()
                                            .Element(CelulaFinal)
                                            .Column(cell =>
                                            {
                                                cell.Item()
                                                    .Text("CÉLULAS DETECTADAS")
                                                    .FontSize(8)
                                                    .Bold()
                                                    .FontColor("#D8C8ED");

                                                cell.Item()
                                                    .PaddingTop(3)
                                                    .Text(
                                                        $"{resultado.NumeroCelulas:F2}")
                                                    .FontSize(20)
                                                    .Bold()
                                                    .FontColor(Branco);
                                            });


                                        table.Cell()
                                            .Element(CelulaFinal)
                                            .Column(cell =>
                                            {
                                                cell.Item()
                                                    .Text("CÉLULAS INFECTADAS")
                                                    .FontSize(8)
                                                    .Bold()
                                                    .FontColor("#F3B5D4");

                                                cell.Item()
                                                    .PaddingTop(3)
                                                    .Text(
                                                        $"{resultado.NumeroInfectadas:F2}")
                                                    .FontSize(20)
                                                    .Bold()
                                                    .FontColor(Branco);
                                            });


                                        table.Cell()
                                            .Element(CelulaFinal)
                                            .Column(cell =>
                                            {
                                                cell.Item()
                                                    .Text("CÉLULAS GRANDES")
                                                    .FontSize(8)
                                                    .Bold()
                                                    .FontColor("#D8C8ED");

                                                cell.Item()
                                                    .PaddingTop(3)
                                                    .Text(
                                                        $"{resultado.NumeroCelulasGrandes:F2}")
                                                    .FontSize(18)
                                                    .Bold()
                                                    .FontColor(Branco);
                                            });


                                        table.Cell()
                                            .Element(CelulaFinal)
                                            .Column(cell =>
                                            {
                                                cell.Item()
                                                    .Text("CÉLULAS PEQUENAS")
                                                    .FontSize(8)
                                                    .Bold()
                                                    .FontColor("#D8C8ED");

                                                cell.Item()
                                                    .PaddingTop(3)
                                                    .Text(
                                                        $"{resultado.NumeroCelulasPequenas:F2}")
                                                    .FontSize(18)
                                                    .Bold()
                                                    .FontColor(Branco);
                                            });
                                    });
                            });
                    });


                // =================================================
                // RODAPÉ
                // =================================================

                page.Footer()
                    .PaddingTop(12)
                    .Row(row =>
                    {
                        row.RelativeItem()
                            .Text("L.U.M.A. — Relatório de análise")
                            .FontSize(9)
                            .FontColor(TextoSecundario);


                        row.AutoItem()
                            .Text(text =>
                            {
                                text.Span("Página ")
                                    .FontSize(9)
                                    .FontColor(TextoSecundario);

                                text.CurrentPageNumber()
                                    .FontSize(9)
                                    .FontColor(Roxo)
                                    .Bold();

                                text.Span(" de ")
                                    .FontSize(9)
                                    .FontColor(TextoSecundario);

                                text.TotalPages()
                                    .FontSize(9)
                                    .FontColor(Roxo)
                                    .Bold();
                            });
                    });
            });
        })
        .GeneratePdf(caminho);
    }


    // ============================================================
    // TÍTULO DE SEÇÃO
    // ============================================================

    private static void TituloSecao(
        ColumnDescriptor column,
        string texto)
    {
        column.Item()
            .Row(row =>
            {
                row.AutoItem()
                    .Width(4)
                    .Height(18)
                    .Background(Rosa);

                row.AutoItem()
                    .PaddingLeft(8)
                    .Text(texto)
                    .FontSize(15)
                    .Bold()
                    .FontColor(Roxo);
            });
    }


    // ============================================================
    // CÉLULAS DE IDENTIFICAÇÃO
    // ============================================================

    private static IContainer CelulaIdentificacao(
        IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Borda)
            .Padding(10);
    }


    // ============================================================
    // CÉLULAS DOS RESULTADOS
    // ============================================================

    private static IContainer CelulaResultado(
        IContainer container)
    {
        return container
            .Background(Fundo)
            .Border(1)
            .BorderColor(Borda)
            .Padding(9);
    }


    // ============================================================
    // CÉLULAS DO RESULTADO FINAL
    // ============================================================

    private static IContainer CelulaFinal(
        IContainer container)
    {
        return container
            .Background("#4B267F")
            .Border(1)
            .BorderColor("#68469A")
            .Padding(10);
    }


    // ============================================================
    // GRADIENTE DA IDENTIDADE VISUAL
    // ============================================================

}