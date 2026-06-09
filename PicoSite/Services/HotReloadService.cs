using PicoServer;
using System.Timers;
using Timer = System.Timers.Timer;

namespace PicoSite.Services;

public class HotReloadService : IDisposable
{
    private readonly WebAPIServer _server;
    private readonly FileSystemWatcher _watcher;
    private readonly Timer _debounce;
    private readonly Action? _onChanged;
    private bool _disposed;

    private static readonly HashSet<string> s_watchedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".md", ".html", ".css", ".js", ".json", ".yaml", ".yml"
    };

    public HotReloadService(WebAPIServer server, string watchDir, Action? onChanged = null)
    {
        _server = server;
        _onChanged = onChanged;

        // 文件监视器
        _watcher = new FileSystemWatcher(watchDir)
        {
            EnableRaisingEvents = true,
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.LastWrite
                         | NotifyFilters.FileName
                         | NotifyFilters.DirectoryName,
        };

        // 200ms 防抖
        _debounce = new Timer(200) { AutoReset = false };
        _debounce.Elapsed += OnDebounceElapsed;

        _watcher.Changed += OnFileChange;
        _watcher.Created += OnFileChange;
        _watcher.Deleted += OnFileChange;
        _watcher.Renamed += OnFileChange;
        _watcher.Error += (_, e) =>
            Console.Error.WriteLine($"[热重载] 文件监视器缓冲区溢出: {e.GetException()?.Message}");

        Console.WriteLine($"[热重载] 正在监视: {watchDir}");
    }

    private void OnFileChange(object sender, FileSystemEventArgs e)
    {
        // 过滤隐藏文件和临时文件
        var name = Path.GetFileName(e.Name ?? "");
        if (string.IsNullOrEmpty(name)) return;
        if (name.StartsWith('.') || name.StartsWith('~')) return;

        // 只关注内容/模板相关文件
        var ext = Path.GetExtension(name);
        if (!s_watchedExtensions.Contains(ext)) return;

        _debounce.Stop();
        _debounce.Start();
    }

    private async void OnDebounceElapsed(object? sender, ElapsedEventArgs e)
    {
        // 先刷新服务端数据（如页面列表）
        try
        {
            _onChanged?.Invoke();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[热重载] 刷新数据失败: {ex.Message}");
        }

        // 再通知浏览器刷新
        try
        {
            await _server.WsBroadcastAsync("reload");
            Console.WriteLine("[热重载] 已发送刷新信号");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[热重载] 广播失败: {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _debounce.Dispose();
        _watcher.Dispose();
    }
}
