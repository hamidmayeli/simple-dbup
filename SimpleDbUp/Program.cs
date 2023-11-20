using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using SimpleDbUp;

var command = new RootCommand();

var scriptDirectory = new Option<string>(["-s", "--scripts"], Directory.GetCurrentDirectory, "Source scripts directory");

var connectionString = new Option<string>(["-C", "--connection-string"], description: "Connection string to target database")
{
    IsRequired = true
};

var commands = new Option<IEnumerable<string>>(
    ["-c", "--command"],
    description: "Commands to run after update. Note: the number of commands and command names should be equal.");

var commandNames = new Option<IEnumerable<string>>(
    ["-cn", "--command-name"],
    description: "Name of the commands to store and prevent multiple execution.");

var nonTransactional = new Option<bool>(["--non-transactional"], "Create a transaction per scripts");

command.AddOption(connectionString);
command.AddOption(scriptDirectory);
command.AddOption(nonTransactional);
command.AddOption(commands);
command.AddOption(commandNames);

command.SetHandler(Worker.Run, connectionString, scriptDirectory, nonTransactional, commands, commandNames);

var parser = new CommandLineBuilder(command)
    .UseHelp()
    .UseDefaults()
    .Build();

parser.Invoke(args);
