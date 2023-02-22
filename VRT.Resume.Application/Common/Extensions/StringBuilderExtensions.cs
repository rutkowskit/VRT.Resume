using System.Text;

namespace VRT.Resume;
public static class StringBuilderExtensions
{
    public static StringBuilder AppendNonEmpty(this StringBuilder builder,
        string value )
    {
        if (string.IsNullOrWhiteSpace(value)) 
            return builder;
        return builder.Append(value);
    }
}