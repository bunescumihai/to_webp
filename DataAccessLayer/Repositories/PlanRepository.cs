using Microsoft.EntityFrameworkCore;
using NivelAccesDate_CodeFirst.Data;
using NivelAccesDate_CodeFirst.Models;

namespace DataAccessLayer.Repositories;

public class PlanRepository : Repository<Plan>, IPlanRepository
{
    public PlanRepository(ConversionsDbContext context) : base(context)
    {
    }

    public override async Task<Plan?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<IEnumerable<Plan>> GetAllAsync()
    {
        return await _dbSet
            .Include(p => p.Users)
            .ToListAsync();
    }
}

