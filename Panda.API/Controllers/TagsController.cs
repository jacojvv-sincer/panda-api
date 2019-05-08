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
    public class TagsController : ControllerBase
    {
        private readonly User _user;
        private readonly ITagService _tagService;
        private readonly IRelationAnalyticsService _relationAnalyticsService;

        public TagsController(IHttpContextAccessor http, ITagService tagService, IRelationAnalyticsService relationAnalyticsService)
        {
            _user = (User)http.HttpContext.Items["ApplicationUser"];
            _tagService = tagService;
            _relationAnalyticsService = relationAnalyticsService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TagViewModel>>> Get()
        {
            var tags = await _tagService.GetTagsForUserTransactions(_user.Id);
            return Mapper.Map<List<TagViewModel>>(tags);
        }

        [HttpPost]
        public async Task<ActionResult<TagViewModel>> Post([FromBody] TagViewModel tagModel)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var tag = await _tagService.CreateTag(Mapper.Map<Tag>(tagModel));
            return Mapper.Map<TagViewModel>(tag);
        }

        [HttpGet]
        [Route("{id}/Analytics")]
        public async Task<RelationAnalyticsViewModel> Analytics(int id)
        {
            return await _relationAnalyticsService.GetRelationAnalytics<Tag>(_user.Id, id);
        }
    }
}