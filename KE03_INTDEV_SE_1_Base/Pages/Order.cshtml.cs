using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccessLayer;
using DataAccessLayer.Models;
using System.Threading.Tasks;
using MatrixInc.Extensions;
using MatrixInc.Models;

namespace MatrixInc.Pages
{
    public class OrderModel : PageModel
    {
        private readonly MatrixIncDbContext _context;

        public OrderModel(MatrixIncDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int ItemId { get; set; }

        [BindProperty]
        public string ItemType { get; set; } 

        public Product Product { get; set; }
        public Part Part { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, string type)
        {
            ItemType = type;

            if (type == "Product")
            {
                Product = await _context.Products.FindAsync(id);
                if (Product == null)
                {
                    return RedirectToPage("/Products");
                }
            }
            else if (type == "Part")
            {
                Part = await _context.Parts.FindAsync(id);
                if (Part == null)
                {
                    return RedirectToPage("/Parts");
                }
            }
            else
            {
                return RedirectToPage("/Products");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int productId, int partId, int quantity)
        {
            CartItem newItem = null;
            string itemType = "";
            string itemName = "";

            if (productId > 0)
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return NotFound();
                }
                itemType = "Product";
                itemName = product.Name;
                newItem = new CartItem
                {
                    ItemId = product.Id,
                    ItemType = itemType,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl,
                    Description = product.Description
                };
            }
            else if (partId > 0)
            {
                var part = await _context.Parts.FindAsync(partId);
                if (part == null)
                {
                    return NotFound();
                }
                itemType = "Part";
                itemName = part.Name;
                newItem = new CartItem
                {
                    ItemId = part.Id,
                    ItemType = itemType,
                    Name = part.Name,
                    Price = part.Price,
                    Quantity = quantity,
                    ImageUrl = part.ImageUrl,
                    Description = part.Description
                };
            }
            else
            {
                return NotFound();
            }

            var cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();

            var existingItem = cart.Items.FirstOrDefault(i => i.ItemId == newItem.ItemId && i.ItemType == itemType);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(newItem);
            }

            HttpContext.Session.Set("Cart", cart);
            TempData["Message"] = $"{itemName} is toegevoegd aan je winkelwagen.";

            return RedirectToPage(itemType == "Product" ? "/Products" : "/Parts");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ItemType == "Product")
            {
                Product = await _context.Products.FindAsync(ItemId);
                if (Product == null)
                {
                    return RedirectToPage("/Products");
                }

                TempData["Message"] = $"Bedankt voor je bestelling van {Product.Name}!";
                return RedirectToPage("/Products");
            }
            else if (ItemType == "Part")
            {
                Part = await _context.Parts.FindAsync(ItemId);
                if (Part == null)
                {
                    return RedirectToPage("/Parts");
                }

                TempData["Message"] = $"Bedankt voor je bestelling van {Part.Name}!";
                return RedirectToPage("/Parts");
            }

            return RedirectToPage("/Products");
        }
    }
}