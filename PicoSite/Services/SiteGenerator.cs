using PicoSite.Models;

namespace PicoSite.Services;

public class SiteGenerator
{
    private readonly MarkdownParser _parser;
    private readonly TemplateEngine _templates;
    private readonly SiteConfig _config;

    public SiteGenerator(MarkdownParser parser, TemplateEngine templates, SiteConfig config)
    {
        _parser = parser;
        _templates = templates;
        _config = config;
    }

    public List<PageModel> LoadPages(string sourceDir)
    {
        var pages = new List<PageModel>();
        if (!Directory.Exists(sourceDir)) return pages;

        foreach (var file in Directory.GetFiles(sourceDir, "*.md", SearchOption.AllDirectories))
        {
            var page = ParseFile(file, sourceDir);
            if (page is not null) pages.Add(page);
        }

        return pages.OrderBy(p => p.Url).ToList();
    }

    public PageModel? LoadPage(string sourceDir, string requestPath)
    {
        // 将请求路径转为可能的 .md 文件路径
        var relative = requestPath.TrimStart('/');
        if (string.IsNullOrEmpty(relative)) relative = "index";

        // 路径遍历防护：确保最终路径在 sourceDir 内
        var candidates = new[]
        {
            relative + ".md",
            Path.Combine(relative, "index.md"),
        };

        foreach (var candidate in candidates)
        {
            var fullPath = Path.GetFullPath(Path.Combine(sourceDir, candidate));
            var sourceDirFull = Path.GetFullPath(sourceDir) + Path.DirectorySeparatorChar;
            if (!fullPath.StartsWith(sourceDirFull, StringComparison.Ordinal))
                continue;

            if (File.Exists(fullPath))
                return ParseFile(fullPath, sourceDir);
        }

        return null;
    }

    private PageModel? ParseFile(string filePath, string sourceDir)
    {
        try
        {
            var markdown = File.ReadAllText(filePath);
            var (frontMatter, html) = _parser.Parse(markdown);

            var relative = Path.GetRelativePath(sourceDir, filePath)
                .Replace('\\', '/')
                .Replace(".md", "");

            if (relative.EndsWith("/index")) relative = relative[..^6];
            else if (relative == "index") relative = "";

            var page = new PageModel
            {
                Title = frontMatter?.GetValueOrDefault("title")?.ToString()
                         ?? Path.GetFileNameWithoutExtension(filePath),
                Url = "/" + relative,
                Content = html,
                SourcePath = filePath,
                FrontMatter = frontMatter,
            };

            if (frontMatter?.TryGetValue("date", out var dateObj) == true
                && DateTime.TryParse(dateObj.ToString(), out var date))
                page.Date = date;

            page.Excerpt = ExtractExcerpt(html);
            return page;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[警告] 无法解析文件 {filePath}: {ex.Message}");
            return null;
        }
    }

    private static string? ExtractExcerpt(string html)
    {
        const string tag = "<p>";
        var start = html.IndexOf(tag, StringComparison.Ordinal);
        if (start < 0) return null;

        var end = html.IndexOf("</p>", start, StringComparison.Ordinal);
        if (end < 0) return null;

        start += tag.Length;
        var excerpt = html[start..end];
        // 截断时避开 HTML 标签边界
        if (excerpt.Length <= 150) return excerpt;

        // 先剥离 HTML 标签再截断
        var textOnly = System.Text.RegularExpressions.Regex.Replace(excerpt, "<[^>]+>", "");
        return textOnly.Length > 150 ? textOnly[..150] + "..." : textOnly;
    }

    // ─── Build 模式 ──────────────────────────────────────────

    public void Build(string sourceDir, string outputDir)
    {
        var pages = LoadPages(sourceDir);
        var site = new SiteModel
        {
            Title = _config.Title ?? "PicoSite",
            Description = _config.Description,
            Pages = pages,
        };

        Directory.CreateDirectory(outputDir);

        foreach (var page in pages)
        {
            try
            {
                var html = _templates.Render("index", site, page, page.Content);
                var outPath = ResolveOutputPath(outputDir, page.Url);
                Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);
                File.WriteAllText(outPath, html);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[错误] 页面 \"{page.Url}\" 生成失败: {ex.Message}");
            }
        }

        CopyThemeAssets(outputDir);
    }

    private static string ResolveOutputPath(string outputDir, string url)
    {
        if (string.IsNullOrEmpty(url) || url == "/")
            return Path.Combine(outputDir, "index.html");

        var relative = url.TrimStart('/');
        if (!relative.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
            relative += ".html";

        return Path.Combine(outputDir, relative);
    }

    private void CopyThemeAssets(string outputDir)
    {
        var themeAssetsDir = Path.Combine(AppContext.BaseDirectory, "Themes", _config.Theme ?? "default", "assets");
        if (!Directory.Exists(themeAssetsDir)) return;

        var dest = Path.Combine(outputDir, "themes", _config.Theme ?? "default", "assets");
        CopyDirectory(themeAssetsDir, dest);
    }

    private static void CopyDirectory(string src, string dest)
    {
        if (!Directory.Exists(dest))
            Directory.CreateDirectory(dest);

        foreach (var file in Directory.GetFiles(src))
            File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), true);

        foreach (var dir in Directory.GetDirectories(src))
            CopyDirectory(dir, Path.Combine(dest, Path.GetFileName(dir)));
    }

    // ─── 源目录自动检测 ──────────────────────────────────────

    public static string FindSourceDir(string workingDir)
    {
        foreach (var dir in new[] { "content", "docs", "." })
        {
            var full = Path.Combine(workingDir, dir);
            if (Directory.Exists(full) &&
                Directory.GetFiles(full, "*.md", SearchOption.AllDirectories).Length > 0)
                return Path.GetFullPath(full);
        }
        return workingDir;
    }
}
