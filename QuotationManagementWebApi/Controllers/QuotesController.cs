using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuotationManagementWebApi.Application.Commands.Quotes;
using QuotationManagementWebApi.Application.Handlers.Quotes;
using QuotationManagementWebApi.Application.Queries.Quotes;
using QuotationManagementWebApi.Common;
using QuotationManagementWebApi.DTOs.Requests;
using QuotationManagementWebApi.DTOs.Responses;

namespace QuotationManagementWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class QuotesController : ControllerBase
    {
        private readonly GetAllQuotesQueryHandler _getAllQuotesQueryHandler;
        private readonly GetQuoteByIdQueryHandler _getQuoteByIdQueryHandler;
        private readonly CreateQuoteCommandHandler _createQuoteCommandHandler;
        private readonly UpdateQuoteCommandHandler _updateQuoteCommandHandler;
        private readonly DeleteQuoteCommandHandler _deleteQuoteCommandHandler;
        private readonly ChangeQuoteStatusCommandHandler _changeQuoteStatusCommandHandler;

        public QuotesController(
            GetAllQuotesQueryHandler getAllQuotesQueryHandler,
            GetQuoteByIdQueryHandler getQuoteByIdQueryHandler,
            CreateQuoteCommandHandler createQuoteCommandHandler,
            UpdateQuoteCommandHandler updateQuoteCommandHandler,
            DeleteQuoteCommandHandler deleteQuoteCommandHandler,
            ChangeQuoteStatusCommandHandler changeQuoteStatusCommandHandler)
        {
            _getAllQuotesQueryHandler = getAllQuotesQueryHandler;
            _getQuoteByIdQueryHandler = getQuoteByIdQueryHandler;
            _createQuoteCommandHandler = createQuoteCommandHandler;
            _updateQuoteCommandHandler = updateQuoteCommandHandler;
            _deleteQuoteCommandHandler = deleteQuoteCommandHandler;
            _changeQuoteStatusCommandHandler = changeQuoteStatusCommandHandler;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<QuotationSummaryResponse>>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _getAllQuotesQueryHandler.HandleAsync(new GetAllQuotesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            return Ok(ApiResponse<List<QuotationSummaryResponse>>
                .SuccessResponse(result, "Quotations fetched successfully."));
        }

        [Authorize(Roles = "SalesRep,SalesManager,Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<QuotationResponse>>> GetById(int id)
        {
            var result = await _getQuoteByIdQueryHandler.HandleAsync(new GetQuoteByIdQuery
            {
                QuoteId = id
            });

            if (result == null)
            {
                return NotFound(ApiResponse<QuotationResponse>
                    .FailResponse("Quotation not found."));
            }

            return Ok(ApiResponse<QuotationResponse>
                .SuccessResponse(result, "Quotation fetched successfully."));
        }

        [Authorize(Roles = "SalesRep")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CreateQuotationResponse>>> Create([FromBody] CreateQuotationRequest request)
        {
            var result = await _createQuoteCommandHandler.HandleAsync(new CreateQuoteCommand
            {
                Request = request
            });

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.QuoteId },
                ApiResponse<CreateQuotationResponse>
                    .SuccessResponse(result, "Quotation created successfully."));
        }

        [Authorize(Roles = "SalesRep")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(int id, [FromBody] UpdateQuotationRequest request)
        {
            await _updateQuoteCommandHandler.HandleAsync(new UpdateQuoteCommand
            {
                QuoteId = id,
                Request = request
            });

            return Ok(ApiResponse<string>
                .SuccessResponse("Updated", "Quotation updated successfully."));
        }

        [Authorize(Roles = "SalesManager")]
        [HttpPatch("{id}/status")]
        public async Task<ActionResult<ApiResponse<string>>> ChangeStatus(int id, [FromBody] ChangeQuotationStatusRequest request)
        {
            await _changeQuoteStatusCommandHandler.HandleAsync(new ChangeQuoteStatusCommand
            {
                QuoteId = id,
                NewStatus = request.NewStatus
            });

            return Ok(ApiResponse<string>
                .SuccessResponse("Status changed", "Quotation status updated successfully."));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            var deleted = await _deleteQuoteCommandHandler.HandleAsync(new DeleteQuoteCommand
            {
                QuoteId = id
            });

            if (!deleted)
            {
                return NotFound(ApiResponse<string>
                    .FailResponse("Quotation not found."));
            }

            return Ok(ApiResponse<string>
                .SuccessResponse("Deleted", "Quotation deleted successfully."));
        }
    }
}