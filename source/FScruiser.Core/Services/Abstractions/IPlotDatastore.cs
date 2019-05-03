using FScruiser.Models;
using System.Collections.Generic;

namespace FScruiser.Services
{
    public interface IPlotDatastore
    {
        #region plot

        string AddNewPlot(string cuttingUnitCode);

        Plot GetPlot(string plotID);

        Plot GetPlot(string cuttingUnitCode, int plotNumber);

        IEnumerable<Plot> GetPlotsByUnitCode(string unitCode);

        void DeletePlot(string unitCode, int plotNumber);

        void UpdatePlot(Plot plot);

        #endregion plot

        #region plot stratum

        void InsertPlot_Stratum(Plot_Stratum stratumPlot);

        IEnumerable<Plot_Stratum> GetPlot_Strata(string unitCode, int plotNumber, bool insertIfNotExists = false);

        Plot_Stratum GetPlot_Stratum(string unitCode, string stratumCode, int plotNumber);

        void UpdatePlot_Stratum(Plot_Stratum stratumPlot);

        void DeletePlot_Stratum(string cuttingUnitCode, string stratumCode, int plotNumber);

        #endregion plot stratum

        #region tally populations

        IEnumerable<FixCntTallyPopulation> GetFixCNTTallyPopulations(string stratumCode);

        IEnumerable<TallyPopulation_Plot> GetPlotTallyPopulationsByUnitCode(string unitCode, int plotNumber);

        #endregion tally populations

        #region tree

        void InsertTree(TreeStub_Plot tree);

        string CreatePlotTree(string unitCode,
            int plotNumber,
            string stratumCode,
            string sampleGroupCode = null,
            string species = null,
            string liveDead = "L",
            string countMeasure = "M",
            int treeCount = 1,
            int kpi = 0,
            bool stm = false);

        Tree GetFixCNTTallyTree(string unitCode,
            int plotNumber,
            string stratumCode,
            string sgCode,
            string species,
            string liveDead,
            string fieldName,
            double value);

        Tree CreateFixCNTTallyTree(string unitCode,
            int plotNumber,
            string stratumCode,
            string sgCode,
            string species,
            string liveDead,
            string fieldName,
            double value,
            int treeCount = 0);

        


        #endregion tree

        IEnumerable<PlotError> GetPlotErrors(string plotID);

        IEnumerable<PlotError> GetPlotErrors(string unit, int plotNumber);

        int GetNextPlotNumber(string unitCode);

        bool IsPlotNumberAvalible(string unitCode, int plotNumber);

        void AddPlotRemark(string cuttingUnitCode, int plotNumber, string remark);

        int GetNumTreeRecords(string unitCode, string stratumCode, int plotNumber);

    }
}