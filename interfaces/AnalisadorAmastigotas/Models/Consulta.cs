using System.Collections.Generic;

namespace AnalisadorAmastigotas.Models;

public class Consulta
{
    public string? Doenca { get; set; }

    public string? Linhagem { get; set; }

    public int? Lamina { get; set; }

    public int? Poco { get; set; }

    public int? Quadrante { get; set; }

    public List<AnaliseImagem> Imagens { get; set; } = new();

    public List<ResultadoQuadrante> ResultadosQuadrantes { get; set; } = new();
}