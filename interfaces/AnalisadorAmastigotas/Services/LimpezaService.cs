using System;
using System.IO;

namespace AnalisadorAmastigotas.Services;

public static class LimpezaService
{
    public static void LimparResultadosTemporarios()
    {
        try
        {
            foreach (var imagem in Models.ConsultaAtual.Imagens)
            {
                if (string.IsNullOrWhiteSpace(
                    imagem.ResultadoTrovo?.PastaResultados))
                {
                    continue;
                }

                string pasta =
                    imagem.ResultadoTrovo.PastaResultados;

                if (Directory.Exists(pasta))
                {
                    Directory.Delete(
                        pasta,
                        recursive: true);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Erro ao limpar arquivos temporários: {ex.Message}");
        }
    }


    public static void LimparConsulta()
    {
        LimparResultadosTemporarios();

        Models.ConsultaAtual.Imagens.Clear();

        Models.ConsultaAtual.Dados =
            new Models.Consulta();
    }
}