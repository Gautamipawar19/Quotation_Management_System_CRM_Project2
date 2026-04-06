using QuotationManagementWebApi.Entities.Enums;

namespace QuotationManagementWebApi.DTOs.Responses
{
    public class QuotationResponse
    {
        public int QuoteId { get; set; }
        public string QuoteNumber { get; set; } = string.Empty;
        public int? LeadId { get; set; }
        public int? CustomerId { get; set; }
        public string QuoteDate { get; set; }
        public string ExpiryDate { get; set; }
        public string Status { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedDate { get; set; }
        public int RevisionNumber { get; set; }
        public List<QuotationLineItemResponse> LineItems { get; set; } = new();
    }
}