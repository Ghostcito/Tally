// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SoftWC.Models;
using SoftWC.Service;

namespace SoftWC.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly IUserStore<Usuario> _userStore;
        private readonly IUserEmailStore<Usuario> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        private readonly UserService _userService;

        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<Usuario> userManager,
            IUserStore<Usuario> userStore,
            SignInManager<Usuario> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            UserService userService, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _userService = userService;
            _roleManager = roleManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = new();

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            // Campos adicionales (opcionales)
            [Display(Name = "Nombre")]
            public string Nombre { get; set; }

            [Display(Name = "Apellido")]
            public string Apellido { get; set; }

            [Required]
            [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener exactamente 8 dígitos")]
            [RegularExpression(@"^\d+$", ErrorMessage = "El DNI solo debe contener números")]
            [Display(Name = "DNI")]
            public string DNI { get; set; }

            [Display(Name = "FechaIngreso")]
            public DateTime? FechaIngreso { get; set; }

            [Display(Name = "FechaNacimiento")]
            public DateTime? FechaNacimiento { get; set; }

            [Display(Name = "NivelAcceso")]
            public string NivelAcceso { get; set; }

            [Display(Name = "Estado")]
            public string Estado { get; set; }

            [Display(Name = "Salario")]
            public decimal? Salario { get; set; }

            [StringLength(9, MinimumLength = 9, ErrorMessage = "El celular debe tener exactamente 9 dígitos")]
            [RegularExpression(@"^\d+$", ErrorMessage = "debe contener números")]
            [Display(Name = "Telefono")]
            public string Telefono { get; set; }
            
            [Required(ErrorMessage = "Debe seleccionar un rol")]
            public string RolSeleccionado { get; set; }
            
            [BindProperty]
            public List<string> RolesDisponibles { get; set; } = new List<string>();
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            Input.RolesDisponibles = new List<string> { "Administrador", "Supervisor", "Empleado" };
        }   


        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                // Validar que el DNI no exista usando el UserService
                var existingUserWithDni = await _userService.FindByDniAsync(Input.DNI);
                if (existingUserWithDni != null)
                {
                    Input.RolesDisponibles = new List<string> { "Administrador", "Supervisor", "Empleado" };
                    ModelState.AddModelError(string.Empty, "El DNI ingresado ya está registrado.");
                    return Page();
                }

                // if (!await _userService.IsDniAvailableAsync(Input.DNI))
                // {
                //     ModelState.AddModelError(string.Empty, "El DNI ingresado ya está registrado.");
                //     return Page();
                // }

                var user = new Usuario  // Usa el constructor directamente
                {
                    UserName = Input.Nombre.Replace(" ", "_"),
                    Email = Input.Email,
                    // Asigna todas las propiedades adicionales
                    Nombre = Input.Nombre,
                    Apellido = Input.Apellido,
                    DNI = Input.DNI,
                    PhoneNumber = Input.Telefono,  // Identity usa PhoneNumber, no Telefono
                    FechaIngreso = Input.FechaIngreso?.ToUniversalTime(),
                    FechaNacimiento = Input.FechaNacimiento?.ToUniversalTime(),
                    NivelAcceso = Input.NivelAcceso,
                    Estado = Input.Estado,
                    Salario = Input.Salario
                };

                await _userStore.SetUserNameAsync(user, user.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (!string.IsNullOrEmpty(Input.RolSeleccionado))
                    {
                        await _userManager.AddToRoleAsync(user, Input.RolSeleccionado);
                    }

                    // var userId = await _userManager.GetUserIdAsync(user);
                    // var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    // code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    // var callbackUrl = Url.Page(
                    //     "/Account/ConfirmEmail",
                    //     pageHandler: null,
                    //     values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    //     protocol: Request.Scheme);

                    // await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //     $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Usuario"); // modificar cuando se implemente el dashboard
                    }
                }
                foreach (var error in result.Errors)
                {
                    Input.RolesDisponibles = new List<string> { "Administrador", "Supervisor", "Empleado" };
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private Usuario CreateUser()
        {
            try
            {
                return Activator.CreateInstance<Usuario>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(Usuario)}'. " +
                    $"Ensure that '{nameof(Usuario)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<Usuario> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<Usuario>)_userStore;
        }
    }
}
