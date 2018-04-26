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

        void RefreshData(bool force = false);

        #endregion

        CuttingUnit Unit { get; }

        IEnumerable<StratumDO> Strata { get; }

        IEnumerable<SampleGroup> SampleGroups { get; }

        IEnumerable<TallyPopulation> TallyPopulations { get; }

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