using CruiseDAL;
using CruiseDAL.Schema;
using FMSC.ORM.Core.SQL.QueryBuilder;
using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public class CuttingUnitDatastore : ICuttingUnitDatastore
    {
        private const int NUMBER_OF_TALLY_ENTRIES_PERPAGE = 20;

        private readonly string PLOT_METHODS = String.Join(", ", CruiseMethods.PLOT_METHODS
            .Append(CruiseMethods.THREEPPNT)
            .Append(CruiseMethods.FIXCNT)
            .Select(x => "'" + x + "'").ToArray());

        private DAL _database;

        public DAL Database
        {
            get { return _database; }
            set
            {
                _database = value;
                OnDatabaseChanged();
            }
        }

        protected string CurrentTimeStamp => DateTime.Now.ToString();
        protected string UserName => "AndroidUser";

        public CuttingUnitDatastore(string path)
        {
            var database = new DAL(path ?? throw new ArgumentNullException(nameof(path)));

            Database = database;
        }

        public CuttingUnitDatastore(DAL database)
        {
            Database = database;
        }

        private void OnDatabaseChanged()
        {
            var database = Database;
            if (database == null) { return; }

            //DatabaseUpdater.Update(database);
        }

        public string GetCruisePurpose()
        {
            return Database.ExecuteScalar<string>("SELECT Purpose FROM Sale LIMIT 1;");
        }

        #region units

        public CuttingUnit_Ex GetUnit(string code)
        {
            var unit = Database.From<CuttingUnit_Ex>()
                .Where("Code = @p1")
                .Query(code).FirstOrDefault();

            unit.HasPlotStrata = Database.ExecuteScalar<int>("SELECT count(*) FROM CuttingUnit_Stratum AS cust " +
                "JOIN Stratum AS st ON cust.StratumCode = st.Code " +
                "JOIN CuttingUnit AS cu ON cust.CuttingUnitCode = cu.Code " +
                "WHERE cust.CuttingUnitCode = @p1 " +
                $"AND st.Method IN ({PLOT_METHODS});", code) > 0;

            unit.HasTreeStrata = Database.ExecuteScalar<int>("SELECT count(*) FROM CuttingUnit_Stratum AS [cust] " +
                "JOIN Stratum AS st ON cust.StratumCode = st.Code " +
                "JOIN CuttingUnit AS cu ON cust.CuttingUnitCode = cu.Code " +
                "WHERE cust.CuttingUnitCode = @p1 " +
                $"AND st.Method NOT IN ({PLOT_METHODS});", code) > 0;

            return unit;
        }

        public IEnumerable<CuttingUnit> GetUnits()
        {
            return Database.From<CuttingUnit>()
                .Query().ToArray();
        }

        #endregion units

        #region plot

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

        public IEnumerable<Plot> GetPlotsByUnitCode(string unit)
        {
            return Database.Query<Plot>("SELECT *  FROM Plot_V3 " +
                "WHERE CuttingUnitCode = @p1;"
                , new object[] { unit });
        }

        //public Plot GetPlot(string unitCode, int plotNumber)
        //{
        //    return Database.Query<Plot>("SELECT PlotNumber, CuttingUnit_CN  FROM Plot " +
        //        "JOIN CuttingUnit USING (CuttingUnit_CN) " +
        //        "WHERE CuttingUnit.Code = @p1 " +
        //        "AND PlotNumber = @p2 " +
        //        "GROUP BY PlotNumber, CuttingUnit_CN;"
        //        , new object[] { unitCode, plotNumber }).FirstOrDefault();
        //}

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

        public void AddPlotRemark(string cuttingUnitCode, int plotNumber, string remark)
        {
            Database.Execute(
                "UPDATE Plot_V3 SET Remarks = Remarks || ', ' || @p3 " +
                "WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2;", cuttingUnitCode, plotNumber, remark);
        }

        public void DeletePlot_Stratum(string cuttingUnitCode, string stratumCode, int plotNumber)
        {
            Database.Execute("DELETE FROM Plot_Stratum WHERE CuttingUnitCode = @p1 AND StratumCode = @p2 AND PlotNumber = @p3; "
                , cuttingUnitCode, stratumCode, plotNumber);
        }

        public void DeletePlot(string unitCode, int plotNumber)
        {
            Database.Execute(
                "DELETE FROM Plot_V3 WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2 ;", new object[] { unitCode, plotNumber });
        }

        public int GetNumTreeRecords(string unitCode, string stratumCode, int plotNumber)
        {
            return Database.ExecuteScalar<int>("SELECT Count(*) FROM Tree_V3 " +
                "WHERE CuttingUnitCode = @p1 AND StratumCode = @p2 AND PlotNumber = @p3;",
                unitCode, stratumCode, plotNumber);
        }

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
                "FROM FixCNTTallyPopulation AF ftp " +
                "JOIN FixCNTTallyClass AS tc USING (StratumCode) " +
                "WHERE StratumCode = @p1;",
                new object[] { stratumCode });
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
                    "AND t.StratumCode = @p4" +
                    "AND t.SampleGroupCode = @p5 " +
                    "AND ifnull(t.Species, '') = ifnull(@p6, '') " +
                    "AND ifnull(t.LiveDead, '') = ifnull(@p7, '') " +
                "LIMIT 1;",
                new object[] { value, plotNumber, unitCode, stratumCode, sgCode, species, liveDead })
                .FirstOrDefault();
        }

        public Tree CreateFixCNTTallyTree(string unitCode, int plotNumber,
            string stratumCode, string sgCode, string species, string liveDead,
            string fieldName, double value, int treeCount = 0)
        {
            var tree_guid = Guid.NewGuid().ToString();

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

                "INSERT INTO TreeMeasurment (" +
                $"({fieldNameStr}) VALUES (@value);" +

                "INSERT INTO TallyLedger (" +
                    "TreeID, " +
                    "CuttingUnitCode, " +
                    "PlotNumber, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "TreeCount" +
                ") VALUES (" +
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
                    TreeID = tree_guid,
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    SampleGroupCode = sgCode,
                    Species = species,
                    LiveDead = liveDead,
                    TreeCount = treeCount
                }
            );

            var tree = QueryTree_Base().Where("TreeID = @p1").Query(tree_guid).First();
            return tree;
        }

        #endregion plot

        #region strata

        public IEnumerable<string> GetStratumCodesByUnit(string unitCode)
        {
            var stratumCodes = Database.ExecuteScalar<string>(
                "SELECT group_concat(StratumCode, ',') FROM CuttingUnit_Stratum " +
                "WHERE CuttingUnitCode = @p1;", unitCode);

            return stratumCodes.Split(',');
        }

        public IEnumerable<Stratum> GetStrataByUnitCode(string unitCode)
        {
            return Database.Query<Stratum>(
                "SELECT " +
                "st.* " +
                "FROM Stratum AS st " +
                "JOIN CuttingUnit_Stratum AS cust ON st.Code = cust.StratumCode " +
                $"WHERE CuttingUnitCode = @p1 AND st.Method NOT IN ({PLOT_METHODS})",
                new object[] { unitCode })
                .ToArray();
        }

        public IEnumerable<StratumProxy> GetStrataProxiesByUnitCode(string unitCode)
        {
            return Database.Query<StratumProxy>(
                "SELECT " +
                "st.* " +
                "FROM Stratum AS st " +
                "JOIN CuttingUnit_Stratum AS cust ON st.Code = cust.StratumCode " +
                $"WHERE CuttingUnitCode = @p1 AND st.Method NOT IN ({PLOT_METHODS})",
                new object[] { unitCode })
                .ToArray();
        }

        public IEnumerable<StratumProxy> GetPlotStrataProxies(string unitCode)
        {
            return Database.Query<StratumProxy>(
                "SELECT " +
                "st.* " +
                "FROM Stratum AS st " +
                "JOIN CuttingUnit_Stratum AS cust ON st.Code = cust.StratumCode " +
                $"WHERE CuttingUnitCode = @p1 AND st.Method IN ({PLOT_METHODS})",
                new object[] { unitCode })
                .ToArray();
        }

        #endregion strata

        #region sampleGroup

        //public IEnumerable<SampleGroup> GetSampleGroupsByUnitCode(string unitCode)
        //{
        //    return Database.From<SampleGroup>()
        //        .Join("Stratum", "USING (Stratum_CN)")
        //        .Join("CuttingUnitStratum", "USING (Stratum_CN)")
        //        .Join("CuttingUnit", "USING (CuttingUnit_CN)")
        //        .Where("CuttingUnit.Code = @p1").Query(unitCode).ToArray();
        //}

        public IEnumerable<string> GetSampleGroupCodes(string stratumCode)
        {
            var sgCode = Database.ExecuteScalar<string>("SELECT group_concat(SampleGroupCode,',') FROM SampleGroup_V3 " +
                "WHERE StratumCode = @p1;", stratumCode);

            return sgCode.Split(',');
        }

        public IEnumerable<SampleGroupProxy> GetSampleGroupProxiesByUnit(string unitCode)
        {
            throw new NotImplementedException();
        }

        public SampleGroup GetSampleGroup(string stratumCode, string sgCode)
        {
            return Database.Query<SampleGroup>(
                "SELECT " +
                    "sg.*, " +
                    "st.Method AS StratumMethod " +
                "FROM SampleGroup_V3 AS sg " +
                "JOIN Stratum AS st ON sg.StratumCode = st.Code " +
                "WHERE sg.StratumCode = @p1 AND sg.SampleGroupCode = @p2;",
                new object[] { stratumCode, sgCode })
                .FirstOrDefault();
        }

        public IEnumerable<SampleGroupProxy> GetSampleGroupProxies(string stratumCode)
        {
            return Database.From<SampleGroupProxy>()
                .Where("StratumCode = @p1")
                .Query(stratumCode);
        }

        public SampleGroupProxy GetSampleGroupProxy(string stratumCode, string sampleGroupCode)
        {
            return Database.From<SampleGroupProxy>()
                .Where("StratumCode = @p1 AND SampleGroupCode = @p2")
                .Query(stratumCode, sampleGroupCode).FirstOrDefault();
        }

        public SamplerState GetSamplerState(string stratumCode, string sampleGroupCode)
        {
            return Database.Query<SamplerState>(
                "SELECT " +
                    "sg.StratumCode," +
                    "sg.SampleGroupCode, " +
                    "st.Method, " +
                    "sg.SamplingFrequency, " +
                    "sg.InsuranceFrequency, " +
                    "sg.KZ, " +
                    "ss.SampleSelectorState, " +
                    "ss.SampleSelectorType " +
                "FROM SampleGroup_V3 AS sg " +
                "JOIN Stratum AS st ON sg.StratumCode = st.Code " +
                "LEFT JOIN SamplerState AS ss USING (StratumCode, SampleGroupCode) " +
                "WHERE sg.StratumCode = @p1 " +
                "AND sg.SampleGroupCode = @p2;",
                new object[] { stratumCode, sampleGroupCode }).FirstOrDefault();
        }

        public void UpdateSamplerState(SamplerState samplerState)
        {
            Database.Execute2(
                "INSERT INTO SamplerState ( " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "SampleSelectorType, " +
                    "SampleSelectorState " +
                ") VALUES ( " +
                    "@StratumCode, " +
                    "@SampleGroupCode, " +
                    "@SampleSelectorType, " +
                    "@SampleSelectorState" +
                ") ON CONFLICT (StratumCode, SampleGroupCode) " +
                "DO UPDATE SET " +
                    "SampleSelectorType = @SampleSelectorType, " +
                    "SampleSelectorState = @SampleSelectorState " +
                "WHERE StratumCode = @StratumCode " +
                "AND SampleGroupCode = @SampleGroupCode;",
                samplerState);
        }

        private string SELECT_TALLYPOPULATION_CORE =
            "WITH tallyPopTreeCounts AS (" +
                "SELECT CuttingUnitCode, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "sum(TreeCount) AS TreeCount, " +
                    "sum(KPI) AS SumKPI " +
                "FROM TallyLedger AS tl " +
                "GROUP BY " +
                    "CuttingUnitCode, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "ifnull(Species, ''), " +
                    "ifnull(LiveDead, ''))\r\n" +

                "SELECT " +
                    "tp.Description, " +
                    "tp.StratumCode, " +
                    "st.Method AS StratumMethod, " +
                    "tp.SampleGroupCode, " +
                    "tp.Species, " +
                    "tp.LiveDead, " +
                    "tp.HotKey, " +
                    "ifnull(tl.TreeCount, 0) AS TreeCount, " +
                    "ifnull(tl.SumKPI, 0) AS SumKPI, " +
                    //"sum(tl.KPI) SumKPI, " +
                    "sg.SamplingFrequency AS Frequency, " +
                    "sg.MinKPI AS sgMinKPI, " +
                    "sg.MaxKPI AS sgMaxKPI, " +
                    $"ss.SampleSelectorType == '{CruiseMethods.CLICKER_SAMPLER_TYPE}' AS IsClickerTally " +
                "FROM TallyPopulation AS tp " +
                "JOIN SampleGroup_V3 AS sg USING (StratumCode, SampleGroupCode) " +
                "Left JOIN SamplerState ss USING (StratumCode, SampleGroupCode) " +
                "JOIN Stratum AS st ON tp.StratumCode = st.Code " +
                "JOIN CuttingUnit_Stratum AS cust ON tp.StratumCode = cust.StratumCode AND cust.CuttingUnitCode = @p1 " +
                "LEFT JOIN tallyPopTreeCounts AS tl " +
                    "ON tl.CuttingUnitCode = @p1 " +
                    "AND tp.StratumCode = tl.StratumCode " +
                    "AND tp.SampleGroupCode = tl.SampleGroupCode " +
                    "AND ifnull(tp.Species, '') = ifnull(tl.Species, '') " +
                    "AND ifnull(tp.LiveDead, '') = ifnull(tl.LiveDead, '') ";

        public IEnumerable<TallyPopulation> GetTallyPopulationsByUnitCode(string unitCode)
        {
            return Database.Query<TallyPopulation>(
                SELECT_TALLYPOPULATION_CORE +
                $"WHERE st.Method NOT IN ({PLOT_METHODS})"
                , new object[] { unitCode }).ToArray();

            //return Database.From<TallyPopulation>()
            //    .Join("Tally", "USING (Tally_CN)")
            //    .Join("SampleGroup", "USING (SampleGroup_CN)")
            //    .LeftJoin("TreeDefaultValue", "USING (TreeDefaultValue_CN)")
            //    .Join("Stratum", "USING (Stratum_CN)")
            //    .Join("CuttingUnit", "USING (CuttingUnit_CN)")
            //    .Where($"CuttingUnit.Code = @p1 AND Stratum.Method NOT IN ({PLOT_METHODS})")
            //    .Query(unitCode).ToArray();
        }

        public TallyPopulation GetTallyPopulation(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead)
        {
            //var tPops = Database.QueryGeneric(
            //    "SELECT * FROM TallyPopulation AS tp " +
            //    "WHERE tp.StratumCode = @p2 " +
            //        "AND tp.SampleGroupCode = @p3 " +
            //        "AND ifNull(tp.Species, '') = ifNull(@p4,'') " +
            //        "AND ifNull(tp.LiveDead, '') = ifNull(@p5,'')"
            //    , new  { p1 = unitCode, p2 = stratumCode, p3= sampleGroupCode, p4 = species, p5 = liveDead }).ToArray();

            return Database.Query<TallyPopulation>(
                SELECT_TALLYPOPULATION_CORE +
                "WHERE tp.StratumCode = @p2 " +
                    "AND tp.SampleGroupCode = @p3 " +
                    "AND ifNull(tp.Species, '') = ifNull(@p4,'') " +
                    "AND ifNull(tp.LiveDead, '') = ifNull(@p5,'')"
                , new object[] { unitCode, stratumCode, sampleGroupCode, species, liveDead }).FirstOrDefault();
        }

        private class SampleGroupStratumInfo
        {
            [Field("SgCode")]
            public string SgCode { get; set; }

            [Field("StCode")]
            public string StCode { get; set; }

            [Field("Method")]
            public string Method { get; set; }
        }

        //TODO: update this method
        public IEnumerable<TallyPopulation_Plot> GetPlotTallyPopulationsByUnitCode(string unitCode, int plotNumber)
        {
            var tallyPops = Database.Query<TallyPopulation_Plot>(
                SELECT_TALLYPOPULATION_CORE +
                $"WHERE st.Method IN ({PLOT_METHODS})"
                , new object[] { unitCode }).ToArray();

            foreach (var pop in tallyPops)
            {
                pop.InCruise = GetIsTallyPopInCruise(unitCode, plotNumber, pop.StratumCode);
                pop.IsEmpty = Database.ExecuteScalar<int>("SELECT ifnull(IsEmpty, 0) FROM Plot_Stratum " +
                    "WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2 AND StratumCode = @p3;",
                    unitCode, plotNumber, pop.StratumCode) == 1;
            }

            return tallyPops;
        }

        private bool GetIsTallyPopInCruise(string unitCode, int plotNumber, string stratumCode)
        {
            return Database.ExecuteScalar<bool?>(
                "SELECT EXISTS (" +
                    "SELECT * " +
                    "FROM Plot_Stratum " +
                    "WHERE StratumCode = @p1 " +
                        "AND CuttingUnitCode = @p2 " +
                        "AND PlotNumber = @p3);",
                stratumCode, unitCode, plotNumber) ?? false;
        }

        #endregion sampleGroup

        #region subpopulation

        public IEnumerable<SubPopulation> GetSubPopulations(string stratumCode, string sampleGroupCode)
        {
            return Database.Query<SubPopulation>("SELECT * FROM SubPopulation " +
                "WHERE StratumCode = @p1 AND SampleGroupCode = @p2;", stratumCode, sampleGroupCode).ToArray();
        }

        #endregion subpopulation

        #region TreeFields

        //public IEnumerable<TreeFieldSetup> GetTreeFieldsByUnitCode(string unitCode)
        //{
        //    return Database.Query<TreeFieldSetup>(
        //        "SELECT " +
        //        "Field, " +
        //        "Heading, " +
        //        "min(FieldOrder) AS FieldOrder " +
        //        "FROM TreeFieldSetup_V3 AS tfs " +
        //        "JOIN CuttingUnit_Stratum AS cust USING (StratumCode) " +
        //        "WHERE CuttingUnitCode = @p1 AND min(FieldOrder) >= 0 " +
        //        "GROUP BY CuttingUnitCode, Field " +
        //        "ORDER BY min(FieldOrder);",
        //        new object[] { unitCode }).ToArray();
        //}

        public IEnumerable<TreeFieldSetup> GetTreeFieldsByStratumCode(string stratumCode)
        {
            return Database.Query<TreeFieldSetup>(
                "SELECT " +
                "Field, " +
                "Heading, " +
                "FieldOrder " +
                "FROM TreeFieldSetup_V3 AS tfs " +
                "WHERE StratumCode = @p1 AND FieldOrder >= 0 " +
                "GROUP BY Field " +
                "ORDER BY FieldOrder;",
                new object[] { stratumCode }).ToArray();
        }

        #endregion TreeFields

        #region Tree

        public void UpdateTreeFieldValue(TreeFieldValue treeFieldValue)
        {
            Database.Execute(
                "INSERT INTO TreeMeasurment " +
                $"(TreeID, {treeFieldValue.Field})" +
                $"VALUES (@p1, @p2)" +
                $"ON CONFLICT (TreeID) DO " +
                $"UPDATE SET {treeFieldValue.Field} = @p2 WHERE TreeID = @p1;", 
                treeFieldValue.TreeID, treeFieldValue.Value);
        }

        public IEnumerable<TreeFieldValue> GetTreeFieldValues(string treeID)
        {
            return Database.Query<TreeFieldValue>(
                "SELECT " +
                "t.TreeID, " +
                "tf.Field, " +
                "tfs.Heading, " +
                "tf.DbType, " +
                "tfv.ValueReal, " +
                "tfv.ValueBool, " +
                "tfv.ValueText, " +
                "tfv.ValueInt " +
                "FROM Tree_V3 AS t " +
                "JOIN TreeFieldSetup_V3 AS tfs USING (StratumCode) " +
                "JOIN TreeField AS tf USING (Field) " +
                "LEFT JOIN TreeFieldValue_TreeMeasurment AS tfv USING (TreeID, Field) " +
                "WHERE t.TreeID = @p1 " +
                "ORDER BY tfs.FieldOrder;", treeID).ToArray(); 


            //return Database.Query<TreeFieldValue>(
            //    "SELECT " +
            //    "tfv.TreeID, " +
            //    "tfv.Field, " +
            //    "tfs.Heading, " +
            //    "tfv.DbType, " +
            //    "tfv.ValueReal, " +
            //    "tfv.ValueBool, " +
            //    "tfv.ValueText, " +
            //    "tfv.ValueInt " +
            //    "FROM TreeFieldValue_TreeMeasurment AS tfv " +
            //    "JOIN Tree_V3 AS t USING (TreeID) " +
            //    "JOIN TreeFieldSetup_V3 AS tfs ON tfv.Field = tfs.Field AND t.StratumCode = tfs.StratumCode " +
            //    "WHERE tfv.TreeID = @p1 ; ", treeID).ToArray();
        }

        public bool IsTreeNumberAvalible(string unit, int treeNumber, int? plotNumber = null)
        {
            if (plotNumber != null)
            {
                return Database.ExecuteScalar<int>("SELECT count(*) FROM Tree_V3 " +
                    "WHERE CuttingUnitCode = @p1 " +
                    "AND PlotNumber = @p2 " +
                    "AND TreeNumber = @p3;",
                    unit, plotNumber, treeNumber) == 0;
            }
            else
            {
                return Database.ExecuteScalar<int>("SELECT count(*) FROM Tree_V3 " +
                    "WHERE CuttingUnitCode = @p1 " +
                    "AND PlotNumber IS NULL " +
                    "AND TreeNumber = @p2;",
                    unit, treeNumber) == 0;
            }
        }

        public IEnumerable<Tree> GetTreesByUnitCode(string unitCode)
        {
            return QueryTree_Base()
                //.Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where("CuttingUnit.Code = @p1 AND PlotNumber IS NULL")
                .Query(unitCode).ToArray();
        }

        public TreeStub GetTreeStub(string treeID)
        {
            return QueryTreeStub()
                .Where("TreeID = @p1")
                .Query(treeID).FirstOrDefault();
        }

        public void UpdateTreeInitials(string tree_guid, string value)
        {
            Database.Execute(
                "UPDATE TreeMeasurment SET " +
                "Initials = @p1 " +
                "WHERE TreeID = @p2",
                value, tree_guid);
        }

        public void DeleteTree(string tree_guid)
        {
            Database.Execute("Delete FROM Tree_V3 WHERE TreeID = @p1", tree_guid);
        }

        public void DeleteTree(Tree tree)
        {
            DeleteTree(tree.TreeID);
        }

        private IQuerryAcceptsJoin<TreeStub> QueryTreeStub()
        {
            return Database.From<TreeStub>()
                .LeftJoin("TreeMeasurment", "USING (TreeID)");
        }

        public IEnumerable<TreeStub> GetTreeStubsByUnitCode(string unitCode)
        {
            return QueryTreeStub()
                .Where("CuttingUnitCode = @p1 AND PlotNumber IS NULL")
                .Query(unitCode);
        }

        public IEnumerable<TreeStub_Plot> GetPlotTreeProxies(string unitCode, int plotNumber)
        {
            return Database.Query<TreeStub_Plot>(
                "SELECT " +
                "t.TreeID, " +
                "t.CuttingUnitCode, " +
                "t.TreeNumber, " +
                "t.PlotNumber, " +
                "t.StratumCode, " +
                "t.SampleGroupCode, " +
                "t.Species, " +
                "t.LiveDead, " +
                "tl.TreeCount, " +
                "tl.STM, " +
                "tl.KPI, " +
                "max(tm.TotalHeight, tm.MerchHeightPrimary, tm.UpperStemHeight) AS Height, " +
                "max(tm.DBH, tm.DRC, tm.DBHDoubleBarkThickness) AS Diameter, " +
                "t.CountOrMeasure " +
                "FROM Tree_V3 AS t " +
                "JOIN TallyLedger AS tl USING (TreeID) " +
                "LEFT JOIN TreeMeasurment AS tm USING (TreeID) " +
                "WHERE t.CuttingUnitCode = @p1 " +
                "AND t.PlotNumber = @p2;", new object[] { unitCode, plotNumber });
        }

        //public Tree GetTree(string unitCode, int treeNumber)
        //{
        //    return QueryTree_Base()
        //        .Join("CuttingUnit", "USING (CuttingUnit_CN)")
        //        .Where("CuttingUnit.Code = @p1 AND TreeNumber = @p2")
        //        .Query(unitCode, treeNumber).FirstOrDefault();
        //}

        public Tree GetTree(string treeID)
        {
            return QueryTree_Base()
                .Where("Tree_V3.TreeID = @p1")
                .Query(treeID).FirstOrDefault();
        }

        private IQuerryAcceptsJoin<Tree_Ex> QueryTree_Base()
        {
            return Database.From<Tree_Ex>()
               .LeftJoin("TreeMeasurment", "USING (TreeID)");
        }

        // used for add tree
        public string CreateMeasureTree(string unitCode, string stratumCode,
            string sampleGroupCode = null, string species = null, string liveDead = "L",
            int treeCount = 1, int kpi = 0, bool stm = false)
        {
            var tree_guid = Guid.NewGuid().ToString();
            CreateMeasureTree(tree_guid, unitCode, stratumCode, sampleGroupCode, species, liveDead, treeCount, kpi, stm);
            return tree_guid;
        }

        protected void CreateMeasureTree(string treeID, string unitCode, string stratumCode,
            string sampleGroupCode = null, string species = null, string liveDead = "L",
            int treeCount = 1, int kpi = 0, bool stm = false)
        {
            var tallyLedgerID = treeID;

            Database.Execute2(
                "INSERT INTO Tree_V3 (" +
                    "TreeID, " +
                    "TreeNumber, " +
                    "CuttingUnitCode, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "CountOrMeasure " +
                ") VALUES ( " +
                    "@TreeID, " +
                    "(SELECT ifnull(max(TreeNumber), 0) +1 " +
                        "FROM Tree_V3 " +
                        "WHERE CuttingUnitCode = @CuttingUnitCode " +
                        "AND PlotNumber IS NULL), " +
                    "@CuttingUnitCode, " +
                    "@StratumCode, " +
                    "@SampleGroupCode, " +
                    "@Species, " +
                    "@LiveDead, " +
                    "'M'" +
                "); " +
                "INSERT INTO TallyLedger ( " +
                    "TallyLedgerID, " +
                    "TreeID, " +
                    "CuttingUnitCode, " +
                    "StratumCode, " +
                    "SampleGroupCode," +
                    "Species, " +
                    "LiveDead, " +
                    "TreeCount, " +
                    "KPI, " +
                    "STM " +
                ") VALUES ( " +
                    "@TallyLedgerID, " +
                    "@TreeID, " +
                    "@CuttingUnitCode, " +
                    "@StratumCode, " +
                    "@SampleGroupCode, " +
                    "@Species, " +
                    "@LiveDead, " +
                    "@TreeCount, " +
                    "@KPI, " +
                    "@STM" +
                ");",
                new
                {
                    TallyLedgerID = tallyLedgerID,
                    TreeID = treeID,
                    CuttingUnitCode = unitCode,
                    StratumCode = stratumCode,
                    SampleGroupCode = sampleGroupCode,
                    Species = species,
                    LiveDead = liveDead,
                    TreeCount = treeCount,
                    KPI = kpi,
                    STM = stm,
                });
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

        public int GetNextPlotTreeNumber(string unitCode, string stratumCode, int plotNumber, bool isRecon)
        {
            if (isRecon)
            {
                // if cruise is a recon cruise we do number trees seperatly for each stratum
                return Database.ExecuteScalar<int>("SELECT ifnull(max(TreeNumber), 0) + 1  FROM Tree_V3 " +
                    "WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2 AND StratumCode = @p3;"
                    , unitCode, plotNumber, stratumCode);
            }
            else
            {
                return Database.ExecuteScalar<int>("SELECT ifnull(max(TreeNumber), 0) + 1  FROM Tree_V3 " +
                    "WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2;"
                    , unitCode, plotNumber);
            }
        }

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

        private const string UPSERT_TREEMEASURMENT_COMMAND =
            "INSERT INTO TreeMeasurment ( " +
                "TreeID, " +

                "SeenDefectPrimary," +
                "SeenDefectSecondary, " +
                "RecoverablePrimary, " +
                "HiddenPrimary, " +
                "Grade, " +

                "HeightToFirstLiveLimb, " +
                "PoleLength, " +
                "ClearFace, " +
                "CrownRatio, " +
                "DBH, " +

                "DRC, " +
                "TotalHeight, " +
                "MerchHeightPrimary, " +
                "MerchHeightSecondary, " +
                "FormClass, " +

                "UpperStemDiameter, " +
                "UpperStemHeight, " +
                "DBHDoubleBarkThickness, " +
                "TopDIBPrimary, " +
                "TopDIBSecondary, " +

                "DefectCode, " +
                "DiameterAtDefect, " +
                "VoidPercent, " +
                "Slope, " +
                "Aspect, " +

                "Remarks, " +
                "IsFallBuckScale, " +
                "Initials, " +
                "CreatedBy" +

            ") VALUES ( " +
                "@TreeID, " +

                "@SeenDefectPrimary, " +
                "@SeenDefectSecondary, " +
                "@RecoverablePrimary, " +
                "@HiddenPrimary, " +
                "@Grade, " +

                "@HeightToFirstLiveLimb, " +
                "@PoleLength, " +
                "@ClearFace, " +
                "@CrownRatio, " +
                "@DBH, " +

                "@DRC, " +
                "@TotalHeight, " +
                "@MerchHeightPrimary, " +
                "@MerchHeightSecondary, " +
                "@FormClass, " +

                "@UpperStemDiameter, " +
                "@UpperStemHeight, " +
                "@DBHDoubleBarkThickness, " +
                "@TopDIBPrimary, " +
                "@TopDIBSecondary, " +

                "@DefectCode, " +
                "@DiameterAtDefect, " +
                "@VoidPercent, " +
                "@Slope, " +
                "@Aspect, " +

                "@Remarks, " +
                "@IsFallBuckScale, " +
                "@Initials, " +
                "@UserName ) " +
            "ON CONFLICT (TreeID) DO UPDATE SET " +

                "SeenDefectPrimary = @SeenDefectPrimary, " +
                "SeenDefectSecondary = @SeenDefectSecondary, " +
                "RecoverablePrimary = @RecoverablePrimary, " +
                "HiddenPrimary = @HiddenPrimary, " +
                "Grade = @Grade, " +

                "HeightToFirstLiveLimb = @HeightToFirstLiveLimb, " +
                "PoleLength = @PoleLength, " +
                "ClearFace = @ClearFace, " +
                "CrownRatio = @CrownRatio, " +
                "DBH = @DBH, " +

                "DRC = @DRC, " +
                "TotalHeight = @TotalHeight, " +
                "MerchHeightPrimary = @MerchHeightPrimary, " +
                "MerchHeightSecondary = @MerchHeightSecondary, " +
                "FormClass = @FormClass, " +

                "UpperStemDiameter = @UpperStemDiameter, " +
                "UpperStemHeight = @UpperStemHeight, " +
                "DBHDoubleBarkThickness = @DBHDoubleBarkThickness, " +
                "TopDIBPrimary = @TopDIBPrimary, " +
                "TopDIBSecondary = @TopDIBSecondary, " +

                "DefectCode = @DefectCode, " +
                "DiameterAtDefect = @DiameterAtDefect, " +
                "VoidPercent = @VoidPercent, " +
                "Slope = @Slope, " +
                "Aspect = @Aspect, " +

                "Remarks = @Remarks, " +
                "IsFallBuckScale = @IsFallBuckScale, " +
                "Initials = @Initials, " +

                "ModifiedBy = @UserName " +
            "WHERE TreeID = @TreeID;";

        public void UpdateTree(Tree tree)
        {
            //if (tree.IsPersisted == false) { throw new InvalidOperationException("tree is not persisted before calling update"); }
            //Database.Update(tree);

            Database.Execute2(
                "UPDATE Tree_V3 SET \r\n " +
                    "TreeNumber = @TreeNumber, " +
                    "StratumCode = @StratumCode, " +
                    "SampleGroupCode = @SampleGroupCode, " +
                    "Species = @Species," +
                    "LiveDead = @LiveDead, " +
                    "CountOrMeasure = @CountOrMeasure, " +
                    "ModifiedBy = @UserName " +
                "WHERE TreeID = @TreeID; ",
                new
                {
                    tree.TreeID,
                    tree.TreeNumber,
                    tree.StratumCode,
                    tree.SampleGroupCode,
                    tree.Species,
                    tree.LiveDead,
                    tree.CountOrMeasure,

                    UserName,
                });
        }

        public void UpdateTree(Tree_Ex tree)
        {
            //if (tree.IsPersisted == false) { throw new InvalidOperationException("tree is not persisted before calling update"); }
            //Database.Update(tree);

            Database.Execute2(
                "UPDATE Tree_V3 SET \r\n " +
                    "TreeNumber = @TreeNumber, " +
                    "StratumCode = @StratumCode, " +
                    "SampleGroupCode = @SampleGroupCode, " +
                    "Species = @Species," +
                    "LiveDead = @LiveDead, " +
                    "CountOrMeasure = @CountOrMeasure, " +
                    "ModifiedBy = @UserName " +
                "WHERE TreeID = @TreeID; " +
                UPSERT_TREEMEASURMENT_COMMAND,
                new
                {
                    tree.TreeID,
                    tree.StratumCode,
                    tree.SampleGroupCode,
                    tree.Species,
                    tree.LiveDead,
                    CountOrMeasure = tree.CountOrMeasure ?? "",

                    tree.SeenDefectPrimary,
                    tree.SeenDefectSecondary,
                    tree.RecoverablePrimary,
                    tree.HiddenPrimary,
                    tree.Grade,

                    tree.HeightToFirstLiveLimb,
                    tree.PoleLength,
                    tree.ClearFace,
                    tree.CrownRatio,
                    tree.DBH,

                    tree.DRC,
                    tree.TotalHeight,
                    tree.MerchHeightPrimary,
                    tree.MerchHeightSecondary,
                    tree.FormClass,

                    tree.UpperStemDiameter,
                    tree.UpperStemHeight,
                    tree.DBHDoubleBarkThickness,
                    tree.TopDIBPrimary,
                    tree.TopDIBSecondary,

                    tree.DefectCode,
                    tree.DiameterAtDefect,
                    tree.VoidPercent,
                    tree.Slope,
                    tree.Aspect,

                    tree.Remarks,
                    tree.IsFallBuckScale,
                    tree.Initials,
                    UserName,
                });
        }

        public void UpsertTreeMeasurments(TreeMeasurment mes)
        {
            Database.Execute2(
                UPSERT_TREEMEASURMENT_COMMAND,
                new
                {
                    mes.TreeID,

                    mes.SeenDefectPrimary,
                    mes.SeenDefectSecondary,
                    mes.RecoverablePrimary,
                    mes.HiddenPrimary,
                    mes.Grade,

                    mes.HeightToFirstLiveLimb,
                    mes.PoleLength,
                    mes.ClearFace,
                    mes.CrownRatio,
                    mes.DBH,

                    mes.DRC,
                    mes.TotalHeight,
                    mes.MerchHeightPrimary,
                    mes.MerchHeightSecondary,
                    mes.FormClass,

                    mes.UpperStemDiameter,
                    mes.UpperStemHeight,
                    mes.DBHDoubleBarkThickness,
                    mes.TopDIBPrimary,
                    mes.TopDIBSecondary,

                    mes.DefectCode,
                    mes.DiameterAtDefect,
                    mes.VoidPercent,
                    mes.Slope,
                    mes.Aspect,

                    mes.Remarks,
                    mes.IsFallBuckScale,
                    mes.Initials,
                    UserName,
                }
                );
        }

        public Task UpdateTreeAsync(Tree_Ex tree)
        {
            return Task.Run(() => UpdateTree(tree));
        }

        //private Guid CreateTree(IDbConnection conn, IDbTransaction trans, string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead, string countMeasure, int treeCount = 1, int kpi = 0, bool stm = false)
        //{
        //    var tree_guid = Guid.NewGuid();
        //    return Database.ExecuteScalar<Guid>(conn, CREATE_TREE_COMMAND
        //        //+"SELECT Tree_GUID FROM Tree WHERE Tree_CN == last_insert_rowid();"
        //        , new object[] { tree_guid,
        //        unitCode,
        //        stratumCode,
        //        sampleGroupCode,
        //        species,
        //        liveDead,
        //        countMeasure,
        //        treeCount,
        //        kpi,
        //        (stm) ? "Y" : "N"},
        //        trans);
        //}

        //public int GetNextTreeNumber(string unitCode)
        //{
        //    return Database.ExecuteScalar<int>("SELECT max(TreeNumber) + 1 FROM Tree JOIN CuttingUnit USING (CuttingUnit_CN) WHERE CuttingUnit.Code = @p1 AND Plot_CN IS NULL;", unitCode);
        //}

        //public void InsertTree(Tree tree)
        //{
        //    if (tree.IsPersisted == true) { throw new InvalidOperationException("tree is persisted, should be calling update instead of insert"); }
        //    Database.Insert(tree);
        //}

        //public Task InsertTreeAsync(Tree tree)
        //{
        //    return Task.Run(() => InsertTree(tree));
        //}

        #endregion Tree

        #region Tree Audits and ErrorLog

        public IEnumerable<TreeError> GetTreeErrorsByUnit(string cuttingUnitCode)
        {
            return Database.Query<TreeError>(
                "SELECT " +
                "te.TreeID, " +
                "te.Field, " +
                "te.Message, " +
                "te.Resolution " +
                "FROM TreeError AS te " +
                "JOIN Tree_V3 AS t USING (TreeID) " +
                "LEFT JOIN TreeAuditResolution AS ter USING (TreeAuditRuleID, TreeID) " +
                "WHERE t.CuttingUnitCode = @p1 AND PlotNumber IS NULL;",
                new object[] { cuttingUnitCode });
        }

        public IEnumerable<TreeError> GetTreeErrorsByUnit(string cuttingUnitCode, int PlotNumber)
        {
            return Database.Query<TreeError>(
                "SELECT " +
                "te.TreeID, " +
                "te.Field, " +
                "te.Message, " +
                "te.Resolution " +
                "FROM TreeAuditError AS te " +
                "JOIN Tree_V3 AS t USING (TreeID) " +
                "LEFT JOIN TreeAuditResolution AS ter USING (TreeAuditRuleID, TreeID) " +
                "WHERE t.CuttingUnitCode = @p1 AND PlotNumber = @plotNumber;",
                new object[] { cuttingUnitCode, PlotNumber });
        }

        public IEnumerable<TreeError> GetTreeErrors(string treeID)
        {
            return Database.Query<TreeError>(
                "SELECT " +
                "te.TreeID, " +
                "te.Field, " +
                "te.Level, " +
                "te.Message, " +
                "te.Resolution " +
                "FROM TreeError AS te " +
                "WHERE te.TreeID = @p1;",
                new object[] { treeID }).ToArray();
        }

        public IEnumerable<LogError> GetLogErrorsByLog(string logID)
        {
            return Database.Query<LogError>(
                "SELECT " +
                "LogID, Message " +
                "FROM LogGradeError " +
                "WHERE LogID = @p1;",
                new object[] { logID })
                .ToArray();
        }

        public IEnumerable<LogError> GetLogErrorsByTree(string treeID)
        {
            return Database.Query<LogError>(
                "SELECT " +
                "LogID, Message " +
                "FROM LogGradeError " +
                "JOIN Log_V3 USING (LogID) " +
                "WHERE TreeID  = @p1;",
                new object[] { treeID })
                .ToArray();
        }

        //public IEnumerable<TreeAuditRule> GetTreeAuditRules(string stratum, string sampleGroup, string species, string livedead)
        //{
        //    return Database.Query<TreeAuditRule>("SELECT * FROM TreeAuditValue " +
        //        "JOIN TreeDefaultValueTreeAuditValue USING (TreeAuditValue_CN) " +
        //        "JOIN TreeDefaultValue AS TDV USING (TreeDefaultValue_CN) " +
        //        "JOIN SampleGroup ON TDV.PrimaryProduct = SampleGroup.PrimaryProduct " +
        //        "JOIN Stratum USING (Stratum_CN) " +
        //        "WHERE Stratum.Code = @p1 " +
        //        "AND SampleGroup.Code = @p2 " +
        //        "AND TDV.Species = @p3 " +
        //        "AND TDV.LiveDead = @p4;", new object[] { stratum, sampleGroup, species, livedead });
        //}

        //public void UpdateTreeErrors(string tree_GUID, IEnumerable<ValidationError> errors)
        //{
        //    Database.Execute("DELETE FROM ErrorLog WHERE TableName = 'Tree' " +
        //        "AND CN_Number = (SELECT Tree_CN FROM Tree WHERE Tree_GUID = @p1) " +
        //        "AND Suppress = 0;", tree_GUID);

        //    foreach (var error in errors)
        //    {
        //        Database.Execute("INSERT OR IGNORE INTO ErrorLog (TableName, CN_Number, ColumnName, Level, Message, Program) " +
        //            "VALUES ('Tree', (SELECT Tree_CN FROM Tree WHERE Tree_GUID = @p1), @p2, @p3, @p4, 'FScruiser');",
        //            tree_GUID,
        //            error.Property,
        //            error.Level.ToString(),
        //            error.Message);
        //    }
        //}

        #endregion Tree Audits and ErrorLog

        #region Tally Entry

        public IEnumerable<TallyEntry> GetTallyEntriesByUnitCode(string unitCode)
        {
            return Database.Query<TallyEntry>(
                "SELECT " +
                    "tl.TreeID, " +
                    "tl.TallyLedgerID, " +
                    "tl.CuttingUnitCode, " +
                    "tl.StratumCode, " +
                    "tl.SampleGroupCode, " +
                    "tl.Species, " +
                    "tl.LiveDead, " +
                    "TreeCount, " +
                    "Reason, " +
                    "KPI, " +
                    "EntryType, " +
                    "Remarks, " +
                    "Signature, " +
                    "tl.CreatedDate, " +
                    "t.TreeNumber, " +
                    "tl.STM " +
                "FROM TallyLedger AS tl " +
                "LEFT JOIN Tree_V3 AS t USING (TreeID) " +
                "WHERE tl.CuttingUnitCode = @p1 " +
                "ORDER BY tl.CreatedDate DESC;",
                new object[] { unitCode })
                .ToArray();

            //From<TallyEntry>()
            ////.Where("UnitCode = @p1 AND PlotNumber IS NULL ")
            //.Where("UnitCode = @p1")
            //.OrderBy("TimeStamp DESC")
            //.Limit(NUMBER_OF_TALLY_ENTRIES_PERPAGE, 0 * NUMBER_OF_TALLY_ENTRIES_PERPAGE)
            //.Query(unitCode);
        }

        public IEnumerable<TallyEntry> GetTallyEntries(string unitCode, int plotNumber)
        {
            return Database.Query<TallyEntry>(
                "SELECT " +
                    "TreeID, " +
                    "TallyLedgerID, " +
                    "CuttingUnitCode, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "TreeCount, " +
                    "Reason, " +
                    "KPI, " +
                    "EntryType, " +
                    "Remarks, " +
                    "Signature, " +
                    "CreatedDate, " +
                    "t.TreeNumber, " +
                    "tl.STM " +
                "FROM TallyLedger AS tl " +
                "LEFT JOIN Tree_V3 USING (TreeID) " +
                "WHERE tl.CuttingUnitCode = @p1;",
                new object[] { unitCode })
                .ToArray();

            //return Database.From<TallyEntry>()
            //    .LeftJoin("Tree", "USING (Tree_GUID)")
            //    .Where("UnitCode = @p1 AND PlotNumber = @p2 ")
            //    .OrderBy("TimeStamp DESC")
            //    .Query(unitCode, plotNumber);
        }

        public void InsertTallyLedger(TallyLedger tallyLedger)
        {
            var tallyLedgerID = tallyLedger.TallyLedgerID ?? Guid.NewGuid().ToString();

            Database.Execute2(
                "INSERT INTO TallyLedger (" +
                    "TallyLedgerID, " +
                    "CuttingUnitCode, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "PlotNumber, " +
                    "Species, " +
                    "LiveDead," +
                    "TreeCount, " +
                    "KPI, " +
                    "ThreePRandomValue, " +
                    "TreeID, " +
                    "CreatedBy, " +
                    "Reason, " +
                    "Signature, " +
                    "Remarks, " +
                    "EntryType" +
                ") VALUES ( " +
                    "@TallyLedgerID, " +
                    "@CuttingUnitCode, " +
                    "@StratumCode, " +
                    "@SampleGroupCode, " +
                    "@PlotNumber, " +
                    "@Species, " +
                    "@LiveDead, " +
                    "@TreeCount, " +
                    "@KPI, " +
                    "@ThreePRandomValue, " +
                    "@TreeID, " +
                    "@CreatedBy, " +
                    "@Reason, " +
                    "@Signature, " +
                    "@Remarks, " +
                    "@EntryType" +
                ");",
                new
                {
                    TallyLedgerID = tallyLedgerID,
                    tallyLedger.CuttingUnitCode,
                    tallyLedger.StratumCode,
                    tallyLedger.SampleGroupCode,
                    tallyLedger.PlotNumber,
                    tallyLedger.Species,
                    tallyLedger.LiveDead,
                    tallyLedger.TreeCount,
                    tallyLedger.KPI,
                    tallyLedger.ThreePRandomValue,
                    tallyLedger.TreeID,
                    tallyLedger.CreatedBy,
                    tallyLedger.Reason,
                    tallyLedger.Signature,
                    tallyLedger.Remarks,
                    tallyLedger.EntryType,
                });

            tallyLedger.TallyLedgerID = tallyLedgerID;
        }

        public TallyEntry InsertTallyAction(TallyAction atn)
        {
            if (atn.IsInsuranceSample == true && atn.IsSample == false) { throw new InvalidOperationException("If action is insurance sample it must be sample aswell"); }

            Database.BeginTransaction();
            try
            {
                var tallyEntry = new TallyEntry(atn);

                tallyEntry.TallyLedgerID = Guid.NewGuid().ToString();

                if (atn.IsSample)
                {
                    tallyEntry.TreeID = tallyEntry.TallyLedgerID;

                    tallyEntry.TreeNumber = Database.ExecuteScalar2<int>(
                        "SELECT " +
                        "ifnull(max(TreeNumber), 0) + 1 " +
                        "FROM Tree_V3 " +
                        "WHERE CuttingUnitCode = @CuttingUnitCode " +
                        "AND ifnull(PlotNumber, -1) = ifnull(@PlotNumber, -1)",
                        new { atn.CuttingUnitCode, atn.PlotNumber });

                    Database.Execute2(
                        "INSERT INTO Tree_V3 ( " +
                            "TreeID, " +
                            "CuttingUnitCode, " +
                            "PlotNumber, " +
                            "StratumCode, " +
                            "SampleGroupCode, " +
                            "Species, " +
                            "LiveDead, " +
                            "TreeNumber, " +
                            "CountOrMeasure, " +
                            "CreatedBy " +
                        ") VALUES ( " +
                            "@TreeID, " +
                            "@CuttingUnitCode, " +
                            "@PlotNumber, " +
                            "@StratumCode, " +
                            "@SampleGroupCode, " +
                            "@Species, " +
                            "@LiveDead, " +
                            "@TreeNumber," +
                            "@CountOrMeasure," +
                            "@UserName " +
                        ");",
                        new
                        {
                            tallyEntry.TreeID,
                            tallyEntry.TreeNumber,
                            atn.CuttingUnitCode,
                            atn.PlotNumber,
                            atn.StratumCode,
                            atn.SampleGroupCode,
                            atn.Species,
                            atn.LiveDead,
                            CountOrMeasure = (atn.IsInsuranceSample ? "I" : "M"),
                            UserName,
                        });
                }

                Database.Execute2(
                    "INSERT INTO TallyLedger ( " +
                        "TreeID, " +
                        "TallyLedgerID, " +
                        "CuttingUnitCode, " +
                        "PlotNumber, " +
                        "StratumCode, " +
                        "SampleGroupCode, " +
                        "Species, " +
                        "LiveDead, " +
                        "TreeCount, " +
                        "KPI, " +
                        "STM, " +
                        "ThreePRandomValue, " +
                        "EntryType, " +
                        "CreatedBy" +
                    ") VALUES ( " +
                        "@TreeID, " +
                        "@TallyLedgerID, " +
                        "@CuttingUnitCode, " +
                        "@PlotNumber, " +
                        "@StratumCode, " +
                        "@SampleGroupCode, " +
                        "@Species, " +
                        "@LiveDead, " +
                        "@TreeCount, " +
                        "@KPI, " +
                        "@STM, " +
                        "@ThreePRandomValue," +
                        "@EntryType," +
                        "@CreatedBy" +
                    ");",
                    new
                    {
                        tallyEntry.TreeID,
                        tallyEntry.TallyLedgerID,
                        atn.CuttingUnitCode,
                        atn.PlotNumber,
                        atn.StratumCode,
                        atn.SampleGroupCode,
                        atn.Species,
                        atn.LiveDead,
                        atn.TreeCount,
                        atn.KPI,
                        atn.STM,
                        atn.ThreePRandomValue,
                        atn.EntryType,
                        CreatedBy = UserName,
                    });

                Database.CommitTransaction();

                return tallyEntry;
            }
            catch
            {
                Database.RollbackTransaction();
                throw;
            }
        }

        public void DeleteTallyEntry(string tallyLedgerID)
        {
            Database.BeginTransaction();
            try
            {
                Database.Execute("DELETE FROM TREE_V3 WHERE TreeID IN (SELECT TreeID FROM TallyLedger WHERE TallyLedgerID = @p1);", tallyLedgerID);
                Database.Execute("DELETE FROM TallyLedger WHERE TallyLedgerID = @p1;", tallyLedgerID);

                Database.CommitTransaction();
            }
            catch
            {
                Database.RollbackTransaction();
                throw;
            }
        }

        #endregion Tally Entry

        #region Log

        public IEnumerable<Log> GetLogs(string treeID)
        {
            return Database.From<Log>()
                .Where("TreeID = @p1")
                .Query(treeID).ToArray();
        }

        public Log GetLog(string logID)
        {
            return Database.From<Log>()
                .Where("LogID = @p1")
                .Query(logID).FirstOrDefault();
        }

        public Log GetLog(string treeID, int logNumber)
        {
            return Database.From<Log>()
                .Where("TreeID = @p1 AND LogNumber = @p2")
                .Query(treeID, logNumber).FirstOrDefault();
        }

        public void InsertLog(Log log)
        {
            var logID = Guid.NewGuid().ToString();

            var logNumber = Database.ExecuteScalar<int>("SELECT ifnull(max(LogNumber), 0) + 1 FROM Log_V3 WHERE TreeID = @p1", log.TreeID);

            Database.Execute2(
                "INSERT INTO Log_V3 ( " +
                    "LogID , " +
                    "TreeID, " +
                    "LogNumber, " +

                    "Grade, " +
                    "SeenDefect, " +
                    "PercentRecoverable, " +
                    "Length, " +
                    "ExportGrade, " +

                    "SmallEndDiameter, " +
                    "LargeEndDiameter, " +
                    "GrossBoardFoot, " +
                    "NetBoardFoot, " +
                    "GrossCubicFoot, " +

                    "NetCubicFoot, " +
                    "BoardFootRemoved, " +
                    "CubicFootRemoved, " +
                    "DIBClass, " +
                    "BarkThickness, " +

                    "CreatedBy " +
                ") VALUES ( " +
                    "@LogID, " +
                    "@TreeID, " +
                    "@LogNumber, " +

                    "@Grade, " +
                    "@SeenDefect, " +
                    "@PercentRecoverable, " +
                    "@Length," +
                    "@ExportGrade, " +

                    "@SmallEndDiameter, " +
                    "@LargeEndDiameter, " +
                    "@GrossBoardFoot, " +
                    "@NetBoardFoot, " +
                    "@GrossCubicFoot, " +

                    "@NetCubicFoot, " +
                    "@BoardFootRemoved, " +
                    "@CubicFootRemoved, " +
                    "@DIBClass, " +
                    "@BarkThickness, " +

                    "@CreatedBy" +
                ");",
                new
                {
                    LogID = logID,
                    log.TreeID,
                    LogNumber = logNumber,

                    log.Grade,
                    log.SeenDefect,
                    log.PercentRecoverable,
                    log.Length,
                    log.ExportGrade,

                    log.SmallEndDiameter,
                    log.LargeEndDiameter,
                    log.GrossBoardFoot,
                    log.NetBoardFoot,
                    log.GrossCubicFoot,

                    log.NetCubicFoot,
                    log.BoardFootRemoved,
                    log.CubicFootRemoved,
                    log.DIBClass,
                    log.BarkThickness,

                    CreatedBy = UserName,
                });

            log.LogNumber = logNumber;
            log.LogID = logID;
        }

        public void UpdateLog(Log log)
        {
            Database.Execute("UPDATE Log_V3 SET " +
                "LogNumber = @p1, " +
                "Grade = @p2, " +
                "SeenDefect = @p3, " +
                "PercentRecoverable = @p4, " +
                "Length = @p5, " +
                "ExportGrade = @p6, " +
                "SmallEndDiameter = @p7, " +
                "LargeEndDiameter = @p8, " +
                "GrossBoardFoot = @p9, " +
                "NetBoardFoot = @p10, " +
                "GrossCubicFoot = @p11, " +
                "NetCubicFoot = @p12, " +
                "BoardFootRemoved = @p13, " +
                "CubicFootRemoved = @p14, " +
                "DIBClass = @p15, " +
                "BarkThickness = @p16, " +
                "ModifiedBy = @p17 " +
                "WHERE LogID = @p18;",
                log.LogNumber,
                log.Grade,
                log.SeenDefect,
                log.PercentRecoverable,
                log.Length,
                log.ExportGrade,
                log.SmallEndDiameter,
                log.LargeEndDiameter,
                log.GrossBoardFoot,
                log.NetBoardFoot,
                log.GrossCubicFoot,
                log.NetCubicFoot,
                log.BoardFootRemoved,
                log.CubicFootRemoved,
                log.DIBClass,
                log.BarkThickness,
                UserName,
                log.LogID);
        }

        public void DeleteLog(string logID)
        {
            Database.Execute("DELETE FROM Log_V3 WHERE LogID = @p1;", logID);
        }

        private static readonly LogFieldSetup[] DEFAULT_LOG_FIELDS = new LogFieldSetup[]{
            new LogFieldSetup(){
                Field = nameof(Log.LogNumber), Heading = "LogNum"},
            new LogFieldSetup(){
                Field = nameof(Log.Grade), Heading = "Grade"},
            new LogFieldSetup() {
                Field = nameof(Log.SeenDefect), Heading = "PctSeenDef"}
        };

        public IEnumerable<LogFieldSetup> GetLogFields(string treeID)
        {
            var fields = Database.From<LogFieldSetup>()
                .Where("StratumCode = (SELECT StratumCode FROM Tree_V3 WHERE TreeID = @p1)")
                .OrderBy("FieldOrder")
                .Query(treeID).ToArray();

            if (fields.Length == 0)
            {
                return DEFAULT_LOG_FIELDS;
            }
            else
            {
                return fields;
            }
        }

        #endregion Log

        public void LogMessage(string message, string level)
        {
            Database.LogMessage(message, level);

            //Database.Execute2("INSERT INTO MessageLog (Message, Level) VALUES (@message, @level);",
            //    new { Message = message, Level = level });
        }
    }
}