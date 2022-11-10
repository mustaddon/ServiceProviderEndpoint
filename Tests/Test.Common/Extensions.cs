using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public static class Extensions
    {
        public static async Task<string> ToText(this Stream stream)
        {
            if (stream.Position != 0)
                stream.Position = 0;

            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms).ConfigureAwait(false);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
