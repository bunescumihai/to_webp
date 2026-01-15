using CodeFirst.Models;
using DAL.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace SL.Services;

public class PlanService
{
    private readonly IPlanRepository _planRepository;
    private readonly IUserRepository _userRepository;
    private readonly ConversionLogger _logger;
    private readonly IMemoryCache _cache;
    private const string PLANS_CACHE_KEY = "all_plans";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public PlanService(
        IPlanRepository planRepository, 
        IUserRepository userRepository, 
        ConversionLogger logger,
        IMemoryCache cache)
    {
        _planRepository = planRepository;
        _userRepository = userRepository;
        _logger = logger;
        _cache = cache;
    }

    public async Task<IEnumerable<Plan>> GetAllPlansAsync()
    {
        // Try to get from cache
        if (_cache.TryGetValue(PLANS_CACHE_KEY, out IEnumerable<Plan>? cachedPlans) && cachedPlans != null)
        {
            _logger.LogCacheHit(PLANS_CACHE_KEY);
            return cachedPlans;
        }

        // Not in cache, fetch from database
        _logger.LogCacheMiss(PLANS_CACHE_KEY);
        var plans = await _planRepository.GetAllAsync();
        
        // Store in cache
        _cache.Set(PLANS_CACHE_KEY, plans, CacheDuration);
        _logger.LogCacheSet(PLANS_CACHE_KEY, plans.Count(), CacheDuration.TotalMinutes);
        
        return plans;
    }

    public async Task<Plan?> GetPlanByIdAsync(int id)
    {
        // Get all plans from cache (this will use cached data if available)
        var plans = await GetAllPlansAsync();
        return plans.FirstOrDefault(p => p.Id == id);
    }

    public async Task<Plan> CreatePlanAsync(string name, int limit, int price)
    {
        var plan = new Plan
        {
            Name = name,
            Limit = limit,
            Price = price
        };

        plan = await _planRepository.InsertAsync(plan);
        
        // Clear cache
        _cache.Remove(PLANS_CACHE_KEY);
        _logger.LogCacheCleared(PLANS_CACHE_KEY, "Plan created");
        
        // Log plan creation
        _logger.LogPlanCreated(plan.Id, plan.Name, plan.Limit, plan.Price);
        
        return plan;
    }

    public async Task<Plan?> UpdatePlanAsync(int id, string name, int limit, int price)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
        {
            return null;
        }

        // Store old values for logging
        var oldName = plan.Name;
        var oldLimit = plan.Limit;
        var oldPrice = plan.Price;

        plan.Name = name;
        plan.Limit = limit;
        plan.Price = price;

        var updatedPlan = await _planRepository.UpdateAsync(plan);
        
        // Clear cache
        _cache.Remove(PLANS_CACHE_KEY);
        _logger.LogCacheCleared(PLANS_CACHE_KEY, "Plan updated");
        
        // Log plan update
        _logger.LogPlanUpdated(id, oldName, name, oldLimit, limit, oldPrice, price);
        
        return updatedPlan;
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

        var planName = plan.Name;
        var result = await _planRepository.DeleteByIdAsync(id);
        
        if (result)
        {
            // Clear cache
            _cache.Remove(PLANS_CACHE_KEY);
            _logger.LogCacheCleared(PLANS_CACHE_KEY, "Plan deleted");
            
            // Log plan deletion
            _logger.LogPlanDeleted(id, planName);
        }
        
        return result;
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

        // Get old plan info for logging
        var oldPlanId = user.PlanId;
        var oldPlan = await _planRepository.GetByIdAsync(oldPlanId);
        var oldPlanName = oldPlan?.Name ?? "Unknown";

        user.PlanId = newPlanId;
        var updatedUser = await _userRepository.UpdateAsync(user);
        
        // Log user plan change
        _logger.LogUserPlanChanged(userId, user.Email, oldPlanId, oldPlanName, newPlanId, newPlan.Name);
        
        return updatedUser;
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
