# fonttobm
Utility to convert fonts (TTF) to Bitmap fonts (BMFont format + image). It is implemented in a way that it works under Linux, Windows, and probably other operating systems.

BMFont format is usually used in applications, that performance depends on using fonts from textures (for example, games with large amounts of texts). One of popular applications, that works this format is [bmfont](https://angelcode.com/products/bmfont/), that has only Windows builds and requires Wine to run under other platforms. This utility is created with a goal to avoid running under Wine and be easily automated as command line tool can be.

## Command line arguments
| Name          | Description
|:--------------|:--
| -f, --font    | Required. Path to the font file.
| -o, --output  | Output directory.
| -n, --name    | Output file name prefix, for example, 'MyFont'.
| -s, --size    | Font size.
| -c, --chars   | Characters to include in the bitmap font.
| --charsPerRow | Number of characters per row in the bitmap font.
| --antialias   | Use antialiasing for the font.

## Examples

Generate from font file in path and output results in same directory, using prefix `MyFont`:
```bash
dotnet run --font /path/to/directory/PixelOperator-Bold.ttf --output /path/to/directory/ --name=MyFont
```

Generate from system font file and output results to specific directory, also apply antialiasing:
```bash
dotnet run --font /usr/share/fonts/google-droid-sans-fonts/DroidSans.ttf --output /home/andrius/Temp/ --antialias=true
```
