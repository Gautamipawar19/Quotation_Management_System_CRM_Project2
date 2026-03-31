using System.ComponentModel.DataAnnotations;

namespace QuotationManagementWebApi.Entities
{
    public class QuotationTemplate
    {
        [Key]
        public int TemplateId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TemplateName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        public string CompanyAddress { get; set; } = string.Empty;

        [Required]
        public string TermsAndConditions { get; set; } = string.Empty;

        public ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
    }
}