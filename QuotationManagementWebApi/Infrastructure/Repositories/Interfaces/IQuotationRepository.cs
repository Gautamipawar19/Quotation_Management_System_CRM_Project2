using QuotationManagementWebApi.Entities;

namespace QuotationManagementWebApi.Infrastructure.Repositories.Interfaces
{
    public interface IQuotationRepository
    {
        Task<List<Quotation>> GetAllAsync();
        Task<Quotation?> GetByIdAsync(int id);
        Task AddAsync(Quotation quotation);
        void Update(Quotation quotation);
        Task SaveAsync();
    }
}