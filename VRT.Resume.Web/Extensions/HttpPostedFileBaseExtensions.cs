using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace VRT.Resume.Web
{
    public static class HttpPostedFileBaseExtensions
    {
        public static async Task<byte[]> ReadAllBytes(this HttpPostedFileBase file)
        {
            if (file == null) 
                return new byte[0];

            using (var memStream = new MemoryStream())
            {
                if (file.InputStream.CanSeek)
                    file.InputStream.Position = 0;
                await file.InputStream.CopyToAsync(memStream);
                return memStream.ToArray();
            }                            
        }
    }
}