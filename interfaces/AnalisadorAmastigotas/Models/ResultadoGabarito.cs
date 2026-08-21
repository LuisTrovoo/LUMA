using System.Collections.Generic;

namespace AnalisadorAmastigotas.Models;

public class ResultadoGabarito
{
    // ============================================================
    // IDENTIFICAÇÃO DA CONSULTA
    // ============================================================

    public string? Doenca { get; set; }

    public string? Linhagem { get; set; }

    public int? Lamina { get; set; }

    public int? Poco { get; set; }


    // ============================================================
    // QUADRANTES
    // ============================================================

    public int QuadrantesAnalisados { get; set; }

    public List<ResultadoQuadrante> Quadrantes { get; set; } = new();


    // ============================================================
    // RESULTADO FINAL DA LÂMINA
    // ============================================================

    public double NumeroCelulas { get; set; }

    public double NumeroCelulasGrandes { get; set; }

    public double NumeroCelulasPequenas { get; set; }

    public double NumeroInfectadas { get; set; }
}