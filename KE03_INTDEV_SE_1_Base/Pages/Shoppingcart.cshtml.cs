using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MatrixInc.Models;
using System.Linq;
using MatrixInc.Extensions;
using DataAccessLayer;
using DataAccessLayer.Models;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MatrixInc.Pages
{
    public class ShoppingCartModel : PageModel
    {
        private readonly MatrixIncDbContext _context;

        public ShoppingCartModel(MatrixIncDbContext context)
        {
            _context = context;
        }

        public ShoppingCart Cart { get; set; }

        public void OnGet()
        {
            Cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();
        }

        public IActionResult OnPostUpdateQuantity(int itemId, string itemType, int quantity)
        {
            Cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();
            var item = Cart.Items.FirstOrDefault(i => i.ItemId == itemId && i.ItemType == itemType);

            if (item != null)
            {
                item.Quantity = quantity;
                HttpContext.Session.Set("Cart", Cart);
            }

            return RedirectToPage();
        }

        public IActionResult OnPostRemoveItem(int itemId, string itemType)
        {
            Cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();
            var item = Cart.Items.FirstOrDefault(i => i.ItemId == itemId && i.ItemType == itemType);

            if (item != null)
            {
                Cart.Items.Remove(item);
                HttpContext.Session.Set("Cart", Cart);
                if (item.Quantity > 1)
                {
                    TempData["WarningMessage"] = $"{item.Name} (x{item.Quantity}) is verwijderd uit je winkelwagen.";
                }
                else
                {
                    TempData["WarningMessage"] = $"{item.Name} is verwijderd uit je winkelwagen.";
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCheckoutAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Je moet ingelogd zijn om een bestelling te plaatsen.";
                return RedirectToPage("/ShoppingCart");
            }

            Cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();
            if (!Cart.Items.Any())
            {
                TempData["Message"] = "Je winkelwagen is leeg.";
                return RedirectToPage();
            }

            // 1. Nieuwe order aanmaken
            var order = new Order
            {
                CustomerId = userId.Value,
                OrderDate = DateTime.Now
            };

            // 2. Producten expliciet toevoegen
            var productIds = Cart.Items
                .Where(i => i.ItemType == "Product")
                .Select(i => i.ItemId)
                .ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            foreach (var product in products)
            {
                order.Products.Add(product);
            }

            // 3. Opslaan
            _context.Orders.Add(order);

            try
            {
                await _context.SaveChangesAsync();
                HttpContext.Session.Remove("Cart");

                // 5. Direct message tonen
                HttpContext.Session.Set("OrderSuccessMessage", $"Bestelling #{order.Id} is geplaatst!");
                return RedirectToPage("/OrderHistory");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving order: {ex.Message}");
                TempData["ErrorMessage"] = "Er ging iets mis bij het plaatsen van je bestelling.";
                return RedirectToPage();
            }
        }
    }
}