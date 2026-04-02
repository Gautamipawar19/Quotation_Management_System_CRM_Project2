using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuotationManagementWebApi.Application.Handlers.Quotes;
using QuotationManagementWebApi.Application.Queries.Quotes;
using QuotationManagementWebApi.Common;
using QuotationManagementWebApi.DTOs.Responses;

namespace QuotationManagementWebApi.Controllers
{
    [ApiController]
    [Route("api/quotes/analytics")]
    [Produces("application/json")]
    public class QuoteAnalyticsController : ControllerBase
    {
        private readonly GetQuoteAnalyticsQueryHandler _analyticsHandler;

        public QuoteAnalyticsController(GetQuoteAnalyticsQueryHandler analyticsHandler)
        {
            _analyticsHandler = analyticsHandler;
        }

        [Authorize(Roles = "SalesManager,Admin")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<QuoteAnalyticsResponse>>> GetAnalytics()
        {
            var result = await _analyticsHandler.HandleAsync(new GetQuoteAnalyticsQuery());

            return Ok(ApiResponse<QuoteAnalyticsResponse>
                .SuccessResponse(result, "Quote analytics fetched successfully."));
        }
    }
}