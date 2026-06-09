using DotLiquid;
using DotLiquid.FileSystems;
using PicoSite.Models;

namespace PicoSite.Services;

public class TemplateEngine
{
    private readonly string _themeDir;

    public TemplateEngine(string themeDir)
    {
        _themeDir = themeDir;
        Template.FileSystem = new ThemeFileSystem(themeDir);
    }

    public string Render(string templateName, SiteModel site, PageModel page, string content)
    {
        var path = Path.Combine(_themeDir, $"{templateName}.html");
        if (!File.Exists(path))
            throw new FileNotFoundException($"主题模板缺失: {path}");

        var source = File.ReadAllText(path);
        var template = Template.Parse(source);

        var hash = Hash.FromAnonymousObject(new
        {
            site = new
            {
                title = site.Title,
                description = site.Description,
                pages = site.Pages.Select(p => new { p.Title, p.Url }).ToList()
            },
            page = new
            {
                title = page.Title,
                url = page.Url,
                date = page.Date?.ToString("yyyy-MM-dd"),
                excerpt = page.Excerpt
            },
            current_url = page.Url,
            content = content,
            theme = new { assets = "/themes/" + Path.GetFileName(_themeDir) + "/assets" }
        });

        return template.Render(hash);
    }
}

internal class ThemeFileSystem : IFileSystem
{
    private readonly string _root;
    public ThemeFileSystem(string root) => _root = root;

    public string ReadTemplateFile(Context context, string templateName)
    {
        var path = Path.Combine(_root, templateName);
        if (!File.Exists(path))
            throw new FileNotFoundException($"子模板未找到: {templateName}", path);
        return File.ReadAllText(path);
    }
}
