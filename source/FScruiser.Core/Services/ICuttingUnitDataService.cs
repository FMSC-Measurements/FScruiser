using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface ICuttingUnitDataService : IDataService
    {
        void AddTree(Tree tree);

        Plot CreateNewPlot(string stratumCode);

        Tree CreateNewTree(TallyPopulation tallyPop, long? plotCN = null);

        Tree CreateNewTree(string stratumCode, string sampleGroupCode, string species);

        Tree CreateNewTree(Stratum stratum, SampleGroup sampleGroup, TreeDefaultValue tdv = null, Plot plot = null);

        void LogTreeEstimate(int KPI, TallyPopulation tallyPop);

        void LogTreeEstimate(int KPI, string stratumCode, string sampleGroupCode, string species);

        IEnumerable<UnitStratum> GetAllUnitStrata();

        #region Tree

        Tree GetTree(long tree_CN);

        Tree GetTree(Guid tree_GUID);

        IEnumerable<Tree> GetAllTrees();

        IEnumerable<Tree> GetTrees(Stratum stratum, Plot plot = null);

        #endregion Tree

        Plot GetPlot(string stratumCode, int plotNumber);

        IEnumerable<TreeField> GetTreeFieldsByStratum(string code);

        IEnumerable<TallyPopulation> GetTallyPopulationByStratum(string code);

        IEnumerable<TreeDefaultValue> GetTreeDefaultsBySampleGroup(SampleGroup sg);

        IEnumerable<SampleGroup> GetSampleGroupsByStratum(Stratum stratum);
    }
}