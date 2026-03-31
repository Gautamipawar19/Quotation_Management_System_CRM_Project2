using QuotationManagementWebApi.Application.Commands.Quotes;
using QuotationManagementWebApi.Services.Interfaces;

namespace QuotationManagementWebApi.Application.Handlers.Quotes
{
    public class ChangeQuoteStatusCommandHandler
    {
        private readonly IQuotationService _quotationService;

        public ChangeQuoteStatusCommandHandler(IQuotationService quotationService)
        {
            _quotationService = quotationService;
        }

        public async Task HandleAsync(ChangeQuoteStatusCommand command)
        {
            await _quotationService.ChangeStatusAsync(command.QuoteId, command.NewStatus);
        }
    }
}