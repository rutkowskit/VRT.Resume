using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using VRT.Resume.Web.Models;

namespace VRT.Resume.Web.Controllers
{
    public sealed class PersonEduController : PersonEditControllerBase
    {
        public PersonEduController(IMediator mediator) : base(mediator)
        {
        }
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            //var result = await Mediator.Send(new GetPersonDataQuery())
            //    .Map(r => new PersonDataViewModel(r));

            //return ToActionResult(result);
            await Task.Yield();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(PersonEduViewModel data)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View("Edit");
            //}
            //var cmd = new UpsertPersonDataCommand()
            //{
            //    FirstName = data.FirstName,
            //    LastName = data.LastName,
            //    DateOfBirth = data.DateOfBirth
            //};
            //await Mediator.Send(cmd);
            //TempData[TempDataKeys.TabName] = TabNames.Profile;
            await Task.Yield();
            return ToProfileAfterSave(TabNames.Education);
        }
        public override ActionResult Cancel() 
            => ToProfile(TabNames.Education);
    }
}