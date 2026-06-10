using PicoSite.Commands;
using PicoSite.Services;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

var configLoader = new ConfigLoader();
var markdownParser = new MarkdownParser();

var root = new RootCommand("PicoSite - 简单轻量的静态站点生成器").VersionCommand();
root.AddCommand(new ServeCommand(configLoader, markdownParser));
root.AddCommand(new BuildCommand(configLoader, markdownParser));

var builder = new CommandLineBuilder(root)
    //.UseLocalizationResources(new ChineseLocalization())//汉化
    .UseHelp().Build();

return await builder.InvokeAsync(args);
