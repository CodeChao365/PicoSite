using System.Text.Json.Serialization;
using PicoSite.Models;

namespace PicoSite.Services;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(SiteConfig))]
internal partial class PicoSiteJsonContext : JsonSerializerContext
{
}
