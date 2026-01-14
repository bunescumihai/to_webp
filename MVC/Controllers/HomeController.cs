using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        // Check if user is logged in
        var userId = Request.Cookies["user_id"];
        ViewBag.IsLoggedIn = !string.IsNullOrEmpty(userId);
        
        return View();
    }
}

