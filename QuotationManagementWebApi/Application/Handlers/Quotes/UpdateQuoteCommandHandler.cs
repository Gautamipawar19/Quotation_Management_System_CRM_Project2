using QuotationManagementWebApi.Application.Commands.Quotes;
using QuotationManagementWebApi.Entities;
using QuotationManagementWebApi.Services.Interfaces;

namespace QuotationManagementWebApi.Application.Handlers.Quotes
{
    public class UpdateQuoteCommandHandler
    {
        private readonly IQuotationService _quotationService;

        public UpdateQuoteCommandHandler(IQuotationService quotationService)
        {
            _quotationService = quotationService;
        }

        public async Task HandleAsync(UpdateQuoteCommand command)
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

            await _quotationService.UpdateQuotationAsync(command.QuoteId, quotation);
        }
    }
}