using CommandLine;

namespace FontToBm;

public class CommandLineOptions
{
    [Option('f', "font", Required = true, HelpText = "Path to the font file.")]
    public string FontPath { get; set; } = string.Empty;

    [Option('o', "output", Required = false, HelpText = "Output directory.")]
    public string OutputDirectory { get; set; } = string.Empty;
    
    [Option('n', "name", Required = false, HelpText = "Output file name prefix, for example, 'MyFont'.")]
    public string OutputFileName { get; set; } = string.Empty;

    [Option('s', "size", Required = false, HelpText = "Font size.")]
    public int FontSize { get; set; } = Constants.DefaultFontSize;
    
    [Option('c', "chars", Required = false, HelpText = "Characters to include in the bitmap font.")]
    public string Chars { get; set; } = Constants.DefaultChars;
    
    [Option("charsPerRow", Required = false, HelpText = "Number of characters per row in the bitmap font.")]
    public int CharsPerRow { get; set; } = Constants.DefaultCharsPerRow;
    
    [Option("antialias", Required = false, HelpText = "Use antialiasing for the font.")]
    public bool UseAntialias { get; set; } = Constants.DefaultUseAntialias;
}
