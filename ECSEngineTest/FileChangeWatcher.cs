using System;

namespace ECSEngineTest;

public static class FileChangeWatcher
{
    private static readonly List<FileSystemWatcher> _watchers = [];

    public static void SubscribeForFileChanges(string filePath, Action<object, FileSystemEventArgs> fileChangedAction)
    {
        var dir = Path.GetDirectoryName(filePath)!;
        var fileName = Path.GetFileName(filePath);

        if (_watchers.Any(x => x.Path == dir && x.Filter == fileName) || !File.Exists(filePath))
            return;

        var watcher = new FileSystemWatcher(dir, fileName)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
            EnableRaisingEvents = true
        };
        watcher.Changed += (sender, args) =>
        {
            if (args.ChangeType == WatcherChangeTypes.Deleted)
            {
                UnsubscribeFromFileChanges(args.FullPath);
            }
            else if (args.ChangeType == WatcherChangeTypes.Changed)
            {
                fileChangedAction?.Invoke(sender, args);
            }
        };
        watcher.Renamed += (sender, args) =>
        {
            watcher.Path = Path.GetDirectoryName(args.FullPath)!;
            watcher.Filter = args.Name!;
        };

        _watchers.Add(watcher);
    }

    public static void UnsubscribeFromFileChanges(string filePath)
    {
        var watcher = _watchers.FirstOrDefault(x => Path.Combine(x.Path, x.Filter) == filePath);
        if (watcher == null) return;

        _watchers.Remove(watcher);
        watcher.Dispose();
    }

    public static void Dispose()
    {
        foreach (var watcher in _watchers)
        {
            watcher.Dispose();
        }
        _watchers.Clear();
    }
}
