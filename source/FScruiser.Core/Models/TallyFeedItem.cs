using CruiseDAL.DataObjects;
using FScruiser.Services;

namespace FScruiser.Models
{
    public class TallyFeedItem
    {
        private Tree _tree;
        private TreeEstimateDO _treeEstimate;
        private TallyPopulation _count;

        public TallyAction Data { get; set; } = new TallyAction();

        public Tree Tree
        {
            get { return _tree; }
            set
            {
                _tree = value;
                Data.TreeCN = value?.Tree_CN ?? 0;
            }
        }

        public TreeEstimateDO TreeEstimate
        {
            get { return _treeEstimate; }
            set
            {
                _treeEstimate = value;
                Data.TreeEstimateCN = value?.TreeEstimate_CN ?? -1;
            }
        }

        public TallyPopulation Count
        {
            get { return _count; }
            set
            {
                _count = value;
                Data.CountCN = value?.CountTree_CN ?? -1;
            }
        }

        #region readonly props

        public bool HasTree => Tree != null;

        public long? TreeNumber => Tree?.TreeNumber;

        public string Initials => Tree?.Initials;

        public string TallyDescription => Count.TallyDescription;

        public string StratumCode => Count.StratumCode;

        #endregion readonly props

        public void Inflate(ICuttingUnitDataService dataService)
        {
            Count = dataService.GetCount(Data.CountCN);
            Tree = dataService.GetTree(Data.TreeCN);
            TreeEstimate = dataService.GetTreeEstimate(Data.TreeEstimateCN);
        }
    }
}