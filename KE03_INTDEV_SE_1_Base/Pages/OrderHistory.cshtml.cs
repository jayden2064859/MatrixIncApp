using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccessLayer;
using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MatrixInc.Pages
{
    public class OrderHistoryModel : PageModel
    {
        private readonly MatrixIncDbContext _context;

        public OrderHistoryModel(MatrixIncDbContext context)
        {
            _context = context;
        }

        public List<Order> Orders { get; set; }
        public Customer Customer { get; set; }

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            Customer = _context.Customers.FirstOrDefault(c => c.Id == userId);
            if (Customer == null)
            {
                return RedirectToPage("/Login");
            }

            Orders = _context.Orders
                .Where(o => o.CustomerId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Include(o => o.Products) // Include de gerelateerde producten
                .ToList();

            return Page();
        }
    }
}