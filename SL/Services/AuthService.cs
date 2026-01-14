using CodeFirst.Models;
using DAL.Repositories;

namespace SL.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Email == email && u.Password == password);
        
        return user;
    }

    public async Task<User?> RegisterAsync(string email, string password, int planId = 1, string role = "user")
    {
        var users = await _userRepository.GetAllAsync();
        var existingUser = users.FirstOrDefault(u => u.Email == email);
        if (existingUser != null)
        {
            return null;
        }

        var user = new User
        {
            Email = email,
            Password = password,
            PlanId = planId,
            Role = role
        };

        return await _userRepository.InsertAsync(user);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }
}

