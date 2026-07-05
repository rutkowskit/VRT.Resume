using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VRT.Resume.Application.Persons.Commands.DeletePersonEducation;
using VRT.Resume.Application.Persons.Commands.UpsertPersonEducation;
using VRT.Resume.Application.Persons.Queries.GetPersonEducation;
using VRT.Resume.Mvc.Models;

namespace VRT.Resume.Mvc.Controllers;

public sealed class PersonEduController : PersonEditControllerBase
{

    public PersonEduController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    public async Task<ActionResult> Edit(int id)
    {
        var query = new GetPersonEducationQuery(id);
        var result = await Mediator.Send(query)
            .Map(ToViewModel)
            .ConfigureAwait(false);

        return ToActionResult(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Save(UpsertPersonEducationCommand data)
    {
        var result = await Send(data).ConfigureAwait(false);
        return result.IsFailure
            ? View("Edit")
            : ToProfile(TabNames.Education);
    }

    [HttpDelete]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(int entityId)
    {
        var result = await Send(new DeletePersonEducationCommand(entityId))
            .ConfigureAwait(false);
        return result.IsFailure
            ? ToRequestReferer()
            : ToProfile(TabNames.Education);
    }

    public override ActionResult Cancel()
        => ToProfile(TabNames.Education);


    private PersonEducationViewModel ToViewModel(PersonEducationInListVM dto)
    {
        return new PersonEducationViewModel
        {
            EducationId = dto.EducationId,
            Degree = dto.Degree ?? string.Empty,
            Field = dto.Field,
            FromDate = dto.FromDate,
            ToDate = dto.ToDate.GetValueOrDefault(),
            Grade = dto.Grade,
            SchoolName = dto.SchoolName,
            Specialization = dto.Specialization,
            ThesisTitle = dto.ThesisTitle
        };
    }
}