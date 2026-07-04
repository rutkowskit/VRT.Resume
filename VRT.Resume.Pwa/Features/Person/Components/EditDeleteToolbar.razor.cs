using Microsoft.AspNetCore.Components;

namespace VRT.Resume.Pwa.Features.Person.Components;

public partial class EditDeleteToolbar
{
    [Parameter] public EventCallback OnEdit { get; set; }
    [Parameter] public EventCallback OnDelete { get; set; }
}