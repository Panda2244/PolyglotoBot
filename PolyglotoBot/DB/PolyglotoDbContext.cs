using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PolyglotoBot.Models.DBModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PolyglotoBot.DB
{
    public class PolyglotoDbContext : DbContext
    {
        private readonly string DbName = "PolyglotoSqlLite.db";

        public DbSet<EnRuDictionary> EnRuDictionary { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={DbName}", options =>
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

            modelBuilder.Entity<UserConfigurations>(entity =>
            {
                entity.HasKey(e => e.ChatId);
                entity.HasIndex(e => e.ChatId).IsUnique();
                entity.Property(e => e.WordCount);
                entity.Property(e => e.RetryCount);
            });

            modelBuilder.Seed();

            base.OnModelCreating(modelBuilder);
        }
    }
}
