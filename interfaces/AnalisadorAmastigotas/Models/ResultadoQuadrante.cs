namespace AnalisadorAmastigotas.Models;

public class ResultadoQuadrante
{
    public int Poco { get; set; }

    public int Quadrante { get; set; }

    public int QuantidadeImagens { get; set; }

    public double NumeroCelulas { get; set; }

    public double NumeroCelulasGrandes { get; set; }

    public double NumeroCelulasPequenas { get; set; }

    public double NumeroInfectadas { get; set; }

    public bool Analisado { get; set; }
}