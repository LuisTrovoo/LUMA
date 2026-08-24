using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AnalisadorAmastigotas.Models;
using AnalisadorAmastigotas.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnalisadorAmastigotas.Views;

public partial class AnaliseImagemWindow : Window
{
    private int indiceImagemAtual = -1;

    private int QuadranteAtual =>
        ConsultaAtual.Dados.Quadrante ?? 1;

    private int PocoAtual =>
        ConsultaAtual.Dados.Poco ?? 1;


    public AnaliseImagemWindow()
    {
        InitializeComponent();

        CarregarInformacoesConsulta();
        AtualizarTela();
    }


    // ============================================================
    // OBTÉM AS IMAGENS DO POÇO E QUADRANTE ATUAIS
    // ============================================================

    private List<ImagemAnalise> ObterImagensDoQuadranteAtual()
    {
        return ConsultaAtual.Imagens
            .Where(imagem =>
                imagem.Poco == ConsultaAtual.Dados.Poco &&
                imagem.Quadrante == ConsultaAtual.Dados.Quadrante)
            .ToList();
    }


    // ============================================================
    // CARREGA AS INFORMAÇÕES DA CONSULTA
    // ============================================================

    private void CarregarInformacoesConsulta()
    {
        DoencaTextBlock.Text =
            ConsultaAtual.Dados.Doenca ?? "—";

        LinhagemTextBlock.Text =
            ConsultaAtual.Dados.Linhagem ?? "—";

        LaminaTextBlock.Text =
            ConsultaAtual.Dados.Lamina.HasValue
                ? $"Lâmina {ConsultaAtual.Dados.Lamina}"
                : "—";

        PocoTextBlock.Text =
            ConsultaAtual.Dados.Poco.HasValue
                ? $"Poço {ConsultaAtual.Dados.Poco}"
                : "—";

        QuadranteTextBlock.Text =
            ConsultaAtual.Dados.Quadrante.HasValue
                ? $"Quadrante {ConsultaAtual.Dados.Quadrante}"
                : "—";
    }


    // ============================================================
    // ATUALIZA A TELA
    // ============================================================

    private void AtualizarTela()
    {
        var imagensDoQuadrante =
            ObterImagensDoQuadranteAtual();

        int quantidade =
            imagensDoQuadrante.Count;


        // ========================================================
        // NENHUMA IMAGEM NO QUADRANTE
        // ========================================================

        if (quantidade == 0)
        {
            indiceImagemAtual = -1;

            MostrarEstadoSemImagem();

            ImagemAtualTextBlock.Text =
                "Imagem 0 de 0";

            NumeroImagemTextBlock.Text =
                "Imagem 0";

            ResultadoTextBlock.Text =
                "Nenhuma imagem analisada.";

            AnteriorButton.IsEnabled = false;
            ProximaButton.IsEnabled = false;
            RemoverButton.IsEnabled = false;
            RemoverCelulaButton.IsEnabled = false;

            ProximoQuadranteButton.IsEnabled =
                false;

            return;
        }


        // ========================================================
        // GARANTE ÍNDICE VÁLIDO
        // ========================================================

        if (indiceImagemAtual < 0)
            indiceImagemAtual = 0;

        if (indiceImagemAtual >= quantidade)
            indiceImagemAtual = quantidade - 1;


        var imagem =
            imagensDoQuadrante[indiceImagemAtual];


        // ========================================================
        // INFORMAÇÕES DA IMAGEM
        // ========================================================

        ImagemAtualTextBlock.Text =
            $"Imagem {indiceImagemAtual + 1} de {quantidade}";

        NumeroImagemTextBlock.Text =
            $"Imagem {imagem.Numero}";


        AnteriorButton.IsEnabled =
            indiceImagemAtual > 0;

        ProximaButton.IsEnabled =
            indiceImagemAtual < quantidade - 1;

        RemoverButton.IsEnabled = true;
        RemoverCelulaButton.IsEnabled = true;


        // ========================================================
        // PRÓXIMO QUADRANTE
        // ========================================================

        ProximoQuadranteButton.IsEnabled =
            QuadranteAtual < 4;


        // ========================================================
        // RESULTADO DA ANÁLISE
        // ========================================================

        if (imagem.ResultadoTrovo != null)
        {
            var resultado =
                imagem.ResultadoTrovo;

            ResultadoTextBlock.Text =
                $"Células detectadas: {resultado.NumeroCelulas}\n"
                + $"Células grandes: {resultado.NumeroCelulasGrandes}\n"
                + $"Células pequenas: {resultado.NumeroCelulasPequenas}\n"
                + $"Células infectadas: {resultado.NumeroInfectadas}";


            if (!string.IsNullOrEmpty(
                    resultado.CaminhoImagemOriginal)
                && File.Exists(
                    resultado.CaminhoImagemOriginal))
            {
                CellViewer.CarregarResultado(
                    resultado.CaminhoImagemOriginal,
                    resultado);

                CellViewer.IsVisible = true;
                SemImagemTextBlock.IsVisible = false;
            }
            else
            {
                CellViewer.Limpar();

                CellViewer.IsVisible = false;
                SemImagemTextBlock.IsVisible = true;
            }
        }
        else
        {
            ResultadoTextBlock.Text =
                "Imagem carregada.\n"
                + "Aguardando resultado do Trovo.";

            CellViewer.Limpar();

            CellViewer.IsVisible = false;
            SemImagemTextBlock.IsVisible = true;
        }
    }


