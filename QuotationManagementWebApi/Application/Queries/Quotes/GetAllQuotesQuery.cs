namespace QuotationManagementWebApi.Application.Queries.Quotes
{
    public class GetAllQuotesQuery
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}