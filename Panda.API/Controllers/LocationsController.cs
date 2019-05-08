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
    public class LocationsController : ControllerBase
    {
        private readonly User _user;
        private readonly ILocationService _locationService;
        private readonly IRelationAnalyticsService _relationAnalyticsService;

        public LocationsController(IHttpContextAccessor http, IRelationAnalyticsService relationAnalyticsService, ILocationService locationService)
        {
            _user = (User)http.HttpContext.Items["ApplicationUser"];
            _locationService = locationService;
            _relationAnalyticsService = relationAnalyticsService;
        }

        [HttpGet]
        public async Task<ActionResult<List<LocationViewModel>>> Get()
        {
            var locations = await _locationService.GetLocationsForUserTransactions(_user.Id);
            return Mapper.Map<List<LocationViewModel>>(locations);
        }

        [HttpPost]
        public async Task<ActionResult<LocationViewModel>> Post([FromBody] LocationViewModel locationModel)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var location = await _locationService.CreateLocation(Mapper.Map<Location>(locationModel));
            return Ok(Mapper.Map<LocationViewModel>(location));
        }

        [HttpGet]
        [Route("{id}/Analytics")]
        public async Task<RelationAnalyticsViewModel> Analytics(int id)
        {
            return await _relationAnalyticsService.GetRelationAnalytics<Location>(_user.Id, id);
        }
    }
}