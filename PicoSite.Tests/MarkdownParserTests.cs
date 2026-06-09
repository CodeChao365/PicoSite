using PicoSite.Services;
using Xunit;

namespace PicoSite.Tests;

public class MarkdownParserTests
{
    private readonly MarkdownParser _parser = new();

    [Fact]
    public void Parse_PlainMarkdown_ReturnsHtmlWithoutFrontMatter()
    {
        var (frontMatter, html) = _parser.Parse("## Hello\n\nWorld");

        Assert.Null(frontMatter);
        Assert.Contains("<h2>Hello</h2>", html);
        Assert.Contains("<p>World</p>", html);
    }

    [Fact]
    public void Parse_MarkdownWithFrontMatter_ExtractsTitle()
    {
        var md = "---\ntitle: 测试文章\n---\n\n## 正文";

        var (frontMatter, html) = _parser.Parse(md);

        Assert.NotNull(frontMatter);
        Assert.Equal("测试文章", frontMatter!["title"]?.ToString());
        Assert.Contains("<h2>正文</h2>", html);
    }

    [Fact]
    public void Parse_FrontMatterWithCRLF_WorksCorrectly()
    {
        var md = "---\r\ntitle: CRLF测试\r\ndate: 2026-06-09\r\n---\r\n\r\n## CRLF 标题";

        var (frontMatter, html) = _parser.Parse(md);

        Assert.NotNull(frontMatter);
        Assert.Equal("CRLF测试", frontMatter!["title"]?.ToString());
        Assert.Contains("<h2>CRLF 标题</h2>", html);
    }

    [Fact]
    public void Parse_FrontMatterWithoutDate_DateNull()
    {
        var md = "---\ntitle: 无日期\n---\n\n内容";

        var (frontMatter, html) = _parser.Parse(md);

        Assert.NotNull(frontMatter);
        Assert.Equal("无日期", frontMatter!["title"]?.ToString());
        Assert.DoesNotContain("date", frontMatter!.Keys);
    }
}
