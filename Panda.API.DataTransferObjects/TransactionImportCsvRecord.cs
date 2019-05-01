using System;

namespace Panda.API.DataTransferObjects
{
    public class TransactionImportCsvRecord
    {
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
    }
}