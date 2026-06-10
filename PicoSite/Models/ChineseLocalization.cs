using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace PicoSite.Models
{
    public class ChineseLocalization : LocalizationResources
    {
        // 帮助标题
        public override string HelpUsageTitle() => "用法：";
        public override string HelpOptionsTitle() => "选项：";
        public override string HelpArgumentsTitle() => "参数：";
        public override string HelpCommandsTitle() => "命令：";
        public override string HelpDescriptionTitle() => "说明：";

        // 选项描述
        public override string HelpOptionDescription() => "显示帮助信息";
        public override string VersionOptionDescription() => "显示版本信息";

        // 错误提示汉化（可选）
        public override string UnrecognizedCommandOrArgument(string token) => $"无法识别的命令或参数：{token}";
    }
}
