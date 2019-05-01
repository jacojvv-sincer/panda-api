using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Panda.API.Data;
using Panda.API.Data.Models;
using Panda.API.DataTransferObjects;

namespace Panda.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportsController : ControllerBase
    {
        private ApplicationDbContext _context;
        private User _user;

        public ImportsController(ApplicationDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _user = (User)http.HttpContext.Items["ApplicationUser"];
        }


        [HttpPost]
        [Route("UploadCsv")]
        public async Task<IActionResult> UploadCSV(IFormFile file)
        {
            var fileName = file.Name;

            using (StreamReader reader = new StreamReader(file.OpenReadStream()))
            {
                using (CsvReader csv = new CsvReader(reader))
                {
                    TransactionImportCsvRecord record = new TransactionImportCsvRecord();
                    IEnumerable<TransactionImportCsvRecord> records = csv.EnumerateRecords(record);
                    foreach (TransactionImportCsvRecord r in records)
                    {
                        var category = await _context.Categories.Where(c => c.Name == r.Category.Trim()).FirstOrDefaultAsync();
                        if (category == null)
                        {
                            _context.Categories.Add(new Category() { Name = r.Category.Trim() });
                            await _context.SaveChangesAsync();
                            category = await _context.Categories.Where(c => c.Name == r.Category).FirstOrDefaultAsync();
                        }

                        Transaction transaction = new Transaction()
                        {
                            IsExtraneous = false,
                            User = _user,
                            Amount = r.Category == "Income" ? r.Amount : -r.Amount,
                            Date = r.Date,
                            Category = category
                        };

                        _context.Transactions.Add(transaction);
                    }
                    await _context.SaveChangesAsync();
                }
            }

            return Ok();
        }
    }
}