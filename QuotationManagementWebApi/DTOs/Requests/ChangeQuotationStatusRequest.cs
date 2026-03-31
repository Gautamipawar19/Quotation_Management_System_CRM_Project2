using System.ComponentModel.DataAnnotations;
using QuotationManagementWebApi.Entities.Enums;

namespace QuotationManagementWebApi.DTOs.Requests
{
    public class ChangeQuotationStatusRequest
    {
        [Required]
        public QuoteStatus NewStatus { get; set; }
    }
}