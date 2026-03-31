using QuotationManagementWebApi.DTOs.Requests;

namespace QuotationManagementWebApi.Application.Commands.Quotes
{
    public class UpdateQuoteCommand
    {
        public int QuoteId { get; set; }
        public UpdateQuotationRequest Request { get; set; } = new();
    }
}