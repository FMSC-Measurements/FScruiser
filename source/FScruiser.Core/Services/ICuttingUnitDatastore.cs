using CruiseDAL.DataObjects;
using FScruiser.Models;
using FScruiser.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface ICuttingUnitDatastore
    {
        #region stratra

        IEnumerable<Stratum> GetStrataByUnitCode(string unitCode);

        IEnumerable<StratumProxy> GetStrataProxiesByUnitCode(string unitCode);

        #endregion stratra

        #region sampleGroups

        SampleGroup GetSampleGroup(string stratumCode, string sgCode);

        //IEnumerable<SampleGroupProxy> GetSampleGroupProxiesByUnit(string unitCode);

        IEnumerable<SampleGroupProxy> GetSampleGroupProxies(string stratumCode);

        SampleGroupProxy GetSampleGroupProxy(string stratumCode, string sampleGroupCode);

        #endregion sampleGroups

        IEnumerable<TreeDefaultProxy> GetTreeDefaultProxies(string stratumCode, string sampleGroupCode);

        IEnumerable<TallyPopulation> GetTallyPopulationsByUnitCode(string unitCode);

        #region treeFields

        IEnumerable<TreeFieldSetupDO> GetTreeFieldsByUnitCode(string unitCode);

        IEnumerable<TreeFieldSetupDO> GetTreeFieldsByStratumCode(string stratum);

        #endregion treeFields

        IEnumerable<TreeAuditRule> GetTreeAuditRules(string stratum, string sampleGroup, string species, string livedead);

        #region tree

        string CreateTree(string unitCode, string stratumCode, string sampleGroupCode = null, string species = null, string liveDead = "L", string countMeasure = "M", int treeCount = 1, int kpi = 0, bool stm = false);

        Tree GetTree(string tree_GUID);

        TreeStub GetTreeStub(string tree_GUID);

        IEnumerable<Tree> GetTreesByUnitCode(string unitCode);

        IEnumerable<TreeStub> GetTreeStubsByUnitCode(string unitCode);

        void UpdateTree(Tree tree);

        void UpdateTreeInitials(string tree_guid, string value);

        void UpdateTreeErrors(string tree_GUID, IEnumerable<ValidationError> errors);

        Task UpdateTreeAsync(Tree tree);

        void DeleteTree(Tree tree);

        void DeleteTree(string tree_guid);

        #endregion tree

        IEnumerable<TallyEntry> GetTallyEntriesByUnitCode(string unitCode);

        void InsertTallyEntry(TallyEntry entry);

        void DeleteTally(TallyEntry tallyEntry);

        void LogMessage(string message, string level);
    }
}