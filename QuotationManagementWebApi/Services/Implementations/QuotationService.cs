using Microsoft.Extensions.Logging;
using QuotationManagementWebApi.Entities;
using QuotationManagementWebApi.Entities.Enums;
using QuotationManagementWebApi.Infrastructure.Repositories.Interfaces;
using QuotationManagementWebApi.Services.Interfaces;

namespace QuotationManagementWebApi.Services.Implementations
{
    public class QuotationService : IQuotationService
    {
        private readonly IQuotationRepository _repository;
        private readonly ITaxCalculator _taxCalculator;
        private readonly IQuoteNumberGenerator _quoteNumberGenerator;
        private readonly IQuoteStatusValidator _statusValidator;
        private readonly ILogger<QuotationService> _logger;

        public QuotationService(
            IQuotationRepository repository,
            ITaxCalculator taxCalculator,
            IQuoteNumberGenerator quoteNumberGenerator,
            IQuoteStatusValidator statusValidator,
            ILogger<QuotationService> logger)
        {
            _repository = repository;
            _taxCalculator = taxCalculator;
            _quoteNumberGenerator = quoteNumberGenerator;
            _statusValidator = statusValidator;
            _logger = logger;
        }

        public async Task<int> CreateQuotationAsync(Quotation quotation)
        {
            _logger.LogInformation(
                "Starting quotation creation for user {CreatedBy}. QuoteDate: {QuoteDate}, ExpiryDate: {ExpiryDate}",
                quotation.CreatedBy,
                quotation.QuoteDate,
                quotation.ExpiryDate);

            if (quotation.LineItems == null || !quotation.LineItems.Any())
            {
                _logger.LogWarning(
                    "Quotation creation failed for user {CreatedBy} because no line items were provided.",
                    quotation.CreatedBy);

                throw new ArgumentException("Quotation must contain at least one line item.");
            }

            if (quotation.ExpiryDate <= quotation.QuoteDate)
            {
                _logger.LogWarning(
                    "Quotation creation failed for user {CreatedBy} because ExpiryDate {ExpiryDate} is not greater than QuoteDate {QuoteDate}.",
                    quotation.CreatedBy,
                    quotation.ExpiryDate,
                    quotation.QuoteDate);

                throw new ArgumentException("Expiry date must be greater than quote date.");
            }

            quotation.CreatedDate = DateTime.UtcNow;
            quotation.Status = QuoteStatus.Draft;
            quotation.IsDeleted = false;
            quotation.RevisionNumber = 1;
            quotation.QuoteNumber = await _quoteNumberGenerator.GenerateQuoteNumberAsync();

            _logger.LogInformation(
                "Generated quote number {QuoteNumber} for user {CreatedBy}.",
                quotation.QuoteNumber,
                quotation.CreatedBy);

            RecalculateTotals(quotation);

            await _repository.AddAsync(quotation);
            await _repository.SaveAsync();

            _logger.LogInformation(
                "Quotation created successfully. QuoteId: {QuoteId}, QuoteNumber: {QuoteNumber}, GrandTotal: {GrandTotal}",
                quotation.QuoteId,
                quotation.QuoteNumber,
                quotation.GrandTotal);

            return quotation.QuoteId;
        }

        public async Task<List<Quotation>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all active quotations.");

            var quotations = await _repository.GetAllAsync();

            _logger.LogInformation("Fetched {Count} quotations.", quotations.Count);

            return quotations;
        }

        public async Task<Quotation?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching quotation with QuoteId {QuoteId}.", id);

            var quotation = await _repository.GetByIdAsync(id);

            if (quotation == null)
            {
                _logger.LogWarning("Quotation with QuoteId {QuoteId} was not found.", id);
                return null;
            }

            _logger.LogInformation(
                "Quotation with QuoteId {QuoteId} fetched successfully. Current Status: {Status}",
                id,
                quotation.Status);

