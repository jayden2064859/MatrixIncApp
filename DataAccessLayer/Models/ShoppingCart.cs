using System.Collections.Generic;
using System.Linq;

namespace MatrixInc.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public decimal TotalPrice
        {
            get => Items.Sum(i => i.Price * i.Quantity);
        }

        public int TotalItems => Items.Sum(i => i.Quantity);
    }

    public class CartItem
    {
        public int ItemId { get; set; }
        public string ItemType { get; set; } 
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }
}