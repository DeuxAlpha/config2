using Config2.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Config2;

/// <summary>Static configuration provider for global utilization of User Secrets, Environment Variables, and YAML and
/// JSON files, while providing type safe accessors to variables stored therein.</summary>
public class StaticConfig
{
    
        private static IConfiguration _configuration;

        private static IEnumerable<string> _yamlFiles = new List<string> {"appsettings.yaml"};
        private static IEnumerable<string> _jsonFiles = new List<string> {"appsettings.json"};
        private static FileChangeWatcher _watcher;

        /// <summary>Instructs whether the configuration should be built utilizing User Secrets, or not.<br/><b>Default:</b> false</summary>
        /// <remarks>If true, you will need to provide the UserSecretsId in the project file.
        /// <see href="https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets">See here for more info.</see>
        /// </remarks>
        public static bool UseUserSecrets { get; set; } = false;

        /// <summary>Instructs whether the configuration should watch the configuration files and rebuild itself upon
        /// any changes being made to them.<br/><b>Default:</b> false</summary>
        public static bool WatchForFileChanges { get; set; } = false;

        private static IConfiguration Instance => GetConfiguration();
        /// <summary>The configuration of the StaticConfig class.</summary>
        public static IConfiguration Configuration => Instance;

        private static IConfiguration GetConfiguration()
        {
            if (_configuration != null) return _configuration;
            _configuration = BaseConfig.BuildConfiguration(_yamlFiles, _jsonFiles, UseUserSecrets);
            SetUpWatcher();
            return _configuration;
        }

        private static void SetUpWatcher()
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
        public static void RebuildConfiguration()
        {
            _configuration = BaseConfig.BuildConfiguration(_yamlFiles, _jsonFiles, UseUserSecrets);
            SetUpWatcher();
        }

        /// <inheritdoc cref="IConfig.AssignYamlFiles"/>
        public static void AssignYamlFiles(params string[] files)
        {
            _yamlFiles = files;
        }

        /// <inheritdoc cref="IConfig.AssignJsonFiles"/>
        public static void AssignJsonFiles(params string[] files)
        {
            _jsonFiles = files;
        }

        /// <inheritdoc cref="IConfig.GetObject"/>
        public static dynamic GetObject(string key)
        {
            return BaseConfig.GetObject(Instance, key);
        }

        /// <inheritdoc cref="IConfig.GetObject{T}"/>
        public static T GetObject<T>(string key)
        {
            return BaseConfig.GetObject<T>(Instance, key);
        }

        /// <inheritdoc cref="IConfig.GetObjects"/>
        public static IEnumerable<dynamic> GetObjects(string key)
        {
            return BaseConfig.GetObjects(Instance, key);
        }

        /// <inheritdoc cref="IConfig.GetObjects{T}"/>
        public static IEnumerable<T> GetObjects<T>(string key)
        {
            return BaseConfig.GetObjects<T>(Instance, key);
        }

        /// <inheritdoc cref="IConfig.GetCollection{T}"/>
        public static IEnumerable<T> GetCollection<T>(string key)
        {
            return BaseConfig.GetCollection<T>(Instance, key);
        }

        /// <inheritdoc cref="IConfig.GetCollection"/>
        public static IEnumerable<dynamic> GetCollection(string key)
        {
            return BaseConfig.GetCollection(Instance, key);
        }

        /// <inheritdoc cref="IConfig.GetValue"/>
        public static dynamic GetValue(string key)
        {
            return BaseConfig.GetValue(Instance, key);
        }

        /// <inheritdoc cref="IConfig.GetValue{T}"/>
        public static T GetValue<T>(string key)
        {
            return BaseConfig.GetValue<T>(Instance, key);
        }

        /// <inheritdoc cref="IConfig.GetValueOrNull{T}"/>
        public static T? GetValueOrNull<T>(string key) where T : struct
        {
            return BaseConfig.GetValueOrNull<T>(Instance, key);
        }
}