using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace VRT.Resume.Mvc
{
    public static class HttpPostedFileBaseExtensions
    {
        public static async Task<byte[]> ReadAllBytes(this IFormFile file)
        {
            if (file == null) 
                return System.Array.Empty<byte>();            
            using (var memStream = new MemoryStream())
            {                
                await file.CopyToAsync(memStream);
                return memStream.ToArray();
            }                            
        }
    }
}