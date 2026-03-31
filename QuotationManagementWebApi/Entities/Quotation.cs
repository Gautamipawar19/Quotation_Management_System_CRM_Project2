using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuotationManagementWebApi.Entities.Enums;

namespace QuotationManagementWebApi.Entities
{
    public class Quotation
    {
        [Key]
        public int QuoteId { get; set; }

        [Required]
        [MaxLength(20)]
        public string QuoteNumber { get; set; } = string.Empty;

        public int? LeadId { get; set; }

        public int? CustomerId { get; set; }

        [Required]
        public DateTime QuoteDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public QuoteStatus Status { get; set; } = QuoteStatus.Draft;

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GrandTotal { get; set; }

        [Required]
        [MaxLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int RevisionNumber { get; set; } = 1;

        public bool IsDeleted { get; set; } = false;

        public ICollection<QuotationLineItem> LineItems { get; set; }
            = new List<QuotationLineItem>();
    }
}