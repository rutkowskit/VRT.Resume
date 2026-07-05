using System.Runtime.CompilerServices;
using VerifyTests;

namespace VRT.Resume.Pwa.Tests.Fixtures;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.ScrubLinesContaining("blazor:");
    }
}