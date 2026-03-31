using System.ComponentModel.DataAnnotations;

namespace QuotationManagementWebApi.DTOs.Requests
{
    public class UpdateQuotationRequest
    {
        public int? LeadId { get; set; }

        public int? CustomerId { get; set; }

        [Required]
        public DateTime QuoteDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
        public decimal DiscountAmount { get; set; }

        [Required]
        [MaxLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        [MinLength(1)]
        public List<CreateQuotationLineItemRequest> LineItems { get; set; } = new();
    }
}