using CruiseDAL.Schema;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FScruiser.Services
{
    public partial class CuttingUnitDatastore : IPlotDatastore
    {
        #region plot

        public string AddNewPlot(string cuttingUnitCode)
        {
            var plotID = Guid.NewGuid().ToString();

            var plotNumber = GetNextPlotNumber(cuttingUnitCode);

            Database.Execute2(
                "INSERT INTO Plot_V3 (" +
                    "PlotID, " +
                    "PlotNumber, " +
                    "CuttingUnitCode, " +
                    "CreatedBy" +
                ") VALUES ( " +
                    "@PlotID," +
                    "@PlotNumber, " +
                    "@CuttingUnitCode, " +
                    "@CreatedBy " +
                ");" +
                "INSERT INTO Plot_Stratum (" +
                    "CuttingUnitCode, " +
                    "PlotNumber, " +
                    "StratumCode, " +
                    "CreatedBy " +
                ")" +
                "SELECT " +
                    "p.CuttingUnitCode, " +
                    "p.PlotNumber, " +
                    "st.Code, " +
                    "@CreatedBy AS CreatedBy " +
                "FROM Plot_V3 AS p " +
                "JOIN CuttingUnit_Stratum AS cust USING (CuttingUnitCode) " +
                "JOIN Stratum AS st ON cust.StratumCode = st.Code " +
                $"WHERE p.PlotID = @PlotID AND st.Method IN ({PLOT_METHODS}) " +
                $"AND st.Method != '{CruiseMethods.THREEPPNT}';",
                new { CuttingUnitCode = cuttingUnitCode, PlotID = plotID, PlotNumber = plotNumber, CreatedBy = UserName }); // dont automaticly add plot_stratum for 3ppnt methods

            return plotID;
        }

        public Plot GetPlot(string plotID)
        {
            return Database.Query<Plot>(
                "SELECT " +
                    "p.PlotID, " +
                    "p.CuttingUnitCode, " +
                    "p.PlotNumber, " +
                    "p.Slope, " +
                    "p.Aspect, " +
                    "p.Remarks, " +
                    "p.XCoordinate, " +
                    "p.YCoordinate, " +
                    "p.ZCoordinate " +
                "FROM Plot_V3 AS p " +
                "WHERE PlotID = @p1;", new object[] { plotID })
                .FirstOrDefault();
        }

        public Plot GetPlot(string cuttingUnitCode, int plotNumber)
        {
            return Database.Query<Plot>(
                "SELECT " +
                    "p.PlotID, " +
                    "p.CuttingUnitCode, " +
                    "p.PlotNumber, " +
                    "p.Slope, " +
                    "p.Aspect, " +
                    "p.Remarks, " +
                    "p.XCoordinate, " +
                    "p.YCoordinate, " +
                    "p.ZCoordinate " +
                "FROM Plot_V3 AS p " +
                "WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2;", new object[] { cuttingUnitCode, plotNumber })
                .FirstOrDefault();
        }

        public IEnumerable<Plot> GetPlotsByUnitCode(string unit)
        {
            return Database.Query<Plot>("SELECT *  FROM Plot_V3 " +
                "WHERE CuttingUnitCode = @p1;"
                , new object[] { unit });
        }

        public void UpdatePlot(Plot plot)
        {
            Database.Execute2(
                "UPDATE Plot_V3 SET " +
                    "PlotNumber = @PlotNumber, " +
                    "Slope = @Slope, " +
                    "Aspect = @Aspect, " +
                    "Remarks = @Remarks, " +
                    "XCoordinate = @XCoordinate, " +
                    "YCoordinate = @YCoordinate, " +
                    "ZCoordinate = @ZCoordinate, " +
                    "ModifiedBy = @UserName " +
                "WHERE PlotID = @PlotID; ",
                    new
                    {
                        plot.PlotNumber,
                        plot.Slope,
                        plot.Aspect,
                        plot.Remarks,
                        plot.XCoordinate,
                        plot.YCoordinate,
                        plot.ZCoordinate,
                        UserName,
                        plot.PlotID,
                    });
        }

        public void DeletePlot(string unitCode, int plotNumber)
        {
            Database.Execute(
                "DELETE FROM Plot_V3 WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2 ;", new object[] { unitCode, plotNumber });
        }

        #endregion plot

        #region plot stratum

        public void InsertPlot_Stratum(Plot_Stratum plotStratum)
        {
            var plot_stratum_CN = Database.ExecuteScalar2<long?>(
                "INSERT INTO Plot_Stratum (" +
                   "CuttingUnitCode, " +
                   "PlotNumber, " +
                   "StratumCode, " +
                   "IsEmpty, " +
                   "KPI, " +
                   "ThreePRandomValue, " +
                   "CreatedBy " +
                ") VALUES (" +
                    $"@CuttingUnitCode, " +
                    $"@PlotNumber, " +
                    $"@StratumCode, " +
                    $"@IsEmpty, " +
                    $"@KPI, " +
                    $"@ThreePRandomValue, " +
                    $"'{UserName}'" +
                ");" +
                "SELECT last_insert_rowid();",
                plotStratum);

            plotStratum.InCruise = true;
            plotStratum.Plot_Stratum_CN = plot_stratum_CN;
        }

        public IEnumerable<Plot_Stratum> GetPlot_Strata(string unitCode, int plotNumber, bool insertIfNotExists = false)
        {
            return Database.Query<Plot_Stratum>(
                "SELECT " +
                    "ps.Plot_Stratum_CN, " +
                    "(CASE WHEN ps.Plot_Stratum_CN IS NOT NULL THEN 1 ELSE 0 END) AS InCruise, " +
                    "st.Code AS StratumCode, " +
                    "p.CuttingUnitCode, " +
                    "p.PlotNumber, " +
                    "st.BasalAreaFactor AS BAF, " +
                    "st.FixedPlotSize AS FPS, " +
                    "st.Method AS CruiseMethod, " +
                    "st.KZ3PPNT AS KZ, " +
                    "ps.IsEmpty," +
                    "ps.KPI " +
                "FROM Plot_V3 AS p " +
                "JOIN CuttingUnit_Stratum AS cust USING (CuttingUnitCode) " +
                "JOIN Stratum AS st ON cust.StratumCode = st.Code " +
                "LEFT JOIN Plot_Stratum AS ps USING (CuttingUnitCode, StratumCode, PlotNumber) " +
                "WHERE p.CuttingUnitCode = @p1 " +
                "AND p.PlotNumber = @p2;",
                new object[] { unitCode, plotNumber }).ToArray();
        }

        public Plot_Stratum GetPlot_Stratum(string unitCode, string stratumCode, int plotNumber)
        {
            // we're going to read from the plot table instead of the stratum table
            // because we want to return a dummy record with InCruise set to false
            // when a plot_stratum record doesn't exist
            return Database.Query<Plot_Stratum>(
                "SELECT " +
                    "ps.Plot_Stratum_CN, " +
                    "(ps.Plot_Stratum_CN IS NOT NULL) AS InCruise, " +
                    "p.PlotNumber, " +
                    "st.Code AS StratumCode, " +
                    "p.CuttingUnitCode, " +
                    "st.BasalAreaFactor AS BAF, " +
                    "st.FixedPlotSize AS FPS, " +
                    "st.Method AS CruiseMethod, " +
                    "st.KZ3PPNT AS KZ, " +
                    "ps.IsEmpty," +
                    "ps.KPI " +
                "FROM Plot_V3 AS p " +
                "JOIN CuttingUnit_Stratum AS cust USING (CuttingUnitCode) " +
                "JOIN Stratum AS st ON cust.StratumCode = st.Code " +
                "LEFT JOIN Plot_Stratum AS ps USING (CuttingUnitCode, StratumCode, PlotNumber) " +
                "WHERE p.CuttingUnitCode = @p1 " +
                "AND st.Code = @p2 " +
                "AND p.PlotNumber = @p3; ",
                new object[] { unitCode, stratumCode, plotNumber }).FirstOrDefault();

            //var stratumPlot = Database.Query<StratumPlot>(
            //    "SELECT " +
            //        "CAST (1 AS BOOLEAN) AS InCruise, " +
            //        "ps.StratumCode, " +
            //        "ps.CuttingUnitCode, " +
            //        "st.BasalAreaFactor AS BAF, " +
            //        "st.FixedPlotSize AS FPS, " +
            //        "st.Method AS CruiseMethod, " +
            //        "st.KZ3PPNT AS KZ, " +
            //        "ps.* " +
            //    "FROM Plot_Stratum " +
            //    "JOIN Stratum AS st ON ps.StratumCode = st.Code " +
            //    "WHERE ps.CuttingUnitCode = @p1 " +
            //    "AND ps.StratumCode = @p2 " +
            //    "AND ps.PlotNumber = @p3;", new object[] { unitCode, stratumCode, plotNumber }).FirstOrDefault();

            //if (stratumPlot == null)
            //{
            //    stratumPlot = Database.Query<StratumPlot>(
            //        "SELECT " +
            //            "CAST (0 AS BOOLEAN) AS InCruise, " +
            //            "st.Code AS StratumCode, " +
            //            "st.BasalAreaFactor AS BAF, " +
            //            "st.FixedPlotSize AS FPS, " +
            //            "st.Method AS CruiseMethod, " +
            //            "st.KZ3PPNT AS KZ " +
            //        "FROM Stratum AS st " +
            //            "WHERE Stratum.Code = @p1;"
            //            , new object[] { stratumCode }).FirstOrDefault();

            //    stratumPlot.UnitCode = unitCode;
            //    stratumPlot.PlotNumber = plotNumber;
            //}
            //else
            //{
            //    stratumPlot.InCruise = true;
            //}

            //return stratumPlot;
        }

        public void UpdatePlot_Stratum(Plot_Stratum stratumPlot)
        {
            Database.Execute2(
                "UPDATE Plot_Stratum SET " +
                    "IsEmpty = @IsEmpty, " +
                    "KPI = @KPI " +
                "WHERE " +
                "CuttingUnitCode = @CuttingUnitCode " +
                "AND StratumCode = @StratumCode " +
                "AND PlotNumber = @PlotNumber;",
                stratumPlot);
        }

        public void DeletePlot_Stratum(string cuttingUnitCode, string stratumCode, int plotNumber)
        {
            Database.Execute("DELETE FROM Plot_Stratum WHERE CuttingUnitCode = @p1 AND StratumCode = @p2 AND PlotNumber = @p3; "
                , cuttingUnitCode, stratumCode, plotNumber);
        }

        #endregion plot stratum

        #region tree

        public void InsertTree(TreeStub_Plot tree)
        {
            var treeID = tree.TreeID ?? Guid.NewGuid().ToString();

            Database.Execute2(
                "INSERT INTO Tree_V3 ( " +
                    "TreeID, " +
                    "TreeNumber, " +
                    "CuttingUnitCode, " +
                    "PlotNumber, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "CountOrMeasure " +
                ") VALUES ( " +
                    "@TreeID,\r\n " +
                    "@TreeNumber,\r\n " +
                    "@CuttingUnitCode,\r\n " +
                    "@PlotNumber,\r\n " +
                    "@StratumCode,\r\n " +
                    "@SampleGroupCode,\r\n " +
                    "@Species,\r\n" + // species
                    "@LiveDead,\r\n" + // liveDead
                    "@CountOrMeasure " + // countMeasure
                "); " +
                "INSERT INTO TallyLedger ( " +
                    "TallyLedgerID, " +
                    "TreeID, " +
                    "CuttingUnitCode, " +
                    "PlotNumber, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "TreeCount, " +
                    "KPI, " +
                    "STM " +
                ") VALUES ( " +
                    "@TreeID, " +
                    "@TreeID, " +
                    "@CuttingUnitCode, " +
                    "@PlotNumber, " +
                    "@StratumCode, " +
                    "@SampleGroupCode, " +
                    "@Species, " +
                    "@LiveDead, " +
                    "@TreeCount, " +
                    "@KPI, " +
                    "@STM " +
                "); "
                , new
                {
                    TreeID = treeID,
                    tree.TreeNumber,
                    tree.CuttingUnitCode,
                    tree.PlotNumber,
                    tree.StratumCode,
                    tree.SampleGroupCode,
                    tree.Species,
                    tree.LiveDead,
                    tree.CountOrMeasure,
                    tree.TreeCount,
                    tree.KPI,
                    tree.STM,
                });

            tree.TreeID = treeID;
        }

        public string CreatePlotTree(string unitCode, int plotNumber,
            string stratumCode, string sampleGroupCode,
            string species = null, string liveDead = "L",
            string countMeasure = "M", int treeCount = 1,
            int kpi = 0, bool stm = false)
        {
            var tree_guid = Guid.NewGuid().ToString();
            CreatePlotTree(tree_guid, unitCode, plotNumber, stratumCode, sampleGroupCode, species, liveDead, countMeasure, treeCount, kpi, stm);
            return tree_guid;
        }

        protected void CreatePlotTree(string treeID, string unitCode, int plotNumber,
            string stratumCode, string sampleGroupCode,
            string species = null, string liveDead = "L",
            string countMeasure = "M", int treeCount = 1,
            int kpi = 0, bool stm = false)
        {
            var tallyLedgerID = treeID;

            Database.Execute2(
                "INSERT INTO Tree_V3 ( " +
                    "TreeID, " +
                    "TreeNumber, " +
                    "CuttingUnitCode, " +
                    "PlotNumber, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "CountOrMeasure " +
                ") VALUES (" +
                    "@TreeID,\r\n " +
                    "(SELECT ifnull(max(TreeNumber), 0) + 1 FROM Tree_V3 WHERE CuttingUnitCode = @CuttingUnitCode AND PlotNumber = @PlotNumber),\r\n " +
                    "@CuttingUnitCode,\r\n " +
                    "@PlotNumber,\r\n " +
                    "@StratumCode,\r\n " +
                    "@SampleGroupCode,\r\n " +
                    "@Species,\r\n" + //species
                    "@LiveDead,\r\n" + //liveDead
                    "@CountOrMeasure);\r\n" + //countMeasure
                "INSERT INTO TallyLedger ( " +
                    "TallyLedgerID, " +
                    "TreeID, " +
                    "CuttingUnitCode, " +
                    "PlotNumber, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "TreeCount, " +
                    "KPI, " +
                    "STM" +
                ") VALUES ( " +
                    "@TallyLedgerID," +
                    "@TreeID, " +
                    "@CuttingUnitCode, " +
                    "@PlotNumber, " +
                    "@StratumCode, " +
                    "@SampleGroupCode, " +
                    "@Species, " +
                    "@LiveDead, " +
                    "@TreeCount, " +
                    "@KPI, " +
                    "@STM " +
                ");"
                , new
                {
                    TallyLedgerID = tallyLedgerID,
                    TreeID = treeID,
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    SampleGroupCode = sampleGroupCode,
                    Species = species ?? "",
                    LiveDead = liveDead ?? "",
                    CountOrMeasure = countMeasure,
                    TreeCount = treeCount,
                    KPI = kpi,
                    STM = (stm) ? "Y" : "N",
                }
            );
        }

        public Tree CreateFixCNTTallyTree(string unitCode, int plotNumber,
            string stratumCode, string sgCode, string species, string liveDead,
            string fieldName, double value, int treeCount = 0)
        {
            var treeID = Guid.NewGuid().ToString();

            var fieldNameStr = fieldName.ToString();

            Database.Execute2(
                "INSERT INTO Tree_V3 (" +
                    "TreeID, " +
                    "TreeNumber, " +
                    "CuttingUnitCode, " +
                    "PlotNumber, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "CountOrMeasure " +
                ") VALUES (" +
                    "@TreeID,\r\n " + //treeID
                    "(SELECT ifnull(max(TreeNumber), 0) + 1  " +
                        "FROM Tree_V3 AS t1 " +
                        "WHERE CuttingUnitCode = @CuttingUnitCode AND PlotNumber = @PlotNumber),\r\n " + //get highest tree number using unitCode and plot_cn
                    "@CuttingUnitCode,\r\n " +
                    "@PlotNumber,\r\n " + //plot_cn
                    "@StratumCode,\r\n " + //stratum_cn
                    "@SampleGroupCode,\r\n " + //sampleGroup_CN
                    "@Species,\r\n" + //species
                    "@LiveDead,\r\n" + //liveDead
                    "'M'" +
                ");" + //countMeasure

                "INSERT INTO TreeMeasurment " +
                $"(TreeID, {fieldNameStr}) VALUES (@TreeID, @value);" +

                "INSERT INTO TallyLedger ( " +
                    "TallyLedgerID, " +
                    "TreeID, " +
                    "CuttingUnitCode, " +
                    "PlotNumber, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "TreeCount" +
                ") VALUES ( " +
                    "@TallyLedgerID, " +
                    "@TreeID, " +
                    "@CuttingUnitCode, " +
                    "@PlotNumber, " +
                    "@StratumCode, " +
                    "@SampleGroupCode, " +
                    "@Species, " +
                    "@LiveDead, " +
                    "@TreeCount" +
                ");"
                ,
                new
                {
                    TreeID = treeID,
                    TallyLedgerID = treeID,
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    SampleGroupCode = sgCode,
                    Species = species,
                    LiveDead = liveDead,
                    TreeCount = treeCount,
                    value,
                }
            );

            var tree = QueryTree_Base().Where("TreeID = @p1").Query(treeID).First();
            return tree;
        }

        public Tree GetFixCNTTallyTree(string unitCode, int plotNumber,
            string stratumCode, string sgCode, string species, string liveDead,
            string fieldName, double value)
        {
            var fieldNameStr = fieldName.ToString();

            return Database.Query<Tree>(
                "SELECT " +
                    "t.*, " +
                    "tm.* " +
                "FROM Tree_V3 AS t " +
                "JOIN TreeMeasurment AS tm USING (TreeID) " +
                $"WHERE tm.{fieldNameStr} = @p1 " +
                    "AND t.PlotNumber = @p2 " +
                    "AND t.CuttingUnitCode = @p3 " +
                    "AND t.StratumCode = @p4 " +
                    "AND t.SampleGroupCode = @p5 " +
                    "AND ifnull(t.Species, '') = ifnull(@p6, '') " +
                    "AND ifnull(t.LiveDead, '') = ifnull(@p7, '') " +
                "LIMIT 1;",
                new object[] { value, plotNumber, unitCode, stratumCode, sgCode, species, liveDead })
                .FirstOrDefault();
        }

        #endregion

        #region tally population

        public IEnumerable<FixCntTallyPopulation> GetFixCNTTallyPopulations(string stratumCode)
        {
            return Database.Query<FixCntTallyPopulation>(
                "SELECT " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "Field, " +
                    "Min, " +
                    "Max, " +
                    "IntervalSize " +
                "FROM FixCNTTallyPopulation_V3 AS ftp " +
                "JOIN FixCNTTallyClass_V3 AS tc USING (StratumCode) " +
                "WHERE StratumCode = @p1;",
                new object[] { stratumCode });
        }

        #endregion

        public void AddPlotRemark(string cuttingUnitCode, int plotNumber, string remark)
        {
            Database.Execute(
                "UPDATE Plot_V3 SET Remarks = Remarks || ', ' || @p3 " +
                "WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2;", cuttingUnitCode, plotNumber, remark);
        }

        public int GetNumTreeRecords(string unitCode, string stratumCode, int plotNumber)
        {
            return Database.ExecuteScalar<int>("SELECT Count(*) FROM Tree_V3 " +
                "WHERE CuttingUnitCode = @p1 AND StratumCode = @p2 AND PlotNumber = @p3;",
                unitCode, stratumCode, plotNumber);
        }

        public int GetNextPlotNumber(string unitCode)
        {
            return Database.ExecuteScalar<int>("SELECT ifnull(max(PlotNumber), 0) + 1 FROM Plot_V3 AS p " +
                "JOIN CuttingUnit AS cu ON cu.Code = p.CuttingUnitCode " +
                "WHERE p.CuttingUnitCode = @p1;", unitCode);
        }

        public bool IsPlotNumberAvalible(string unitCode, int plotNumber)
        {
            return Database.ExecuteScalar<int>("SELECT count(*) FROM Plot_V3 AS p " +
                "WHERE p.CuttingUnitCode = @p1 AND p.PlotNumber = @p2;", unitCode, plotNumber) == 0;
        }

        public IEnumerable<PlotError> GetPlotErrors(string unit, int plotNumber)
        {
            return Database.Query<PlotError>("SELECT * FROM PlotError AS pe " +
                "WHERE pe.CuttingUnitCode = @p1 " +
                "AND pe.PlotNumber = @p2;",
                unit, plotNumber).ToArray();
        }

        public IEnumerable<PlotError> GetPlotErrors(string plotID)
        {
            return Database.Query<PlotError>("SELECT * FROM PlotError AS pe " +
                "WHERE pe.PlotID = @p1;",
                plotID).ToArray();
        }

        public IEnumerable<TreeError> GetTreeErrorsByPlot(string plotID)
        {
            return Database.Query<TreeError>(
                "SELECT " +
                "te.TreeID, " +
                "te.Field, " +
                "te.Level, " +
                "te.Message, " +
                "te.Resolution " +
                "FROM TreeError AS te " +
                "JOIN Tree_V3 AS t USING (TreeID) " +
                "JOIN Plot_V3 AS p USING (CuttingUnitCode, PlotNumber) " +
                "WHERE p.PlotID = @p1;",
                new object[] { plotID }).ToArray();
        }


    }
}