using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Panda.API.Data.Models;
using Panda.API.Interfaces;
using Panda.API.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Panda.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly User _user;
        private readonly IPersonService _personService;
        private readonly IRelationAnalyticsService _relationAnalyticsService;

        public PeopleController(IHttpContextAccessor http, IRelationAnalyticsService relationAnalyticsService, IPersonService personService)
        {
            _user = (User)http.HttpContext.Items["ApplicationUser"];
            _personService = personService;
            _relationAnalyticsService = relationAnalyticsService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PersonViewModel>>> Get()
        {
            var people = await _personService.GetPeopleForUserTransactions(_user.Id);
            return Ok(Mapper.Map<List<PersonViewModel>>(people));
        }

        [HttpPost]
        public async Task<ActionResult<PersonViewModel>> Post([FromBody] PersonViewModel personModel)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var person = await _personService.CreatePerson(Mapper.Map<Person>(personModel));
            return Ok(Mapper.Map<PersonViewModel>(person));
        }

        [HttpGet]
        [Route("{id}/Analytics")]
        public async Task<RelationAnalyticsViewModel> Analytics(int id)
        {
            return await _relationAnalyticsService.GetRelationAnalytics<Person>(_user.Id, id);
        }
    }
}