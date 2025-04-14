using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VRT.Resume.Application.Persons.Commands.DeletePersonExperience;
using VRT.Resume.Application.Persons.Commands.UpsertPersonExperience;
using VRT.Resume.Application.Persons.Queries.GetPersonExperience;
using VRT.Resume.Mvc.Models;

namespace VRT.Resume.Mvc.Controllers
{
    public sealed class PersonExpController : PersonEditControllerBase
    {
        public PersonExpController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var query = new GetPersonExperienceQuery()
            {
                ExperienceId = id
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
            var result = await Send(new DeletePersonExperienceCommand(entityId))
                .ConfigureAwait(false);
            return result.IsFailure
                ? ToRequestReferer()
                : ToProfile(TabNames.WorkExp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(UpsertPersonExperienceCommand data)
        {
            var result = await Send(data).ConfigureAwait(false);

            return result.IsFailure
                ? View(data.ExperienceId > 0 ? nameof(Edit) : nameof(Add))
                : ToProfile(TabNames.WorkExp);
        }
        public override ActionResult Cancel()
            => ToProfile(TabNames.WorkExp);

        private PersonExperienceViewModel ToViewModel(PersonExperienceVM dto)
        {
            return new PersonExperienceViewModel
            {
                ExperienceId = dto.ExperienceId,
                CompanyName = dto.CompanyName,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                Location = dto.Location,
                Position = dto.Position
            };
        }
    }
}