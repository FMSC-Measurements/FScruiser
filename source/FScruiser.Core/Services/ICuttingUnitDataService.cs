using CruiseDAL.DataObjects;
using FScruiser.Models;
using System.Collections.Generic;

namespace FScruiser.Services
{
    public interface ICuttingUnitDataService
    {
        #region

        Tree CreateTree(TallyPopulation tallyPopulation);

        TreeEstimateDO LogTreeEstimate(CountTreeDO count, int kpi);

        #endregion

        #region query methods

        IEnumerable<UnitStratum> QueryStrataByUnitCode(string unitCode);

        IEnumerable<TreeFieldSetupDO> GetTreeFieldsByStratum(string stratumCode);

        IEnumerable<SampleGroup> GetSampleGroupsByStratum(string stratumCode);

        IEnumerable<TreeDefaultValueDO> GetTreeDefaultsBySampleGroup(string sampleGroupCode);

        IEnumerable<TallyPopulation> GetTalliesByStratum(string stratumCode);

        #endregion query methods

        #region update methods

        void UpdateTree(Tree tree);

        #endregion

        #region insert methods

        void InsertTree(Tree tree);

        void InsertTreeEstimate(TreeEstimateDO treeEstimate);
        TallyPopulation GetCount(long countCN);
        Tree GetTree(long treeCN);
        TreeEstimateDO GetTreeEstimate(long treeEstimateCN);
        void UpdateCount(TallyPopulation count);

        #endregion
    }
}