using System.Linq;
using System.Text.RegularExpressions;
using System;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonContact
{
    internal static class StringExtensions
    {
        const RegexOptions opt = RegexOptions.Singleline | RegexOptions.IgnoreCase;
        /// <summary>
        /// Get first mached supported image in safe manner (to avoid script injection)
        /// </summary>
        /// <param name="iconText">Image passed by user</param>
        /// <returns>Safe image text</returns>
        internal static string ToSafeImage(this string iconText)
        {
            if (string.IsNullOrWhiteSpace(iconText))
                return null;

            var match = new Func<Match>[]
            {
                ()=>Regex.Match(iconText, @"(?<img>^\s*\<svg[^\>]*\>.*?\<\/svg\>)", opt),
                ()=>Regex.Match(iconText, @"(?<img>^\s*\<img\s*[^\<\>]*>)", opt)
            }
            .Select(m => m())
            .FirstOrDefault(m => m.Success);

            return match?.Success ?? false
                ? match.Groups["img"].Value
                : null;            
        }
    }
}
