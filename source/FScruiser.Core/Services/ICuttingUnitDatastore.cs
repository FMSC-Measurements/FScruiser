using CruiseDAL.DataObjects;
using FScruiser.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface ICuttingUnitDatastore
    {
        IEnumerable<Stratum> GetStrataByUnitCode(string unitCode);

        IEnumerable<SampleGroup> GetSampleGroupsByUnitCode(string unitCode);

        IEnumerable<TallyPopulation> GetTallyPopulationsByUnitCode(string unitCode);

        IEnumerable<TreeFieldSetupDO> GetTreeFieldsByUnitCode(string unitCode);

        IEnumerable<TreeFieldSetupDO> GetTreeFieldsByStratumCode(string stratum);

        IEnumerable<TreeDefaultValueDO> GetTreeDefaultsByUnitCode(string unitCode);

        IEnumerable<TreeDefaultValueDO> GetTreeDefaultsBySampleGroup(string sgCode);

        IEnumerable<SampleGroupTreeDefaultValueDO> GetSampleGroupTreeDefaultMaps(string stratumCode, string sgCode);

        Tree GetTree(string unitCode, int treeNumber);

        IEnumerable<Tree> GetTreesByUnitCode(string unitCode);

        int GetNextTreeNumber(string unitCode);

        IEnumerable<CountTree> GetCountTreeByUnitCode(string unitCode);

        IEnumerable<TallyEntry> GetTallyEntriesByUnitCode(string unitCode);

        #region Tree

        //int GetHighestTreeNumberInUnit(string unit);

        void InsertTree(Tree tree);

        Task InsertTreeAsync(Tree tree);

        void UpdateTree(Tree tree);

        Task UpdateTreeAsync(Tree tree);

        void DeleteTree(Tree tree);

        #endregion Tree

        TreeEstimateDO GetTreeEstimate(long treeEstimateCN);

        void InsertTreeEstimate(TreeEstimateDO treeEstimate);

        void UpdateCount(CountTree count);

        void LogMessage(string message, string level);

        
    }
}