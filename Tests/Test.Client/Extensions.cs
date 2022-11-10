using System.IO;
using System.Linq;
using System.Text;

namespace Test;

internal static class Extensions
{
    public static Stream NextStream(this Random rnd)
    {
        var text = string.Join("\n", Enumerable.Range(0, rnd.Next(1, 10)).Select(i => $"text_{rnd.Next()}").ToArray());
        return new MemoryStream(Encoding.UTF8.GetBytes(text));
    }

    public static IStreamFile NextStreamFile(this Random rnd)
    {
        return new StreamFile
        {
            Content = rnd.NextStream(),
            Name = $"name_{rnd.Next()}",
            Type = "text/plain",
        };
    }

}
