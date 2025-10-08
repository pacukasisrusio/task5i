using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using penkta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace penkta.Pages.Admin
{
    [Authorize]
    public class UsersModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsersModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IList<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Account/Login");
        }

        public async Task<IActionResult> OnPostDeleteSelfAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                await _signInManager.SignOutAsync();
            }

            return RedirectToPage("/Account/Register");
        }

        public void OnGet(string sort = "lastlogin")
        {
            var list = _userManager.Users.ToList();

            Users = sort switch
            {
                "name" => list.OrderBy(u => u.UserName).ToList(),
                "email" => list.OrderBy(u => u.Email).ToList(),
                "lastlogin" => list.OrderByDescending(u => u.LastLoginAt ?? DateTime.MinValue).ToList(),
                _ => list.OrderByDescending(u => u.LastLoginAt ?? DateTime.MinValue).ToList(),
            };
        }

        public async Task<IActionResult> OnPostAsync(string action, string[] selectedIds)
        {
            if (selectedIds == null || selectedIds.Length == 0)
            {
                ModelState.AddModelError("", "No users selected");
                OnGet();
                return Page();
            }

            foreach (var id in selectedIds)
            {
                var target = await _userManager.FindByIdAsync(id);
                if (target == null) continue;

                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser.IsBlocked)
                return RedirectToPage("/Account/Login");

                if (action == "unblock" && target.Id == currentUser.Id)
                continue;

                if (action == "block")
                target.IsBlocked = true;
                else if (action == "unblock")
                    target.IsBlocked = false;
                  else if (action == "delete")
                await _userManager.DeleteAsync(target);
                else if (action == "deleteunverified" && !target.EmailConfirmed)
                    await _userManager.DeleteAsync(target);

                await _userManager.UpdateAsync(target);
            }


            return RedirectToPage();
        }
    }
}
