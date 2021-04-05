using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PolyglotoBot.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PolyglotoBot.DB
{
    public class PolyglotoDbContext : DbContext
    {
        public DbSet<EnRuDictionary> EnRuDictionary { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=PolyglotoSqlLite.db", options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EnRuDictionary>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Id).IsUnique();
                entity.Property(e => e.EnWord);
                entity.Property(e => e.RuWord);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
