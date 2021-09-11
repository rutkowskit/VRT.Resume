using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace VRT.Resume.Application
{
    /// <summary>
    /// Image utilities class
    /// </summary>
    public static class ImageExtensions
    {
        public static byte[] ScaleImage(this byte[] oldImage, int maxSize)
        {            
            using (var img = Image.Load(oldImage))
            {
                var newSize = img.GetNewSize(maxSize);
                img.Mutate(c =>
                {
                    var cropSize = Math.Min(img.Width, img.Height);
                    var opt = new ResizeOptions()
                    {
                        Mode = ResizeMode.Stretch,
                        Size = newSize,
                        Sampler = KnownResamplers.Lanczos3
                    };
                    c.Resize(opt);
                }); 
                
                using (var outStream = new MemoryStream())
                {
                    img.SaveAsJpeg(outStream);
                    outStream.Flush();
                    outStream.Position = 0;
                    return outStream.ToArray();
                }
            }
        }

        private static Size GetNewSize(this Image img, int maxSize)
        {
            double resizeFactor = 1;

            if (img.Width > maxSize || img.Height > maxSize)
            {
                double widthFactor = Convert.ToDouble(img.Width) / maxSize;
                double heightFactor = Convert.ToDouble(img.Height) / maxSize;
                resizeFactor = Math.Max(widthFactor, heightFactor);
            }

            int width = Convert.ToInt32(img.Width / resizeFactor);
            int height = Convert.ToInt32(img.Height / resizeFactor);
            return new Size(width, height);
        }       
    }
}