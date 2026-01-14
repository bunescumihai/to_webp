using CodeFirst.Data;
using CodeFirst.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class ConversionRepository : Repository<Conversion>, IConversionRepository
{
    public ConversionRepository(ConversionsDbContext context) : base(context)
    {
    }

    public override async Task<Conversion?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.ImageFrom)
            .Include(c => c.ImageTo)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public override async Task<IEnumerable<Conversion>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.ImageFrom)
            .Include(c => c.ImageTo)
            .ToListAsync();
    }
}

