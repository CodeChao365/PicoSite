namespace PicoSite.Models;

public class SiteModel
{
    public string Title { get; set; } = "PicoSite";
    public string? Description { get; set; }
    public List<PageModel> Pages { get; set; } = new();
}
