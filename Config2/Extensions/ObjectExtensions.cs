using System.Reflection;
using Newtonsoft.Json;
using Westwind.Utilities;

namespace Config2.Extensions;

internal static class ObjectExtensions
    {
        public static T ConvertTo<T>(this object theObject)
        {
            var converted = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(theObject));
            CleanseJson(converted);
            return converted;
        }

        private static void CleanseJson(object theObject)
        {
            if (theObject == null) return;
            var properties = theObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                try
                {
                    var nestedObject = property.GetValue(theObject);
                    if (nestedObject == null) continue;
                    var nestedType = nestedObject.GetType().ToString();
                    if (nestedType.EndsWith("]")) // Object is a list.
                    {
                        var nestedObjects = ((IEnumerable<object>) nestedObject).ToList();
                        for (var index = 0; index < nestedObjects.Count; index++)
                        {
                            var anObject = nestedObjects[index];
                            var type = anObject.GetType().ToString();
                            if (type.EndsWith("JObject"))
                            {
                                nestedObjects[index] =
                                    JsonConvert.DeserializeObject<Expando>(JsonConvert.SerializeObject(anObject));
                                var newExpando = (Expando) nestedObjects[index];
                                CleanseJson(newExpando); // There are potentially more JObjects and JArrays
                            }
                            else if (type.EndsWith("JArray"))
                            {
                                nestedObjects[index] =
                                    JsonConvert.DeserializeObject<List<dynamic>>(
                                        JsonConvert.SerializeObject(nestedObject));
                                var newValues = (List<dynamic>) nestedObjects[index];
                                CleanseJson(newValues); // There are potentially more JObjects and JArrays
                            }
                            else if (type.Contains("+"))
                            {
                                CleanseJson(anObject);
                            }
                        }

                        property.SetValue(theObject, nestedObjects);
                    }
                    else if (nestedType.EndsWith("JObject"))
                    {
                        property.SetValue(theObject,
                            JsonConvert.DeserializeObject<Expando>(JsonConvert.SerializeObject(nestedObject)));
                        var newExpando = (Expando) property.GetValue(theObject);
                        CleanseJson(newExpando); // There are potentially more JObjects and JArrays
                    }
                    else if (nestedType.EndsWith("JArray"))
                    {
                        property.SetValue(theObject,
                            JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(nestedObject)));
                        var newValues = (List<dynamic>) property.GetValue(theObject);
                        CleanseJson(newValues); // There are potentially more JObjects and JArrays
                    }
                    else
                        CleanseJson(nestedObject);
                }
                catch (TargetParameterCountException targetParameterCountException)
                {
                    if (targetParameterCountException.Message != "Parameter count mismatch") continue;
                    throw;
                }
                catch (ArgumentException argumentException)
                {
                    if (argumentException.Message.Contains("cannot be converted to type")) continue;
                    throw;
                }
                catch (InvalidCastException invalidCastException)
                {
                    if (invalidCastException.Message.StartsWith("Unable to cast object")) continue;
                    throw;
                }
            }
        }

        private static void CleanseJson(IList<object> objects)
        {
            for (var index = 0; index < objects.Count; index++)
            {
                var anObject = objects[index];
                var type = anObject.GetType().ToString();
                if (type.EndsWith("JObject"))
                {
                    objects[index] = JsonConvert.DeserializeObject<Expando>(JsonConvert.SerializeObject(anObject));
                    var expando = (Expando) objects[index];
                    CleanseJson(expando); // There are potentially more JObjects and JArrays
                }
                else if (type.EndsWith("JArray"))
                {
                    objects[index] =
                        JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(anObject));
                    var newValues = (List<object>) objects[index];
                    CleanseJson(newValues);
                }
                else
                {
                    CleanseJson(anObject);
                }
            }
        }

        private static void CleanseJson(Expando expando)
        {
            if (expando == null) return;
            for (var index = 0; index < expando.Properties.Count; index++)
            {
                var property = expando.Properties.ElementAt(index);
                var type = property.Value.GetType().ToString();
                if (type.EndsWith("JObject"))
                {
                    expando[property.Key] =
                        JsonConvert.DeserializeObject<Expando>(JsonConvert.SerializeObject(property.Value));
                    var newExpando = (Expando) expando[property.Key];
                    CleanseJson(newExpando); // There are potentially more JObjects and JArrays
                }
                else if (type.EndsWith("JArray"))
                {
                    expando[property.Key] =
                        JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(property.Value));
                    var newValues = (List<object>) expando[property.Key];
                    CleanseJson(newValues); // There are potentially more JObjects and JArrays
                }
                else if (type == "System.Object")
                {
                    CleanseJson((Expando) property.Value);
                }
            }
        }
    }