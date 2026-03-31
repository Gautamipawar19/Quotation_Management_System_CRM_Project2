using QuotationManagementWebApi.Entities.Enums;

namespace QuotationManagementWebApi.Services.Interfaces
{
    public interface IQuoteStatusValidator
    {
        bool CanTransition(QuoteStatus currentStatus, QuoteStatus nextStatus);
    }
}