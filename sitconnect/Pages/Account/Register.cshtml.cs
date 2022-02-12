using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using sitconnect.Data;
using sitconnect.Models;
using sitconnect.Services;

namespace sitconnect.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly StoreContext _storeContext;
        private readonly IConfiguration _config;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            Crypto crypto,
            IConfiguration config,
            StoreContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _storeContext = context;
            _config = config;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Upload a profile picture.")]
            [Display(Name = " ")]
            [DataType(DataType.Upload)]
            public IFormFile ProfilePic { get; set; }
            
            [Required]
            [Display(Name = "First name")]
            public string FirstName { get; set; }
            
            [Required]
            [Display(Name = "Last name")]
            public string LastName { get; set; }
            
            [Required(ErrorMessage = "The Date of birth field cannot be empty.")]
            [Display(Name = "Date of birth")]
            [DataType(DataType.Date)]
            
            public DateTime DateOfBirth { get; set; }
            
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 12)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
            
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            
            [Required]
            [Display(Name = "Card number")]
            [DataType(DataType.CreditCard)]
            public string CardNumber { get; set; }
            
            [Required]
            [StringLength(2, ErrorMessage = "Invalid month.")]
            [Display(Name = "MM")]
            public string ExpiryMonth { get; set; }
            
            [Required]
            [StringLength(2, ErrorMessage = "Invalid year.")]
            [Display(Name = "YY")]
            public string ExpiryYear { get; set; }
            
            [Required]
            [StringLength(3, ErrorMessage = "Invalid CVV.")]
            [Display(Name = "CVV")]
            public string CVV { get; set; }
            
            [Required]
            [Display(Name = "Billing Address")]
            public string BillingAddress { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = "returnUrl ?? Url.Content(\"~/\");";
            if (ModelState.IsValid)
            {
                Aes aes = Aes.Create();
                var key = aes.Key;
                var IV = aes.IV;
                _config["EncryptionKey"] = Convert.ToBase64String(key);
                _config["EncryptionIV"] = Convert.ToBase64String(IV);
                byte[] file = new byte[] { };
                
                using (var memoryStream = new MemoryStream())
                {
                    await Input.ProfilePic.CopyToAsync(memoryStream);

                    // Upload the file if less than 8 MB
                    if (memoryStream.Length < 8388608)
                    {
                        file = memoryStream.ToArray();
                    }
                    else
                    {
                        ModelState.AddModelError("File", "The file is too large.");
                    }
                }
                
                var user = new User
                {
                    UserName = Input.Email, Email = Input.Email, DateOfBirth = Input.DateOfBirth,
                    FirstName = Input.FirstName, LastName = Input.LastName, ProfilePic = file
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    
                    var creditCard = new CreditCard
                    {
                        UserId = user.Id,
                        Number = Convert.ToBase64String(Crypto.EncryptStringToBytes_Aes(Input.CardNumber, key, IV)), 
                        ExpiryMonth = Input.ExpiryMonth,
                        ExpiryYear = Input.ExpiryYear, 
                        CVV = Convert.ToBase64String(Crypto.EncryptStringToBytes_Aes(Input.CardNumber, key, IV))
                    };

                    _storeContext.CreditCards.Add(creditCard);
                    await _storeContext.SaveChangesAsync();

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account using this link {HtmlEncoder.Default.Encode(callbackUrl)}");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
