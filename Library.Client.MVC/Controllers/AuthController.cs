namespace Library.Client.MVC.Controllers;

using Library.BusinessRules;
using Library.Client.MVC.Models;
using Library.Client.MVC.services;
using Library.DataAccess.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class AuthController : Controller
{
   
    private readonly LoanService _loanService;
    BLUsers usersBL = new BLUsers();
    BLUsers_Roles rolesBL = new BLUsers_Roles();

    public AuthController(LoanService loanService)
    {
        _loanService = loanService;
    }
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Admin");
    }
    [HttpGet]
    public IActionResult Login() //Login de usuarios
    {
        var redirectUrl = Url.Action("GoogleResponse", "Auth");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpPost]
    [AllowAnonymous]
    // [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> LoginAdmin(Users pUser) //Login de admin
    {        
        try
        {
            //Trae al usuario
            var user = await usersBL.LoginAsync(pUser);
            //
            if (user != null && user.USER_ID > 0 && pUser.USER_NAME == user.USER_NAME)
            {

                // user.Users_Roles = await rolesBL.GetRolesByIdAsync(new Users_Roles { USER_ROLE_ID = user.ROlE_ID });
                var claims = new[] { new Claim(ClaimTypes.Name, user.USER_NAME), new Claim(ClaimTypes.Role, user.Users_Roles.ROLE) };
                var identity = new ClaimsIdentity(claims, "AdminScheme");
                await HttpContext.SignInAsync("AdminScheme", new ClaimsPrincipal(identity));
            }
            else
            {
                throw new Exception("Credenciales incorrectas");
            }

            return RedirectToAction("Index", "Admin");
        }
        catch (Exception ex)
        {
            var user = new Users { USER_NAME = pUser.USER_NAME };
            ViewBag.Error = ex.Message;
            return View(user);
        }
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAdmin()
    {
        var isUser = HttpContext.Request.Cookies.ContainsKey("UserAuthCookie"); //Chequea si tiene la coockie de usuario
        
        if(isUser)
        {
            return Unauthorized();
        }

        ViewBag.Users_Roles = await rolesBL.GetAllRolesASync();
        ViewBag.Error = "";
        return View();
    }
    [HttpGet]
    public async Task<ActionResult> ChangePassword()
    {
        var user = await usersBL.GetUsersAsync(new Users { USER_NAME = User.Identity.Name, Top_Aux = 1 });
        ViewBag.Error = "";
        return View(user.FirstOrDefault());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(Users pUsers, string pOldPassword)
    {
        try
        {
            int result = await usersBL.ChangePasswordAsync(pUsers, pOldPassword);
            await HttpContext.SignOutAsync("AdminScheme");
            return RedirectToAction("LoginAdmin", "Auth");
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            var user = await usersBL.GetUsersAsync(pUsers);
            return View(user.FirstOrDefault());
        }
    }
    [HttpGet]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync("UserScheme");

        if (result.Principal != null)
        {
            var emailClaim = result.Principal.FindFirst(ClaimTypes.Email);
            var email = emailClaim?.Value;
            var nameClaim = result.Principal.FindFirst(ClaimTypes.Name);
            var name = nameClaim?.Value; 

            // Verificar si el correo existe
            if (email != null)
            {
                // Verificar si el dominio es @esfe.agape.edu.sv
                string dominioEstudiantil = "@esfe.agape.edu.sv";
                List<Claim> claims;

                if (email.EndsWith(dominioEstudiantil))
                {
                    string codigoEstudiante = email.Split('@')[0];
                    Student student = await _loanService.GetStudentByCode(codigoEstudiante);

                    if(student == null || student.Id == 0)
                    {
                        TempData["ErrorMessage"] = "Ups! Parece que tu cuenta es inaccesible!";
                        await HttpContext.SignOutAsync("UserScheme");
                        return RedirectToAction("Index", "Library");
                    }
                    
                    claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, email),
                        new Claim("CodigoEstudiante", codigoEstudiante),
                        new Claim(ClaimTypes.Name, name ?? string.Empty),
                    };

                    ViewBag.isStudent = true;
                    ViewBag.studentCodeLogin = codigoEstudiante;
                    TempData["SuccessMessage"] = "Has iniciado sesion exitosamente!";
                    
                    // var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsIdentity = new ClaimsIdentity(claims, "UserScheme");
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Mantener la sesión aunque el navegador se cierre
                        ExpiresUtc = DateTime.UtcNow.AddDays(5) // Duración de la sesión
                    };
                        
                    // Iniciar la sesión
                    await HttpContext.SignInAsync("UserScheme", new ClaimsPrincipal(claimsIdentity), authProperties);

                }
                else
                {
                    // Si el dominio no es válido, redirigir al usuario o mostrar un mensaje
                    TempData["ErrorMessage"] = "Solo se permiten correos del dominio estudiantil (@esfe.agape.edu.sv).";
                    await HttpContext.SignOutAsync("UserScheme");
                    return RedirectToAction("Index", "Library"); // Redirigir a la página de login
                }
                return RedirectToAction("Index", "Library");
            }
        }
        return RedirectToAction("Login", "Auth");
    }


    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        // Cerrar la sesión del usuario
        await HttpContext.SignOutAsync("UserScheme");
        TempData["LogoutMessage"] = "Has cerrado sesion exitosamente.";
        return RedirectToAction("Index", "Library");
    }
    [HttpPost]
    public async Task<IActionResult> LogoutAdmin()
    {
        // Cerrar la sesión del admin
        await HttpContext.SignOutAsync("AdminScheme");
        return RedirectToAction("LoginAdmin", "Auth");
    }

    // -------- PERFIL CON PAGINACIÓN --------
    [HttpGet]
    public async Task<IActionResult> Profile(int page = 1, int pageSize = 6)
    {
        return await LoadStudentLoans(page, pageSize, "Profile");
    }

    [HttpGet]
    public async Task<IActionResult> Student(int page = 1, int pageSize = 6)
    {
        return await LoadStudentLoans(page, pageSize, "Profile");
    }

    // -------- MÉTODO PRIVADO REUTILIZABLE --------
    private async Task<IActionResult> LoadStudentLoans(int page, int pageSize, string viewName)
    {
        var isAuthenticated = HttpContext.User.Identity.AuthenticationType == "UserScheme";
        if (!isAuthenticated)
        {
            return RedirectToAction("Login", "Auth");
        }

        var userClaims = User.Identity as ClaimsIdentity;
        string code = userClaims?.FindFirst("CodigoEstudiante")?.Value ?? string.Empty;

        if (string.IsNullOrEmpty(code))
        {
            return RedirectToAction("Login", "Auth");
        }

        var (loans, loansDates) = await _loanService.GetStudentLoans(code);
        ViewBag.LoanDates = loansDates;

        int totalRecords = loans.Count();
        int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        var loansPaged = loans
            .OrderByDescending(l => l.LOAN_ID)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = totalPages;

        return View(viewName, loansPaged);
    }
}