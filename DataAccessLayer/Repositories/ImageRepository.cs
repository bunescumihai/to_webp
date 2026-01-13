using Microsoft.EntityFrameworkCore;
using NivelAccesDate_CodeFirst.Data;
using NivelAccesDate_CodeFirst.Models;

namespace DataAccessLayer.Repositories;

public class ImageRepository : Repository<Image>, IImageRepository
{
    public ImageRepository(ConversionsDbContext context) : base(context)
    {
    }

    public override async Task<Image?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(i => i.ConversionsFrom)
            .Include(i => i.ConversionsTo)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public override async Task<IEnumerable<Image>> GetAllAsync()
    {
        return await _dbSet
            .Include(i => i.ConversionsFrom)
            .Include(i => i.ConversionsTo)
            .ToListAsync();
    }
}

