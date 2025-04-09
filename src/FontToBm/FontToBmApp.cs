using System.Text;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace FontToBm;

public class FontToBmApp(CommandLineOptions options)
{
    private string _fontPath = string.Empty;
    private string _fontName = string.Empty;
    private string _outputFileFntPath = string.Empty;
    private string _outputFilePngName = string.Empty;
    private int _fontSize;
    private string _chars = string.Empty;
    private int _charsPerRow;
    private bool _useAntialias;

    public void Initialize()
    {
        Validate();
        ParseArguments();
    }

    public void Run()
    {
        // Load the font
        var fontCollection = new FontCollection();
        var fontFamily = fontCollection.Add(_fontPath);
        var font = fontFamily.CreateFont(_fontSize);
        
        // Generate the font data file
        GenerateFnt(font);

        // Generate the font image
        GeneratePng(font);
    }

    private void GeneratePng(Font font)
    {
        var rows = (int)Math.Ceiling(_chars.Length / (float)_charsPerRow);
        var lineHeight = _fontSize + 4;
        var bmpWidth = _charsPerRow * lineHeight;
        var bmpHeight = rows * lineHeight;
        
        using var image = new Image<Rgba32>(bmpWidth, bmpHeight);
        
        image.Mutate(ctx =>
        {
            ctx.Fill(Color.Transparent);

            for (var i = 0; i < _chars.Length; i++)
            {
                var c = _chars[i];
                
                var line = (int) Math.Floor((float) i / _charsPerRow);
                var column = i % _charsPerRow;

                var x = (float)(column * lineHeight);
                var y = (float)(line * lineHeight);

                var position = new PointF(x, y);
                
                ctx.DrawText(
                    new DrawingOptions
                    {
                        GraphicsOptions = new GraphicsOptions
                        {
                            Antialias = _useAntialias
                        }
                    },
                    c.ToString(),
                    font,
                    Color.White,
                    position);
            }
        });
        
        image.Save(_outputFilePngName);

        Console.WriteLine($"Font image saved to {_outputFilePngName}");
    }

    private void GenerateFnt(Font font)
    {
        var rows = (int)Math.Ceiling(_chars.Length / (float)_charsPerRow);
        var lineHeight = _fontSize + 4;
        var bmpWidth = _charsPerRow * lineHeight;
        var bmpHeight = rows * lineHeight;
        
        var fnt = new StringBuilder();
        fnt.AppendLine($"info face=\"{_fontName}\" size={_fontSize} bold=0 italic=0 charset=\"\" unicode=0 stretchH=100 smooth=1 aa=1 padding=0,0,0,0 spacing=1,1");
        fnt.AppendLine($"common lineHeight={lineHeight} base=26 scaleW={bmpWidth} scaleH={bmpHeight} pages=1 packed=0");
        fnt.AppendLine($"page id=0 file=\"{Path.GetFileName(_outputFilePngName)}\"");
        fnt.AppendLine($"chars count={_chars.Length}");

        for (var i = 0; i < _chars.Length; i++)
        {
            var c = _chars[i];
            var code = (int)c;
            var line = (int) Math.Floor((float) i / _charsPerRow);
            var column = i % _charsPerRow;

            var x = (float)(column * lineHeight);
            var y = (float)(line * lineHeight);

            var bounds = TextMeasurer.MeasureBounds(c.ToString(), new TextOptions(font));
            var advance = TextMeasurer.MeasureAdvance(c.ToString(), new TextOptions(font));

            var charWidth = (int) Math.Ceiling(bounds.Right - bounds.Left);
            var charHeight = (int) Math.Ceiling(bounds.Bottom - bounds.Top);

            var fntX = (int) Math.Round(x + bounds.Left);
            var fntY = (int) Math.Round(y + bounds.Top);

            var offsetX = (int) Math.Round(bounds.Left);
            var offsetY = (int) Math.Round(bounds.Top);

            var advanceX = (int) Math.Round(advance.Width);

            // Adjust for antialiasing
            if (_useAntialias)
            {
                fntX = (int) Math.Floor(x + bounds.Left);
                fntY = (int) Math.Floor(y + bounds.Top);

                offsetX = (int) Math.Floor(bounds.Left);
                offsetY = (int) Math.Floor(bounds.Top);

                charWidth = (int) (Math.Ceiling(bounds.Right) - Math.Floor(bounds.Left));
                charHeight = (int) (Math.Ceiling(bounds.Bottom) - Math.Floor(bounds.Top));
            }

            fnt.AppendLine($"char id={code} x={fntX} y={fntY} width={charWidth} height={charHeight} xoffset={offsetX} yoffset={offsetY} xadvance={advanceX} page=0 chnl=0");
        }

        File.WriteAllText(_outputFileFntPath, fnt.ToString());

        Console.WriteLine($"Font data saved to {_outputFileFntPath}");
    }

    private void Validate()
    {
        if (!File.Exists(options.FontPath))
            throw new FontToBmException($"Font file '{options.FontPath}' not found.");
        
        if (!string.IsNullOrEmpty(options.OutputDirectory) && !Directory.Exists(options.OutputDirectory))
            throw new FontToBmException($"Output directory '{options.OutputDirectory}' not found.");
        
        if (options.FontSize <= 0)
            throw new FontToBmException("Font size must be greater than 0.");
        
        if (string.IsNullOrEmpty(options.Chars))
            throw new FontToBmException("Character line cannot be empty.");
        
        if (options.CharsPerRow <= 0)
            throw new FontToBmException("Chars per row must be greater than 0.");
    }

    private void ParseArguments()
    {
        _fontPath = options.FontPath;
        _fontName = Path.GetFileNameWithoutExtension(_fontPath);
        
        _outputFileFntPath = $"{_fontName}.fnt";
        _outputFilePngName = $"{_fontName}0.png";

        if (!string.IsNullOrEmpty(options.OutputFileName))
        {
            _outputFileFntPath = $"{options.OutputFileName}.fnt";
            _outputFilePngName = $"{options.OutputFileName}0.png";
        }
        
        if (!string.IsNullOrEmpty(options.OutputDirectory))
        {
            _outputFileFntPath = Path.Combine(options.OutputDirectory, _outputFileFntPath);
            _outputFilePngName = Path.Combine(options.OutputDirectory, _outputFilePngName);
        }

        _fontSize = options.FontSize;

        _chars = options.Chars;
        
        _charsPerRow = options.CharsPerRow;
        
        _useAntialias = options.UseAntialias;
    }
}