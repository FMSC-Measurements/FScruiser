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

        Plot GetPlot(string plotID);

        Plot GetPlot(string cuttingUnitCode, int plotNumber);

        int GetNextPlotNumber(string unitCode);

        bool IsPlotNumberAvalible(string unitCode, int plotNumber);

        IEnumerable<Plot> GetPlotsByUnitCode(string unitCode);

        string AddNewPlot(string cuttingUnitCode);

        //Plot GetPlot(string unitCode, int plotNumber);

        //void UpsertStratumPlot(string unit, StratumPlot stratumPlot);

        IEnumerable<Plot_Stratum> GetPlot_Strata(string unitCode, int plotNumber, bool insertIfNotExists = false);

        Plot_Stratum GetPlot_Stratum(string unitCode, string stratumCode, int plotNumber);

        void InsertPlot_Stratum(Plot_Stratum stratumPlot);

        void UpdatePlot(Plot plot);

        void UpdatePlot_Stratum(Plot_Stratum stratumPlot);

        void AddPlotRemark(string cuttingUnitCode, int plotNumber, string remark);

        void DeletePlot_Stratum(string cuttingUnitCode, string stratumCode, int plotNumber);

        void DeletePlot(string unitCode, int plotNumber);

        int GetNumTreeRecords(string unitCode, string stratumCode, int plotNumber);

        IEnumerable<FixCntTallyPopulation> GetFixCNTTallyPopulations(string stratumCode);

        Tree GetFixCNTTallyTree( string unitCode, int plotNumber, 
            string stratumCode, string sgCode, string species, string liveDead,
            string fieldName, double value);

        Tree CreateFixCNTTallyTree( string unitCode, int plotNumber, 
            string stratumCode, string sgCode, string species, string liveDead,
            string fieldName, double value, int treeCount = 0);

        #endregion plots

        #region stratra

        IEnumerable<string> GetStratumCodesByUnit(string unitCode);

        IEnumerable<Stratum> GetStrataByUnitCode(string unitCode);

        IEnumerable<StratumProxy> GetStrataProxiesByUnitCode(string unitCode);

        IEnumerable<StratumProxy> GetPlotStrataProxies(string unitCode);

        #endregion stratra

        #region sampleGroups

        IEnumerable<string> GetSampleGroupCodes(string stratumCode);

        SampleGroup GetSampleGroup(string stratumCode, string sgCode);

        //IEnumerable<SampleGroupProxy> GetSampleGroupProxiesByUnit(string unitCode);

        IEnumerable<SampleGroupProxy> GetSampleGroupProxies(string stratumCode);

        SampleGroupProxy GetSampleGroupProxy(string stratumCode, string sampleGroupCode);

        SamplerState GetSamplerState(string stratumCode, string sampleGroupCode);

        void UpdateSamplerState(SamplerState samplerState);

        #endregion sampleGroups

        IEnumerable<SubPopulation> GetSubPopulations(string stratumCode, string sampleGroupCode);

        IEnumerable<TallyPopulation> GetTallyPopulationsByUnitCode(string unitCode);

        TallyPopulation GetTallyPopulation(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead);

        IEnumerable<TallyPopulation_Plot> GetPlotTallyPopulationsByUnitCode(string unitCode, int plotNumber);

        #region treeFields

        //IEnumerable<TreeFieldSetup> GetTreeFieldsByUnitCode(string unitCode);

        IEnumerable<TreeFieldSetup> GetTreeFieldsByStratumCode(string stratum);

        #endregion treeFields

        #region validation 

        IEnumerable<TreeError> GetTreeErrorsByUnit(string cuttingUnitCode);

        IEnumerable<TreeError> GetTreeErrorsByUnit(string cuttingUnitCode, int PlotNumber);

        IEnumerable<TreeError> GetTreeErrors(string treeID);

        IEnumerable<LogError> GetLogErrorsByLog(string logID);

        IEnumerable<LogError> GetLogErrorsByTree(string treeID);

        //IEnumerable<TreeAuditRule> GetTreeAuditRules(string stratum, string sampleGroup, string species, string livedead);

        #endregion

        #region tree

        IEnumerable<TreeFieldValue> GetTreeFieldValues(string treeID);

        void UpdateTreeFieldValue(TreeFieldValue treeFieldValue);

        bool IsTreeNumberAvalible(string unitCode, int treeNumber, int? plotNumber = null);

        int GetNextPlotTreeNumber(string unitCode, string stratumCode, int plotNumber, bool isRecon);

        string CreateMeasureTree(string unitCode, string stratumCode, 
            string sampleGroupCode = null, string species = null, string liveDead = "L", 
            int treeCount = 1, int kpi = 0, bool stm = false);

        Tree GetTree(string tree_GUID);

        TreeStub GetTreeStub(string tree_GUID);

        IEnumerable<TreeStub_Plot> GetPlotTreeProxies(string unitCode, int plotNumber);

        IEnumerable<Tree> GetTreesByUnitCode(string unitCode);

        IEnumerable<TreeStub> GetTreeStubsByUnitCode(string unitCode);

        void InsertTree(TreeStub_Plot tree);

        void UpdateTree(Tree tree);

        void UpdateTree(Tree_Ex tree);

        void UpdateTreeInitials(string tree_guid, string value);

        Task UpdateTreeAsync(Tree_Ex tree);

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

        #endregion logs

        #region Tally Entries
        IEnumerable<TallyEntry> GetTallyEntriesByUnitCode(string unitCode);

        IEnumerable<TallyEntry> GetTallyEntries(string unitCode, int plotNumber);

        TallyEntry InsertTallyAction(TallyAction entry);

        void InsertTallyLedger(TallyLedger tallyLedger);

        void DeleteTallyEntry(string tallyLedgerID);

        #endregion

        void LogMessage(string message, string level);
    }
}