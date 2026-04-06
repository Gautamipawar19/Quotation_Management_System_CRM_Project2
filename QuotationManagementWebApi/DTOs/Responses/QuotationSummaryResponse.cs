using QuotationManagementWebApi.Entities.Enums;

namespace QuotationManagementWebApi.DTOs.Responses
{
    public class QuotationSummaryResponse
    {
        public int QuoteId { get; set; }
        public string QuoteNumber { get; set; } = string.Empty;
        public int? LeadId { get; set; }
        public int? CustomerId { get; set; }
        public string Status { get; set; }
        public decimal GrandTotal { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedDate { get; set; }
    }
}