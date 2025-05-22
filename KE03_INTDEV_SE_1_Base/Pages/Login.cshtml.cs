using DataAccessLayer; 
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MatrixInc.Pages
{
    public class LoginModel : PageModel
    {
        private readonly MatrixIncDbContext _db;  

        public LoginModel(MatrixIncDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            var customer = await _db.Customers
                .FirstOrDefaultAsync(c => c.Name == Username);

            if (customer == null)
            {
                return new JsonResult(new { success = false, error = "Gebruiker niet gevonden" });
            }

            // Controleer het wachtwoord (aannemende dat Password een veld is in Customer)
            if (customer.Password != Password)
            {
                return new JsonResult(new { success = false, error = "Ongeldig wachtwoord" });
            }

            HttpContext.Session.SetInt32("UserId", customer.Id);
            return new JsonResult(new { success = true });
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToPage("/Index");
        }
    }
}