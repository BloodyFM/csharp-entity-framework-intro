﻿using exercise.webapi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;

namespace exercise.webapi.Data;

public class DatabaseContext : DbContext
{
    private string _connectionString;

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        _connectionString = configuration.GetValue<string>("ConnectionStrings:DefaultConnectionString")!;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //base.OnConfiguring(optionsBuilder);
        //optionsBuilder.UseInMemoryDatabase("Library");
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.LogTo(message => Debug.WriteLine(message));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().HasMany(x => x.Books).WithOne(x => x.Author).HasForeignKey(x => x.AuthorId);

        Seeder seeder = new Seeder();

        modelBuilder.Entity<Author>().HasData(seeder.Authors);
        modelBuilder.Entity<Book>().HasData(seeder.Books);

    }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
}