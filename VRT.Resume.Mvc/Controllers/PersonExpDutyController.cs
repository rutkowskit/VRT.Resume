using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VRT.Resume.Application.Persons.Commands.DeletePersonExperienceDuty;
using VRT.Resume.Application.Persons.Commands.UpsertPersonExperienceDuty;
using VRT.Resume.Application.Persons.Queries.GetPersonExperienceDuty;
using VRT.Resume.Mvc.Models;

namespace VRT.Resume.Mvc.Controllers;

public sealed class PersonExpDutyController : PersonEditControllerBase
{
    public PersonExpDutyController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Renders view for adding duty to experience
    /// </summary>
    /// <param name="id">experienceId</param>
    /// <returns>Action result</returns>
    [HttpGet]
    public ActionResult Add(int id)
    {
        var model = new PersonExperienceDutyViewModel()
        {
            ExperienceId = id
        };
        return View(model);
    }

    [HttpGet]
    public async Task<ActionResult> Edit(int id)
    {
        var query = new GetPersonExperienceDutyQuery()
        {
            DutyId = id
        };
        var result = await Mediator.Send(query)
            .Map(ToViewModel)
            .ConfigureAwait(false);
        return ToActionResult(result);
    }

    [HttpDelete]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(int entityId)
    {
        var result = await Send(new DeletePersonExperienceDutyCommand(entityId))
            .ConfigureAwait(false);

        return result.IsFailure
            ? ToRequestReferer()
            : ToProfile(TabNames.WorkExp);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Save(UpsertPersonExperienceDutyCommand data)
    {
        var result = await Send(data).ConfigureAwait(false);

        return result.IsFailure
            ? View(data.ExperienceId > 0 ? nameof(Edit) : nameof(Add))
            : ToProfile(TabNames.WorkExp);
    }
    public override ActionResult Cancel() => ToProfile(TabNames.WorkExp);

    private static PersonExperienceDutyViewModel ToViewModel(PersonExperienceDutyVM dto)
    {
        return new PersonExperienceDutyViewModel
        {
            DutyId = dto.DutyId,
            ExperienceId = dto.ExperienceId,
            Name = dto.Name
        };
    }
}