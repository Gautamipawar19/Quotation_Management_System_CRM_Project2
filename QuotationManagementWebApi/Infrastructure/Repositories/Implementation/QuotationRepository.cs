using Microsoft.EntityFrameworkCore;
using QuotationManagementWebApi.Entities;
using QuotationManagementWebApi.Infrastructure.Data;
using QuotationManagementWebApi.Infrastructure.Repositories.Interfaces;

namespace QuotationManagementWebApi.Infrastructure.Repositories.Implementation
{
    public class QuotationRepository : IQuotationRepository
    {
        private readonly QuotationDbContext _context;

        public QuotationRepository(QuotationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Quotation quotation)
        {
            await _context.Quotations.AddAsync(quotation);
        }

        public async Task<List<Quotation>> GetAllAsync()
        {
            return await _context.Quotations
                .AsNoTracking()
                .Include(q => q.LineItems)
                .ToListAsync();
        }

        public async Task<Quotation?> GetByIdAsync(int id)
        {
            return await _context.Quotations
                .Include(q => q.LineItems)
                .FirstOrDefaultAsync(q => q.QuoteId == id);
        }

        public void Update(Quotation quotation)
        {
            _context.Quotations.Update(quotation);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}