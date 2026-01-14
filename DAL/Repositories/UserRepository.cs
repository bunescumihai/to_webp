using CodeFirst.Data;
using CodeFirst.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ConversionsDbContext context) : base(context)
    {
    }

    public override async Task<User?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(u => u.Plan)
            .Include(u => u.Conversions)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public override async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dbSet
            .Include(u => u.Plan)
            .Include(u => u.Conversions)
            .ToListAsync();
    }
}

