using Backpack;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public class CuttingUnitDataService : ICuttingUnitDataService
    {
        public CuttingUnitModel Unit { get; private set; }
        public DatastoreRedux DataStore { get; private set; }

        public CuttingUnitDataService(CuttingUnitModel unit, DatastoreRedux dataStore)
        {
            Unit = unit;
            DataStore = dataStore;
        }

        public IEnumerable<PlotProxy> PlotProxies
        {
            get
            {
                return DataStore.GetEntityCache(typeof(PlotProxy)).Values.OfType<PlotProxy>();
            }
        }

        public IEnumerable<Plot> Plots
        {
            get
            {
                return DataStore.GetEntityCache(typeof(Plot)).Values.OfType<Plot>();
            }
        }

        public IEnumerable<Sampler> Samplers
        {
            get
            {
                return DataStore.GetEntityCache(typeof(Sampler)).Values.OfType<Sampler>();
            }
        }

        public IEnumerable<UnitStratum> Strata
        {
            get
            {
                return DataStore.GetEntityCache(typeof(UnitStratum)).Values.OfType<UnitStratum>();
            }
        }

        public IEnumerable<TallyPopulation> TallyPopulations
        {
            get
            {
                return DataStore.GetEntityCache(typeof(TallyPopulation)).Values.OfType<TallyPopulation>();
            }
        }

        public IEnumerable<TreeField> TreeFields
        {
            get
            {
                return DataStore.GetEntityCache(typeof(TreeField)).Values.OfType<TreeField>();
            }
        }

        public IEnumerable<TreeProxy> TreeProxies
        {
            get
            {
                return DataStore.GetEntityCache(typeof(TreeProxy)).Values.OfType<TreeProxy>();
            }
        }

        public IEnumerable<Tree> Trees
        {
            get
            {
                return DataStore.GetEntityCache(typeof(Tree)).Values.OfType<Tree>();
            }
        }

        public Plot CreateNewPlot(string stratumCode)
        {
            var stratum = Strata.Where(st => st.StratumCode == stratumCode).First();

            var highestPlotNumber = GetPlotProxiesByStratum(stratumCode).Max(p => p.PlotNumber);

            var newPlot = new Plot
            {
                CuttingUnit_CN = Unit.CuttingUnit_CN,
                Stratum_CN = stratum.Stratum_CN,
                PlotNumber = highestPlotNumber + 1
            };

            return newPlot;
        }

        public Tree CreateNewTree(TallyPopulation tallyPop)
        {
            throw new NotImplementedException();
        }

        public Tree CreateNewTree(string stratumCode, string sampleGroupCode, string species)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TreeProxy> GetAllTreeProxiesInUnit()
        {
            return DataStore.From<TreeProxy>().Where($"CuttingUnit_CN = {Unit.CuttingUnit_CN}").Read();
        }

        public Plot GetPlot(string stratumCode, int plotNumber)
        {
            return DataStore.From<Plot>()
                .Join("Stratum", "USING (Stratum_CN)")
                .Where($"Stratum.Code = '{stratumCode}' AND CuttingUnit_CN = {Unit.CuttingUnit_CN} AND PlotNumber = {plotNumber}")
                .Read().FirstOrDefault();
        }

        public IEnumerable<PlotProxy> GetPlotProxiesByStratum(string code)
        {
            return DataStore.From<PlotProxy>()
                .Join("Stratum", "USING (Stratum_CN)")
                .Where($"Stratum.Code = '{code}' AND CuttingUnit_CN = {Unit.CuttingUnit_CN}")
                .Read();
        }

        public Sampler GetSamplerBySampleGroup(string code)
        {
            return DataStore.From<Sampler>()
                .Where($"SampleGroupCode = '{code}'")
                .Read().FirstOrDefault();
        }

        public IEnumerable<UnitStratum> GetStrata()
        {
            return DataStore.From<UnitStratum>()
                .Where($"CuttingUnit_CN = {Unit.CuttingUnit_CN}")
                .Read();
        }

        public IEnumerable<TallyPopulation> GetTallyPopulationByStratum(string code)
        {
            return DataStore.From<TallyPopulation>()
                .Where($"StratumCode = '{code}' AND CuttingUnit_CN = {Unit.CuttingUnit_CN}")
                .Read();
        }

        public Tree GetTree(Guid tree_GUID)
        {
            return DataStore.From<Tree>()
                .Where("Tree_GUID = ?1").Read(tree_GUID).FirstOrDefault();
        }

        public Tree GetTree(long tree_CN)
        {
            return DataStore.From<Tree>()
                .Where($"Tree_CN = {tree_CN}").Read().FirstOrDefault();
        }

        public IEnumerable<TreeField> GetTreeFieldsByStratum(string code)
        {
            return DataStore.From<TreeField>()
                .Join("Stratum", "USING (Stratum_CN)")
                .Where($"Stratum.Code = '{code}'")
                .Read();
        }

        public IEnumerable<TreeProxy> GetTreeProxiesByStratum(string code)
        {
            return DataStore.From<TreeProxy>()
                .Where($"StratumCode = '{code}'")
                .Read();
        }

        public void LogTreeEstimate(int kpi, TallyPopulation tallyPop)
        {
            var est = new TreeEstimate
            {
                CountTree_CN = tallyPop.CountTree_CN,
                KPI = kpi
            };

            DataStore.Insert(est, Backpack.SQL.OnConflictOption.Default);
        }

        public void LogTreeEstimate(int kpi, string stratumCode, string sampleGroupCode, string species)
        {
            throw new NotImplementedException();
        }
    }
}