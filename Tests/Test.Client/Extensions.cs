using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Test;

internal static class Extensions
{
    public static Stream NextStream(this Random rnd, out string text)
    {
        text = string.Join("\n", Enumerable.Range(0, rnd.Next(1, 10)).Select(i => $"text_{rnd.Next()}").ToArray());
        return new MemoryStream(Encoding.UTF8.GetBytes(text));
    }

    public static IStreamFile NextStreamFile(this Random rnd, out string text)
    {
        return new StreamFile
        {
            Content = rnd.NextStream(out text),
            Name = $"name_{rnd.Next()}",
            Type = "text/plain",
        };
    }

    public static IEnumerable<T?> NextEnumerable<T>(this Random rnd, Func<int, T> val)
    {
        return Enumerable.Range(0, rnd.Next(10, 100)).Select(val);
    }
}
