using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia;
using AnalisadorAmastigotas.Models;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

using System;
using System.Collections.Generic;
using System.IO;

using AvaloniaPoint = Avalonia.Point;
using AvaloniaVector = Avalonia.Vector;

//using Avalonia;


namespace AnalisadorAmastigotas.Views;

public partial class CellViewer : UserControl
{
    private Bitmap? imagemBitmap;

    private readonly List<Ellipse> marcacoes = new();

    private List<PontoCelula> centroides = new();

    private List<int> celulasGrandes = new();

    private readonly HashSet<int> celulasRemovidas = new();

    private int? celulaSelecionada = null;

    // ============================================================
    // NAVEGAÇÃO COM MOUSE
    // ============================================================

    private bool arrastandoImagem = false;

    private AvaloniaPoint pontoInicialArraste;

    private double scrollInicialHorizontal;

    private double scrollInicialVertical;

    private TrovoResultado? resultadoAtual;


    // ============================================================
    // TAMANHO ORIGINAL DA IMAGEM
    // ============================================================

    private double larguraImagemOriginal;

    private double alturaImagemOriginal;


    // ============================================================
    // ESCALA ATUAL DA IMAGEM
    // ============================================================

    private double escalaAtual = 1.0;

    private const double ZoomMinimo = 0.25;
    private const double ZoomMaximo = 4.0;
    private const double FatorZoom = 1.15;


    public CellViewer()
    {
        InitializeComponent();

        Focusable = true;
        Focus();
    }


    // ============================================================
    // REDIMENSIONAMENTO DA TELA
    // ============================================================

    private void CellViewer_SizeChanged(
        object? sender,
        SizeChangedEventArgs e)
    {
        if (imagemBitmap == null)
            return;

        AjustarImagemAoEspaco();
    }


    // ============================================================
    // ZOOM COM A RODA DO MOUSE
    // ============================================================

    // ============================================================
    // ZOOM COM CTRL + RODA DO MOUSE / TOUCHPAD
    // ============================================================

    private void ImagemScrollViewer_PointerWheelChanged(
        object? sender,
        PointerWheelEventArgs e)
    {
        if (imagemBitmap == null)
            return;

        // Sem Ctrl:
        // deixa o ScrollViewer cuidar da movimentação normal.
        if (!e.KeyModifiers.HasFlag(KeyModifiers.Control))
            return;

        // ========================================================
        // CTRL + RODA = ZOOM
        // ========================================================

        double novaEscala = escalaAtual;

        if (e.Delta.Y > 0)
        {
            novaEscala *= FatorZoom;
        }
        else if (e.Delta.Y < 0)
        {
            novaEscala /= FatorZoom;
        }

        novaEscala =
            Math.Clamp(
                novaEscala,
                ZoomMinimo,
                ZoomMaximo);

        if (Math.Abs(novaEscala - escalaAtual) < 0.001)
            return;

        escalaAtual = novaEscala;

        AtualizarTamanhoImagem();

        e.Handled = true;
    }

    // ============================================================
    // ATALHOS DE TECLADO PARA ZOOM
    // ============================================================

    private void CellViewer_KeyDown(
        object? sender,
        KeyEventArgs e)
    {
        if (imagemBitmap == null)
            return;


        // ========================================================
        // CTRL + +
        // ========================================================

        if (e.KeyModifiers.HasFlag(KeyModifiers.Control) &&
            (e.Key == Key.Add || e.Key == Key.OemPlus))
        {
            AumentarZoom();

            e.Handled = true;
            return;
        }

        if (e.KeyModifiers.HasFlag(KeyModifiers.Control) &&
            (e.Key == Key.Subtract || e.Key == Key.OemMinus))
        {
            DiminuirZoom();

            e.Handled = true;
            return;
        }

        if (e.KeyModifiers.HasFlag(KeyModifiers.Control) &&
            e.Key == Key.D0)
        {
            RestaurarZoom();

            e.Handled = true;
        }   
    }

    // ============================================================
    // INICIAR ARRASTE DA IMAGEM
    // ============================================================

    private void ImagemScrollViewer_PointerPressed(
        object? sender,
        PointerPressedEventArgs e)
    {
        if (imagemBitmap == null)
            return;

        var propriedades =
            e.GetCurrentPoint(ImagemScrollViewer).Properties;

        // Somente botão esquerdo
        if (!propriedades.IsLeftButtonPressed)
            return;

        arrastandoImagem = true;

        pontoInicialArraste =
            e.GetPosition(ImagemScrollViewer);

        scrollInicialHorizontal =
            ImagemScrollViewer.Offset.X;

        scrollInicialVertical =
            ImagemScrollViewer.Offset.Y;

        e.Pointer.Capture(ImagemScrollViewer);

        e.Handled = true;
    }


