using Microsoft.EntityFrameworkCore;
using QuotationManagementWebApi.Application.Queries.Quotes;
using QuotationManagementWebApi.DTOs.Responses;
using QuotationManagementWebApi.Infrastructure.Data;

namespace QuotationManagementWebApi.Application.Handlers.Quotes
{
    public class GetQuoteByIdQueryHandler
    {
        private readonly QuotationDbContext _context;

        public GetQuoteByIdQueryHandler(QuotationDbContext context)
        {
            _context = context;
        }

        public async Task<QuotationResponse?> HandleAsync(GetQuoteByIdQuery query)
        {
            var q = await _context.Quotations
                .AsNoTracking()
                .Include(x => x.LineItems)
                .FirstOrDefaultAsync(x => x.QuoteId == query.QuoteId);

            if (q == null)
                return null;

            return new QuotationResponse
            {
                QuoteId = q.QuoteId,
                QuoteNumber = q.QuoteNumber,
                LeadId = q.LeadId,
                CustomerId = q.CustomerId,
                QuoteDate = q.QuoteDate.ToString("yyyy-MM-dd"),
                ExpiryDate = q.ExpiryDate.ToString("yyyy-MM-dd"),
                Status = q.Status.ToString(),
                SubTotal = q.SubTotal,
                TaxAmount = q.TaxAmount,
                DiscountAmount = q.DiscountAmount,
                GrandTotal = q.GrandTotal,
                CreatedBy = q.CreatedBy,
                CreatedDate = q.CreatedDate.ToString("yyyy-MM-dd"),
                RevisionNumber = q.RevisionNumber,
                LineItems = q.LineItems.Select(li => new QuotationLineItemResponse
                {
                    LineItemId = li.LineItemId,
                    ItemDescription = li.ItemDescription,
                    Quantity = li.Quantity,
                    UnitPrice = li.UnitPrice,
                    Discount = li.Discount,
                    LineTotal = li.LineTotal
                }).ToList()
            };
        }
    }
}