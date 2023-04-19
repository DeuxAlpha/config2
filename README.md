# Config2
Config2 is a C# library for managing application configuration using JSON and YAML files, environment variables, and user secrets. It provides an easy-to-use interface to read configuration data, with support for type-safe accessors and automatic change monitoring.

## Features
- Supports JSON and YAML configuration files
- Supports environment variables and user secrets
- Type-safe accessors for configuration values
- Automatic monitoring of configuration files for changes
- Flexible, easy-to-use interface

## Installation
To use Config2, add the library as a dependency to your project using your preferred package manager or by downloading the source code directly.

## Usage
Here's a basic example of how to use Config2 in your C# project:

```csharp
using Config2;

// Get a value from the configuration
string connectionString = StaticConfig.GetValue<string>("ConnectionStrings:DefaultConnection");

// Get a strongly typed object from the configuration
DatabaseSettings dbSettings = StaticConfig.GetObject<DatabaseSettings>("DatabaseSettings");

// Get a collection of objects from the configuration
IEnumerable<Endpoint> endpoints = StaticConfig.GetObjects<Endpoint>("Endpoints");
```
## Configuration Files
By default, Config2 looks for appsettings.json and appsettings.yaml files in your project's root directory. You can specify additional or different files using the AssignJsonFiles and AssignYamlFiles methods:

```csharp
StaticConfig.AssignJsonFiles("config.json");
StaticConfig.AssignYamlFiles("config.yaml");
```
### User Secrets
To enable user secrets in your configuration, set the UseUserSecrets property to true:

```csharp
StaticConfig.UseUserSecrets = true;
```
Note that you'll also need to provide the UserSecretsId in your project file. Refer to the official Microsoft documentation for more information.

### Watching for File Changes
To enable automatic monitoring of configuration files for changes, set the WatchForFileChanges property to true:

```csharp
StaticConfig.WatchForFileChanges = true;
```
When this feature is enabled, the configuration will be rebuilt automatically whenever a change is detected in any of the monitored files.

# License
Config2 is released under the MIT License.
