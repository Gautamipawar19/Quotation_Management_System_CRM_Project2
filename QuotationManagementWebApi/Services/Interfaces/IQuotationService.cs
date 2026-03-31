using QuotationManagementWebApi.Entities;
using QuotationManagementWebApi.Entities.Enums;

namespace QuotationManagementWebApi.Services.Interfaces
{
    public interface IQuotationService
    {
        Task<int> CreateQuotationAsync(Quotation quotation);
        Task<List<Quotation>> GetAllAsync();
        Task<Quotation?> GetByIdAsync(int id);
        Task UpdateQuotationAsync(int id, Quotation quotation);
        Task ChangeStatusAsync(int id, QuoteStatus newStatus);
        Task<bool> SoftDeleteAsync(int id);
    }
}