using System.Collections.Generic;

namespace AnalisadorAmastigotas.Models;

public static class ConsultaAtual
{
    public static Consulta Dados { get; set; } = new Consulta();

    public static List<ImagemAnalise> Imagens { get; set; } = new();
}