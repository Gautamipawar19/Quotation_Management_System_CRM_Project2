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
            return await _context.Quotations
                .AsNoTracking()
                .OrderByDescending(q => q.CreatedDate)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(q => new QuotationSummaryResponse
                {
                    QuoteId = q.QuoteId,
                    QuoteNumber = q.QuoteNumber,
                    Status = q.Status,
                    GrandTotal = q.GrandTotal,
                    CreatedBy = q.CreatedBy,
                    CreatedDate = q.CreatedDate
                })
                .ToListAsync();
        }
    }
}