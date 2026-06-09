namespace PicoSite.Models;

public class SiteConfig
{
    public string? Title { get; set; } = "PicoSite";
    public string? Description { get; set; }
    public string? Theme { get; set; } = "default";
    public int Port { get; set; } = 8080;
    public string? Output { get; set; } = "./_site";
}
