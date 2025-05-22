using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccessLayer;
using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Linq;
using MatrixInc.Models;
using MatrixInc.Extensions;
using Microsoft.AspNetCore.Http;


namespace MatrixInc.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly MatrixIncDbContext _context;

        public ProductsModel(MatrixIncDbContext context)
        {
            _context = context;
        }

        public List<Product> Products { get; set; }

        public void OnGet()
        {
            Products = _context.Products.ToList();
        }


        public async Task<IActionResult> OnPostAddToCartAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();

            var existingItem = cart.Items.FirstOrDefault(i => i.ItemId == productId && i.ItemType == "Product");
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ItemId = product.Id,
                    ItemType = "Product",
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl,
                    Description = product.Description
                });
            }

            HttpContext.Session.Set("Cart", cart);
            if (quantity > 1)
            {
                TempData["Message"] = $"{product.Name} (x{quantity}) is toegevoegd aan je winkelwagen.";
            }
            else
            {
                TempData["Message"] = $"{product.Name} is toegevoegd aan je winkelwagen.";
            }
            return RedirectToPage();
        }


    }
}


