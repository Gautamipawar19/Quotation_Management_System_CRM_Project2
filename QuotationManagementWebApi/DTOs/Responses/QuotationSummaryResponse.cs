using QuotationManagementWebApi.Entities.Enums;

namespace QuotationManagementWebApi.DTOs.Responses
{
    public class QuotationSummaryResponse
    {
        public int QuoteId { get; set; }
        public string QuoteNumber { get; set; } = string.Empty;
        public QuoteStatus Status { get; set; }
        public decimal GrandTotal { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}