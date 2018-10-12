using CruiseDAL.DataObjects;
using FScruiser.Models;
using FScruiser.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface ICuttingUnitDatastore
    {
        string GetCruisePurpose();

        CuttingUnit_Ex GetUnit(string code);

        IEnumerable<CuttingUnit> GetUnits();

        #region plots
        int GetNextPlotNumber(string unitCode);

        bool IsPlotNumberAvalible(string unitCode, int plotNumber);

        IEnumerable<Plot> GetPlotsByUnitCode(string unitCode);

        //Plot GetPlot(string unitCode, int plotNumber);

        //void UpsertStratumPlot(string unit, StratumPlot stratumPlot);

        StratumPlot GetStratumPlot(string unitCode, string stratumCode, int plotNumber, bool insertIfNotExists = false);

        void InsertStratumPlot(string unitCode, StratumPlot stratumPlot);

        void UpdateStratumPlot(StratumPlot stratumPlot);

        void DeleteStratumPlot(string plot_guid);

        void DeletePlot(string unitCode, int plotNumber);

        int GetNumTreeRecords(string unitCode, string stratumCode, int plotNumber);

        #endregion plots

        #region stratra

        IEnumerable<Stratum> GetStrataByUnitCode(string unitCode);

        IEnumerable<StratumProxy> GetStrataProxiesByUnitCode(string unitCode);

        IEnumerable<StratumProxy> GetPlotStrataProxies(string unitCode);

        #endregion stratra

        #region sampleGroups

        SampleGroup GetSampleGroup(string stratumCode, string sgCode);

        //IEnumerable<SampleGroupProxy> GetSampleGroupProxiesByUnit(string unitCode);

        IEnumerable<SampleGroupProxy> GetSampleGroupProxies(string stratumCode);

        SampleGroupProxy GetSampleGroupProxy(string stratumCode, string sampleGroupCode);

        #endregion sampleGroups

        IEnumerable<TreeDefaultProxy> GetTreeDefaultProxies(string stratumCode, string sampleGroupCode);

        IEnumerable<TallyPopulation> GetTallyPopulationsByUnitCode(string unitCode);

        IEnumerable<TallyPopulation_Plot> GetPlotTallyPopulationsByUnitCode(string unitCode, int plotNumber);

        #region treeFields

        IEnumerable<TreeFieldSetupDO> GetTreeFieldsByUnitCode(string unitCode);

        IEnumerable<TreeFieldSetupDO> GetTreeFieldsByStratumCode(string stratum);

        #endregion treeFields

        IEnumerable<TreeAuditRule> GetTreeAuditRules(string stratum, string sampleGroup, string species, string livedead);

        #region tree

        bool IsTreeNumberAvalible(string unitCode, int treeNumber, int? plotNumber = null);

        int GetNextPlotTreeNumber(string unitCode, string stratumCode, int plotNumber, bool isRecon);

        string CreateTree(string unitCode, string stratumCode, string sampleGroupCode = null, string species = null, string liveDead = "L", string countMeasure = "M", int treeCount = 1, int kpi = 0, bool stm = false);

        Tree GetTree(string tree_GUID);

        TreeStub GetTreeStub(string tree_GUID);

        IEnumerable<TreeStub_Plot> GetPlotTreeProxies(string unitCode, int plotNumber);

        IEnumerable<Tree> GetTreesByUnitCode(string unitCode);

        IEnumerable<TreeStub> GetTreeStubsByUnitCode(string unitCode);

        void InsertTree(TreeStub_Plot tree);

        void UpdateTree(Tree tree);

        void UpdateTreeInitials(string tree_guid, string value);

        void UpdateTreeErrors(string tree_GUID, IEnumerable<ValidationError> errors);

        Task UpdateTreeAsync(Tree tree);

        void DeleteTree(Tree tree);

        void DeleteTree(string tree_guid);

        #endregion tree

        #region plot tree

        string CreatePlotTree(string unitCode, int plotNumber, string stratumCode, string sampleGroupCode = null, string species = null, string liveDead = "L", string countMeasure = "M", int treeCount = 1, int kpi = 0, bool stm = false);

        #endregion plot tree

        #region logs

        IEnumerable<Log> GetLogs(string tree_guid);

        Log GetLog(string log_guid);

        Log GetLog(string tree_guid, int logNumber);

        void InsertLog(Log log);

        void UpdateLog(Log log);

        void DeleteLog(string log_guid);

        IEnumerable<LogFieldSetup> GetLogFields(string tree_guid);


        #endregion

        IEnumerable<TallyEntry> GetTallyEntriesByUnitCode(string unitCode);

        IEnumerable<TallyEntry> GetTallyEntries(string unitCode, int plotNumber);

        void InsertTallyEntry(TallyEntry entry);

        void DeleteTally(TallyEntry tallyEntry);

        void LogMessage(string message, string level);
    }
}