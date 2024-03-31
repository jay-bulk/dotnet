// Demo 7 - Shopping Cart; LV
// Defines a shopping cart object

// add this namespace

using Newtonsoft.Json;

namespace Demo7.Models
{
    public class Cart
    {
        // a list collection to hold cart item objects

        [JsonProperty]
        private List<CartItem> cartItems = new();

        // setting the maximum order quantity to 20

        const int MaxQuantity = 20;

        public CartItem? GetCartItem(int? productPK)
        {
            CartItem? aItem = cartItems.Where(p => p.Product?.ProductPK == productPK).FirstOrDefault();

            return aItem;
        }

        public void AddItem(Product aProduct)
        {
            CartItem? aItem = GetCartItem(aProduct.ProductPK);

            // If it is a new item

            if (aItem == null)
            {
                cartItems.Add(new CartItem { Product = aProduct, Quantity = 1 });
            }

            else
            {
                // Increase quantity by 1 if the current quantity is less than 20

                if (aItem.Quantity < MaxQuantity)
                {
                    aItem.Quantity += 1;
                }
            }
        }

        public void UpdateItem(int? productPK, int quantity)
        {
            CartItem? aItem = GetCartItem(productPK);

            if (aItem != null)
            {
                aItem.Quantity = (quantity <= MaxQuantity) ? quantity : MaxQuantity;
            }
        }

        public void RemoveItem(int? productPK)
        {
            cartItems.RemoveAll(r => r.Product?.ProductPK == productPK);
        }

        public void ClearCart()
        {
            cartItems.Clear();
        }

        public decimal? ComputeOrderTotal()
        {
            return cartItems.Sum(s => s.Product?.UnitCost * s.Quantity);
        }

        public IEnumerable<CartItem> CartItems()
        {
            return cartItems;
        }
    }
}
