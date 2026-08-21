using AnalisadorAmastigotas.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnalisadorAmastigotas.Services;

public static class GabaritoService
{
    // ============================================================
    // CALCULA O RESULTADO FINAL DA CONSULTA
    // ============================================================

    public static ResultadoGabarito Calcular()
    {
        var imagens =
            ConsultaAtual.Imagens;


        if (imagens.Count == 0)
        {
            throw new InvalidOperationException(
                "Não existem imagens analisadas.");
        }


        // ========================================================
        // RESULTADOS DOS QUADRANTES
        // ========================================================

        var resultadosQuadrantes =
            new List<ResultadoQuadrante>();


        for (int quadrante = 1; quadrante <= 4; quadrante++)
        {
            var imagensDoQuadrante =
                imagens
                    .Where(imagem =>
                        imagem.Quadrante == quadrante &&
                        imagem.ResultadoTrovo != null)
                    .ToList();


            if (imagensDoQuadrante.Count == 0)
            {
                resultadosQuadrantes.Add(
                    new ResultadoQuadrante
                    {
                        Quadrante = quadrante,
                        Analisado = false
                    });

                continue;
            }


            var resultado =
                CalcularMediaDoQuadrante(
                    imagensDoQuadrante);


            resultado.Quadrante =
                quadrante;

            resultado.Analisado =
                true;


            resultadosQuadrantes.Add(
                resultado);
        }


        // ========================================================
        // QUADRANTES REALMENTE ANALISADOS
        // ========================================================

        var quadrantesAnalisados =
            resultadosQuadrantes
                .Where(q => q.Analisado)
                .ToList();


        if (quadrantesAnalisados.Count == 0)
        {
            throw new InvalidOperationException(
                "Nenhum quadrante possui resultado.");
        }


        // ========================================================
        // MÉDIA FINAL DA LÂMINA
        // ========================================================

        double mediaCelulas =
            quadrantesAnalisados
                .Average(q => q.NumeroCelulas);

        double mediaGrandes =
            quadrantesAnalisados
                .Average(q => q.NumeroCelulasGrandes);

        double mediaPequenas =
            quadrantesAnalisados
                .Average(q => q.NumeroCelulasPequenas);

        double mediaInfectadas =
            quadrantesAnalisados
                .Average(q => q.NumeroInfectadas);


        // ========================================================
        // RESULTADO FINAL
        // ========================================================

        return new ResultadoGabarito
        {
            Doenca =
                ConsultaAtual.Dados.Doenca,

            Linhagem =
                ConsultaAtual.Dados.Linhagem,

            Lamina =
                ConsultaAtual.Dados.Lamina,

            Poco =
                ConsultaAtual.Dados.Poco,

            QuadrantesAnalisados =
                quadrantesAnalisados.Count,

            Quadrantes =
                resultadosQuadrantes,

            NumeroCelulas =
                mediaCelulas,

            NumeroCelulasGrandes =
                mediaGrandes,

            NumeroCelulasPequenas =
                mediaPequenas,

            NumeroInfectadas =
                mediaInfectadas
        };
    }


    // ============================================================
    // MÉDIA DAS IMAGENS DE UM QUADRANTE
    // ============================================================

    private static ResultadoQuadrante
        CalcularMediaDoQuadrante(
            List<ImagemAnalise> imagens)
    {
        var resultados =
            imagens
                .Where(imagem =>
                    imagem.ResultadoTrovo != null)
                .Select(imagem =>
                    imagem.ResultadoTrovo!)
                .ToList();


        if (resultados.Count == 0)
        {
            return new ResultadoQuadrante();
        }


        return new ResultadoQuadrante
        {
            NumeroCelulas =
                resultados.Average(
                    resultado =>
                        resultado.NumeroCelulas),

            NumeroCelulasGrandes =
                resultados.Average(
                    resultado =>
                        resultado.NumeroCelulasGrandes),

            NumeroCelulasPequenas =
                resultados.Average(
                    resultado =>
                        resultado.NumeroCelulasPequenas),

            NumeroInfectadas =
                resultados.Average(
                    resultado =>
                        resultado.NumeroInfectadas)
        };
    }
}