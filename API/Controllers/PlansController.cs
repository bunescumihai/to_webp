using Microsoft.AspNetCore.Mvc;
using SL.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlansController : ControllerBase
{
    private readonly PlanService _planService;

    public PlansController(PlanService planService)
    {
        _planService = planService;
    }

    /// <summary>
    /// Get all available plans
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllPlans()
    {
        try
        {
            var plans = await _planService.GetAllPlansAsync();
            var result = plans.Select(p => new
            {
                id = p.Id,
                name = p.Name,
                limit = p.Limit,
                price = p.Price
            });

            return Ok(new { plans = result, count = result.Count() });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Error retrieving plans: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get a specific plan by ID
    /// </summary>
    [HttpGet("{planId}")]
    public async Task<IActionResult> GetPlan(int planId)
    {
        try
        {
            var plan = await _planService.GetPlanByIdAsync(planId);
            if (plan == null)
            {
                return NotFound(new { error = "Plan not found" });
            }

            return Ok(new
            {
                id = plan.Id,
                name = plan.Name,
                limit = plan.Limit,
                price = plan.Price
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Error retrieving plan: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get the current plan for a user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserPlan(int userId)
    {
        try
        {
            var plan = await _planService.GetUserPlanAsync(userId);
            if (plan == null)
            {
                return NotFound(new { error = "User or plan not found" });
            }

            return Ok(new
            {
                id = plan.Id,
                name = plan.Name,
                limit = plan.Limit,
                price = plan.Price
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Error retrieving user plan: {ex.Message}" });
        }
    }

    /// <summary>
    /// Change a user's plan
    /// </summary>
    [HttpPost("user/{userId}/change")]
    public async Task<IActionResult> ChangeUserPlan(int userId, [FromBody] ChangePlanRequest request)
    {
        if (request.NewPlanId <= 0)
        {
            return BadRequest(new { error = "Invalid plan ID" });
        }

        try
        {
            var user = await _planService.ChangeUserPlanAsync(userId, request.NewPlanId);
            if (user == null)
            {
                return NotFound(new { error = "User or plan not found" });
            }

            var newPlan = await _planService.GetPlanByIdAsync(user.PlanId);

            return Ok(new
            {
                success = true,
                message = "Plan changed successfully",
                user = new
                {
                    id = user.Id,
                    email = user.Email
                },
                newPlan = new
                {
                    id = newPlan!.Id,
                    name = newPlan.Name,
                    limit = newPlan.Limit,
                    price = newPlan.Price
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Error changing plan: {ex.Message}" });
        }
    }

    /// <summary>
    /// Create a new plan (Admin only)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreatePlan([FromBody] CreatePlanRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { error = "Plan name is required" });
        }

        if (request.Limit <= 0)
        {
            return BadRequest(new { error = "Limit must be greater than 0" });
        }

        if (request.Price < 0)
        {
            return BadRequest(new { error = "Price cannot be negative" });
        }

        try
        {
            var plan = await _planService.CreatePlanAsync(request.Name, request.Limit, request.Price);

            return CreatedAtAction(nameof(GetPlan), new { planId = plan.Id }, new
            {
                id = plan.Id,
                name = plan.Name,
                limit = plan.Limit,
                price = plan.Price
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Error creating plan: {ex.Message}" });
        }
    }

    /// <summary>
    /// Update an existing plan (Admin only)
    /// </summary>
    [HttpPut("{planId}")]
    public async Task<IActionResult> UpdatePlan(int planId, [FromBody] UpdatePlanRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { error = "Plan name is required" });
        }

        if (request.Limit <= 0)
        {
            return BadRequest(new { error = "Limit must be greater than 0" });
        }

        if (request.Price < 0)
        {
            return BadRequest(new { error = "Price cannot be negative" });
        }

        try
        {
            var plan = await _planService.UpdatePlanAsync(planId, request.Name, request.Limit, request.Price);
            if (plan == null)
            {
                return NotFound(new { error = "Plan not found" });
            }

            return Ok(new
            {
                id = plan.Id,
                name = plan.Name,
                limit = plan.Limit,
                price = plan.Price
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Error updating plan: {ex.Message}" });
        }
    }

    /// <summary>
    /// Delete a plan (Admin only)
    /// </summary>
    [HttpDelete("{planId}")]
    public async Task<IActionResult> DeletePlan(int planId)
    {
        try
        {
            var success = await _planService.DeletePlanAsync(planId);
            if (!success)
            {
                return BadRequest(new { error = "Cannot delete plan. Either plan not found or users are using this plan." });
            }

            return Ok(new { message = "Plan deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Error deleting plan: {ex.Message}" });
        }
    }
}

// Request DTOs
public class ChangePlanRequest
{
    public int NewPlanId { get; set; }
}

public class CreatePlanRequest
{
    public string Name { get; set; } = string.Empty;
    public int Limit { get; set; }
    public int Price { get; set; }
}

public class UpdatePlanRequest
{
    public string Name { get; set; } = string.Empty;
    public int Limit { get; set; }
    public int Price { get; set; }
}

