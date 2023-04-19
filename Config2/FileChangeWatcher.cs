namespace Config2;

internal class FileChangeWatcher
{
    public FileChangeWatcher(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);
            if (fileInfo.DirectoryName == null)
                throw new DirectoryNotFoundException($"Could not find directory for file '{fileInfo.FullName}'");
            var watcher = new FileSystemWatcher(fileInfo.DirectoryName) {Filter = fileInfo.Name};
            watcher.Changed += OnFileChanged;
            watcher.EnableRaisingEvents = true;
        }
    }

    public event EventHandler FileChanged;

    private void OnFileChanged(object sender, EventArgs eventArgs)
    {
        var handler = FileChanged;
        handler?.Invoke(this, eventArgs);
    }
}