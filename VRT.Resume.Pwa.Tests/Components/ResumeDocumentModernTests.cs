using VRT.Resume.Pwa.Features.Resumes.Components;
using VRT.Resume.Pwa.Tests.Fixtures;

namespace VRT.Resume.Pwa.Tests.Components;

public sealed class ResumeDocumentModernTests
{
    [Fact]
    public async Task RendersSeededResumeFullVM()
    {
        string markup;
        using (var ctx = new PwaTestContext())
        {
            var cut = ctx.RenderWithMudProviders<ResumeDocumentModern>(parameters =>
                parameters.Add(component => component.Model, ResumeTestData.CreateSampleResume()));
            markup = cut.Markup;
        }

        await Verify(markup);
    }
}