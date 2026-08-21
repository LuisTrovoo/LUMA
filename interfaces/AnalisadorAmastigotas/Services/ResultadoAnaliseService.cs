using System;
using System.Collections.Generic;
using System.Linq;
using AnalisadorAmastigotas.Models;

namespace AnalisadorAmastigotas.Services;

public static class ResultadoAnaliseService
{
    // ============================================================
    // CALCULA O RESULTADO DE UM QUADRANTE
    // ============================================================

    public static ResultadoQuadrante CalcularQuadrante(
        int poco,
        int quadrante,
        IEnumerable<ImagemAnalise> imagens)
    {
        var lista =
            imagens
                .Where(x =>
                    x.Poco == poco &&
                    x.Quadrante == quadrante &&
                    x.ResultadoTrovo != null)
                .ToList();


        // ========================================================
        // QUADRANTE SEM IMAGENS
        // ========================================================

        if (lista.Count == 0)
        {
            return new ResultadoQuadrante
            {
                Poco = poco,
                Quadrante = quadrante,
                QuantidadeImagens = 0,
                Analisado = false
            };
        }


        // ========================================================
        // CALCULA A MÉDIA DAS IMAGENS DO QUADRANTE
        // ========================================================

        return new ResultadoQuadrante
        {
            Poco = poco,
            Quadrante = quadrante,

            QuantidadeImagens =
                lista.Count,

            NumeroCelulas =
                lista.Average(
                    x => x.ResultadoTrovo!.NumeroCelulas),

            NumeroCelulasGrandes =
                lista.Average(
                    x => x.ResultadoTrovo!.NumeroCelulasGrandes),

            NumeroCelulasPequenas =
                lista.Average(
                    x => x.ResultadoTrovo!.NumeroCelulasPequenas),

            NumeroInfectadas =
                lista.Average(
                    x => x.ResultadoTrovo!.NumeroInfectadas),

            Analisado = true
        };
    }


    // ============================================================
    // COMPLETA OS 4 QUADRANTES
    // ============================================================

    public static List<ResultadoQuadrante>
        CompletarQuadrantes(
            IEnumerable<ResultadoQuadrante> resultados,
            int poco)
    {
        var existentes =
            resultados
                .Where(x => x.Poco == poco)
                .ToList();


        if (existentes.Count == 0)
            return new List<ResultadoQuadrante>();


        // ========================================================
        // MÉDIA DOS QUADRANTES REALMENTE ANALISADOS
        // ========================================================

        double mediaCelulas =
            existentes
                .Where(x => x.Analisado)
                .Average(x => x.NumeroCelulas);

        double mediaGrandes =
            existentes
                .Where(x => x.Analisado)
                .Average(x => x.NumeroCelulasGrandes);

        double mediaPequenas =
            existentes
                .Where(x => x.Analisado)
                .Average(x => x.NumeroCelulasPequenas);

        double mediaInfectadas =
            existentes
                .Where(x => x.Analisado)
                .Average(x => x.NumeroInfectadas);


        // ========================================================
        // GERA OS 4 QUADRANTES
        // ========================================================

        var resultadoFinal =
            new List<ResultadoQuadrante>();


        for (int quadrante = 1;
             quadrante <= 4;
             quadrante++)
        {
            var existente =
                existentes.FirstOrDefault(
                    x => x.Quadrante == quadrante);


            // ====================================================
            // QUADRANTE JÁ ANALISADO
            // ====================================================

            if (existente != null &&
                existente.Analisado)
            {
                resultadoFinal.Add(
                    existente);

                continue;
            }


            // ====================================================
            // QUADRANTE NÃO ANALISADO
            // Usa a média dos quadrantes disponíveis
            // ====================================================

            resultadoFinal.Add(
                new ResultadoQuadrante
                {
                    Poco = poco,

                    Quadrante = quadrante,

                    QuantidadeImagens = 0,

                    NumeroCelulas =
                        mediaCelulas,

                    NumeroCelulasGrandes =
                        mediaGrandes,

                    NumeroCelulasPequenas =
                        mediaPequenas,

                    NumeroInfectadas =
                        mediaInfectadas,

                    Analisado = false
                });
        }


        return resultadoFinal;
    }
}