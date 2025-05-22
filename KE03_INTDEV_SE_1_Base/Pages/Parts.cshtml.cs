using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccessLayer;
using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Linq;
using MatrixInc.Extensions;
using MatrixInc.Models;


namespace MatrixInc.Pages
{
    public class PartsModel : PageModel
    {
        private readonly MatrixIncDbContext _context;

        public PartsModel(MatrixIncDbContext context)
        {
            _context = context;
        }

        public List<Part> Parts { get; set; }

        public void OnGet()
        {
            Parts = _context.Parts.ToList();
        }
    

     public async Task<IActionResult> OnPostAddToCartAsync(int partId, int quantity)
        {

            var part = await _context.Parts.FindAsync(partId);
            if (part == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.Get<ShoppingCart>("Cart") ?? new ShoppingCart();

            var existingItem = cart.Items.FirstOrDefault(i => i.ItemId == partId && i.ItemType == "Part");
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ItemId = part.Id,
                    ItemType = "Part",
                    Name = part.Name,
                    Price = part.Price,
                    Quantity = quantity,
                    ImageUrl = part.ImageUrl,
                    Description = part.Description
                });
            }

            HttpContext.Session.Set("Cart", cart);
            if (quantity > 1)
            {
                TempData["Message"] = $"{part.Name} (x{quantity}) is toegevoegd aan je winkelwagen.";
            }
            else
            {
                TempData["Message"] = $"{part.Name} is toegevoegd aan je winkelwagen.";
            }
            return RedirectToPage();
        }


    }

}


