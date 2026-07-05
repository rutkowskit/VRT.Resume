using VRT.Resume.Pwa.Features.Resumes.Components;
using VRT.Resume.Pwa.Tests.Fixtures;

namespace VRT.Resume.Pwa.Tests.Components;

public sealed class ResumeDocumentCompactTests
{
    [Fact]
    public async Task RendersSeededResumeFullVM()
    {
        string markup;
        using (var ctx = new PwaTestContext())
        {
            var cut = ctx.RenderWithMudProviders<ResumeDocumentCompact>(parameters =>
                parameters.Add(component => component.Model, ResumeTestData.CreateSampleResume()));
            markup = cut.Markup;
        }

        await Verify(markup);
    }
}