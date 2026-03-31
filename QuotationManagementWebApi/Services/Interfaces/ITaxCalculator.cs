namespace QuotationManagementWebApi.Services.Interfaces
{
    public interface ITaxCalculator
    {
        decimal CalculateTax(decimal amount);
    }
}