            return quotation;
        }

        public async Task UpdateQuotationAsync(int id, Quotation quotation)
        {
            _logger.LogInformation("Starting update for quotation with QuoteId {QuoteId}.", id);

            var existingQuotation = await _repository.GetByIdAsync(id);

            if (existingQuotation == null)
            {
                _logger.LogWarning("Quotation update failed because QuoteId {QuoteId} was not found.", id);
                throw new KeyNotFoundException("Quotation not found.");
            }

            if (existingQuotation.Status != QuoteStatus.Draft)
            {
                _logger.LogWarning(
                    "Quotation update failed for QuoteId {QuoteId}. Only Draft quotations can be edited. Current Status: {Status}",
                    id,
                    existingQuotation.Status);

                throw new InvalidOperationException("Only draft quotations can be edited.");
            }

            if (quotation.ExpiryDate <= quotation.QuoteDate)
            {
                _logger.LogWarning(
                    "Quotation update failed for QuoteId {QuoteId}. ExpiryDate {ExpiryDate} is not greater than QuoteDate {QuoteDate}.",
                    id,
                    quotation.ExpiryDate,
                    quotation.QuoteDate);

                throw new ArgumentException("Expiry date must be greater than quote date.");
            }

            existingQuotation.LeadId = quotation.LeadId;
            existingQuotation.CustomerId = quotation.CustomerId;
            existingQuotation.QuoteDate = quotation.QuoteDate;
            existingQuotation.ExpiryDate = quotation.ExpiryDate;
            existingQuotation.DiscountAmount = quotation.DiscountAmount;
            existingQuotation.CreatedBy = quotation.CreatedBy;
            existingQuotation.LineItems = quotation.LineItems;
            existingQuotation.RevisionNumber += 1;

            RecalculateTotals(existingQuotation);

            _repository.Update(existingQuotation);
            await _repository.SaveAsync();

            _logger.LogInformation(
                "Quotation updated successfully. QuoteId: {QuoteId}, RevisionNumber: {RevisionNumber}, GrandTotal: {GrandTotal}",
                existingQuotation.QuoteId,
                existingQuotation.RevisionNumber,
                existingQuotation.GrandTotal);
        }

        public async Task ChangeStatusAsync(int id, QuoteStatus newStatus)
        {
            _logger.LogInformation(
                "Starting status change for QuoteId {QuoteId}. Requested new status: {NewStatus}",
                id,
                newStatus);

            var quotation = await _repository.GetByIdAsync(id);

            if (quotation == null)
            {
                _logger.LogWarning("Status change failed because QuoteId {QuoteId} was not found.", id);
                throw new KeyNotFoundException("Quotation not found.");
            }

            var oldStatus = quotation.Status;

            if (quotation.Status == QuoteStatus.Expired && newStatus == QuoteStatus.Accepted)
            {
                _logger.LogWarning(
                    "Invalid status change attempted for QuoteId {QuoteId}. Cannot change from {OldStatus} to {NewStatus}.",
                    id,
                    oldStatus,
                    newStatus);

                throw new InvalidOperationException("Expired quotation cannot be accepted.");
            }

            if (!_statusValidator.CanTransition(quotation.Status, newStatus))
            {
                _logger.LogWarning(
                    "Invalid status transition attempted for QuoteId {QuoteId}. From {OldStatus} to {NewStatus}.",
                    id,
                    oldStatus,
                    newStatus);

                throw new InvalidOperationException(
                    $"Cannot change status from {quotation.Status} to {newStatus}.");
            }

            quotation.Status = newStatus;

            _repository.Update(quotation);
            await _repository.SaveAsync();

            _logger.LogInformation(
                "Status changed successfully for QuoteId {QuoteId}. OldStatus: {OldStatus}, NewStatus: {NewStatus}",
                quotation.QuoteId,
                oldStatus,
                newStatus);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            _logger.LogInformation("Starting soft delete for QuoteId {QuoteId}.", id);

            var quotation = await _repository.GetByIdAsync(id);

            if (quotation == null)
            {
                _logger.LogWarning("Soft delete failed because QuoteId {QuoteId} was not found.", id);
                return false;
            }

            if (quotation.Status == QuoteStatus.Accepted)
            {
                _logger.LogWarning(
                    "Soft delete failed for QuoteId {QuoteId}. Accepted quotations cannot be deleted.",
                    id);

                throw new InvalidOperationException("Accepted quotations cannot be deleted.");
            }

            quotation.IsDeleted = true;

            _repository.Update(quotation);
            await _repository.SaveAsync();

            _logger.LogInformation("Quotation soft deleted successfully. QuoteId: {QuoteId}", id);

            return true;
        }

        private void RecalculateTotals(Quotation quotation)
        {
            _logger.LogInformation(
                "Recalculating totals for QuoteNumber {QuoteNumber}. LineItemCount: {LineItemCount}",
                quotation.QuoteNumber,
                quotation.LineItems?.Count ?? 0);

            decimal subTotal = 0;

            foreach (var item in quotation.LineItems)
            {
                if (item.Quantity <= 0)
                {
                    _logger.LogError(
                        "Calculation error in QuoteNumber {QuoteNumber}. Invalid quantity {Quantity} for item {ItemDescription}.",
                        quotation.QuoteNumber,
                        item.Quantity,
                        item.ItemDescription);

                    throw new ArgumentException("Quantity must be greater than zero.");
                }

                if (item.UnitPrice <= 0)
                {
                    _logger.LogError(
                        "Calculation error in QuoteNumber {QuoteNumber}. Invalid unit price {UnitPrice} for item {ItemDescription}.",
                        quotation.QuoteNumber,
                        item.UnitPrice,
                        item.ItemDescription);

                    throw new ArgumentException("Unit price must be positive.");
                }

                if (item.Discount < 0)
                {
                    _logger.LogError(
                        "Calculation error in QuoteNumber {QuoteNumber}. Negative discount {Discount} for item {ItemDescription}.",
                        quotation.QuoteNumber,
                        item.Discount,
                        item.ItemDescription);

                    throw new ArgumentException("Discount cannot be negative.");
                }

                item.LineTotal = (item.Quantity * item.UnitPrice) - item.Discount;
                subTotal += item.LineTotal;
            }

            quotation.SubTotal = subTotal;
            quotation.TaxAmount = _taxCalculator.CalculateTax(subTotal);
            quotation.GrandTotal = quotation.SubTotal + quotation.TaxAmount - quotation.DiscountAmount;

            _logger.LogInformation(
                "Totals recalculated successfully for QuoteNumber {QuoteNumber}. SubTotal: {SubTotal}, TaxAmount: {TaxAmount}, DiscountAmount: {DiscountAmount}, GrandTotal: {GrandTotal}",
                quotation.QuoteNumber,
                quotation.SubTotal,
                quotation.TaxAmount,
                quotation.DiscountAmount,
                quotation.GrandTotal);
        }
    }
}