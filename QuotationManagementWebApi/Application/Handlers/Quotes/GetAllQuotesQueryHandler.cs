using Microsoft.EntityFrameworkCore;
using QuotationManagementWebApi.Application.Queries.Quotes;
using QuotationManagementWebApi.DTOs.Responses;
using QuotationManagementWebApi.Infrastructure.Data;

namespace QuotationManagementWebApi.Application.Handlers.Quotes
{
    public class GetAllQuotesQueryHandler
    {
        private readonly QuotationDbContext _context;

        public GetAllQuotesQueryHandler(QuotationDbContext context)
        {
            _context = context;
        }

        public async Task<List<QuotationSummaryResponse>> HandleAsync(GetAllQuotesQuery query)
        {
            var quotations = await _context.Quotations
                .AsNoTracking()
                .OrderByDescending(q => q.CreatedDate)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return quotations.Select(q => new QuotationSummaryResponse
            {
                QuoteId = q.QuoteId,
                QuoteNumber = q.QuoteNumber,
                Status = q.Status.ToString(),
                GrandTotal = q.GrandTotal,
                CreatedBy = q.CreatedBy,
                CreatedDate = q.CreatedDate.ToString("yyyy-MM-dd")
            }).ToList();
        }
    }
}