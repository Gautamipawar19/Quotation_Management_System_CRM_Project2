using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using QuotationManagementWebApi.Entities;
using QuotationManagementWebApi.Entities.Enums;
using QuotationManagementWebApi.Infrastructure.Repositories.Interfaces;
using QuotationManagementWebApi.Services.Implementations;
using QuotationManagementWebApi.Services.Interfaces;

namespace QuotationManagementWebApi.Tests.Services
{
    public class QuotationServiceTests
    {
        private readonly Mock<IQuotationRepository> _repositoryMock;
        private readonly Mock<ITaxCalculator> _taxCalculatorMock;
        private readonly Mock<IQuoteNumberGenerator> _quoteNumberGeneratorMock;
        private readonly Mock<IQuoteStatusValidator> _statusValidatorMock;
        private readonly Mock<ILogger<QuotationService>> _loggerMock;
        private readonly QuotationService _service;

        public QuotationServiceTests()
        {
            _repositoryMock = new Mock<IQuotationRepository>();
            _taxCalculatorMock = new Mock<ITaxCalculator>();
            _quoteNumberGeneratorMock = new Mock<IQuoteNumberGenerator>();
            _statusValidatorMock = new Mock<IQuoteStatusValidator>();
            _loggerMock = new Mock<ILogger<QuotationService>>();

            _quoteNumberGeneratorMock
                .Setup(x => x.GenerateQuoteNumberAsync())
                .ReturnsAsync("QT-2026-0001");

            _taxCalculatorMock
                .Setup(x => x.CalculateTax(It.IsAny<decimal>()))
                .Returns<decimal>(amount => Math.Round(amount * 0.18m, 2));

            _service = new QuotationService(
                _repositoryMock.Object,
                _taxCalculatorMock.Object,
                _quoteNumberGeneratorMock.Object,
                _statusValidatorMock.Object,
                _loggerMock.Object);
        }

        private static Quotation CreateValidQuotation()
        {
            return new Quotation
            {
                LeadId = 1,
                CustomerId = null,
                QuoteDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(10),
                DiscountAmount = 100,
                CreatedBy = "sakshi.lahoti",
                LineItems = new List<QuotationLineItem>
                {
                    new QuotationLineItem
                    {
                        ItemDescription = "Laptop",
                        Quantity = 2,
                        UnitPrice = 1000,
                        Discount = 100
                    }
                }
            };
        }

        private static Quotation CreateExistingDraftQuotation()
        {
            return new Quotation
            {
                QuoteId = 1,
                QuoteNumber = "QT-2026-0001",
                LeadId = 1,
                CustomerId = null,
                QuoteDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(10),
                Status = QuoteStatus.Draft,
                DiscountAmount = 100,
                CreatedBy = "sakshi.lahoti",
                CreatedDate = DateTime.UtcNow,
                RevisionNumber = 1,
                LineItems = new List<QuotationLineItem>
                {
                    new QuotationLineItem
                    {
                        ItemDescription = "Laptop",
                        Quantity = 2,
                        UnitPrice = 1000,
                        Discount = 100
                    }
                }
            };
        }

        [Fact]
        public async Task CreateQuotationAsync_ShouldThrow_WhenNoLineItems()
        {
            var quotation = new Quotation
            {
                QuoteDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(1),
                CreatedBy = "sakshi",
                LineItems = new List<QuotationLineItem>()
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateQuotationAsync(quotation));
        }

        [Fact]
        public async Task CreateQuotationAsync_ShouldThrow_WhenExpiryDateIsInvalid()
        {
            var quotation = CreateValidQuotation();
            quotation.ExpiryDate = quotation.QuoteDate.AddDays(-1);

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateQuotationAsync(quotation));
        }

