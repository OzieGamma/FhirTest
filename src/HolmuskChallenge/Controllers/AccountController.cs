﻿// <copyright company="Oswald MASKENS" file="AccountController.cs">
// Copyright 2014-2016 Oswald MASKENS
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
// 
//  http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
// </copyright>

using System.Threading.Tasks;
using HolmuskChallenge.EmailViewModels.Account;
using HolmuskChallenge.Helpers;
using HolmuskChallenge.Models;
using HolmuskChallenge.Services.Email;
using HolmuskChallenge.ViewModels.Account;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using ResetPasswordViewModel = HolmuskChallenge.EmailViewModels.Account.ResetPasswordViewModel;

namespace HolmuskChallenge.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITemplatedEmailSender _emailSender;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITemplatedEmailSender emailSender,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            this.ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            this.ViewData["ReturnUrl"] = returnUrl;
            if (this.ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                SignInResult result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return this.RedirectToLocal(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }
                this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = new ApplicationUser {UserName = model.Email, Email = model.Email};
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Send an email with this link
                    string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    string callbackUrl =
                        this.Url.Action("ConfirmEmail", "Account", new {userId = user.Id, code}, this.HttpContext.Request.Scheme);

                    await _emailSender.SendEmailAsync("AccountConfirmationViewModel", model.Email, "Confirm your account",
                        new AccountConfirmationViewModel {ConfirmationLink = callbackUrl});

                    await _signInManager.SignInAsync(user, false);

                    _logger.LogInformation(3, "User created a new account with password.");
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if ((userId == null) || (code == null))
            {
                return View("Error");
            }
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByNameAsync(model.Email);
                if ((user == null) || !await _userManager.IsEmailConfirmedAsync(user))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // Send an email with this link
                string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                string callbackUrl = this.Url.Action("ResetPasswordViewModel", "Account", new {userId = user.Id, code},
                    this.HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync("ResetPasswordViewModel", model.Email,
                    "Reset Password", new ResetPasswordViewModel {ResetLink = callbackUrl});

                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ViewModels.Account.ResetPasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
            }

            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        #endregion
    }
}