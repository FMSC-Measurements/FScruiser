﻿using FScruiser.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface ICuttingUnitDatastore : IPlotDatastore, ITreeDatastore
    {
        string GetCruisePurpose();

        CuttingUnit_Ex GetUnit(string code);

        IEnumerable<CuttingUnit> GetUnits();

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

        #region validation

        IEnumerable<TreeError> GetTreeErrorsByUnit(string cuttingUnitCode);

        IEnumerable<TreeError> GetTreeErrorsByUnit(string cuttingUnitCode, int PlotNumber);

        IEnumerable<LogError> GetLogErrorsByLog(string logID);

        IEnumerable<LogError> GetLogErrorsByTree(string treeID);

        //IEnumerable<TreeAuditRule> GetTreeAuditRules(string stratum, string sampleGroup, string species, string livedead);

        #endregion validation

        #region tree

        int GetNextPlotTreeNumber(string unitCode, string stratumCode, int plotNumber, bool isRecon);

        TreeStub GetTreeStub(string tree_GUID);

        IEnumerable<TreeStub_Plot> GetPlotTreeProxies(string unitCode, int plotNumber);

        IEnumerable<Tree> GetTreesByUnitCode(string unitCode);

        IEnumerable<TreeStub> GetTreeStubsByUnitCode(string unitCode);

        #endregion tree

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

        #endregion Tally Entries

        void LogMessage(string message, string level);
    }
}