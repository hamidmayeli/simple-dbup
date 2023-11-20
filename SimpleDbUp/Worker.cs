using DbUp;
using DbUp.Engine;

namespace SimpleDbUp;

public class Worker
{
    public static void Run(
        string connectionString,
        string scriptsPath,
        bool nonTransactional,
        IEnumerable<string> commands,
        IEnumerable<string> commandNames
        )
    {
        Console.WriteLine("Running the upgrade using" +
            $"{Environment.NewLine}\tConnection String: {connectionString.Length} chars" +
            $"{Environment.NewLine}\tScripts Path: {new DirectoryInfo(scriptsPath).Name}" +
            $"{Environment.NewLine}\tNon transactional: {nonTransactional}" +
            $"{Environment.NewLine}\tCommands len: {commands.Count()}" +
            $"{Environment.NewLine}\tCommand names: {string.Join(", ", commandNames)}");

        if(commandNames.Count() != commands.Count())
        {
            Console.WriteLine("Invalid count of commands and their names");
            Environment.Exit(1);
        }

        var commandList = commandNames.Zip(commands).ToList();

        try
        {
            EnsureDatabase.For.MySqlDatabase(connectionString);
        }
        catch (Exception exception)
        {
            ExitWithError(exception);
        }

        var builder = DeployChanges.To
            .MySqlDatabase(connectionString)
            .WithScriptsFromFileSystem(
                scriptsPath,
                (filename) => filename.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase),
                new SqlScriptOptions
                {
                    ScriptType = DbUp.Support.ScriptType.RunOnce
                })
            .LogToConsole()
            .LogScriptOutput();

        commandList.ForEach(x => builder.WithScript(x.First, x.Second));

        if (nonTransactional)
            builder.WithTransactionPerScript();
        else
            builder.WithTransaction();

        var upgrade = builder.Build();

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
