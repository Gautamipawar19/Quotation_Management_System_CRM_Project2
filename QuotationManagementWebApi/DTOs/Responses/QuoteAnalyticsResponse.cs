namespace QuotationManagementWebApi.DTOs.Responses
{
    public class QuoteAnalyticsResponse
    {
        public int TotalQuotes { get; set; }
        public int AcceptedQuotes { get; set; }
        public int RejectedQuotes { get; set; }
        public int PendingQuotes { get; set; }
        public decimal SuccessRate { get; set; }
        public decimal AverageQuoteValue { get; set; }
    }
}