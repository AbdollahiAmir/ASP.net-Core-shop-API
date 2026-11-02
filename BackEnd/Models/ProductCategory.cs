using System.Text.Json.Serialization;

namespace BackEnd.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public string? ImgPath { get; set; }


        [JsonIgnore]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
