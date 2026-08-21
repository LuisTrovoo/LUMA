using System.Collections.Generic;

namespace AnalisadorAmastigotas.Models;

public class TrovoResultado
{
    public string? CaminhoImagemOriginal { get; set; }

    public string? PastaResultados { get; set; }

    public string? ArquivoResultados { get; set; }

    public string? CaminhoImagemAnalisada { get; set; }

    public int NumeroCelulas { get; set; }

    public int NumeroCelulasGrandes { get; set; }

    public int NumeroCelulasPequenas { get; set; }

    public int NumeroInfectadas { get; set; }

    public List<PontoCelula> Centroides { get; set; } = new();

    public List<int> CelulasGrandes { get; set; } = new();

    public Dictionary<int, List<int>> GruposInfeccao { get; set; } = new();

    public List<int> CelulasRemovidas { get; set; } = new();
}