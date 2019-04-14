namespace Panda.API.ViewModels
{
    public class RelationAnalyticsViewModel
    {
        public int LifetimeTotalTransactions { get; set; }
        public decimal LifetimeSumOfTransactions { get; set; }
        public decimal LifetimeAveragePerTransaction { get; set; }
        public int MonthTotalTransactions { get; set; }
        public decimal MonthSumOfTransactions { get; set; }
        public decimal MonthAveragePerTransaction { get; set; }
    }
}
