namespace DomainModel
{
    public class ShoppingItem
    {
        public int ShoppingItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? BarCode { get; set; }
    }
}
