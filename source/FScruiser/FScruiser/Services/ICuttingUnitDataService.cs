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
        void AddTree(Tree tree);

        Plot CreateNewPlot(string stratumCode);

        Tree CreateNewTree(TallyPopulation tallyPop, long? plotCN = null);

        Tree CreateNewTree(string stratumCode, string sampleGroupCode, string species);

        void LogTreeEstimate(int KPI, TallyPopulation tallyPop);

        void LogTreeEstimate(int KPI, string stratumCode, string sampleGroupCode, string species);

        IEnumerable<UnitStratum> GetStrata();

        IEnumerable<TreeProxy> GetAllTreeProxiesInUnit();

        IEnumerable<TreeProxy> GetTreeProxiesByStratum(string code);

        Tree GetTree(long tree_CN);

        Tree GetTree(Guid tree_GUID);

        Plot GetPlot(string stratumCode, int plotNumber);

        //IEnumerable<PlotProxy> GetPlotProxiesByStratum(string code);

        IEnumerable<TreeField> GetTreeFieldsByStratum(string code);

        IEnumerable<TallyPopulation> GetTallyPopulationByStratum(string code);

        Sampler GetSamplerBySampleGroup(string stCode, string sgCode);
    }
}