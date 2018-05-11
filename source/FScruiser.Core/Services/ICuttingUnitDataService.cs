using CruiseDAL.DataObjects;
using FScruiser.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface ICuttingUnitDataService
    {
        #region

        Tree CreateTree(TallyPopulation tallyPopulation);

        TreeEstimateDO LogTreeEstimate(CountTreeDO count, int kpi);

        Task RefreshDataAsync(bool force = false);

        #endregion

        CuttingUnit Unit { get; }

        IEnumerable<Stratum> Strata { get; }

        IEnumerable<SampleGroup> SampleGroups { get; }

        IEnumerable<TallyPopulation> TallyPopulations { get; }

        IEnumerable<TreeFieldSetupDO> TreeFields { get; }

        IList<Tree> Trees { get; }

        List<TallyFeedItem> TallyFeed { get; }

        #region update methods

        void UpdateTree(Tree tree);

        Task UpdateTreeAsync(Tree tree);

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