using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Panda.API.Data;
using Panda.API.Data.Models;
using Panda.API.Interfaces;
using Panda.API.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Panda.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly User _user;
        private readonly IReportService _reportsService;

        public ReportsController(ApplicationDbContext context, IHttpContextAccessor http, IReportService reportsService)
        {
            _context = context;
            _user = (User)http.HttpContext.Items["ApplicationUser"];
            _reportsService = reportsService;
        }

        [HttpGet]
        [Route("Balance")]
        public async Task<ActionResult<decimal>> Balance()
        {
            return await _reportsService.GetUserBalance(_user.Id);
        }

        [HttpGet]
        [Route("Burndown")]
        public async Task<ActionResult<List<BurndownValuesViewModel>>> Burndown([FromQuery] int days = 30)
        {
            return await _reportsService.GetUserBurndown(_user.Id, days);
        }
    }
}