using System.ComponentModel.DataAnnotations;

namespace QuotationManagementWebApi.DTOs.Requests
{
    public class CreateQuotationLineItemRequest
    {
        [Required]
        [MaxLength(200)]
        public string ItemDescription { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }

        [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Unit price must be positive.")]
        public decimal UnitPrice { get; set; }

        [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Discount cannot be negative.")]
        public decimal Discount { get; set; }
    }
}