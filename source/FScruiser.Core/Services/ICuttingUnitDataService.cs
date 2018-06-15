using CruiseDAL.DataObjects;
using FScruiser.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface ICuttingUnitDataService
    {
        #region

        Tree CreateTree(string stratumCode);

        Tree CreateTree(TallyPopulation tallyPopulation);

        TreeEstimateDO LogTreeEstimate(CountTreeDO count, int kpi);

        Task RefreshDataAsync(bool force = false);

        #endregion

        CuttingUnit Unit { get; }

        IEnumerable<Stratum> Strata { get; }

        IEnumerable<SampleGroup> SampleGroups { get; }

        IEnumerable<TallyPopulation> TallyPopulations { get; }

        IEnumerable<TreeFieldSetupDO> TreeFields { get; }

        IEnumerable<TallyEntry> TallyFeed { get; }

        IEnumerable<CountTree> Counts { get; }

        Dictionary<string, IEnumerable<TreeDefaultValueDO>> TreeDefaultSampleGroupLookup { get; }

        IEnumerable<TreeFieldSetupDO> GetSimplifiedTreeFieldsByStratumCode(string stratumCode);

        #region update methods

        void UpdateTree(Tree tree);

        Task UpdateTreeAsync(Tree tree);

        #endregion

        void AddTallyEntry(TallyEntry tallyEntry);

        void AddTree(Tree tree);

        #region insert methods

        void InsertTree(Tree tree);

        TallyPopulation GetCount(long countCN);

        //Tree GetTree(long treeCN);

        Tree GetTree(int treeNumber);

        IEnumerable<Tree> GetTrees();

        TreeEstimateDO GetTreeEstimate(long treeEstimateCN);

        void UpdateCount(CountTree count);

        #endregion

        void LogMessage(string message, string level);
    }
}