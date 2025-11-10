// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SoftWC.Models;
using SoftWC.Service;

namespace SoftWC.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserService _userService;
        private readonly ILogger<LoginModel> _logger;

        private readonly UserManager<Usuario> _userManager;

        public LoginModel(SignInManager<Usuario> signInManager, ILogger<LoginModel> logger, UserService userService, UserManager<Usuario> userManager)
        {
            _userService = userService;
            _signInManager = signInManager;
            _logger = logger;
            _userManager = signInManager.UserManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required(ErrorMessage = "El DNI es obligatorio.")]
            [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 caracteres.")]
            public string DNI { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {

                var user = await _userService.FindByDniAsync(Input.DNI);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Credenciales inválidas");
                    return Page();
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    var userIdentity = await _userManager.FindByNameAsync(user.UserName);

                    // Obtén los roles del usuario
                    var roles = await _userManager.GetRolesAsync(userIdentity);

                    // Redirige según el rol
                    if (roles.Contains("Administrador") || roles.Contains("Supervisor") || roles.Contains("Controltotal") || roles.Contains("Contador"))
                    {
                        return RedirectToAction("Index", "Admin"); 
                    }
                    else if (roles.Contains("Supervisor"))
                    {
                        return RedirectToAction("Index", "Admin"); 
                    }
                    else if (roles.Contains("Empleado"))
                    {
                        return RedirectToAction("Index", "Empleado"); 
                    }
                    else
                    {
                        // Rol no autorizado
                        return RedirectToPage("AccessDenied", "Account");
                        // esta vista es del mismo identiy, podriamos perfecionar la vista en Areas/Account/manage/AccessDenied.cshtml
                        //por ahora no hay roles, falta implementar ASP.net roles
                    }


                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
