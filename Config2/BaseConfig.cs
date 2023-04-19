using System.ComponentModel;
using Config2.Extensions;
using Microsoft.Extensions.Configuration;
using Westwind.Utilities;

namespace Config2;

internal class BaseConfig
{
    private static readonly List<Type> SensibleTypes = new List<Type>
            {typeof(bool), typeof(int), typeof(double), typeof(decimal), typeof(string)};

        public static IConfiguration BuildConfiguration(
            IEnumerable<string> yamlFiles,
            IEnumerable<string> jsonFiles,
            bool useUserSecrets)
        {
            var builder = new ConfigurationBuilder();
            if (useUserSecrets) builder.AddUserSecrets<Config>();
            builder
                .AddInMemoryCollection()
                .AddEnvironmentVariables();
            var yamlList = yamlFiles.ToList();
            var jsonList = jsonFiles.ToList();
            foreach (var file in yamlList) builder.AddYamlFile(file, true);
            foreach (var file in jsonList) builder.AddJsonFile(file, true);
            var configuration = builder.Build();
            return configuration;
        }

        public static dynamic GetObject(IConfiguration configuration, string key)
        {
            return GetDynamicObject(configuration, key);
        }

        public static T GetObject<T>(IConfiguration configuration, string key)
        {
            return ((object) GetDynamicObject(configuration, key)).ConvertTo<T>();
        }

        public static IEnumerable<dynamic> GetObjects(IConfiguration configuration, string key)
        {
            var parentSection = configuration.GetSection(key).GetChildren().ToList();
            return parentSection.Any()
                ? parentSection
                    .Select(section => section.Value == null
                        ? GetObject(configuration, GetNestedKey(key, section.Key))
                        : ConvertToSensibleType(section.Value)).ToList()
                : null;
        }

        public static IEnumerable<T> GetObjects<T>(IConfiguration configuration, string key)
        {
            var parentSection = configuration.GetSection(key).GetChildren().ToList();
            if (!parentSection.Any()) return null;
            return parentSection
                .Select(section => GetObject<T>(configuration, GetNestedKey(key, section.Key)))
                .ToList();
        }

        public static IEnumerable<T> GetCollection<T>(IConfiguration configuration, string key)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            var section = configuration.GetSection(key).GetChildren();

            return section.Select(keyValuePair => (T) converter.ConvertFromString(keyValuePair.Value)).ToList();
        }

        public static IEnumerable<dynamic> GetCollection(IConfiguration configuration, string key)
        {
            var section = configuration.GetSection(key).GetChildren();

            return section
                .Where(item => int.TryParse(item.Key, out _))
                .Select(keyValuePair => ConvertToSensibleType(keyValuePair.Value))
                .Cast<dynamic>()
                .ToList();
        }

        public static dynamic GetValue(IConfiguration configuration, string key)
        {
            var section = configuration.GetSection(key);

            return ConvertToSensibleType(section.Value);
        }

        public static T GetValue<T>(IConfiguration configuration, string key)
        {
            var section = configuration.GetSection(key);
            var converter = TypeDescriptor.GetConverter(typeof(T));

            return (T) converter.ConvertFromString(section.Value);
        }

        public static T? GetValueOrNull<T>(IConfiguration configuration, string key) where T : struct
        {
            var section = configuration.GetSection(key);
            var converter = TypeDescriptor.GetConverter(typeof(T));
            try
            {
                return (T?) converter.ConvertFromString(section.Value);
            }
            catch (NotSupportedException)
            {
                return null;
            }
        }

        private static dynamic GetDynamicObject(IConfiguration configuration, string key)
        {
            var parentSection = configuration.GetSection(key).GetChildren().ToList();
            if (!parentSection.Any()) return null;
            dynamic dynamicObject = new Expando();
            var collection = GetCollection(configuration, key).ToList();
            if (int.TryParse(key.Split(':').LastOrDefault(), out _) && collection.Any() &&
                !collection.Any(item => item.Equals(""))) // Checks for nested arrays
                return collection;
            foreach (var childSection in parentSection)
            {
                var nestedSections = childSection.GetChildren().ToList();
                if (!nestedSections.Any()) dynamicObject[childSection.Key] = ConvertToSensibleType(childSection.Value);
                else AssignNestedObjects(configuration, key, childSection, nestedSections, dynamicObject);
            }

            return dynamicObject;
        }

        private static void AssignNestedObjects(
            IConfiguration configuration,
            string rootKey,
            IConfigurationSection parentSection,
            List<IConfigurationSection> childSections,
            dynamic dynamicObject)
        {
            var collection = GetCollection(configuration, GetNestedKey(rootKey, parentSection.Key)).ToList();
            if (collection.Count > 0)
            {
                dynamicObject[parentSection.Key] = new List<dynamic>();
                for (var index = 0; index < collection.Count; index++)
                {
                    var item = collection[index];
                    ((List<dynamic>) dynamicObject[parentSection.Key]).Add(item.Equals("")
                        ? GetDynamicObject(configuration, GetNestedKey(rootKey, parentSection.Key, index.ToString()))
                        : collection[index]);
                }

                return;
            }

            dynamicObject[parentSection.Key] = new Expando();
            var rootNestedSections = childSections.Split(section => !section.GetChildren().Any());
            foreach (var root in rootNestedSections)
            {
                dynamicObject[parentSection.Key][root.Key] = ConvertToSensibleType(root.Value);
            }

            foreach (var nested in childSections)
            {
                var joinedKey = GetNestedKey(rootKey, parentSection.Key, nested.Key);
                var nestedCollection = GetCollection(configuration, joinedKey).ToList();
                if (nestedCollection.Any())
                {
                    if (nestedCollection.Any(item => item.Equals(""))
                    ) // If items = "", the collection is actually an array of objects.
                        dynamicObject[parentSection.Key][nested.Key] = GetObjects(configuration, joinedKey);
                    else dynamicObject[parentSection.Key][nested.Key] = nestedCollection;
                }
                else dynamicObject[parentSection.Key][nested.Key] = GetDynamicObject(configuration, joinedKey);
            }
        }

        private static object ConvertToSensibleType(string value)
        {
            for (var i = 0; i < SensibleTypes.Count; i++)
            {
                var type = SensibleTypes[i];
                var converter = TypeDescriptor.GetConverter(type);
                try
                {
                    return converter.ConvertFromString(value);
                }
                catch (Exception ex)
                {
                    if (!(ex is FormatException) && !(ex is NotSupportedException) && !(ex is ArgumentException)) throw;
                    if (i + 1 >= SensibleTypes.Count) throw; // If there are no more types to try, throw the exception.
                }
            }

            throw new InvalidOperationException($"Unable to convert value {value} to any available type.");
        }

        private static string GetNestedKey(params string[] keys)
        {
            return string.Join(":", keys);
        }
}