using Config2.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Config2;

public class Config : IConfig
{
    private IConfiguration _configuration;

        private IEnumerable<string> _yamlFiles = new List<string> {"appsettings.yaml"};
        private IEnumerable<string> _jsonFiles = new List<string> {"appsettings.json"};
        private FileChangeWatcher _watcher;

        private IConfiguration Instance => GetConfiguration();

        /// <summary>The configuration of the Config class.</summary>
        public IConfiguration Configuration => Instance;

        /// <inheritdoc cref="StaticConfig.UseUserSecrets"/>
        public bool UseUserSecrets { get; set; }
        /// <inheritdoc cref="StaticConfig.WatchForFileChanges"/>
        public bool WatchForFileChanges { get; set; }

        private IConfiguration GetConfiguration()
        {
            if (_configuration != null) return _configuration;
            _configuration = BaseConfig.BuildConfiguration(_yamlFiles, _jsonFiles, UseUserSecrets);
            SetUpWatcher();
            return _configuration;
        }

        private void SetUpWatcher()
        {
            if (WatchForFileChanges)
            {
                _watcher = new FileChangeWatcher(_yamlFiles.Concat(_jsonFiles));
                _watcher.FileChanged += (sender, args) =>
                {
                    _configuration = BaseConfig.BuildConfiguration(_yamlFiles, _jsonFiles, UseUserSecrets);
                };
            }
            else
                _watcher = null;
        }

        /// <inheritdoc cref="IConfig.RebuildConfiguration"/>
        public void RebuildConfiguration()
        {
            _configuration = BaseConfig.BuildConfiguration(_yamlFiles, _jsonFiles, UseUserSecrets);
            SetUpWatcher();
        }

        /// <inheritdoc cref="IConfig.AssignYamlFiles"/>
        public void AssignYamlFiles(params string[] files)
        {
            _yamlFiles = files;
        }

        /// <inheritdoc cref="IConfig.AssignJsonFiles"/>
        public void AssignJsonFiles(params string[] files)
        {
            _jsonFiles = files;
        }

        /// <inheritdoc cref="IConfig.GetObject"/>
        public dynamic GetObject(string key)
        {
            return BaseConfig.GetObject(Instance, key);
        }

        /// <inheritdoc cref="IConfig.GetObject{T}"/>
        public T GetObject<T>(string key) where T: class
        {
            return BaseConfig.GetObject<T>(Instance, key);
        }

        /// <inheritdoc cref="IConfig.GetObjects"/>
        public IEnumerable<dynamic> GetObjects(string key)
        {
            return BaseConfig.GetObjects(Instance, key);
        }

        /// <inheritdoc cref="IConfig.GetObjects{T}"/>
        public IEnumerable<T> GetObjects<T>(string key)
        {
            return BaseConfig.GetObjects<T>(Instance, key);
        }

        /// <inheritdoc cref="IConfig.GetCollection{T}"/>
        public IEnumerable<T> GetCollection<T>(string key)
        {
            return BaseConfig.GetCollection<T>(Instance, key);
        }

        /// <inheritdoc cref="IConfig.GetCollection"/>
        public IEnumerable<dynamic> GetCollection(string key)
        {
            return BaseConfig.GetCollection(Instance, key);
        }

        /// <inheritdoc cref="IConfig.GetValue"/>
        public dynamic GetValue(string key)
        {
            return BaseConfig.GetValue(Instance, key);
        }

        /// <inheritdoc cref="IConfig.GetValue{T}"/>
        public T GetValue<T>(string key)
        {
            return BaseConfig.GetValue<T>(Instance, key);
        }

        /// <inheritdoc cref="IConfig.GetValueOrNull{T}"/>
        public T? GetValueOrNull<T>(string key) where T : struct
        {
            return BaseConfig.GetValueOrNull<T>(Instance, key);
        }
}