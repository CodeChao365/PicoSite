using PicoSite.Commands;
using PicoSite.Services;
using System.CommandLine;

var configLoader = new ConfigLoader();
var markdownParser = new MarkdownParser();

var root = new RootCommand("PicoSite - 简单轻量的静态站点生成器");
root.AddCommand(new ServeCommand(configLoader, markdownParser));
root.AddCommand(new BuildCommand(configLoader, markdownParser));

return await root.InvokeAsync(args);
