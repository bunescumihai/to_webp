using Microsoft.EntityFrameworkCore;
using NivelAccesDate_CodeFirst.Data;
using NivelAccesDate_CodeFirst.Models;

namespace ServiceLayer.Services;

public class AuthService
{
    private readonly ConversionsDbContext _context;

    public AuthService(ConversionsDbContext context)
    {
        _context = context;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        
        return user;
    }

    public async Task<User?> RegisterAsync(string email, string password, int planId = 1, string role = "user")
    {
        // Check if user already exists
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser != null)
        {
            return null; // User already exists
        }

        var user = new User
        {
            Email = email,
            Password = password,
            PlanId = planId,
            Role = role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }
}

