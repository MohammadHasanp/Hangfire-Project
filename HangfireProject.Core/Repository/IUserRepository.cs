using HangfireProject.Data.Context;
using HangfireProject.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HangfireProject.Core.Repository;

public interface IUserRepository
{
    public Task Create(User user);
    public Task Edit(User user);
    public Task Delete(int userId);
    public Task<User?> GetById(int userId);
    public Task<List<User?>> GetAllUser();
}

public class UserRepository(HangfireDbContext context) : IUserRepository
{
    public async Task Create(User user)
    {
        await context.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task Edit(User user)
    {
        var result = await context.Users.FirstOrDefaultAsync(d => d.Id == user.Id);
        if (result == null)
            return;

        result.Name = user.Name;
        result.Family = user.Family;
        result.Password = user.Password;

        context.Update(result);
        await context.SaveChangesAsync();
    }
    public async Task Delete(int userId)
    {
        var result = await context.Users.FirstOrDefaultAsync(d => d.Id == userId);
        if (result == null)
            return;

        context.Remove(result);
        await context.SaveChangesAsync();
    }

    public async Task<User?> GetById(int userId)
    {
        return await context.Users.FirstOrDefaultAsync(d => d.Id == userId);
    }

    public async Task<List<User?>> GetAllUser()
    {
        var result= await context.Users.ToListAsync();
        return result;
    }
}