    // ============================================================
    // ARRASTAR A IMAGEM
    // ============================================================

    private void ImagemScrollViewer_PointerMoved(
        object? sender,
        PointerEventArgs e)
    {
        if (!arrastandoImagem)
            return;

        AvaloniaPoint pontoAtual =
            e.GetPosition(ImagemScrollViewer);

        double deslocamentoX =
            pontoInicialArraste.X -
            pontoAtual.X;

        double deslocamentoY =
            pontoInicialArraste.Y -
            pontoAtual.Y;

        ImagemScrollViewer.Offset =
            new Vector(
                scrollInicialHorizontal + deslocamentoX,
                scrollInicialVertical + deslocamentoY);

        e.Handled = true;
    }


    // ============================================================
    // FINALIZAR ARRASTE
    // ============================================================

    private void ImagemScrollViewer_PointerReleased(
        object? sender,
        PointerReleasedEventArgs e)
    {
        if (!arrastandoImagem)
            return;

        arrastandoImagem = false;

        e.Pointer.Capture(null);

        e.Handled = true;
    }

    // ============================================================
    // AUMENTAR ZOOM
    // ============================================================

    private void AumentarZoom()
    {
        double novaEscala =
            escalaAtual * FatorZoom;


        novaEscala =
            Math.Clamp(
                novaEscala,
                ZoomMinimo,
                ZoomMaximo);


        escalaAtual =
            novaEscala;


        AtualizarTamanhoImagem();
    }

    // ============================================================
    // DIMINUIR ZOOM
    // ============================================================

    private void DiminuirZoom()
    {
        double novaEscala =
            escalaAtual / FatorZoom;


        novaEscala =
            Math.Clamp(
                novaEscala,
                ZoomMinimo,
                ZoomMaximo);


        escalaAtual =
            novaEscala;


        AtualizarTamanhoImagem();
    }

    // ============================================================
    // RESTAURAR ZOOM
    // ============================================================

    private void RestaurarZoom()
    {
        if (imagemBitmap == null)
            return;


        double larguraDisponivel =
            Bounds.Width - 20;

        double alturaDisponivel =
            Bounds.Height - 20;


        if (larguraDisponivel <= 0 ||
            alturaDisponivel <= 0)
            return;


        double escalaHorizontal =
            larguraDisponivel /
            larguraImagemOriginal;

        double escalaVertical =
            alturaDisponivel /
            alturaImagemOriginal;


        escalaAtual =
            Math.Min(
                escalaHorizontal,
                escalaVertical);


        if (escalaAtual > 1.0)
            escalaAtual = 1.0;


        AtualizarTamanhoImagem();
    }

    // ============================================================
    // CARREGAR RESULTADO
    // ============================================================

