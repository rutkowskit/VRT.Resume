using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace VRT.Resume.Mvc
{
    public static class WebHostEnvironmentExtensions
    {
        public static IEnumerable<(string relativePath, string caption)>
            GetImages(this IWebHostEnvironment hosting, string relativeDirPath)
        {
            var path = hosting.GetFullPath(relativeDirPath);
            var names = Directory.GetFiles(path)
                .Where(s => s.EndsWith(".svg") || s.EndsWith(".png"))
                .Select(s =>
                {
                    var fileName = Path.GetFileName(s);                    
                    var relativePath = Path.Combine(relativeDirPath, fileName);
                    return (relativePath.AsHtmPath(), Path.GetFileNameWithoutExtension(fileName));
                });                
            return names;
        }

        public static string GetImageInfo(this IWebHostEnvironment hosting, string relativePath)
            => hosting.GetFullPath(relativePath);        
        
        private static string GetFullPath(this IWebHostEnvironment hosting, string relativePath)
            => Path.Combine(hosting.WebRootPath, relativePath);
        
        private static string AsHtmPath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;
            return Regex.Replace($"/{path}", @"[\\/]{1,}", "/");
        }
    }
}
