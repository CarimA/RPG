using PhotoVs.Models.Assets;
using PhotoVs.Utils.Logging;
using System;
using System.IO;
using System.Linq;

namespace PhotoVs.Engine.Assets.AssetLoaders
{
    public class DebugHotReloadAssetLoader : AssetLoader, IDisposable
    {
        private readonly FileSystemWatcher _fsWatcher;

        public DebugHotReloadAssetLoader(IStreamProvider streamProvider)
            : base(streamProvider)
        {
            Debug.Log.Info("Initialised DebugHotReloadAssetLoader");

            _fsWatcher = new FileSystemWatcher
            {
                Path = "\\",
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.*",
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };
            _fsWatcher.Changed += FileWatcher_Changed;
        }

        public void Dispose()
        {
            _fsWatcher.Dispose();
        }

        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (_streamProvider == null)
                return;

            if (e.ChangeType != WatcherChangeTypes.Changed)
                return;

            var fullName = e.Name;
            var dir = AppDomain.CurrentDomain.BaseDirectory.Substring(3);
            var final = fullName.Replace(dir, "");

            if (final.Count(f => f == '.') > 1)
                final = final.Substring(0, final.LastIndexOf('.'));

            final = final.ToLowerInvariant();

            if (!final.StartsWith(_streamProvider.RootDirectory.ToLowerInvariant()))
                return;

            final = final.Substring(_streamProvider.RootDirectory.Length);

            UnloadAsset(final);
        }
    }
}