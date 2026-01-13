using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ServiceLayer.Services;

namespace MVC.Controllers;

public class AuthController : Controller
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    // GET: /Auth/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Auth/Login
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ViewBag.ErrorMessage = "Email and password are required.";
            return View();
        }

        var user = await _authService.LoginAsync(email, password);

        if (user == null)
        {
            ViewBag.ErrorMessage = "Invalid email or password.";
            return View();
        }

        // Set user_id cookie
        Response.Cookies.Append("user_id", user.Id.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });

        return RedirectToAction("Index", "Home");
    }

    // GET: /Auth/Register
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST: /Auth/Register
    [HttpPost]
    public async Task<IActionResult> Register(string email, string password, string confirmPassword)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ViewBag.ErrorMessage = "Email and password are required.";
            return View();
        }

        if (password != confirmPassword)
        {
            ViewBag.ErrorMessage = "Passwords do not match.";
            return View();
        }

        var user = await _authService.RegisterAsync(email, password);

        if (user == null)
        {
            ViewBag.ErrorMessage = "User with this email already exists.";
            return View();
        }

        // Set user_id cookie
        Response.Cookies.Append("user_id", user.Id.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });

        return RedirectToAction("Index", "Home");
    }

    // GET: /Auth/Logout
    [HttpGet]
    public IActionResult Logout()
    {
        // Delete the user_id cookie
        Response.Cookies.Delete("user_id");

        return RedirectToAction("Login");
    }
}