        [Fact]
        public async Task CreateQuotationAsync_ShouldThrow_WhenQuantityIsZero()
        {
            var quotation = CreateValidQuotation();
            quotation.LineItems.First().Quantity = 0;

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateQuotationAsync(quotation));
        }

        [Fact]
        public async Task CreateQuotationAsync_ShouldThrow_WhenUnitPriceIsZero()
        {
            var quotation = CreateValidQuotation();
            quotation.LineItems.First().UnitPrice = 0;

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateQuotationAsync(quotation));
        }

        [Fact]
        public async Task CreateQuotationAsync_ShouldThrow_WhenDiscountIsNegative()
        {
            var quotation = CreateValidQuotation();
            quotation.LineItems.First().Discount = -10;

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateQuotationAsync(quotation));
        }

        [Fact]
        public async Task CreateQuotationAsync_ShouldSetDefaultStatusToDraft()
        {
            var quotation = CreateValidQuotation();

            await _service.CreateQuotationAsync(quotation);

            Assert.Equal(QuoteStatus.Draft, quotation.Status);
        }

        [Fact]
        public async Task CreateQuotationAsync_ShouldSetDefaultFlagsAndRevision()
        {
            var quotation = CreateValidQuotation();

            await _service.CreateQuotationAsync(quotation);

            Assert.False(quotation.IsDeleted);
            Assert.Equal(1, quotation.RevisionNumber);
            Assert.NotEqual(default, quotation.CreatedDate);
        }

        [Fact]
        public async Task CreateQuotationAsync_ShouldGenerateQuoteNumber()
        {
            var quotation = CreateValidQuotation();

            await _service.CreateQuotationAsync(quotation);

            Assert.Equal("QT-2026-0001", quotation.QuoteNumber);
        }

        [Fact]
        public async Task CreateQuotationAsync_ShouldCalculateTotalsCorrectly()
        {
            var quotation = CreateValidQuotation();

            await _service.CreateQuotationAsync(quotation);

            Assert.Equal(1900, quotation.SubTotal);
            Assert.Equal(342, quotation.TaxAmount);
            Assert.Equal(2142, quotation.GrandTotal);
        }

        [Fact]
        public async Task CreateQuotationAsync_ShouldCallRepositoryAddAndSave()
        {
            var quotation = CreateValidQuotation();

            await _service.CreateQuotationAsync(quotation);

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Quotation>()), Times.Once);
            _repositoryMock.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllQuotations()
        {
            var quotations = new List<Quotation>
            {
                new() { QuoteId = 1, QuoteNumber = "QT-2026-0001" },
                new() { QuoteId = 2, QuoteNumber = "QT-2026-0002" }
            };

            _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(quotations);

            var result = await _service.GetAllAsync();

            Assert.Equal(2, result.Count);
            Assert.Equal("QT-2026-0001", result[0].QuoteNumber);
            Assert.Equal("QT-2026-0002", result[1].QuoteNumber);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnQuotation_WhenFound()
        {
            var quotation = new Quotation
            {
                QuoteId = 1,
                QuoteNumber = "QT-2026-0001"
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(quotation);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result!.QuoteId);
            Assert.Equal("QT-2026-0001", result.QuoteNumber);
        }

        [Fact]
        public async Task UpdateQuotationAsync_ShouldThrow_WhenQuotationNotFound()
        {
            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Quotation?)null);

            var quotation = CreateValidQuotation();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateQuotationAsync(1, quotation));
        }

        [Fact]
        public async Task UpdateQuotationAsync_ShouldThrow_WhenStatusIsNotDraft()
        {
            var existing = new Quotation
            {
                QuoteId = 1,
                Status = QuoteStatus.Sent
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existing);

            var quotation = CreateValidQuotation();

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateQuotationAsync(1, quotation));
        }

        [Fact]
        public async Task UpdateQuotationAsync_ShouldThrow_WhenExpiryDateIsInvalid()
        {
            var existing = CreateExistingDraftQuotation();
            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existing);

            var updatedQuotation = CreateValidQuotation();
            updatedQuotation.ExpiryDate = updatedQuotation.QuoteDate.AddDays(-1);

            await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateQuotationAsync(1, updatedQuotation));
        }

        [Fact]
        public async Task UpdateQuotationAsync_ShouldIncrementRevisionAndRecalculateTotals()
        {
            var existing = CreateExistingDraftQuotation();
            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existing);

            var updatedQuotation = new Quotation
            {
                LeadId = 2,
                CustomerId = null,
                QuoteDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(20),
                DiscountAmount = 50,
                CreatedBy = "sales.rep1",
                LineItems = new List<QuotationLineItem>
                {
                    new QuotationLineItem
                    {
                        ItemDescription = "Keyboard",
                        Quantity = 3,
                        UnitPrice = 1500,
                        Discount = 150
                    }
                }
            };

            await _service.UpdateQuotationAsync(1, updatedQuotation);

            Assert.Equal(2, existing.RevisionNumber);
            Assert.Equal(4350, existing.SubTotal);
            Assert.Equal(783, existing.TaxAmount);
            Assert.Equal(5083, existing.GrandTotal);
            _repositoryMock.Verify(x => x.Update(existing), Times.Once);
            _repositoryMock.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task ChangeStatusAsync_ShouldThrow_WhenQuotationNotFound()
        {
            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Quotation?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ChangeStatusAsync(1, QuoteStatus.Sent));
        }

        [Fact]
        public async Task ChangeStatusAsync_ShouldThrow_WhenTransitionIsInvalid()
        {
            var quotation = new Quotation
            {
                QuoteId = 1,
                Status = QuoteStatus.Sent
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(quotation);
            _statusValidatorMock.Setup(x => x.CanTransition(QuoteStatus.Sent, QuoteStatus.Sent)).Returns(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ChangeStatusAsync(1, QuoteStatus.Sent));
        }

        [Fact]
        public async Task ChangeStatusAsync_ShouldThrow_WhenExpiredQuotationIsAccepted()
        {
            var quotation = new Quotation
            {
                QuoteId = 1,
                Status = QuoteStatus.Expired
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(quotation);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ChangeStatusAsync(1, QuoteStatus.Accepted));
        }

        [Fact]
        public async Task ChangeStatusAsync_ShouldUpdateStatus_WhenTransitionIsValid()
        {
            var quotation = new Quotation
            {
                QuoteId = 1,
                Status = QuoteStatus.Draft
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(quotation);
            _statusValidatorMock.Setup(x => x.CanTransition(QuoteStatus.Draft, QuoteStatus.Sent)).Returns(true);

            await _service.ChangeStatusAsync(1, QuoteStatus.Sent);

            Assert.Equal(QuoteStatus.Sent, quotation.Status);
            _repositoryMock.Verify(x => x.Update(quotation), Times.Once);
            _repositoryMock.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task SoftDeleteAsync_ShouldReturnFalse_WhenQuotationNotFound()
        {
            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Quotation?)null);

            var result = await _service.SoftDeleteAsync(1);

            Assert.False(result);
        }

        [Fact]
        public async Task SoftDeleteAsync_ShouldThrow_WhenQuotationIsAccepted()
        {
            var quotation = new Quotation
            {
                QuoteId = 1,
                Status = QuoteStatus.Accepted
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(quotation);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.SoftDeleteAsync(1));
        }

        [Fact]
        public async Task SoftDeleteAsync_ShouldSetIsDeletedTrue_WhenValid()
        {
            var quotation = new Quotation
            {
                QuoteId = 1,
                Status = QuoteStatus.Draft
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(quotation);

            var result = await _service.SoftDeleteAsync(1);

            Assert.True(result);
            Assert.True(quotation.IsDeleted);
            _repositoryMock.Verify(x => x.Update(quotation), Times.Once);
            _repositoryMock.Verify(x => x.SaveAsync(), Times.Once);
        }
    }
}