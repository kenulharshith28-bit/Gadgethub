using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GadgetHubWeb.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Remove("AuthToken");
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("UserRole");
            return RedirectToPage("/Index");
        }
    }
}










