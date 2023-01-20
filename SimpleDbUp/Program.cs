using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

var command = new RootCommand();

var scriptDirectory = new Option<string>(new[] { "-s", "--scripts" }, () => Directory.GetCurrentDirectory(), "Source scripts directory");

var connectionString = new Option<string>(new[] { "-c", "--connection-string" }, description: "Connection string to target database");
connectionString.IsRequired = true;

command.AddOption(connectionString);
command.AddOption(scriptDirectory);

command.SetHandler((conStr, scripts) => Console.WriteLine((conStr + "-" + scripts) ?? "Null"), connectionString, scriptDirectory);

var parser = new CommandLineBuilder(command)
    .UseHelp()
    .UseDefaults()
    .Build();

await parser.InvokeAsync(args);
