using QuotationManagementWebApi.Services.Interfaces;

namespace QuotationManagementWebApi.Services.Implementations
{
    public class TaxCalculator : ITaxCalculator
    {
        private const decimal TaxRate = 0.18m;

        public decimal CalculateTax(decimal amount)
        {
            return amount * TaxRate;
        }
    }
}