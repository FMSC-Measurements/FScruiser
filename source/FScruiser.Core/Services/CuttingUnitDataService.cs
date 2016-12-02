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
                .HasOne(p => p.UnitStratum)
                .WithMany(us => us.Plots)
                .HasForeignKey(p => new { p.CuttingUnit_CN, p.Stratum_CN })
                .HasPrincipalKey(us => new { us.CuttingUnit_CN, us.Stratum_CN });
        }

        protected DbSet<Plot> Plots { get; set; }

        protected DbSet<UnitStratum> UnitStrata { get; set; }

        protected DbSet<SampleGroup> SampleGroups { get; set; }

        protected DbSet<Stratum> Strata { get; set; }

        protected DbSet<TallyPopulation> TallyPopulations { get; set; }

        protected DbSet<TreeField> TreeFields { get; set; }

        protected DbSet<Tree> Trees { get; set; }

        protected DbSet<TreeEstimate> TreeEstimates { get; set; }

        //protected DbSet<TreeDefaultValue> TreeDefaults { get; set; }

        protected DbSet<SampleGroupTreeDefaultValue> SgTDVs { get; set; }

        public Plot CreateNewPlot(string stratumCode)
        {
            var unitStratum = UnitStrata.Where(uSt => uSt.CuttingUnit_CN == Unit.CuttingUnit_CN && uSt.Stratum.Code == stratumCode)
                .FirstOrDefault();

            var highestPlotNumber = unitStratum.Plots.Max(p => p.PlotNumber);

            var newPlot = new Plot
            {
                PlotNumber = highestPlotNumber + 1
            };

            unitStratum.Plots.Add(newPlot);

            Plots.Add(newPlot);

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

            Trees.Add(tree);

            return tree;
        }

        public Tree CreateNewTree(Stratum stratum, SampleGroup sampleGroup, TreeDefaultValue tdv = null, Plot plot = null)
        {
            var tree = new Tree
            {
                CuttingUnit_CN = Unit.CuttingUnit_CN,
                Stratum_CN = stratum.Stratum_CN
            };

            if (sampleGroup == null)
            { tree.SampleGroup_CN = sampleGroup.SampleGroup_CN; }

            if (plot != null)
            { tree.Plot_CN = plot.Plot_CN; }

            Trees.Add(tree);

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

        public IEnumerable<UnitStratum> GetAllUnitStrata()
        {
            return UnitStrata
                .Include(ust => ust.Stratum)
                    .ThenInclude(st => st.SampleGroups)
                .Include(ust => ust.Plots)
                .Where(uSt => uSt.CuttingUnit_CN == Unit.CuttingUnit_CN);
        }

        public UnitStratum GetUnitStratum(string stratumCode)
        {
            return UnitStrata
                .Include(ust => ust.Stratum)
                    .ThenInclude(st => st.SampleGroups)
                .Include(ust => ust.Plots)
                .Where(ust => ust.CuttingUnit_CN == Unit.CuttingUnit_CN && ust.Stratum.Code == stratumCode)
                .FirstOrDefault();
        }

        public IEnumerable<TallyPopulation> GetTallyPopulationByStratum(string code)
        {
            return TallyPopulations
                .Include(tp => tp.TDV)
                .Include(tp => tp.Tally)
                .Include(tp => tp.SampleGroup)
                    .ThenInclude(sg => sg.Stratum)
                .Where(tp => tp.CuttingUnit_CN == Unit.CuttingUnit_CN
            && tp.SampleGroup.Stratum.Code == code);
        }

        public Tree GetTree(Guid tree_GUID)
        {
            return Trees
                .Include(t => t.Stratum)
                .Include(t => t.SampleGroup)
                .Where(t => t.Tree_GUID == tree_GUID).FirstOrDefault();
        }

        public Tree GetTree(long tree_CN)
        {
            return Trees
                .Include(t => t.Stratum)
                .Include(t => t.SampleGroup)
                .Where(t => t.Tree_CN == tree_CN).FirstOrDefault();
        }

        public IEnumerable<Tree> GetAllTrees()
        {
            return Trees
                .Include(t => t.Stratum)
                .Include(t => t.SampleGroup)
                .Where(t => t.CuttingUnit_CN == Unit.CuttingUnit_CN);
        }

        public IEnumerable<Tree> GetTrees(Stratum stratum, Plot plot = null)
        {
            var resultSet = GetAllTrees();

            if (stratum != null)
            {
                resultSet = resultSet.Where(t => t.Stratum_CN == stratum.Stratum_CN);
            }
            if (plot != null)
            {
                resultSet = resultSet.Where(t => t.Plot_CN == plot.Plot_CN);
            }

            return resultSet;
        }

        public IEnumerable<TreeField> GetTreeFieldsByStratum(string code)
        {
            return TreeFields.Where(tf => tf.Stratum.Code == code);
        }

        public IEnumerable<TreeDefaultValue> GetTreeDefaultsBySampleGroup(SampleGroup sampleGroup)
        {
            return SgTDVs
                    .Include(sgtdv => sgtdv.TDV)
                .Where(sgtdv => sgtdv.SampleGroup_CN == sampleGroup.SampleGroup_CN)
                .Select(sgtdv => sgtdv.TDV);
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

        public IEnumerable<SampleGroup> GetSampleGroupsByStratum(Stratum stratum)
        {
            return SampleGroups.Where(sg => sg.Stratum.Stratum_CN == stratum.Stratum_CN);
        }
    }
}