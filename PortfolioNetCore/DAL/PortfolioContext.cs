using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using PortfolioNetCore.Core.Model;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace PortfolioNetCore.Persistence
{
    public class PortfolioContext : DbContext
    {
        public DbSet<Fund> Funds { get; set; }     
        public DbSet<Management> Managements { get; set; }

       // Database.SetInitializer<PortfolioContext>(new CreateDatabaseIfNotExists<PortfolioContext>());


        IHostingEnvironment _hostingEnvironment;

        private string _contentDirectoryPath = "";    

        public PortfolioContext(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
            _contentDirectoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data","portfolio.db");     
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source="+ _contentDirectoryPath);
        }

       // public PortfolioContext(DbContextOptions<PortfolioContext> options) : base(options) { }
    }

    //public VegaDbContext(DbContextOptions<VegaDbContext> options)
    //    : base(options)
    //{
    //}

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<VehicleFeature>().HasKey(vf =>
    //      new { vf.VehicleId, vf.FeatureId });
    //}

}


