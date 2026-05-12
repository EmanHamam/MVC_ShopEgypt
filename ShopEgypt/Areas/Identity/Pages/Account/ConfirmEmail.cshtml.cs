// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using ShopEgypt.Domain.Entities;

namespace ShopEgypt.Areas.Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string code, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (userId == null || code == null)
            {
                TempData["StatusMessage"] = "The confirmation link is invalid or incomplete.";
                TempData["StatusAlertType"] = "danger";
                return RedirectToPage("./Login", new { returnUrl });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["StatusMessage"] = "We could not confirm your email. The link may be invalid or expired.";
                TempData["StatusAlertType"] = "danger";
                return RedirectToPage("./Login", new { returnUrl });
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                TempData["StatusMessage"] = "Your email has been confirmed. You can sign in now.";
                TempData["StatusAlertType"] = "success";
            }
            else
            {
                TempData["StatusMessage"] = "We could not confirm your email. The link may have expired—you can request a new confirmation email from the login page.";
                TempData["StatusAlertType"] = "danger";
            }

            return RedirectToPage("./Login", new { returnUrl });
        }
    }
}
