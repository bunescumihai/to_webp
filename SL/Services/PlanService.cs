using CodeFirst.Models;
using DAL.Repositories;

namespace SL.Services;

public class PlanService
{
    private readonly IPlanRepository _planRepository;
    private readonly IUserRepository _userRepository;

    public PlanService(IPlanRepository planRepository, IUserRepository userRepository)
    {
        _planRepository = planRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<Plan>> GetAllPlansAsync()
    {
        return await _planRepository.GetAllAsync();
    }

    public async Task<Plan?> GetPlanByIdAsync(int id)
    {
        return await _planRepository.GetByIdAsync(id);
    }

    public async Task<Plan> CreatePlanAsync(string name, int limit, int price)
    {
        var plan = new Plan
        {
            Name = name,
            Limit = limit,
            Price = price
        };

        return await _planRepository.InsertAsync(plan);
    }

    public async Task<Plan?> UpdatePlanAsync(int id, string name, int limit, int price)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
        {
            return null;
        }

        plan.Name = name;
        plan.Limit = limit;
        plan.Price = price;

        return await _planRepository.UpdateAsync(plan);
    }

    public async Task<bool> DeletePlanAsync(int id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
        {
            return false;
        }

        // Check if any users are using this plan
        var allUsers = await _userRepository.GetAllAsync();
        var usersCount = allUsers.Count(u => u.PlanId == id);
        if (usersCount > 0)
        {
            return false; // Cannot delete plan with active users
        }

        return await _planRepository.DeleteByIdAsync(id);
    }

    public async Task<User?> ChangeUserPlanAsync(int userId, int newPlanId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        var newPlan = await _planRepository.GetByIdAsync(newPlanId);
        if (newPlan == null)
        {
            return null;
        }

        user.PlanId = newPlanId;
        return await _userRepository.UpdateAsync(user);
    }

    public async Task<Plan?> GetUserPlanAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        return await _planRepository.GetByIdAsync(user.PlanId);
    }
}