    public void CarregarResultado(
        string caminhoImagem,
        TrovoResultado resultado)
    {
        Limpar();

        resultadoAtual = resultado;

        Console.WriteLine("=================================");
        Console.WriteLine("CELL VIEWER");
        Console.WriteLine($"Imagem: {caminhoImagem}");

        if (string.IsNullOrWhiteSpace(caminhoImagem))
        {
            Console.WriteLine(
                "ERRO: caminho da imagem vazio.");

            return;
        }

        if (!File.Exists(caminhoImagem))
        {
            Console.WriteLine(
                $"ERRO: arquivo não existe: {caminhoImagem}");

            return;
        }

        try
        {
            // =====================================================
            // 1. CARREGA A IMAGEM COM IMAGESHARP
            // =====================================================

            using var imagem =
                SixLabors.ImageSharp.Image.Load<Rgba32>(
                    caminhoImagem);

            Console.WriteLine(
                $"Imagem carregada pelo ImageSharp: " +
                $"{imagem.Width} x {imagem.Height}");


            larguraImagemOriginal =
                imagem.Width;

            alturaImagemOriginal =
                imagem.Height;


            // =====================================================
            // CARREGA OS DADOS DO TROVO
            // =====================================================

            centroides =
                resultado.Centroides != null
                    ? new List<PontoCelula>(
                        resultado.Centroides)
                    : new List<PontoCelula>();

            celulasGrandes =
                resultado.CelulasGrandes != null
                    ? new List<int>(
                        resultado.CelulasGrandes)
                    : new List<int>();


            // =====================================================
            // RECUPERA CÉLULAS REMOVIDAS
            // =====================================================

            celulasRemovidas.Clear();

            foreach (int indice in resultado.CelulasRemovidas)
            {
                celulasRemovidas.Add(indice);
            }

            // =====================================================
            // 3. CONVERTE PARA PNG EM MEMÓRIA
            // =====================================================

            using var stream =
                new MemoryStream();

            imagem.Save(
                stream,
                new PngEncoder());

            stream.Position = 0;


            // =====================================================
            // 4. ENTREGA O PNG PARA O AVALONIA
            // =====================================================

            imagemBitmap =
                new Bitmap(stream);

            ImagemBase.Source =
                imagemBitmap;


            // =====================================================
            // 5. MOSTRA A IMAGEM
            // =====================================================

            ImagemBase.IsVisible = true;


            // =====================================================
            // 6. AJUSTA A IMAGEM AO ESPAÇO DISPONÍVEL
            // =====================================================

            AjustarImagemAoEspaco();


            // =====================================================
            // 7. GARANTE O DESENHO DOS CENTROIDES
            // =====================================================

            DesenharMarcacoes();


            Console.WriteLine(
                $"Centroides: {centroides.Count}");

            Console.WriteLine(
                "Imagem exibida com sucesso.");

            Console.WriteLine(
                "=================================");
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"ERRO AO CARREGAR IMAGEM: {ex}");

            ImagemBase.Source = null;
            ImagemBase.IsVisible = false;
        }
    }


    // ============================================================
    // AJUSTAR IMAGEM AO ESPAÇO DISPONÍVEL
    // ============================================================

    private void AjustarImagemAoEspaco()
    {
        if (imagemBitmap == null)
            return;

        if (larguraImagemOriginal <= 0 ||
            alturaImagemOriginal <= 0)
            return;


        double larguraDisponivel =
            Bounds.Width - 20;

        double alturaDisponivel =
            Bounds.Height - 20;


        if (larguraDisponivel <= 0 ||
            alturaDisponivel <= 0)
            return;


        // ========================================================
        // CALCULA ESCALA PARA A IMAGEM CABER INTEIRA
        // ========================================================

        double escalaHorizontal =
            larguraDisponivel /
            larguraImagemOriginal;

        double escalaVertical =
            alturaDisponivel /
            alturaImagemOriginal;


        escalaAtual =
            Math.Min(
                escalaHorizontal,
                escalaVertical);


        // Não deixa a imagem aumentar além
        // do tamanho original.
        if (escalaAtual > 1.0)
            escalaAtual = 1.0;


        // ========================================================
        // DIMENSIONA A IMAGEM E O CANVAS
        // ========================================================

        AtualizarTamanhoImagem();


        Console.WriteLine(
            $"Escala da imagem: {escalaAtual:F3}");

        Console.WriteLine(
            $"Tamanho exibido: " +
            $"{ImagemCanvas.Width:F0} x " +
            $"{ImagemCanvas.Height:F0}");
    }


    // ============================================================
    // ATUALIZAR TAMANHO DA IMAGEM
    // ============================================================

    private void AtualizarTamanhoImagem()
    {
        if (imagemBitmap == null)
            return;

        if (larguraImagemOriginal <= 0 ||
            alturaImagemOriginal <= 0)
            return;


        double novaLargura =
            larguraImagemOriginal *
            escalaAtual;

        double novaAltura =
            alturaImagemOriginal *
            escalaAtual;


        // ========================================================
        // DIMENSIONA A IMAGEM
        // ========================================================

        ImagemBase.Width =
            novaLargura;

        ImagemBase.Height =
            novaAltura;


        // ========================================================
        // DIMENSIONA O CANVAS
        // ========================================================

        ImagemCanvas.Width =
            novaLargura;

        ImagemCanvas.Height =
            novaAltura;


        // ========================================================
        // REDESENHA AS MARCAÇÕES
        // ========================================================

        if (centroides.Count > 0)
            DesenharMarcacoes();
    }


    // ============================================================
    // DESENHAR MARCAÇÕES
    // ============================================================

    private void DesenharMarcacoes()
    {
        Console.WriteLine(
            $"DESENHANDO MARCAÇÕES: {centroides.Count}");

        foreach (var marcador in marcacoes)
        {
            ImagemCanvas.Children.Remove(
                marcador);
        }

        marcacoes.Clear();


        for (int i = 0;
             i < centroides.Count;
             i++)
        {
            if (celulasRemovidas.Contains(i))
                continue;


            var ponto =
                centroides[i];


            double tamanho =
                12 * escalaAtual;


            // Evita que os círculos desapareçam
            // completamente quando a imagem estiver muito reduzida.
            if (tamanho < 5)
                tamanho = 5;


            var marcador =
                new Ellipse
                {
                    Width = tamanho,
                    Height = tamanho,

                    Stroke = Brushes.Red,

                    StrokeThickness =
                        Math.Max(
                            1,
                            2 * escalaAtual),

                    Fill =
                        Brushes.Transparent
                };


            // =====================================================
            // CONVERTE COORDENADAS DA IMAGEM
            // PARA COORDENADAS DA IMAGEM EXIBIDA
            // =====================================================

            double x =
                ponto.X * escalaAtual;

            double y =
                ponto.Y * escalaAtual;


            Canvas.SetLeft(
                marcador,
                x - tamanho / 2);

            Canvas.SetTop(
                marcador,
                y - tamanho / 2);


            int id = i;


            marcador.Tag = id;

            marcador.PointerPressed +=
                (_, _) =>
                {
                    SelecionarCelula(id);
                };


            ImagemCanvas.Children.Add(
                marcador);

            marcacoes.Add(
                marcador);
        }
    }


    // ============================================================
    // SELECIONAR CÉLULA
    // ============================================================

    private void SelecionarCelula(
        int id)
    {
        celulaSelecionada = id;

        foreach (var marcador in marcacoes)
        {
            if (marcador.Tag is int indiceOriginal &&
                indiceOriginal == id)
            {
                marcador.Stroke =
                    Brushes.Yellow;

                marcador.StrokeThickness =
                    Math.Max(
                        2,
                        4 * escalaAtual);
            }
            else
            {
                marcador.Stroke =
                    Brushes.Red;

                marcador.StrokeThickness =
                    Math.Max(
                        1,
                        2 * escalaAtual);
            }
        }
    }

    // ============================================================
    // ENCONTRAR ÍNDICE DA MARCAÇÃO
    // ============================================================




    // ============================================================
    // EXCLUIR CÉLULA SELECIONADA
    // ============================================================

    public bool ExcluirSelecionada()
    {
        if (!celulaSelecionada.HasValue)
            return false;

        int id =
            celulaSelecionada.Value;


        // =====================================================
        // REGISTRA A REMOÇÃO NO CELL VIEWER
        // =====================================================

        celulasRemovidas.Add(id);


        // =====================================================
        // REGISTRA A REMOÇÃO NO RESULTADO DA IMAGEM
        // =====================================================

        if (resultadoAtual != null &&
            !resultadoAtual.CelulasRemovidas.Contains(id))
        {
            resultadoAtual.CelulasRemovidas.Add(id);
        }


        // =====================================================
        // ATUALIZA OS CONTADORES
        // =====================================================

        if (resultadoAtual != null)
        {
            // Toda célula removida deixa de ser contabilizada.
            resultadoAtual.NumeroCelulas--;


            // Verifica se era uma célula grande.
            if (resultadoAtual.CelulasGrandes.Contains(id))
            {
                resultadoAtual.NumeroCelulasGrandes--;
            }
            else
            {
                // Se não era grande, era pequena.
                resultadoAtual.NumeroCelulasPequenas--;
            }


            // Verifica se a célula fazia parte de algum
            // grupo de infecção.
            foreach (var grupo in resultadoAtual.GruposInfeccao.Values)
            {
                if (grupo.Contains(id))
                {
                    resultadoAtual.NumeroInfectadas--;
                    break;
                }
            }
        }


        celulaSelecionada = null;


        // =====================================================
        // ATUALIZA A IMAGEM
        // =====================================================

        DesenharMarcacoes();

        return true;
    }


    // ============================================================
    // LIMPAR
    // ============================================================

    public void Limpar()
    {
        foreach (var marcador in marcacoes)
        {
            ImagemCanvas.Children.Remove(
                marcador);
        }

        marcacoes.Clear();


        imagemBitmap?.Dispose();

        imagemBitmap = null;


        ImagemBase.Source = null;

        ImagemBase.IsVisible = false;


        centroides.Clear();

        celulasGrandes.Clear();

        celulasRemovidas.Clear();

        celulaSelecionada = null;

        resultadoAtual = null;


        larguraImagemOriginal = 0;

        alturaImagemOriginal = 0;

        escalaAtual = 1.0;


        ImagemCanvas.Width = double.NaN;
        ImagemCanvas.Height = double.NaN;
    }
}