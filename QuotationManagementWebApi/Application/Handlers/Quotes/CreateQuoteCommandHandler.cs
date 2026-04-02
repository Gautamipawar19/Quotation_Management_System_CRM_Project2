using QuotationManagementWebApi.Application.Commands.Quotes;
using QuotationManagementWebApi.DTOs.Responses;
using QuotationManagementWebApi.Entities;
using QuotationManagementWebApi.Services.Interfaces;

namespace QuotationManagementWebApi.Application.Handlers.Quotes
{
    public class CreateQuoteCommandHandler
    {
        private readonly IQuotationService _quotationService;
        private readonly GetQuoteAnalyticsQueryHandler _analyticsHandler;

        public CreateQuoteCommandHandler(
            IQuotationService quotationService,
            GetQuoteAnalyticsQueryHandler analyticsHandler)
        {
            _quotationService = quotationService;
            _analyticsHandler = analyticsHandler;
        }

        public async Task<CreateQuotationResponse> HandleAsync(CreateQuoteCommand command)
        {
            var request = command.Request;

            var quotation = new Quotation
            {
                LeadId = request.LeadId,
                CustomerId = request.CustomerId,
                QuoteDate = request.QuoteDate,
                ExpiryDate = request.ExpiryDate,
                DiscountAmount = request.DiscountAmount,
                CreatedBy = request.CreatedBy,
                LineItems = request.LineItems.Select(item => new QuotationLineItem
                {
                    ItemDescription = item.ItemDescription,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount
                }).ToList()
            };

            var quoteId = await _quotationService.CreateQuotationAsync(quotation);
            await _analyticsHandler.InvalidateCacheAsync();

            return new CreateQuotationResponse
            {
                QuoteId = quoteId
            };
        }
    }
}