namespace Config2.Extensions;

internal static class ListExtensions
{
    /// <summary>
    /// Removes items from List based on a filter and returns a new list with those values.
    /// </summary>
    /// <param name="list">The list to split from.</param>
    /// <param name="valid">The filter that validates which items to split.</param>
    /// <returns>The items removed from the original list.</returns>
    public static IEnumerable<T> Split<T>(this List<T> list, Func<T, bool> valid)
    {
        var splitList = list.Where(valid).ToList();

        list.RemoveAll(item => valid(item));

        return splitList;
    }
}