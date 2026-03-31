namespace QuotationManagementWebApi.Services.Interfaces
{
    public interface IQuoteNumberGenerator
    {
        Task<string> GenerateQuoteNumberAsync();
    }
}