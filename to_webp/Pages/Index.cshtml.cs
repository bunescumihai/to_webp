using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NivelAccesDate_CodeFirst.Data;
using NivelAccesDate_CodeFirst.Models;

namespace to_webp.Pages;

public class IndexModel : PageModel
{
    private readonly ConversionsDbContext _context;
    
    public string ConnectionStatus { get; set; } = "";
    public List<User> Users { get; set; } = new();
    public List<Conversion> Conversions { get; set; } = new();

    public IndexModel(ConversionsDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        try
        {
            // Test database connection
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (canConnect)
            {
                ConnectionStatus = "Database connection successful!";
                
                // Load some data to verify
                Users = await _context.Users.Include(u => u.Plan).ToListAsync();
                Conversions = await _context.Conversions.Include(c => c.User).ToListAsync();
            }
            else
            {
                ConnectionStatus = "Database connection failed!";
            }
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Error: {ex.Message}";
        }
    }
}