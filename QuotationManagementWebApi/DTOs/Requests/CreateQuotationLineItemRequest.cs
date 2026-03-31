using System.ComponentModel.DataAnnotations;

namespace QuotationManagementWebApi.DTOs.Requests
{
    public class CreateQuotationLineItemRequest
    {
        [Required]
        [MaxLength(200)]
        public string ItemDescription { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal UnitPrice { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
        public decimal Discount { get; set; }
    }
}