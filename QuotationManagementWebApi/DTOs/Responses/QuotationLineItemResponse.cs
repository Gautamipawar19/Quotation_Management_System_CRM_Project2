namespace QuotationManagementWebApi.DTOs.Responses
{
    public class QuotationLineItemResponse
    {
        public int LineItemId { get; set; }
        public string ItemDescription { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal LineTotal { get; set; }
    }
}