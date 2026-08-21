namespace AnalisadorAmastigotas.Models;

public class ImagemAnalise
{
    public int Numero { get; set; }

    public int? Poco { get; set; }

    public int? Quadrante { get; set; }

    public TrovoResultado? ResultadoTrovo { get; set; }
}