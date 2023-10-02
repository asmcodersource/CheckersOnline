using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using CheckersOnlineSPA.Data;

namespace CheckersOnlineSPA.Data
{
    public class DatabaseContext: DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Games> Games { get; set; } = null!;

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
