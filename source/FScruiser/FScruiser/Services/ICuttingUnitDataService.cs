using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface ICuttingUnitDataService
    {
        CuttingUnitModel Unit { get; }

        IEnumerable<Tree> Trees { get; }

        IEnumerable<TreeProxy> TreeProxies { get; }

        IEnumerable<TreeField> TreeFields { get; }

        IEnumerable<PlotProxy> PlotProxies { get; }

        IEnumerable<Plot> Plots { get; }

        IEnumerable<UnitStratum> Strata { get; }

        IEnumerable<TallyPopulation> TallyPopulations { get; }

        IEnumerable<Sampler> Samplers { get; }

        Plot CreateNewPlot(string stratumCode);

        Tree CreateNewTree(TallyPopulation tallyPop);

        Tree CreateNewTree(string stratumCode, string sampleGroupCode, string species);

        void LogTreeEstimate(int KPI, TallyPopulation tallyPop);

        void LogTreeEstimate(int KPI, string stratumCode, string sampleGroupCode, string species);

        IEnumerable<UnitStratum> GetStrata();

        IEnumerable<TreeProxy> GetAllTreeProxiesInUnit();

        IEnumerable<TreeProxy> GetTreeProxiesByStratum(string code);

        Tree GetTree(long tree_CN);

        Tree GetTree(Guid tree_GUID);

        Plot GetPlot(string stratumCode, int plotNumber);

        IEnumerable<PlotProxy> GetPlotProxiesByStratum(string code);

        IEnumerable<TreeField> GetTreeFieldsByStratum(string code);

        IEnumerable<TallyPopulation> GetTallyPopulationByStratum(string code);

        Sampler GetSamplerBySampleGroup(string code);
    }
}