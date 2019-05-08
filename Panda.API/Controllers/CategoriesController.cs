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
    public class CategoriesController : ControllerBase
    {
        private readonly User _user;
        private readonly ICategoryService _categoryService;
        private readonly IRelationAnalyticsService _relationAnalyticsService;

        public CategoriesController(IHttpContextAccessor http, ICategoryService categoryService, IRelationAnalyticsService relationAnalyticsService)
        {
            _user = (User)http.HttpContext.Items["ApplicationUser"];
            _categoryService = categoryService;
            _relationAnalyticsService = relationAnalyticsService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryViewModel>>> Get()
        {
            var categories = await _categoryService.GetCategoriesForUserTransactions(_user.Id);
            return Mapper.Map<List<CategoryViewModel>>(categories);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryViewModel>> Post([FromBody] CategoryViewModel categoryModel)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var category = await _categoryService.CreateCategory(Mapper.Map<Category>(categoryModel));
            return Mapper.Map<CategoryViewModel>(category);
        }

        [HttpGet]
        [Route("{id}/Analytics")]
        public async Task<RelationAnalyticsViewModel> Analytics(int id)
        {
            return await _relationAnalyticsService.GetRelationAnalytics<Category>(_user.Id, id);
        }
    }
}