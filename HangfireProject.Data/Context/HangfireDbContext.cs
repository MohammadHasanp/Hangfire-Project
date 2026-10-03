using HangfireProject.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HangfireProject.Data.Context;

public class HangfireDbContext(DbContextOptions<HangfireDbContext> options) : DbContext(options)
{


    #region DbSets

    public DbSet<User> Users { get; set; }

    #endregion

    #region Config

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    #endregion

}