    // ============================================================
    // ESTADO CENTRAL - SEM IMAGEM
    // ============================================================

    private void MostrarEstadoSemImagem()
    {
        SemImagemTextBlock.IsVisible = true;
        CellViewer.IsVisible = false;

        EstadoImagemIconeTextBlock.Text =
            "＋";

        EstadoImagemTituloTextBlock.Text =
            "NENHUMA IMAGEM ADICIONADA";

        EstadoImagemSubtituloTextBlock.Text =
            "Adicione uma imagem para iniciar a análise";

        AdicionarImagemVazioButton.IsVisible =
            true;

        AdicionarImagemVazioButton.IsEnabled =
            true;
    }


    // ============================================================
    // ESTADO CENTRAL - PROCESSANDO A PRIMEIRA IMAGEM
    // ============================================================

    private void MostrarEstadoProcessando()
    {
        CellViewer.Limpar();

        CellViewer.IsVisible =
            false;

        SemImagemTextBlock.IsVisible =
            true;

        EstadoImagemIconeTextBlock.Text =
            "⌛";

        EstadoImagemTituloTextBlock.Text =
            "PROCESSANDO IMAGEM";

        EstadoImagemSubtituloTextBlock.Text =
            "O processamento está sendo realizado. Aguarde...";

        AdicionarImagemVazioButton.IsVisible =
            false;
    }


    // ============================================================
    // IMAGEM ANTERIOR
    // ============================================================

    private void Anterior_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (indiceImagemAtual <= 0)
            return;

        indiceImagemAtual--;

