using QuotationManagementWebApi.Entities.Enums;
using QuotationManagementWebApi.Services.Interfaces;

namespace QuotationManagementWebApi.Services.Implementations
{
    public class QuoteStatusValidator : IQuoteStatusValidator
    {
        private readonly Dictionary<QuoteStatus, List<QuoteStatus>> _allowedTransitions =
            new()
            {
                { QuoteStatus.Draft, new() { QuoteStatus.Sent } },
                { QuoteStatus.Sent, new() { QuoteStatus.Viewed, QuoteStatus.Expired } },
                { QuoteStatus.Viewed, new() { QuoteStatus.Accepted, QuoteStatus.Rejected } },
                { QuoteStatus.Accepted, new() },
                { QuoteStatus.Rejected, new() },
                { QuoteStatus.Expired, new() }
            };

        public bool CanTransition(QuoteStatus currentStatus, QuoteStatus nextStatus)
        {
            return _allowedTransitions.ContainsKey(currentStatus)
                   && _allowedTransitions[currentStatus].Contains(nextStatus);
        }
    }
}