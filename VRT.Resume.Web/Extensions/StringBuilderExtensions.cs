using System.Text;

namespace VRT.Resume.Web
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendNonEmpty(this StringBuilder builder,
            string value )
        {
            if (builder==null || string.IsNullOrWhiteSpace(value)) 
                return builder;
            return builder.Append(value);
        }
    }
}