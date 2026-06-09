using Markdig;
using YamlDotNet.Serialization;

namespace PicoSite.Services;

public class MarkdownParser
{
    private readonly IDeserializer _yaml = new DeserializerBuilder().Build();
    private readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder().Build();

    public (Dictionary<string, object>? FrontMatter, string Html) Parse(string markdown)
    {
        Dictionary<string, object>? frontMatter = null;
        var body = markdown;

        // Front Matter: --- 包裹的 YAML 块，兼容 CRLF / LF
        const string sep = "---";
        if (body.StartsWith(sep + "\n") || body.StartsWith(sep + "\r\n"))
        {
            // 前导分隔符长度: "---\n" = 4, "---\r\n" = 5
            int headerLen = body[3] == '\r' ? 5 : 4;

            // 在后继内容中找结東分隔符: \n---\n 或 \n---\r\n 或 \r\n---\n 或 \r\n---\r\n
            int searchLimit = Math.Min(body.Length, 4096);
            int closePos = -1;

            for (int i = headerLen; i < searchLimit - 4; i++)
            {
                // 从 i 开始是 \r\n--- 或 \n--- 就算命中
                if (body[i] == '\r' && body[i + 1] == '\n' &&
                    body[i + 2] == '-' && body[i + 3] == '-' && body[i + 4] == '-')
                {
                    closePos = i;
                    break;
                }
                if (body[i] == '\n' &&
                    body[i + 1] == '-' && body[i + 2] == '-' && body[i + 3] == '-')
                {
                    closePos = i;
                    break;
                }
            }

            if (closePos > 0)
            {
                var yamlBlock = body[headerLen..closePos];
                // 跳过关闭分隔符: \n--- (4) 或 \r\n--- (5)
                int closeLen = body[closePos] == '\r' ? 5 : 4;
                body = body[(closePos + closeLen)..];
                // 剥离 body 开头可能残留的 \r\n 或 \n
                if (body.StartsWith("\r\n")) body = body[2..];
                else if (body.StartsWith("\n")) body = body[1..];

                try { frontMatter = _yaml.Deserialize<Dictionary<string, object>>(yamlBlock); }
                catch { /* YAML 解析失败，按无 Front Matter 处理 */ }
            }
        }

        var html = Markdown.ToHtml(body, _pipeline);
        return (frontMatter, html);
    }
}
