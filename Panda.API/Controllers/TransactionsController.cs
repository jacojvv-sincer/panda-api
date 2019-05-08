using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Panda.API.Data.Models;
using Panda.API.Exceptions;
using Panda.API.Interfaces;
using Panda.API.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Panda.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private User _user;
        private ITransactionService _transactionService { get; }

        public TransactionsController(IHttpContextAccessor http, ITransactionService transactionService)
        {
            _transactionService = transactionService;
            _user = (User)http.HttpContext.Items["ApplicationUser"];
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedContentViewModel<Transaction>>> Get([FromQuery]int page = 1, [FromQuery]int perPage = 30)
        {
            int totalItems = await _transactionService.GetCountOfUserTransactions(_user.Id);
            List<Transaction> items = await _transactionService.GetUserTransactions(_user.Id, page, perPage);

            return Ok(new PaginatedContentViewModel<TransactionViewModel>()
            {
                Items = AutoMapper.Mapper.Map<List<TransactionViewModel>>(items),
                Page = page,
                TotalItems = totalItems,
                TotalPages = Convert.ToInt32(Math.Round(totalItems / (double)perPage, MidpointRounding.AwayFromZero))
            });
        }

        [HttpPost]
        public async Task<ActionResult<Transaction>> Post([FromBody] AddTransactionViewModel transactionModel)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var transaction = await _transactionService.SaveTransaction(_user, transactionModel);

            return Ok(AutoMapper.Mapper.Map<TransactionViewModel>(transaction));
        }

        [HttpPut]
        public async Task<ActionResult<Transaction>> Update([FromBody] AddTransactionViewModel transaction)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var updatedTransaction = await _transactionService.UpdateTransaction(_user, transaction);

            if (updatedTransaction == null)
            {
                return NotFound();
            }

            return Ok(AutoMapper.Mapper.Map<TransactionViewModel>(updatedTransaction));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _transactionService.DeleteTransaction(_user.Id, id);
                return NoContent();
            }
            catch (TransactionNotFoundException)
            {
                return NotFound();
            }
        }
    }
}