using System.Threading.Tasks;
using VRT.Resume.Application.Persons.Queries.GetPersonData;
using CSharpFunctionalExtensions;
using MediatR;
using VRT.Resume.Application.Persons.Commands.UpdatePersonData;
using Microsoft.AspNetCore.Mvc;
using VRT.Resume.Mvc.Models;

namespace VRT.Resume.Mvc.Controllers
{
    public partial class PersonDataController : PersonEditControllerBase
    {
        public PersonDataController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]                
        public async Task<ActionResult> Edit()
        {            
            var result = await Mediator.Send(new GetPersonDataQuery())
                .Map(r => new PersonDataViewModel(r));

            return ToActionResult(result);
        }               

        [HttpPost]       
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(PersonDataViewModel data)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit");
            }
            var cmd = new UpdatePersonDataCommand()
            {
                FirstName = data.FirstName,
                LastName = data.LastName,
                DateOfBirth = data.DateOfBirth
            };
            await Mediator.Send(cmd);            
            return ToProfile();            
        }             
    }
}