using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using QuotationManagementWebApi.Application.Queries.Quotes;
using QuotationManagementWebApi.DTOs.Responses;
using QuotationManagementWebApi.Entities.Enums;
using QuotationManagementWebApi.Infrastructure.Data;
using System.Text.Json;

namespace QuotationManagementWebApi.Application.Handlers.Quotes
{
    public class GetQuoteAnalyticsQueryHandler
    {
        private const string CacheKey = "quote_analytics";
        private readonly QuotationDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<GetQuoteAnalyticsQueryHandler> _logger;

        public GetQuoteAnalyticsQueryHandler(
            QuotationDbContext context,
            IDistributedCache cache,
            ILogger<GetQuoteAnalyticsQueryHandler> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<QuoteAnalyticsResponse> HandleAsync(GetQuoteAnalyticsQuery query)
        {
            try
            {
                var cachedData = await _cache.GetStringAsync(CacheKey);

                if (!string.IsNullOrWhiteSpace(cachedData))
                {
                    _logger.LogInformation("Returning analytics from Redis cache.");
                    return JsonSerializer.Deserialize<QuoteAnalyticsResponse>(cachedData)!;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis read failed. Fetching analytics from database.");
            }

            _logger.LogInformation("Fetching analytics from database.");

            var quotations = await _context.Quotations
                .AsNoTracking()
                .ToListAsync();

            var totalQuotes = quotations.Count;
            var acceptedQuotes = quotations.Count(q => q.Status == QuoteStatus.Accepted);
            var rejectedQuotes = quotations.Count(q => q.Status == QuoteStatus.Rejected);
            var pendingQuotes = quotations.Count(q =>
                q.Status == QuoteStatus.Draft ||
                q.Status == QuoteStatus.Sent ||
                q.Status == QuoteStatus.Viewed);

            var successRate = totalQuotes == 0
                ? 0
                : Math.Round((decimal)acceptedQuotes / totalQuotes * 100, 2);

            var averageQuoteValue = totalQuotes == 0
                ? 0
                : Math.Round(quotations.Average(q => q.GrandTotal), 2);

            var response = new QuoteAnalyticsResponse
            {
                TotalQuotes = totalQuotes,
                AcceptedQuotes = acceptedQuotes,
                RejectedQuotes = rejectedQuotes,
                PendingQuotes = pendingQuotes,
                SuccessRate = successRate,
                AverageQuoteValue = averageQuoteValue
            };

            try
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };

                await _cache.SetStringAsync(
                    CacheKey,
                    JsonSerializer.Serialize(response),
                    cacheOptions);

                _logger.LogInformation("Analytics cached successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis write failed. Returning database analytics without cache.");
            }

            return response;
        }

        public async Task InvalidateCacheAsync()
        {
            try
            {
                await _cache.RemoveAsync(CacheKey);
                _logger.LogInformation("Quote analytics cache invalidated.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis cache invalidation failed.");
            }
        }
    }
}