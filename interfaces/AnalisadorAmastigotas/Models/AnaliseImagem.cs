namespace AnalisadorAmastigotas.Models;

public class AnaliseImagem
{
    public int Id { get; set; }

    public string? CaminhoImagemOriginal { get; set; }

    public string? CaminhoImagemAnalisada { get; set; }

    public ResultadoAnalise? Resultado { get; set; }
}