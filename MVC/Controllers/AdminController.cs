using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ServiceLayer.Services;
using NivelAccesDate_CodeFirst.Models;

namespace MVC.Controllers;

public class AdminController : Controller
{
    private readonly PlanService _planService;
    private readonly AuthService _authService;

    public AdminController(PlanService planService, AuthService authService)
    {
        _planService = planService;
        _authService = authService;
    }

    // Check if the current user is admin
    private async Task<bool> IsAdminAsync()
    {
        if (!Request.Cookies.TryGetValue("user_id", out var userIdStr))
        {
            return false;
        }

        if (!int.TryParse(userIdStr, out var userId))
        {
            return false;
        }

        var user = await _authService.GetUserByIdAsync(userId);
        return user != null && user.Role == "admin";
    }

    // GET: /Admin/Dashboard
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        if (!await IsAdminAsync())
        {
            return RedirectToAction("Login", "Auth");
        }

        var plans = await _planService.GetAllPlansAsync();
        return View(plans);
    }

    // GET: /Admin/CreatePlan
    [HttpGet]
    public async Task<IActionResult> CreatePlan()
    {
        if (!await IsAdminAsync())
        {
            return RedirectToAction("Login", "Auth");
        }

        return View();
    }

    // POST: /Admin/CreatePlan
    [HttpPost]
    public async Task<IActionResult> CreatePlan(string name, int limit, int price)
    {
        if (!await IsAdminAsync())
        {
            return RedirectToAction("Login", "Auth");
        }

        if (string.IsNullOrEmpty(name))
        {
            ViewBag.ErrorMessage = "Plan name is required.";
            return View();
        }

        await _planService.CreatePlanAsync(name, limit, price);

        return RedirectToAction("Dashboard");
    }

    // GET: /Admin/EditPlan/5
    [HttpGet]
    public async Task<IActionResult> EditPlan(int id)
    {
        if (!await IsAdminAsync())
        {
            return RedirectToAction("Login", "Auth");
        }

        var plan = await _planService.GetPlanByIdAsync(id);
        if (plan == null)
        {
            return NotFound();
        }

        return View(plan);
    }

    // POST: /Admin/EditPlan/5
    [HttpPost]
    public async Task<IActionResult> EditPlan(int id, string name, int limit, int price)
    {
        if (!await IsAdminAsync())
        {
            return RedirectToAction("Login", "Auth");
        }

        if (string.IsNullOrEmpty(name))
        {
            ViewBag.ErrorMessage = "Plan name is required.";
            var plan = await _planService.GetPlanByIdAsync(id);
            return View(plan);
        }

        var updatedPlan = await _planService.UpdatePlanAsync(id, name, limit, price);
        if (updatedPlan == null)
        {
            return NotFound();
        }

        return RedirectToAction("Dashboard");
    }

    // POST: /Admin/DeletePlan/5
    [HttpPost]
    public async Task<IActionResult> DeletePlan(int id)
    {
        if (!await IsAdminAsync())
        {
            return RedirectToAction("Login", "Auth");
        }

        var success = await _planService.DeletePlanAsync(id);
        if (!success)
        {
            TempData["ErrorMessage"] = "Cannot delete plan. It may have users assigned to it.";
        }
        else
        {
            TempData["SuccessMessage"] = "Plan deleted successfully.";
        }

        return RedirectToAction("Dashboard");
    }
}

