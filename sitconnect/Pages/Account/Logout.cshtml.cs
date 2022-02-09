using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sitconnect.Models;

namespace sitconnect.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {   
        private readonly SignInManager<User> _signInManager;
        
        public LogoutModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }
        
        public async Task<IActionResult> OnPost()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }
    }
}