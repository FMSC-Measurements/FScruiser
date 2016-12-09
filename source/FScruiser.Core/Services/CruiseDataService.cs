using FScruiser.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public class CruiseDataService : DbContext, ICruiseDataService
    {
        public CruiseDataService(CruiseFile cf) : this(cf.Path)
        { }

        protected CruiseDataService(string path)
        {
            var builder = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder
            {
                DataSource = path
            };
            _connectionString = builder.ToString();
        }

        string _connectionString;

        public DbSet<Sale> Sale { get; protected set; }

        public DbSet<CuttingUnit> Units { get; protected set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }

        public Sale GetSale()
        {
            return Sale.FirstOrDefault();
        }

        public IEnumerable<CuttingUnit> GetUnits()
        {
            return Units;
        }
    }
}