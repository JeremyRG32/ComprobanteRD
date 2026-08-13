namespace ComprobanteRDAPI.Models
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public decimal MonthlyPrice { get; set; }
        public string? Description { get; set; }
        public int MonthlyTransactionLimit { get; set; }

    }
}
