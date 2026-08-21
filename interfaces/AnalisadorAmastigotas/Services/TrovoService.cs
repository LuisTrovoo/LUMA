using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using AnalisadorAmastigotas.Models;

namespace AnalisadorAmastigotas.Services;

public class TrovoService
{
    // ============================================================
    // PROCESSA A IMAGEM
    // ============================================================

    public async Task<TrovoResultado> ProcessarImagemAsync(
        string caminhoImagem)
    {
        if (!File.Exists(caminhoImagem))
        {
            throw new FileNotFoundException(
                "A imagem não foi encontrada.",
                caminhoImagem);
        }

        // ========================================================
        // LOCALIZA AUTOMATICAMENTE O EXECUTÁVEL
        // DE ACORDO COM O SISTEMA OPERACIONAL
        // ========================================================

        string executavelTrovo =
            LocalizarExecutavelTrovo();

        string diretorioTrovo =
            Path.GetDirectoryName(executavelTrovo)
            ?? AppContext.BaseDirectory;

        // ========================================================
        // EXECUTA O TROVO
        // ========================================================

        using var processo = new Process();

        processo.StartInfo = new ProcessStartInfo
        {
            FileName = executavelTrovo,
            WorkingDirectory = diretorioTrovo,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        // ArgumentList evita problemas com espaços
        // no caminho da imagem.
        processo.StartInfo.ArgumentList.Add(caminhoImagem);

        processo.Start();

        // ========================================================
        // LÊ SAÍDA E ERROS
        // ========================================================

        Task<string> tarefaSaida =
            processo.StandardOutput.ReadToEndAsync();

        Task<string> tarefaErro =
            processo.StandardError.ReadToEndAsync();

        await processo.WaitForExitAsync();

        string saida =
            await tarefaSaida;

        string erro =
            await tarefaErro;

        // ========================================================
        // VERIFICA SE O TROVO TERMINOU COM ERRO
        // ========================================================

        if (processo.ExitCode != 0)
        {
            throw new Exception(
                "O Trovo encontrou um erro durante o processamento.\n\n" +
                $"Erro:\n{erro}\n\n" +
                $"Saída do Trovo:\n{saida}");
        }

        // ========================================================
        // LOCALIZA A PASTA DE RESULTADOS
        // ========================================================

        string nomeImagem =
            Path.GetFileNameWithoutExtension(caminhoImagem);

        string? diretorioImagem =
            Path.GetDirectoryName(caminhoImagem);

        if (string.IsNullOrEmpty(diretorioImagem))
        {
            throw new Exception(
                "Não foi possível determinar a pasta da imagem.");
        }

        string pastaResultados =
            Path.Combine(
                diretorioImagem,
                nomeImagem + "_results");

        if (!Directory.Exists(pastaResultados))
        {
            throw new DirectoryNotFoundException(
                "A pasta de resultados não foi encontrada:\n" +
                pastaResultados);
        }

        // ========================================================
        // LÊ RESULTADO JSON
        // ========================================================

        string arquivoJson =
            Path.Combine(
                pastaResultados,
                "result.json");

        if (!File.Exists(arquivoJson))
        {
            throw new FileNotFoundException(
                "O arquivo result.json não foi encontrado.",
                arquivoJson);
        }

        string json =
            await File.ReadAllTextAsync(arquivoJson);

        var dados =
            JsonSerializer.Deserialize<TrovoResultadoJson>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (dados == null)
        {
            throw new Exception(
                "Não foi possível interpretar o resultado do Trovo.");
        }

        // ========================================================
        // MONTA RESULTADO
        // ========================================================

        var resultado = new TrovoResultado
        {
            CaminhoImagemOriginal =
                caminhoImagem,

            PastaResultados =
                pastaResultados,

            ArquivoResultados =
                Path.Combine(
                    pastaResultados,
                    "results.mat"),

            CaminhoImagemAnalisada =
                Path.Combine(
                    pastaResultados,
                    "labels.png"),

            NumeroCelulas =
                dados.numero_celulas,

            NumeroCelulasGrandes =
                dados.numero_celulas_grandes,

            NumeroCelulasPequenas =
                dados.numero_celulas_pequenas,

            NumeroInfectadas =
                dados.numero_infectadas,

            Centroides =
                dados.centroides ?? new List<PontoCelula>(),

            CelulasGrandes =
                dados.celulas_grandes ?? new List<int>()
        };

        // ========================================================
        // DEBUG
        // ========================================================

        Console.WriteLine();
        Console.WriteLine("=================================");
        Console.WriteLine("TROVO RESULTADO");
        Console.WriteLine(
            $"Células: {resultado.NumeroCelulas}");
        Console.WriteLine(
            $"Células grandes: {resultado.NumeroCelulasGrandes}");
        Console.WriteLine(
            $"Células pequenas: {resultado.NumeroCelulasPequenas}");
        Console.WriteLine(
            $"Células infectadas: {resultado.NumeroInfectadas}");
        Console.WriteLine(
            $"Centroides recebidos: {resultado.Centroides.Count}");
        Console.WriteLine(
            $"Células grandes recebidas: {resultado.CelulasGrandes.Count}");
        Console.WriteLine("=================================");
        Console.WriteLine();

        return resultado;
    }


    // ============================================================
    // LOCALIZA O EXECUTÁVEL DO TROVO
    // ============================================================

    private static string LocalizarExecutavelTrovo()
    {
        string baseAplicacao =
            AppContext.BaseDirectory;

        string caminhoExecutavel;
        string sistema;

        // ========================================================
        // WINDOWS
        // ========================================================

        if (RuntimeInformation.IsOSPlatform(
                OSPlatform.Windows))
        {
            sistema = "Windows";

            caminhoExecutavel =
                Path.Combine(
                    baseAplicacao,
                    "processing",
                    "windows",
                    "process_image.exe");
        }

        // ========================================================
        // LINUX
        // ========================================================

        else if (RuntimeInformation.IsOSPlatform(
                     OSPlatform.Linux))
        {
            sistema = "Linux";

            caminhoExecutavel =
                Path.Combine(
                    baseAplicacao,
                    "processing",
                    "linux",
                    "process_image");
        }

        // ========================================================
        // MACOS
        // ========================================================

        else if (RuntimeInformation.IsOSPlatform(
                     OSPlatform.OSX))
        {
            sistema = "macOS";

            caminhoExecutavel =
                Path.Combine(
                    baseAplicacao,
                    "processing",
                    "macos",
                    "process_image");
        }

        // ========================================================
        // SISTEMA NÃO SUPORTADO
        // ========================================================

        else
        {
            throw new PlatformNotSupportedException(
                "O sistema operacional deste computador " +
                "não é suportado pelo Analisador de Amastigotas.");
        }

        // ========================================================
        // VERIFICA SE O EXECUTÁVEL EXISTE
        // ========================================================

        if (!File.Exists(caminhoExecutavel))
        {
            throw new FileNotFoundException(
                "O executável do Trovo não foi encontrado.\n\n" +
                $"Sistema detectado: {sistema}\n" +
                $"Caminho esperado:\n{caminhoExecutavel}");
        }

        // ========================================================
        // LINUX / MACOS
        // GARANTE PERMISSÃO DE EXECUÇÃO
        // ========================================================

        if (!RuntimeInformation.IsOSPlatform(
                OSPlatform.Windows))
        {
            try
            {
                var informacoes =
                    new FileInfo(caminhoExecutavel);

                if ((informacoes.UnixFileMode &
                     UnixFileMode.UserExecute) == 0)
                {
                    informacoes.UnixFileMode |=
                        UnixFileMode.UserExecute;
                }
            }
            catch
            {
                // Se não for possível alterar a permissão,
                // o erro real será informado ao tentar executar.
            }
        }

        return caminhoExecutavel;
    }


    // ============================================================
    // MODELO DO JSON GERADO PELO TROVO
    // ============================================================

    private class TrovoResultadoJson
    {
        public int numero_celulas { get; set; }

        public int numero_celulas_grandes { get; set; }

        public int numero_celulas_pequenas { get; set; }

        public int numero_infectadas { get; set; }

        public List<PontoCelula>? centroides { get; set; }

        public List<int>? celulas_grandes { get; set; }

        public string? pasta_resultados { get; set; }

        public string? arquivo_mat { get; set; }
    }
}