using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Matriarchy.Data;
using Matriarchy.Models;
using Matriarchy.Models.AccountViewModels;
using Matriarchy.Services;
using Microsoft.EntityFrameworkCore;

namespace Matriarchy.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly MvcMatriarchyContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            MvcMatriarchyContext context)
        {
            try
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _emailSender = emailSender;
                _logger = logger;
                _context = context;
            }
            catch
            {

            }

        }

        [TempData]
        public string ErrorMessage { get; set; }

        //**********************************************************************************************************
        // Login              Login to the application
        //
        // Inputs: string returnUrl = null         Return URL
        //**********************************************************************************************************
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            try
            {
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                ViewData["ReturnUrl"] = returnUrl;
                return View();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        //**********************************************************************************************************
        // Login              Login to the application
        //
        // Inputs:
        // LoginViewModel model               Login Model
        // string returnUrl = null            Return URL
        //**********************************************************************************************************
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            try
            {
                ViewData["ReturnUrl"] = returnUrl;
                if (ModelState.IsValid)
                {
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");

                        return RedirectToLocal(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToAction(nameof(Lockout));
                    }
                    else
                    {
                        var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                        var email = model.Email;

                        var rowsAffected = _context.Database.ExecuteSqlCommand("Call InsertFailedLoginAttempt({0},{1})", remoteIpAddress, email);

                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return View(model);
                    }
                }

                // If we got this far, something failed, redisplay form
                return View(model);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }

        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            try
            {
                // Ensure the user has gone through the username & password screen first
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

                if (user == null)
                {
                    throw new ApplicationException($"Unable to load two-factor authentication user.");
                }

                var model = new LoginWith2faViewModel { RememberMe = rememberMe };
                ViewData["ReturnUrl"] = returnUrl;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }

        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

                var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                    return RedirectToLocal(returnUrl);
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                    ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }

        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            try
            {
                // Ensure the user has gone through the username & password screen first
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load two-factor authentication user.");
                }

                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load two-factor authentication user.");
                }

                var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

                var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                    return RedirectToLocal(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                    ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }

        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            try
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            try
            {
                ViewData["ReturnUrl"] = returnUrl;
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {

                        var resultAddRole = await _userManager.AddToRoleAsync(user, "Registered");


                        _logger.LogInformation("User created a new account with password.");

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                        await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created a new account with password.");
                        return RedirectToLocal(returnUrl);
                    }
                    AddErrors(result);
                }

                // If we got this far, something failed, redisplay form
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out.");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            try
            {
                // Request a redirect to the external login provider.
                var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                return Challenge(properties, provider);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            try
            {

                if (remoteError != null)
                {
                    ErrorMessage = $"Error from external provider: {remoteError}";
                    return RedirectToAction(nameof(Login));
                }
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return RedirectToAction(nameof(Login));
                }

                // Sign in the user with this external login provider if the user already has a login.
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                    return RedirectToLocal(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    // If the user does not have an account, then ask the user to create an account.
                    ViewData["ReturnUrl"] = returnUrl;
                    ViewData["LoginProvider"] = info.LoginProvider;
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Get the information about the user from the external login provider
                    var info = await _signInManager.GetExternalLoginInfoAsync();
                    if (info == null)
                    {
                        throw new ApplicationException("Error loading external login information during confirmation.");
                    }
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    var result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddLoginAsync(user, info);
                        if (result.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                            return RedirectToLocal(returnUrl);
                        }
                    }
                    AddErrors(result);
                }
                ViewData["ReturnUrl"] = returnUrl;
                return View(nameof(ExternalLogin), model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }

        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            try
            {
                if (userId == null || code == null)
                {
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{userId}'.");
                }
                var result = await _userManager.ConfirmEmailAsync(user, code);
                return View(result.Succeeded ? "ConfirmEmail" : "Error");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                    {
                        // Don't reveal that the user does not exist or is not confirmed
                        return RedirectToAction(nameof(ForgotPasswordConfirmation));
                    }

                    // For more information on how to enable account confirmation and password reset please
                    // visit https://go.microsoft.com/fwlink/?LinkID=532713
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                       $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // If we got this far, something failed, redisplay form
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            try
            {
                if (code == null)
                {
                    throw new ApplicationException("A code must be supplied for password reset.");
                }
                var model = new ResetPasswordViewModel { Code = code };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }
                AddErrors(result);
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        [HttpGet]
        public IActionResult AccessDenied()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        #region Helpers
        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************

        private void AddErrors(IdentityResult result)
        {
            try
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
            }

        }

        //**********************************************************************************************************
        //
        //
        //**********************************************************************************************************
        private IActionResult RedirectToLocal(string returnUrl)
        {
            try
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion
    }
}
