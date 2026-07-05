using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace VRT.Resume.Pwa.Components;

public class MudIconButtonWithTitle : MudIconButton
{
    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public string? For { get; set; }

    protected override void OnParametersSet()
    {
        if (!string.IsNullOrEmpty(Title))
        {
            UserAttributes["aria-label"] = Title;
            UserAttributes["title"] = Title;
        }

        if (!string.IsNullOrEmpty(For))
        {
            UserAttributes["for"] = For;
        }
    }
}