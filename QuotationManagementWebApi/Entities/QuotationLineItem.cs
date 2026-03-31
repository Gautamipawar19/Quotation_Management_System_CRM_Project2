using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuotationManagementWebApi.Entities
{
    public class QuotationLineItem
    {
        [Key]
        public int LineItemId { get; set; }

        [Required]
        public int QuoteId { get; set; }

        [ForeignKey(nameof(QuoteId))]
        public Quotation? Quotation { get; set; }

        [Required]
        [MaxLength(200)]
        public string ItemDescription { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }
    }
}