using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrdersApi.Models
{
    public sealed class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; }

        [Required]
        public string Name { get; private set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; private set; }

        private Item() { }
        private Item(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public Item CreateItem(string name, decimal price)
        {
            return new Item(name, price);
        }
    }
}
