using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace PicoSite.Commands
{
    public static class GetVersionCommand
    {
        public static RootCommand VersionCommand(this RootCommand rootCommand)
        {
            var versionOpt = new Option<bool>(new[] { "-v", "--version" }, "显示版本信息");
            rootCommand.AddOption(versionOpt);

            rootCommand.SetHandler((version) =>
            {
                if (version)
                {
                    GetVersion();
                    return;
                }
            }, versionOpt);

            return rootCommand;
        }

        private static void GetVersion()
        {
            // 输出当前程序版本
            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine($"程序版本：{ver}");
        }
    }
}
