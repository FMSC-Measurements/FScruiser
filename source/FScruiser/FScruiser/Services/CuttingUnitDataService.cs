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

        public CuttingUnitModel Unit { get; private set; }

        public CuttingUnitDataService(CuttingUnitModel unit, CruiseFile dataStore)
        {
            Unit = unit;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }

        //public DbSet<PlotProxy> PlotProxies { get; set; }

        public DbSet<Plot> Plots { get; set; }

        public DbSet<Sampler> Samplers { get; set; }

        public DbSet<UnitStratum> Strata { get; set; }

        public DbSet<SampleGroup> SampleGroup { get; set; }

        public DbSet<TallyPopulation> TallyPopulations { get; set; }

        public DbSet<TreeField> TreeFields { get; set; }

        public DbSet<TreeProxy> TreeProxies { get; set; }

        public DbSet<Tree> Trees { get; set; }

        public DbSet<TreeEstimate> TreeEstimates { get; set; }

        public Plot CreateNewPlot(string stratumCode)
        {
            var stratum = Strata.Where(s => s.Stratum.Code == stratumCode).FirstOrDefault();

            var highestPlotNumber = stratum.Plots.Max(p => p.PlotNumber);

            var newPlot = new Plot
            {
                CuttingUnit_CN = stratum.CuttingUnit_CN,
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

        public IEnumerable<TreeProxy> GetAllTreeProxiesInUnit()
        {
            return TreeProxies.Where(t => t.CuttingUnit_CN == Unit.CuttingUnit_CN);
        }

        public Plot GetPlot(string stratumCode, int plotNumber)
        {
            return Strata.Where(s => s.Stratum.Code == stratumCode).FirstOrDefault()
                .Plots.Where(p => p.PlotNumber == plotNumber).FirstOrDefault();

            //return Plots.Where(p => p.Stratum.Code == stratumCode && p.PlotNumber == plotNumber).FirstOrDefault();

            //return DataStore.From<Plot>()
            //    .Join("Stratum", "USING (Stratum_CN)")
            //    .Where($"Stratum.Code = '{stratumCode}' AND CuttingUnit_CN = {Unit.CuttingUnit_CN} AND PlotNumber = {plotNumber}")
            //    .Read().FirstOrDefault();
        }

        public Sampler GetSamplerBySampleGroup(string stCode, string sgCode)
        {
            return Samplers.Where(s => s.StratumCode == stCode && s.SampleGroupCode == sgCode).FirstOrDefault();

            //return DataStore.From<Sampler>()
            //    .Where($"SampleGroupCode = '{sgCode}' AND StratumCode = '{stCode}'")
            //    .Read().FirstOrDefault();
        }

        public IEnumerable<UnitStratum> GetStrata()
        {
            return Strata;

            //return DataStore.From<UnitStratum>()
            //    .Where($"CuttingUnit_CN = {Unit.CuttingUnit_CN}")
            //    .Read();
        }

        public IEnumerable<TallyPopulation> GetTallyPopulationByStratum(string code)
        {
            return TallyPopulations.Where(tp => tp.SampleGroup.Stratum.Code == code);

            //return DataStore.From<TallyPopulation>()
            //    .Where($"StratumCode = '{code}' AND CuttingUnit_CN = {Unit.CuttingUnit_CN}")
            //    .Read();
        }

        public Tree GetTree(Guid tree_GUID)
        {
            return Trees.Where(t => t.Tree_GUID == tree_GUID).FirstOrDefault();

            //return DataStore.From<Tree>()
            //    .Where("Tree_GUID = ?1").Read(tree_GUID).FirstOrDefault();
        }

        public Tree GetTree(long tree_CN)
        {
            return Trees.Where(t => t.Tree_CN == tree_CN).FirstOrDefault();

            //return DataStore.From<Tree>()
            //    .Where($"Tree_CN = {tree_CN}").Read().FirstOrDefault();
        }

        public IEnumerable<TreeField> GetTreeFieldsByStratum(string code)
        {
            return TreeFields.Where(tf => tf.Stratum.Code == code);

            //return DataStore.From<TreeField>()
            //    .Join("Stratum", "USING (Stratum_CN)")
            //    .Where($"Stratum.Code = '{code}'")
            //    .Read();
        }

        public IEnumerable<TreeProxy> GetTreeProxiesByStratum(string code)
        {
            return TreeProxies.Where(t => t.Stratum.Code == code);

            //return DataStore.From<TreeProxy>()
            //    .Where($"StratumCode = '{code}'")
            //    .Read();
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