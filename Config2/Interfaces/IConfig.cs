namespace Config2.Interfaces;

/// <summary>Interface for defining general functionality for configuration providers.</summary>
    public interface IConfig
    {
        /// <summary>Assigns JSON files to be used in the configuration.<br/><b>Default:</b> appsettings.json</summary>
        /// <param name="jsonFiles">The files to be assigned. Can be relative or absolute paths.</param>
        void AssignJsonFiles(params string[] jsonFiles);
        /// <summary>Assigns YAML files to be used in the configuration.<br/><b>Default:</b> appsettings.yaml</summary>
        /// <param name="yamlFiles">The files to be assigned. Can be relative or absolute paths.</param>
        void AssignYamlFiles(params string[] yamlFiles);
        /// <summary>Rebuilds the configuration and applies updated parameters.</summary>
        void RebuildConfiguration();
        /// <summary>Gets a dynamic object based on the supplied key.</summary>
        /// <example>GetObject("Key")<br/>GetObject("Parent:Child")<br/>GetObject("Array:0")</example>
        /// <param name="key">The key by which to retrieve the object.</param>
        /// <returns>The value located at the provided key.</returns>
        dynamic GetObject(string key);
        /// <summary>Gets a object based on the supplied key, and converts it to the supplied type (can be a complex
        /// object, like a nested class).</summary>
        /// <example>GetObject&lt;SampleClass&gt;("Key")<br/>GetObject&lt;SampleClass&gt;("Parent:Child")<br/>GetObject&lt;SampleClass&gt;("Array:0")</example>
        /// <param name="key">The key by which to retrieve the object.</param>
        /// <typeparam name="T">The class in which to convert the retrieved object.</typeparam>
        /// <returns>The converted object.</returns>
        T GetObject<T>(string key) where T: class;
        /// <summary>Gets a collection of dynamic objects based on the supplied key (these can be of different types).</summary>
        /// <remarks>The key should lead to an array of objects.</remarks>
        /// <example>GetObjects("Key")<br/>GetObjects("Parent:Child")<br/>GetObjects("Array:0")</example>
        /// <param name="key">The key by which to retrieve the objects.</param>
        /// <returns>A collection of dynamic objects.</returns>
        IEnumerable<dynamic> GetObjects(string key);
        /// <summary>Gets a collection of objects converted to the type provided.</summary>
        /// <remarks>The key should lead to an array of objects.</remarks>
        /// <example>GetObjects&lt;SampleClass&gt;("Key")<br/>GetObjects&lt;SampleClass&gt;("Parent:Child")<br/>GetObjects&lt;SampleClass&gt;("Array:0")</example>
        /// <param name="key">The key by which to retrieve the objects.</param>
        /// <typeparam name="T">The class in which to convert the retrieved objects.</typeparam>
        /// <returns>A collection of converted objects.</returns>
        IEnumerable<T> GetObjects<T>(string key);
        /// <summary>Gets a collection of primitive types based on the supplied key (these can be of different types).</summary>
        /// <remarks>The key should lead to an array of primitive types.</remarks>
        /// <example>GetCollection("Key")<br/>GetCollection("Parent:Child")<br/>GetCollection("Array:0")</example>
        /// <param name="key">The key by which to retrieve the values.</param>
        /// <returns>A collection of primitive values.</returns>
        IEnumerable<dynamic> GetCollection(string key);
        /// <summary>Gets a collection of primitive types converted to the type provided.</summary>
        /// <remarks>The key should lead to an array of primitive types.</remarks>
        /// <example>GetCollection&lt;string&gt;("Key")<br/>GetCollection&lt;string&gt;("Parent:Child")<br/>GetCollection&lt;string&gt;("Array:0")</example>
        /// <param name="key">The key by which to retrieve the values.</param>
        /// <typeparam name="T">The type in which to convert the retrieved values.</typeparam>
        /// <returns>A collection of converted primitive values.</returns>
        IEnumerable<T> GetCollection<T>(string key);
        /// <summary>Gets the dynamic value of a primitive type based on the supplied key.</summary>
        /// <example>GetValue("Key")<br/>GetValue("Parent:Child")<br/>GetValue("Array:0")</example>
        /// <param name="key">The key by which to retrieve the value.</param>
        /// <returns>The value located at the key.</returns>
        dynamic GetValue(string key);
        /// <summary>Gets a value based on the supplied key and converts it to a primitive type.</summary>
        /// <example>GetValue&lt;string&gt;("Key")<br/>GetValue&lt;string&gt;("Parent:Child")<br/>GetValue&lt;string&gt;("Array:0")</example>
        /// <param name="key">The key by which to retrieve the value.</param>
        /// <typeparam name="T">The type in which to convert the retrieved value.</typeparam>
        /// <returns>The value located at the key.</returns>
        T GetValue<T>(string key);
        /// <summary>Gets a value based on the supplied key or if it can't, null.</summary>
        /// <example>GetValueOrNull&lt;string&gt;("Key")<br/>GetValueOrNull&lt;string&gt;("Parent:Child")<br/>GetValueOrNull&lt;string&gt;("Array:0")</example>
        /// <param name="key">The key by which to retrieve the value.</param>
        /// <typeparam name="T">The type in which to convert the retrieved value.</typeparam>
        /// <returns>The value located at the key, or null.</returns>
        T? GetValueOrNull<T>(string key) where T : struct;
    }