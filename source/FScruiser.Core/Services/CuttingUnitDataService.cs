using FScruiser.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public class CuttingUnitDataService : DbContext, ICuttingUnitDataService
    {
        string _connectionString;

        public CuttingUnit Unit { get; private set; }

        public CuttingUnitDataService(CuttingUnit unit, CruiseFile dataStore)
        {
            _connectionString = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder
            {
                DataSource = dataStore.Path
            }.ToString();

            Unit = unit;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Plot>()
                .Property(p => p.IsEmpty)
                .HasDefaultValue(false);
        }

        //public DbSet<PlotProxy> PlotProxies { get; set; }

        public DbSet<Plot> Plots { get; set; }

        //public DbSet<Sampler> Samplers { get; set; }

        public DbSet<UnitStratum> UnitStrata { get; set; }

        public DbSet<SampleGroup> SampleGroup { get; set; }

        public DbSet<TallyPopulation> TallyPopulations { get; set; }

        public DbSet<TreeField> TreeFields { get; set; }

        //public DbSet<TreeProxy> TreeProxies { get; set; }

        public DbSet<Tree> Trees { get; set; }

        public DbSet<TreeEstimate> TreeEstimates { get; set; }

        public Plot CreateNewPlot(string stratumCode)
        {
            var highestPlotNumber = Plots
                .Where(p => p.CuttingUnit_CN == Unit.CuttingUnit_CN && p.Stratum.Code == stratumCode)
                .Max(p => p.PlotNumber);

            var newPlot = new Plot
            {
                CuttingUnit_CN = Unit.CuttingUnit_CN,
                Stratum_CN = stratum.Stratum_CN,
                PlotNumber = highestPlotNumber + 1
            };

            return newPlot;
        }

        public Tree CreateNewTree(TallyPopulation tallyPop, long? plotCN = null)
        {
            var tree = new Tree()
            {
                CuttingUnit_CN = Unit.CuttingUnit_CN,
                Stratum_CN = tallyPop.SampleGroup.Stratum_CN,
                SampleGroup_CN = tallyPop.SampleGroup_CN,
                TreeDefaultValue_CN = tallyPop.TreeDefaultValue_CN
            };

            if (plotCN != null)
            {
                tree.Plot_CN = plotCN.Value;
                tree.TreeCount = 1;
            }

            return tree;
        }

        public Tree CreateNewTree(string stratumCode, string sampleGroupCode, string species)
        {
            throw new NotImplementedException();
        }

        public void AddTree(Tree tree)
        {
            Trees.Add(tree);
        }

        public Plot GetPlot(string stratumCode, int plotNumber)
        {
            return Plots.Where(p => p.Stratum.Code == stratumCode && p.PlotNumber == plotNumber).FirstOrDefault();
        }

        public IEnumerable<UnitStratum> GetStrata()
        {
            return UnitStrata.Where(uSt => uSt.CuttingUnit_CN == Unit.CuttingUnit_CN);
        }

        public IEnumerable<TallyPopulation> GetTallyPopulationByStratum(string code)
        {
            return TallyPopulations.Where(tp => tp.CuttingUnit_CN == Unit.CuttingUnit_CN
            && tp.SampleGroup.Stratum.Code == code);
        }

        public Tree GetTree(Guid tree_GUID)
        {
            return Trees.Where(t => t.Tree_GUID == tree_GUID).FirstOrDefault();
        }

        public Tree GetTree(long tree_CN)
        {
            return Trees.Where(t => t.Tree_CN == tree_CN).FirstOrDefault();
        }

        public IEnumerable<Tree> GetAllTrees()
        {
            return Trees.Where(t => t.CuttingUnit_CN == Unit.CuttingUnit_CN);
        }

        public IEnumerable<Tree> GetTreeByStratum(string stratumCode)
        {
            return Trees.Where(t => t.CuttingUnit_CN == Unit.CuttingUnit_CN
            && t.Stratum.Code == stratumCode);
        }

        public IEnumerable<TreeField> GetTreeFieldsByStratum(string code)
        {
            return TreeFields.Where(tf => tf.Stratum.Code == code);
        }

        public void LogTreeEstimate(int kpi, TallyPopulation tallyPop)
        {
            var est = new TreeEstimate
            {
                CountTree_CN = tallyPop.CountTree_CN,
                KPI = kpi
            };

            TreeEstimates.Add(est);
        }

        public void LogTreeEstimate(int kpi, string stratumCode, string sampleGroupCode, string species = null)
        {
            TallyPopulation tallyPop = null;
            if (species == null)
            {
                tallyPop = TallyPopulations.Where(tp => tp.SampleGroup.Stratum.Code == stratumCode
                && tp.SampleGroup.Code == sampleGroupCode).FirstOrDefault();
            }
            else
            {
                tallyPop = TallyPopulations.Where(tp => tp.SampleGroup.Stratum.Code == stratumCode
                && tp.SampleGroup.Code == sampleGroupCode
                && tp.TDV.Species == species).FirstOrDefault();
            }

            LogTreeEstimate(kpi, tallyPop);
        }
    }
}