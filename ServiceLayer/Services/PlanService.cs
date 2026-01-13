using NivelAccesDate_CodeFirst.Data;
using NivelAccesDate_CodeFirst.Models;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer.Services;

public class PlanService
{
    private readonly ConversionsDbContext _context;

    public PlanService(ConversionsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Plan>> GetAllPlansAsync()
    {
        return await _context.Plans
            .Include(p => p.Users)
            .ToListAsync();
    }

    public async Task<Plan?> GetPlanByIdAsync(int id)
    {
        return await _context.Plans
            .Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Plan> CreatePlanAsync(string name, int limit, int price)
    {
        var plan = new Plan
        {
            Name = name,
            Limit = limit,
            Price = price
        };

        _context.Plans.Add(plan);
        await _context.SaveChangesAsync();

        return plan;
    }

    public async Task<Plan?> UpdatePlanAsync(int id, string name, int limit, int price)
    {
        var plan = await _context.Plans.FindAsync(id);
        if (plan == null)
        {
            return null;
        }

        plan.Name = name;
        plan.Limit = limit;
        plan.Price = price;

        await _context.SaveChangesAsync();

        return plan;
    }

    public async Task<bool> DeletePlanAsync(int id)
    {
        var plan = await _context.Plans.FindAsync(id);
        if (plan == null)
        {
            return false;
        }

        // Check if any users are using this plan
        var usersCount = await _context.Users.CountAsync(u => u.PlanId == id);
        if (usersCount > 0)
        {
            return false; // Cannot delete plan with active users
        }

        _context.Plans.Remove(plan);
        await _context.SaveChangesAsync();

        return true;
    }
}

