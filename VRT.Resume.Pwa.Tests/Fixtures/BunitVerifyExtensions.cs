using Bunit;
using Microsoft.AspNetCore.Components;

namespace VRT.Resume.Pwa.Tests.Fixtures;

public static class BunitVerifyExtensions
{
    public static Task VerifyMarkupAsync<TComponent>(this IRenderedComponent<TComponent> component)
        where TComponent : IComponent =>
        Verify(component.Markup);
}