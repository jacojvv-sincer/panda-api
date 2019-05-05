using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Panda.API.Data;
using Panda.API.Data.Models;
using Panda.API.Interfaces;
using Panda.API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panda.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private ApplicationDbContext _context;
        private User _user;
        private ICategoryService _categoryService;

        public CategoriesController(ApplicationDbContext context, IHttpContextAccessor http, ICategoryService categoryService)
        {
            _context = context;
            _user = (User)http.HttpContext.Items["ApplicationUser"];
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryViewModel>>> Get()
        {
            var categories = await _categoryService.GetCategoriesForUserTransactions(_user.Id);
            return Ok(AutoMapper.Mapper.Map<CategoryViewModel>(categories));
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Post([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var existingCategory = await _context.Categories.Where(c => c.Name == category.Name).FirstOrDefaultAsync();
            if (existingCategory != null)
            {
                return Ok(existingCategory);
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        [HttpGet]
        [Route("{id}/Analytics")]
        public async Task<RelationAnalyticsViewModel> Analytics(int id)
        {
            IQueryable<Transaction> transactions = _context.Transactions.Where(t => t.User.Id == _user.Id && t.Category.Id == id);

            int lifetimeCount = await transactions.CountAsync();
            decimal lifetimeTotal = await transactions.Select(t => t.Amount).SumAsync();
            int monthCount = await transactions.Where(t => t.Date > DateTime.Now.AddDays(-30)).CountAsync();
            decimal monthTotal = await transactions.Where(t => t.Date > DateTime.Now.AddDays(-30)).Select(t => t.Amount).SumAsync();

            return new RelationAnalyticsViewModel()
            {
                LifetimeTotalTransactions = lifetimeCount,
                LifetimeSumOfTransactions = lifetimeTotal,
                LifetimeAveragePerTransaction = lifetimeCount > 0 ? lifetimeTotal / lifetimeCount : 0,
                MonthTotalTransactions = monthCount,
                MonthSumOfTransactions = monthTotal,
                MonthAveragePerTransaction = monthCount > 0 ? monthTotal / monthCount : 0
            };
        }
    }
}