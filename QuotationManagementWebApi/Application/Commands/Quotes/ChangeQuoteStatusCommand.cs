using QuotationManagementWebApi.Entities.Enums;

namespace QuotationManagementWebApi.Application.Commands.Quotes
{
    public class ChangeQuoteStatusCommand
    {
        public int QuoteId { get; set; }
        public QuoteStatus NewStatus { get; set; }
    }
}