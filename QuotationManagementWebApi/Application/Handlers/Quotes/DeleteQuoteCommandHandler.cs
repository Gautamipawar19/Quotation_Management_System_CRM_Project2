using QuotationManagementWebApi.Application.Commands.Quotes;
using QuotationManagementWebApi.Services.Interfaces;

namespace QuotationManagementWebApi.Application.Handlers.Quotes
{
    public class DeleteQuoteCommandHandler
    {
        private readonly IQuotationService _quotationService;

        public DeleteQuoteCommandHandler(IQuotationService quotationService)
        {
            _quotationService = quotationService;
        }

        public async Task<bool> HandleAsync(DeleteQuoteCommand command)
        {
            return await _quotationService.SoftDeleteAsync(command.QuoteId);
        }
    }
}