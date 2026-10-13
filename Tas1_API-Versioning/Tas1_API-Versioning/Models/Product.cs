namespace Tas1_API_Versioning.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string CategoryName { get; set; } = string.Empty;
    }
}
