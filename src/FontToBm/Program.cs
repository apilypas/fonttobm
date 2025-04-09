using CommandLine;

namespace FontToBm;

public static class Program
{
    public static void Main(string[] args)
    {
        Parser.Default
            .ParseArguments<CommandLineOptions>(args)
            .WithParsed(OnArgumentsParsed);
    }

    private static void OnArgumentsParsed(CommandLineOptions options)
    {
        try
        {
            var app = new FontToBmApp(options);
            app.Initialize();
            app.Run();
        }
        catch (FontToBmException ex)
        {
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unhandled exception: " + ex);
            Environment.Exit(1);
        }
    }
}
