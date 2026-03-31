using Microsoft.EntityFrameworkCore;
using QuotationManagementWebApi.Infrastructure.Data;
using QuotationManagementWebApi.Services.Interfaces;

namespace QuotationManagementWebApi.Services.Implementations
{
    public class QuoteNumberGenerator : IQuoteNumberGenerator
    {
        private readonly QuotationDbContext _context;

        public QuoteNumberGenerator(QuotationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateQuoteNumberAsync()
        {
            var year = DateTime.UtcNow.Year;
            var prefix = $"QT-{year}-";

            var lastQuote = await _context.Quotations
                .IgnoreQueryFilters()
                .Where(q => q.QuoteNumber.StartsWith(prefix))
                .OrderByDescending(q => q.QuoteId)
                .Select(q => q.QuoteNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (!string.IsNullOrWhiteSpace(lastQuote))
            {
                var lastSequencePart = lastQuote.Split('-').Last();
                if (int.TryParse(lastSequencePart, out int lastSequence))
                {
                    nextNumber = lastSequence + 1;
                }
            }

            return $"{prefix}{nextNumber:D4}";
        }
    }
}