        AtualizarTela();
    }


    // ============================================================
    // PRÓXIMA IMAGEM
    // ============================================================

    private void Proxima_Click(
        object? sender,
        RoutedEventArgs e)
    {
        var imagensDoQuadrante =
            ObterImagensDoQuadranteAtual();

        if (indiceImagemAtual >=
            imagensDoQuadrante.Count - 1)
            return;

        indiceImagemAtual++;

        AtualizarTela();
    }


    // ============================================================
    // ADICIONAR IMAGEM
    // ============================================================

    private async void Adicionar_Click(
        object? sender,
        RoutedEventArgs e)
    {
        var arquivos =
            await StorageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    Title = "Selecionar imagem",

                    AllowMultiple = false,

                    FileTypeFilter =
                    [
                        new FilePickerFileType("Imagens")
                        {
                            Patterns =
                            [
                                "*.png",
                                "*.jpg",
                                "*.jpeg",
                                "*.tif",
                                "*.tiff"
                            ]
                        }
                    ]
                });


        if (arquivos.Count == 0)
            return;


        var arquivo =
            arquivos[0];


        string? caminho =
            arquivo.TryGetLocalPath();


        if (string.IsNullOrEmpty(caminho))
        {
            ResultadoTextBlock.Text =
                "Não foi possível obter o caminho da imagem.";

            return;
        }


        bool primeiraImagemDoQuadrante =
            ObterImagensDoQuadranteAtual().Count == 0;


        try
        {
            ResultadoTextBlock.Text =
                "Processando imagem...\n"
                + "Aguarde.";


            // ====================================================
            // PRIMEIRA IMAGEM:
            // TROCA O ESTADO VAZIO PELO ESTADO DE PROCESSAMENTO
            // ====================================================

            if (primeiraImagemDoQuadrante)
            {
                MostrarEstadoProcessando();
            }


            var trovo =
                new TrovoService();


            TrovoResultado resultadoTrovo =
                await trovo.ProcessarImagemAsync(
                    caminho);


            if (string.IsNullOrEmpty(
                    resultadoTrovo.CaminhoImagemOriginal))
            {
                resultadoTrovo.CaminhoImagemOriginal =
                    caminho;
            }


            if (!File.Exists(
                    resultadoTrovo.CaminhoImagemOriginal))
            {
                throw new FileNotFoundException(
                    "A imagem original não foi encontrada.",
                    resultadoTrovo.CaminhoImagemOriginal);
            }


            // ====================================================
            // IMAGENS DO QUADRANTE ATUAL
            // ====================================================

            var imagensDoQuadrante =
                ObterImagensDoQuadranteAtual();


            // ====================================================
            // CRIA NOVA IMAGEM
            // ====================================================

            var novaImagem =
                new ImagemAnalise
                {
                    Numero =
                        imagensDoQuadrante.Count + 1,

                    Poco =
                        ConsultaAtual.Dados.Poco,

                    Quadrante =
                        ConsultaAtual.Dados.Quadrante,

                    ResultadoTrovo =
                        resultadoTrovo
                };


            ConsultaAtual.Imagens.Add(
                novaImagem);


            // ====================================================
            // SELECIONA A IMAGEM RECÉM-ADICIONADA
            // ====================================================

            indiceImagemAtual =
                ObterImagensDoQuadranteAtual().Count - 1;


            AtualizarTela();
        }
        catch (Exception ex)
        {
            ResultadoTextBlock.Text =
                "Erro durante o processamento:\n"
                + ex.Message;


            // Se era a primeira imagem, volta ao estado inicial
            // para permitir que o usuário tente novamente.

            if (primeiraImagemDoQuadrante)
            {
                MostrarEstadoSemImagem();
            }
            else
            {
                AtualizarTela();
            }
        }
    }


    // ============================================================
    // REMOVER IMAGEM
    // ============================================================

    private async void Remover_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (indiceImagemAtual < 0)
            return;


        var imagensDoQuadrante =
            ObterImagensDoQuadranteAtual();


        if (indiceImagemAtual >=
            imagensDoQuadrante.Count)
            return;


        var confirmacao =
            new ConfirmacaoWindow();


        await confirmacao.ShowDialog(this);


        if (!confirmacao.Confirmado)
            return;


        var imagem =
            imagensDoQuadrante[indiceImagemAtual];


        ConsultaAtual.Imagens.Remove(imagem);


        int quantidadeRestante =
            ObterImagensDoQuadranteAtual().Count;


        if (quantidadeRestante == 0)
        {
            indiceImagemAtual = -1;
        }
        else if (indiceImagemAtual >= quantidadeRestante)
        {
            indiceImagemAtual =
                quantidadeRestante - 1;
        }


        AtualizarTela();
    }


    // ============================================================
    // PRÓXIMO QUADRANTE
    // ============================================================

    private async void ProximoQuadrante_Click(
        object? sender,
        RoutedEventArgs e)
    {
        var imagensDoQuadrante =
            ObterImagensDoQuadranteAtual();


        // Não permite avançar sem analisar pelo menos uma imagem.

        if (imagensDoQuadrante.Count == 0)
            return;


        int quadranteAtual =
            ConsultaAtual.Dados.Quadrante ?? 1;


        // Não existe quadrante 5.

        if (quadranteAtual >= 4)
            return;


        var confirmacao =
            new ConfirmacaoWindow();


        await confirmacao.ShowDialog(this);


        if (!confirmacao.Confirmado)
            return;


        // ========================================================
        // AVANÇA PARA O PRÓXIMO QUADRANTE
        // ========================================================

        ConsultaAtual.Dados.Quadrante =
            quadranteAtual + 1;


        // ========================================================
        // COMEÇA PELA PRIMEIRA IMAGEM DO NOVO QUADRANTE
        // ========================================================

        indiceImagemAtual = -1;


        CarregarInformacoesConsulta();

        AtualizarTela();
    }


    // ============================================================
    // FINALIZAR CONSULTA
    // ============================================================

    private async void Finalizar_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (ConsultaAtual.Imagens.Count == 0)
            return;


        var confirmacao =
            new ConfirmacaoWindow();


        await confirmacao.ShowDialog(this);


        if (!confirmacao.Confirmado)
            return;


        try
        {
            // ========================================================
            // CALCULA O GABARITO
            // ========================================================

            ResultadoGabarito resultado =
                GabaritoService.Calcular();


            // ========================================================
            // ABRE O RESULTADO
            // ========================================================

            var resultadoWindow =
                new ResultadoWindow(resultado);


            resultadoWindow.Show();

            Close();
        }
        catch (Exception ex)
        {
            ResultadoTextBlock.Text =
                "Erro ao calcular o resultado:\n"
                + ex.Message;
        }
    }


    // ============================================================
    // REMOVER CÉLULA
    // ============================================================

    private void RemoverCelula_Click(
        object? sender,
        RoutedEventArgs e)
    {
        bool removida =
            CellViewer.ExcluirSelecionada();


        if (!removida)
            return;


        AtualizarTela();
    }
}