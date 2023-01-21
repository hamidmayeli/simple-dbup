using DbUp;
using DbUp.Builder;
using DbUp.Engine;
using MySqlX.XDevAPI.Common;

namespace SimpleDbUp;

public class EmptyClass
{
    public static void Run(string connectionString, string scriptsPath)
    {
        try
        {
            EnsureDatabase.For.MySqlDatabase(connectionString);
        }
        catch (Exception exception)
        {
            ExitWithError(exception);
        }

        var upgrade = DeployChanges.To
            .MySqlDatabase(connectionString)
            .WithScriptsFromFileSystem(
                scriptsPath,
                (filename) => filename.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase),
                new SqlScriptOptions
                {
                    ScriptType = DbUp.Support.ScriptType.RunOnce
                })
            .LogToConsole()
            .LogScriptOutput()
            .Build();

        var result = upgrade.PerformUpgrade();

        if (!result.Successful)
        {
            ExitWithError(result.Error);
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Success!");
        Console.ResetColor();
        Environment.Exit(0);
    }

    private static void ExitWithError(Exception exception)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(exception);
        Console.ResetColor();
#if DEBUG
        Console.ReadLine();
#endif
        Environment.Exit(1);
    }
}
