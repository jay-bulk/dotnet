// Demo 7 - Shopping Cart; LV
// Defines a shopping cart item object

using System.ComponentModel.DataAnnotations;

namespace Demo7.Models
{
    public class CartItem
    {
        public Product? Product { get; set; }

        [Required(ErrorMessage = "Please enter quantity")]
        [Range(1, 20, ErrorMessage = "Please enter an amount between 1 and 20")]
        public int Quantity { get; set; }
    }
}
