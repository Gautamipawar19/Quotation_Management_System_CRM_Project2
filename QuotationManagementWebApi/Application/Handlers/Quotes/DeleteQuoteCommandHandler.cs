using QuotationManagementWebApi.Application.Commands.Quotes;
using QuotationManagementWebApi.Services.Interfaces;

namespace QuotationManagementWebApi.Application.Handlers.Quotes
{
    public class DeleteQuoteCommandHandler
    {
        private readonly IQuotationService _quotationService;
        private readonly GetQuoteAnalyticsQueryHandler _analyticsHandler;

        public DeleteQuoteCommandHandler(
            IQuotationService quotationService,
            GetQuoteAnalyticsQueryHandler analyticsHandler)
        {
            _quotationService = quotationService;
            _analyticsHandler = analyticsHandler;
        }

        public async Task<bool> HandleAsync(DeleteQuoteCommand command)
        {
            var deleted = await _quotationService.SoftDeleteAsync(command.QuoteId);

            if (deleted)
            {
                await _analyticsHandler.InvalidateCacheAsync();
            }

            return deleted;
        }
    }
}