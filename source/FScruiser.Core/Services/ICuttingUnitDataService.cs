using CruiseDAL.DataObjects;
using FMSC.Sampling;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface ICuttingUnitDataService
    {
        CuttingUnit Unit { get; }

        IEnumerable<StratumProxy> GetStratumProxies();

        IEnumerable<TallyEntry> GetTallyEntries();

        IEnumerable<TallyPopulation> GetTallyPopulations();

        IEnumerable<TreeFieldSetupDO> GetTreeFieldsByStratumCode(string stratumCode);

        IEnumerable<TreeFieldSetupDO> GetSimplifiedTreeFieldsByStratumCode(string stratumCode);

        IEnumerable<SampleGroupProxy> GetSampleGroupProxies(string stratumCode);

        IEnumerable<TreeDefaultProxy> GetTreeDefaultProxies(string stratumCode, string sampleGroupCode);

        IEnumerable<SampleSelecter> GetSamplersBySampleGroupCode(string stratumCode, string sgCode);

        #region tree

        Tree GetTree(string tree_guid);

        TreeStub GetTreeStub(string tree_guid);

        IEnumerable<Tree> GetTrees();

        IEnumerable<TreeStub> GetTreeStubs();

        void UpdateTree(Tree tree);

        Task UpdateTreeAsync(Tree tree);

        string CreateTree(string stratumCode);

        string CreateTree(TallyPopulation tallyPopulation);

        void UpdateTreeInitials(string tree_GUID, string result);

        #endregion tree

        #region tally

        TallyEntry CreateTally(TallyPopulation population, int treeCount = 1, int kpi = 0, bool stm = false);

        TallyEntry CreateTallyWithTree(TallyPopulation population, string countOrMeasure, int treeCount = 1, int kpi = 0, bool stm = false);

        void DeleteTally(TallyEntry tallyEntry);

        #endregion tally

        void LogMessage(string message, string level);

        //TreeEstimateDO LogTreeEstimate(CountTreeDO count, int kpi);
    }
}