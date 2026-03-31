using QuotationManagementWebApi.DTOs.Requests;

namespace QuotationManagementWebApi.Application.Commands.Quotes
{
    public class CreateQuoteCommand
    {
        public CreateQuotationRequest Request { get; set; } = new();
    }
}