using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using penkta.Models;

namespace penkta.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public RegisterModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public RegisterInput Input { get; set; } = new();

        public List<string> Errors { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (await _userManager.FindByEmailAsync(Input.Email) != null)
            {
                Errors.Add("Email is already registered.");
                return Page();
            }

            if (_userManager.Users.Any(u => u.UserName == Input.Username))
            {
                Errors.Add("Username is already taken.");
                return Page();
            }

            var user = new ApplicationUser
            {
                UserName = Input.Username,
                Email = Input.Email
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                try
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, token },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Confirm your email",
                        $"<p>Please confirm your account by clicking the link below:</p>" +
                        $"<p><a href='{confirmationLink}'>Confirm Email</a></p>"
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Email Error] Failed to send confirmation: {ex.Message}");
                    Errors.Add("Failed to send confirmation email. Please try again later.");
                    return Page();
                }

                TempData["SuccessMessage"] = "Account created! Please check your email to confirm.";
                return RedirectToPage("/Account/Login");
            }

            foreach (var error in result.Errors)
            {
                Errors.Add(error.Description);
            }

            return Page();
        }

        public class RegisterInput
        {
            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; } = "";

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = "";

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = "";

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            [Display(Name = "Confirm Password")]
            public string ConfirmPassword { get; set; } = "";
        }
    }
}
