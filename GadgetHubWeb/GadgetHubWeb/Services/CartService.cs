using GadgetHubWeb.Models;
using System.Text.Json;

namespace GadgetHubWeb.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartKey = "CartItems";

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<CartItemViewModel> GetCart()
        {
            var session = _httpContextAccessor.HttpContext!.Session;
            var cartJson = session.GetString(CartKey);
            return cartJson == null ? new List<CartItemViewModel>() : JsonSerializer.Deserialize<List<CartItemViewModel>>(cartJson);
        }

        public void AddToCart(string globalId, int quantity)
        {
            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.GlobalId == globalId);
            if (existing != null)
                existing.Quantity += quantity;
            else
                cart.Add(new CartItemViewModel { GlobalId = globalId, Quantity = quantity });

            SaveCart(cart);
        }

        public void RemoveFromCart(string globalId)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.GlobalId == globalId);
            SaveCart(cart);
        }

        public void UpdateQuantity(string globalId, int quantity)
        {
            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.GlobalId == globalId);
            if (existing != null)
            {
                if (quantity <= 0)
                    cart.Remove(existing);
                else
                    existing.Quantity = quantity;
                SaveCart(cart);
            }
        }

        public void ClearCart()
        {
            SaveCart(new List<CartItemViewModel>());
        }

        private void SaveCart(List<CartItemViewModel> cart)
        {
            var session = _httpContextAccessor.HttpContext!.Session;
            session.SetString(CartKey, JsonSerializer.Serialize(cart));
        }
    }
}
