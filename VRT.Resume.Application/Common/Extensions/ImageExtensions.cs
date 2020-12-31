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
        //public static Bitmap ScaleImage(this Image oldImage, int maxSize)
        //{
        //    double resizeFactor = 1;

        //    if (oldImage.Width > maxSize || oldImage.Height > maxSize)
        //    {
        //        double widthFactor = Convert.ToDouble(oldImage.Width) / maxSize;
        //        double heightFactor = Convert.ToDouble(oldImage.Height) / maxSize;
        //        resizeFactor = Math.Max(widthFactor, heightFactor);

        //    }
        //    int width = Convert.ToInt32(oldImage.Width / resizeFactor);
        //    int height = Convert.ToInt32(oldImage.Height / resizeFactor);

        //    Bitmap newImage = new Bitmap(width, height);
        //    newImage.SetResolution(oldImage.VerticalResolution, oldImage.VerticalResolution);
        //    using (var g = Graphics.FromImage(newImage))
        //    {
        //        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //        g.DrawImage(oldImage, 0, 0, newImage.Width, newImage.Height);
        //    }                           
        //    return newImage;
        //}
    }
}