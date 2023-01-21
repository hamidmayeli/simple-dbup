using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using SimpleDbUp;

var command = new RootCommand();

var scriptDirectory = new Option<string>(new[] { "-s", "--scripts" }, () => Directory.GetCurrentDirectory(), "Source scripts directory");

var connectionString = new Option<string>(new[] { "-c", "--connection-string" }, description: "Connection string to target database");
connectionString.IsRequired = true;

var nonTransactional = new Option<bool>(new[] { "--non-transactional" }, "Create a transaction per scripts");

command.AddOption(connectionString);
command.AddOption(scriptDirectory);
command.AddOption(nonTransactional);

command.SetHandler(Worker.Run, connectionString, scriptDirectory, nonTransactional);

var parser = new CommandLineBuilder(command)
    .UseHelp()
    .UseDefaults()
    .Build();

parser.Invoke(args);
