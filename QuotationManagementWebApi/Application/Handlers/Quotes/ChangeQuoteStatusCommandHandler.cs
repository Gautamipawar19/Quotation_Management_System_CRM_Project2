using QuotationManagementWebApi.Application.Commands.Quotes;
using QuotationManagementWebApi.Services.Interfaces;

namespace QuotationManagementWebApi.Application.Handlers.Quotes
{
    public class ChangeQuoteStatusCommandHandler
    {
        private readonly IQuotationService _quotationService;
        private readonly GetQuoteAnalyticsQueryHandler _analyticsHandler;

        public ChangeQuoteStatusCommandHandler(
            IQuotationService quotationService,
            GetQuoteAnalyticsQueryHandler analyticsHandler)
        {
            _quotationService = quotationService;
            _analyticsHandler = analyticsHandler;
        }

        public async Task HandleAsync(ChangeQuoteStatusCommand command)
        {
            await _quotationService.ChangeStatusAsync(command.QuoteId, command.NewStatus);
            await _analyticsHandler.InvalidateCacheAsync();
        }
    }
}