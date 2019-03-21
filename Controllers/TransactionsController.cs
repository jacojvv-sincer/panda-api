using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyManagerApi.Data;
using MoneyManagerApi.Models;

namespace MoneyManagerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private ApplicationDbContext _context;
        private User _user;

        public TransactionsController(ApplicationDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _user = (User)http.HttpContext.Items["ApplicationUser"];
        }

        [HttpGet]
        public async Task<ActionResult<List<Transaction>>> Get()
        {
            return Ok(await _context.Transactions.Where(d => d.User.Id == _user.Id).OrderByDescending(d => d.Date).ToListAsync());
        }

        // [HttpGet]
        // [Route("latest")]
        // public async Task<ActionResult<List<Demarcation>>> GetLatest()
        // {
        //     return Ok(await _context.Demarcations.Include(d => d.Buckets).ThenInclude(b => b.).Where(d => d.User.Id == _user.Id).OrderByDescending(d => d.StartDate).FirstAsync());
        // }

        // [HttpPost]
        // public async Task<ActionResult<Demarcation>> Post([FromBody] Demarcation demarcation)
        // {
        //     // todo fix end & start
        //     if(!ModelState.IsValid){
        //         return UnprocessableEntity(ModelState);
        //     }
        //     if(demarcation.StartDate >= demarcation.EndDate){
        //         return UnprocessableEntity("The start date cannot be larger than the end date");
        //     }
        //     int startCount = await _context.Demarcations.Where(d => d.StartDate > demarcation.StartDate && d.EndDate < demarcation.StartDate).CountAsync();
        //     if(startCount > 0){
        //         return UnprocessableEntity("A demarcation already exists that covers the range of your start date");
        //     }
        //     int endCount = await _context.Demarcations.Where(d => d.EndDate > demarcation.StartDate && d.EndDate < demarcation.EndDate).CountAsync();
        //     if(startCount > 0){
        //         return UnprocessableEntity("A demarcation already exists that covers the range of your end date");
        //     }

        //     demarcation.User = _user;
        //     _context.Demarcations.Add(demarcation);
        //     await _context.SaveChangesAsync();
        //     return Ok(demarcation);
        // }
    }
}
