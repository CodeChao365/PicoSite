using System.Text.Json;
using PicoSite.Models;

namespace PicoSite.Services;

public class ConfigLoader
{
    private const string FileName = "picosite.json";

    public SiteConfig Load(string workingDir)
    {
        var path = Path.Combine(workingDir, FileName);
        if (!File.Exists(path)) return new SiteConfig();

        try
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize(json, PicoSiteJsonContext.Default.SiteConfig) ?? new SiteConfig();
        }
        catch
        {
            Console.WriteLine($"[警告] {FileName} 格式错误，使用默认配置");
            return new SiteConfig();
        }
    }
}
