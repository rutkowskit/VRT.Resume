using CSharpFunctionalExtensions;

namespace VRT.Resume.Application
{
    internal static class ResultExtensions
    {
        internal static string GetErrorSafe(this Result result)
        {
            return result.IsSuccess
                ? ""
                : result.Error;
        }
    }
}
