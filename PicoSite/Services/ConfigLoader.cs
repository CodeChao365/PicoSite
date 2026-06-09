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
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<SiteConfig>(json, options) ?? new SiteConfig();
        }
        catch
        {
            Console.WriteLine($"[警告] {FileName} 格式错误，使用默认配置");
            return new SiteConfig();
        }
    }
}
