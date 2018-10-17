using CruiseDAL;
using CruiseDAL.DataObjects;
using CruiseDAL.Schema;
using FMSC.ORM.Core.SQL.QueryBuilder;
using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Models;
using FScruiser.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public class CuttingUnitDatastore : ICuttingUnitDatastore
    {
        private const int NUMBER_OF_TALLY_ENTRIES_PERPAGE = 20;

        private readonly string PLOT_METHODS = String.Join(", ", CruiseMethods.PLOT_METHODS.Select(x => "'" + x + "'").ToArray());

        private const string CREATE_TREE_COMMAND = "INSERT INTO Tree " +
                "(Tree_GUID, TreeNumber, CuttingUnit_CN, Stratum_CN, SampleGroup_CN, TreeDefaultValue_CN, Species, LiveDead, CountOrMeasure, TreeCount, KPI, STM) " +
                "VALUES (@p1,\r\n " +
                "(SELECT ifnull(max(TreeNumber), 0) + 1  FROM Tree JOIN CuttingUnit USING (CuttingUnit_CN) WHERE CuttingUnit.Code = @p2 AND Tree.Plot_CN IS NULL),\r\n " +
                "(SELECT CuttingUnit_CN FROM CuttingUnit WHERE Code = @p2),\r\n " +
                "(SELECT Stratum_CN FROM Stratum WHERE Code = @p3),\r\n " +
                "(SELECT SampleGroup_CN FROM SampleGroup JOIN Stratum USING (Stratum_CN) WHERE SampleGroup.Code = @p4 AND Stratum.Code = @p3),\r\n " +
                "(SELECT TreeDefaultValue_CN FROM SampleGroupTreeDefaultValue JOIN SampleGroup USING (SampleGroup_CN) JOIN Stratum USING (Stratum_CN) JOIN TreeDefaultValue USING (TreeDefaultValue_CN) WHERE SampleGroup.Code = @p4 AND Stratum.Code = @p3 AND TreeDefaultValue.Species = @p5 AND TreeDefaultValue.LiveDead = @p6),\r\n " +
                "@p5,\r\n" + //species
                "@p6,\r\n" + //liveDead
                "@p7,\r\n " + //countMeasure
                "@p8,\r\n  " + //treecount
                "@p9,\r\n " + //kpi
                "@p10);\r\n "; //stm

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

        public CuttingUnit_Ex GetUnit(string code)
        {
            var unit = Database.From<CuttingUnit_Ex>()
                .Where("Code = @p1")
                .Query(code).FirstOrDefault();

            unit.HasPlotStrata = Database.ExecuteScalar<int>("SELECT count(*) FROM CuttingUnitStratum AS [cust] " +
                "JOIN Stratum USING (Stratum_CN) " +
                "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                "WHERE CuttingUnit.Code = @p1 " +
                $"AND Stratum.Method IN ({PLOT_METHODS});", code) > 0;

            unit.HasTreeStrata = Database.ExecuteScalar<int>("SELECT count(*) FROM CuttingUnitStratum AS [cust] " +
                "JOIN Stratum USING (Stratum_CN) " +
                "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                "WHERE CuttingUnit.Code = @p1 " +
                $"AND Stratum.Method NOT IN ({PLOT_METHODS});", code) > 0;

            return unit;
        }

        public IEnumerable<CuttingUnit> GetUnits()
        {
            return Database.From<CuttingUnit>()
                .Query().ToArray();
        }

        #region plot

        public int GetNextPlotNumber(string unitCode)
        {
            return Database.ExecuteScalar<int>("SELECT ifnull(max(PlotNumber), 0) + 1 FROM Plot " +
                "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                "WHERE CuttingUnit.Code = @p1;", unitCode);
        }

        public bool IsPlotNumberAvalible(string unitCode, int plotNumber)
        {
            return Database.ExecuteScalar<int>("SELECT count(*) FROM Plot " +
                "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                "WHERE CuttingUnit.Code = @p1 AND PlotNumber = @p2;", unitCode, plotNumber) == 0;
        }

        public IEnumerable<Plot> GetPlotsByUnitCode(string unit)
        {
            return Database.Query<Plot>("SELECT PlotNumber, CuttingUnit_CN  FROM Plot " +
                "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                "WHERE CuttingUnit.Code = @p1 " +
                " GROUP BY PlotNumber, CuttingUnit_CN;"
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

        public IEnumerable<StratumPlot> GetStratumPlots(string unitCode, int plotNumber, bool insertIfNotExists = false)
        {
            var stratumPlots = new List<StratumPlot>();

            var strata = GetPlotStrataProxies(unitCode).ToArray();

            foreach (var stratum in strata)
            {
                var stratumPlot = GetStratumPlot(unitCode, stratum.Code, plotNumber, insertIfNotExists);

                stratumPlots.Add(stratumPlot);
            }

            return stratumPlots.ToArray();
        }

        public StratumPlot GetStratumPlot(string unitCode, string stratumCode, int plotNumber, bool insertIfNotExists = false)
        {
            var stratumPlot = Database.Query<StratumPlot>("SELECT 1 AS InCruise, Stratum.Code AS StratumCode, Plot.* FROM Plot " +
                    "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                    "JOIN Stratum USING (Stratum_CN) " +
                    "WHERE CuttingUnit.Code = @p1 " +
                    "AND Stratum.Code = @p2 " +
                    "AND PlotNumber = @p3;", new object[] { unitCode, stratumCode, plotNumber }).FirstOrDefault();

            if (stratumPlot == null)
            {
                stratumPlot = new StratumPlot()
                {
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    InCruise = false
                };

                if (insertIfNotExists)
                {
                    InsertStratumPlot(unitCode, stratumPlot);
                    stratumPlot.InCruise = true;
                }
            }
            else
            {
                stratumPlot.InCruise = true;
            }

            return stratumPlot;
        }

        public void InsertStratumPlot(string unitCode, StratumPlot stratumPlot)
        {
            string plot_guid = Guid.NewGuid().ToString();

            Database.Execute("INSERT INTO Plot " +
                   "(Plot_GUID, CuttingUnit_CN, Stratum_CN, PlotNumber, IsEmpty, Slope, KPI, Aspect, Remarks, XCoordinate, YCoordinate, CreatedBy) " +
                   "VALUES (" +
                   "@p1, " + //plot_guid
                   "(SELECT CuttingUnit_CN FROM CuttingUnit WHERE Code = @p2), " + //unitCode
                   "(SELECT Stratum_CN FROM Stratum WHERE Code = @p3), " + //stratumCode
                   "@p4, " + //plotNumber
                   "@p5, " + //isEmpty
                   "(SELECT Slope FROM Plot JOIN CuttingUnit USING (CuttingUnit_CN) WHERE CuttingUnit.Code = @p2 AND PlotNumber = @p4 LIMIT 1), " + //slope
                   "@p6, " + //kpi
                   "(SELECT Aspect FROM Plot JOIN CuttingUnit USING (CuttingUnit_CN) WHERE CuttingUnit.Code = @p2 AND PlotNumber = @p4 LIMIT 1), " + //aspect
                   "@p7, " + //remarks
                   "(SELECT XCoordinate FROM Plot JOIN CuttingUnit USING (CuttingUnit_CN) WHERE CuttingUnit.Code = @p2 AND PlotNumber = @p4 LIMIT 1), " + //xcoord
                   "(SELECT YCoordinate FROM Plot JOIN CuttingUnit USING (CuttingUnit_CN) WHERE CuttingUnit.Code = @p2 AND PlotNumber = @p4 LIMIT 1), " + //ycoord
                   "@p8);", new object[] {
                        plot_guid,
                        unitCode,
                        stratumPlot.StratumCode,
                        stratumPlot.PlotNumber,
                        stratumPlot.IsEmpty,
                        stratumPlot.KPI,
                        stratumPlot.Remarks,
                        "AndroidUser"});

            stratumPlot.Plot_GUID = plot_guid;
            stratumPlot.InCruise = true;
        }

        public void UpdateStratumPlot(StratumPlot stratumPlot)
        {
            Database.Execute("UPDATE Plot SET " +
                    "PlotNumber = @p2, " +
                    "IsEmpty = @p3, " +
                    "KPI = @p4, " +
                    "Remarks = @p5 " +
                    "Slope = @p6 " +
                    "Aspect = @p7" +
                    "WHERE " +
                    "Plot_GUID = @p1;",
                    new object[] {
                        stratumPlot.Plot_GUID,
                        stratumPlot.PlotNumber,
                        stratumPlot.IsEmpty,
                        stratumPlot.KPI,
                        stratumPlot.Remarks,
                        stratumPlot.Slope,
                        stratumPlot.Aspect
                        });
        }

        public void DeleteStratumPlot(string plot_guid)
        {
            Database.Execute("DELETE FROM Log WHERE Tree_CN IN (SELECT Tree_CN FROM Tree WHERE Plot_CN IN (SELECT Plot_CN FROM Plot WHERE Plot_GUID = @p1)); ", plot_guid);
            Database.Execute("DELETE FROM Tree WHERE Plot_CN = (SELECT Plot_CN FROM Plot WHERE Plot_GUID = @p1); ", plot_guid);
            Database.Execute("DELETE FROM Plot WHERE Plot_GUID = @p1;", plot_guid);
        }

        public void DeletePlot(string unitCode, int plotNumber)
        {
            Database.Execute(
                "DELETE FROM Log WHERE Tree_CN IN " +
                "(SELECT Tree_CN FROM Tree WHERE Plot_CN IN " +
                "(SELECT Plot_CN FROM Plot WHERE " +
                "CuttingUnit_CN = (SELECT CuttingUnit_CN FROM CuttingUnit WHERE Code = @p1) " +
                "AND PlotNumber = @p2));" +

                "DELETE FROM Tree WHERE Plot_CN IN " +
                "(SELECT Plot_CN FROM Plot WHERE " +
                "CuttingUnit_CN = (SELECT CuttingUnit_CN FROM CuttingUnit WHERE Code = @p1) " +
                "AND PlotNumber = @p2);" +

                "DELETE FROM Plot WHERE " +
                "CuttingUnit_CN = (SELECT CuttingUnit_CN FROM CuttingUnit WHERE Code = @p1) " +
                "AND PlotNumber = @p2;", new object[] { unitCode, plotNumber });
        }

        public int GetNumTreeRecords(string unitCode, string stratumCode, int plotNumber)
        {
            return Database.ExecuteScalar<int>("SELECT Count(*) FROM Tree " +
                "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                "JOIN Stratum USING (Stratum_CN) " +
                "JOIN Plot USING (Plot_CN) " +
                "WHERE CuttingUnit.Code = @p1 AND Stratum.Code = @p2 AND Plot.PlotNumber = @p3;",
                unitCode, stratumCode, plotNumber);
        }

        #endregion plot

        #region strata

        public IEnumerable<Stratum> GetStrataByUnitCode(string unitCode)
        {
            return Database.From<Stratum>()
                .Join("CuttingUnitStratum", "USING (Stratum_CN)")
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where($"CuttingUnit.Code = @p1 AND Stratum.Method NOT IN ({PLOT_METHODS})")
                .Query(unitCode).ToArray();
        }

        public IEnumerable<StratumProxy> GetStrataProxiesByUnitCode(string unitCode)
        {
            return Database.From<StratumProxy>()
                .Join("CuttingUnitStratum", "USING (Stratum_CN)")
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where($"CuttingUnit.Code = @p1 AND Stratum.Method NOT IN ({PLOT_METHODS})")
                .Query(unitCode);
        }

        public IEnumerable<StratumProxy> GetPlotStrataProxies(string unitCode)
        {
            return Database.From<StratumProxy>()
                .Join("CuttingUnitStratum", "USING (Stratum_CN)")
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                //.Where($"CuttingUnit.Code = @p1")
                .Where($"CuttingUnit.Code = @p1 AND Stratum.Method IN ({PLOT_METHODS})")
                .Query(unitCode);
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

        public IEnumerable<SampleGroupProxy> GetSampleGroupProxiesByUnit(string unitCode)
        {
            throw new NotImplementedException();
        }

        public SampleGroup GetSampleGroup(string stratumCode, string sgCode)
        {
            return Database.From<SampleGroup>()
                .Join("Stratum", "USING (Stratum_CN)")
                .Where("Stratum.Code = @p1 AND SampleGroup.Code = @p2")
                .Query(stratumCode, sgCode).FirstOrDefault();
        }

        public IEnumerable<SampleGroupProxy> GetSampleGroupProxies(string stratumCode)
        {
            return Database.From<SampleGroupProxy>()
                .Join("Stratum", "USING (Stratum_CN)")
                .Where("Stratum.Code = @p1")
                .Query(stratumCode);
        }

        public SampleGroupProxy GetSampleGroupProxy(string stratumCode, string sampleGroupCode)
        {
            return Database.From<SampleGroupProxy>()
                .Join("Stratum", "USING (Stratum_CN)")
                .Where("Stratum.Code = @p1 AND SampleGroup.Code = @p2")
                .Query(stratumCode, sampleGroupCode).FirstOrDefault();
        }

        public IEnumerable<TallyPopulation> GetTallyPopulationsByUnitCode(string unitCode)
        {
            return Database.From<TallyPopulation>()
                .Join("Tally", "USING (Tally_CN)")
                .Join("SampleGroup", "USING (SampleGroup_CN)")
                .LeftJoin("TreeDefaultValue", "USING (TreeDefaultValue_CN)")
                .Join("Stratum", "USING (Stratum_CN)")
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where($"CuttingUnit.Code = @p1 AND Stratum.Method NOT IN ({PLOT_METHODS})")
                .Query(unitCode).ToArray();
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

        public IEnumerable<TallyPopulation_Plot> GetPlotTallyPopulationsByUnitCode(string unitCode, int plotNumber)
        {
            //get all samplegroups in unit where stratum method is a plot based method
            var sampleGroups = Database.Query<SampleGroupStratumInfo>("SELECT SampleGroup.Code AS SgCode, Stratum.Code AS StCode, Stratum.Method AS Method " +
                "FROM SampleGroup " +
                "JOIN Stratum USING (Stratum_CN) " +
                "JOIN CuttingUnitStratum USING (Stratum_CN) " +
                "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                $"WHERE CuttingUnit.Code = @p1 AND Stratum.Method IN ({PLOT_METHODS});", new object[] { unitCode });

            var tallyPopulations = new List<TallyPopulation_Plot>();

            //for each sampleGroup get a list of tally populations, if there are none create a tally population if the cruis method is a single stage plot cruise method
            foreach (var sg in sampleGroups)
            {
                //get tally populations by reading from the count tree table for all cutting units, allowing us to fill in gaps for units with missing count tree records.
                var sgTallyPops = Database.Query<TallyPopulation_Plot>("SELECT SampleGroup_CN, Tally.Description AS TallyDescription, Tally.HotKey AS TallyHotKey, Stratum.Code AS StratumCode, Stratum.Method AS StratumMethod, " +
                    "SampleGroup.Code AS SampleGroupCode, SampleGroup.SampleSelectorType AS sgSampleSelectorType, SampleGroup.MinKPI AS sgMinKPI, SampleGroup.MaxKPI AS sgMaxKPI, " +
                    "TDV.Species AS tdvSpecies, TDV.LiveDead AS tdvLiveDead " +
                    "FROM CountTree " +
                    "JOIN Tally USING (Tally_CN) " +
                    "JOIN SampleGroup USING (SampleGroup_CN) " +
                    "LEFT JOIN TreeDefaultValue AS TDV USING (TreeDefaultValue_CN) " +
                    "JOIN Stratum USING (Stratum_CN) " +
                    $"WHERE SampleGroup.Code = @p1 AND Stratum.Code = @p2 AND Stratum.Method IN ({PLOT_METHODS}) " +
                    "GROUP BY SampleGroup.Code, Stratum.Code;",
                    new object[]
                    {
                        sg.SgCode, sg.StCode
                    }).ToArray();

                //if there are tally populations then go through them and
                //check if they are in the cruise for this plot number
                if (sgTallyPops.Any())
                {
                    foreach (var tallyPop in sgTallyPops)
                    {
                        tallyPop.CountTree_CN = Database.ExecuteScalar<long?>("SELECT CountTree_CN FROM CountTree " +
                            "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                            "JOIN SampleGroup USING (SampleGroup_CN) " +
                            "JOIN Stratum USING (Stratum_CN) " +
                            "WHERE CuttingUnit.Code = @p1 AND Stratum.Code = @p2 AND SampleGroup.Code = @p3;"
                            , new object[] { unitCode, sg.StCode, sg.SgCode });

                        tallyPop.InCruise = Database.ExecuteScalar<bool?>("SELECT 1 FROM Plot " +
                            "JOIN Stratum USING (Stratum_CN) " +
                            "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                            "WHERE Stratum.Code = @p1 AND CuttingUnit.Code = @p2 AND PlotNumber = @p3;", tallyPop.StratumCode, unitCode, plotNumber) ?? false;
                    }
                }
                //if there are no tally populations in the sample group
                //and the cruise method is a single stage plot method
                else if (sg.Method == CruiseMethods.FIX || sg.Method == CruiseMethods.PNT)
                {
                    sgTallyPops = new TallyPopulation_Plot[]
                    {
                        new TallyPopulation_Plot()
                        {
                            InCruise = true,
                            SampleGroupCode = sg.SgCode,
                            StratumCode = sg.StCode,
                            TallyDescription = sg.StCode + " | " + sg.SgCode,
                            Method = sg.Method
                        }
                    };
                }

                tallyPopulations.AddRange(sgTallyPops);
            }

            return tallyPopulations;

            //return Database.From<TallyPopulation>()
            //    .Join("Tally", "USING (Tally_CN)")
            //    .Join("SampleGroup", "USING (SampleGroup_CN)")
            //    .LeftJoin("TreeDefaultValue", "USING (TreeDefaultValue_CN)")
            //    .Join("Stratum", "USING (Stratum_CN)")
            //    .Join("CuttingUnit", "USING (CuttingUnit_CN)")
            //    .Where($"CuttingUnit.Code = @p1 AND Stratum.Method IN ({string.Join(", ", CruiseMethods.PLOT_METHODS.Select(x => "'" + x + "'").ToArray())})")
            //    .Query(unitCode).ToArray();
        }

        #endregion sampleGroup

        #region TreeDefaults

        public IEnumerable<TreeDefaultProxy> GetTreeDefaultProxies(string stratumCode, string sampleGroupCode)
        {
            return Database.From<TreeDefaultProxy>()
                .Join("SampleGroupTreeDefaultValue", "USING (TreeDefaultValue_CN)")
                .Join("SampleGroup", "USING (SampleGroup_CN)")
                .Join("Stratum", "USING (Stratum_CN)")
                .Where("Stratum.Code = @p1 AND SampleGroup.Code = @p2")
                .Query(stratumCode, sampleGroupCode);
        }

        //public IEnumerable<TreeDefaultValueDO> GetTreeDefaultsByUnitCode(string unitCode)
        //{
        //    return Database.From<TreeDefaultValueDO>()
        //        .Join("SampleGroupTreeDefaultValue", "USING (TreeDefaultValue_CN)")
        //        .Join("SampleGroup", "USING (SampleGroup_CN)")
        //        .Join("CuttingUnitStratum", "USING (Stratum_CN)")
        //        .Join("CuttingUnit", "USING (CuttingUnit_CN)")
        //        .Where("CuttingUnit.Code = @p1")
        //        .GroupBy("TreeDefaultValue_CN")
        //        .Query(unitCode).ToArray();
        //}

        //public IEnumerable<TreeDefaultValueDO> GetTreeDefaultsBySampleGroup(string sgCode)
        //{
        //    return Database.From<TreeDefaultValueDO>()
        //        .Join("SampleGroupTreeDefaultValue", "USING (TreeDefaultValue_CN)")
        //        .Join("SampleGroup", "USING (SampleGroup_CN)")
        //        .Where("SampleGroup.Code = @p1")
        //        .Query(sgCode).ToArray();
        //}

        //public IEnumerable<SampleGroupTreeDefaultValueDO> GetSampleGroupTreeDefaultMaps(string stratumCode, string sgCode)
        //{
        //    return Database.From<SampleGroupTreeDefaultValueDO>()
        //        .Join("SampleGroup", "USING (SampleGroup_CN)")
        //        .Join("Stratum", "USING (Stratum_CN)")
        //        .Where("Stratum.Code = @p1 AND SampleGroup.Code = @p2")
        //        .Query(stratumCode, sgCode).ToArray();
        //}

        #endregion TreeDefaults

        #region TreeFields

        public IEnumerable<TreeFieldSetupDO> GetTreeFieldsByUnitCode(string unitCode)
        {
            return Database.From<TreeFieldSetupDO>()
                .Join("CuttingUnitStratum", "USING (Stratum_CN)")
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Join("Stratum", "USING (Stratum_CN)")
                .Where($"CuttingUnit.Code = @p1 AND Stratum.Method NOT IN ({PLOT_METHODS})")
                .GroupBy("Field")
                .OrderBy("FieldOrder")
                .Query(unitCode).ToArray();
        }

        public IEnumerable<TreeFieldSetupDO> GetTreeFieldsByStratumCode(string stratumCode)
        {
            return Database.From<TreeFieldSetupDO>()
                .Join("Stratum", "USING (Stratum_CN)")
                .Where("Stratum.Code == @p1")
                .OrderBy("FieldOrder")
                .Query(stratumCode);
        }

        #endregion TreeFields

        #region Tree

        public bool IsTreeNumberAvalible(string unit, int treeNumber, int? plotNumber = null)
        {
            if (plotNumber != null)
            {
                return Database.ExecuteScalar<int>("SELECT count(*) FROM Tree " +
                    "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                    "JOIN Plot USING (Plot_CN) " +
                    "WHERE CuttingUnit.Code = @p1 " +
                    "AND Plot.PlotNumber = @p2 " +
                    "AND Tree.TreeNumber = @p3;",
                    unit, plotNumber, treeNumber) > 0;
            }
            else
            {
                return Database.ExecuteScalar<int>("SELECT count(*) FROM Tree " +
                    "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                    "WHERE CuttingUnit.Code = @p1 " +
                    "AND Plot_CN IS NULL " +
                    "AND Tree.TreeNumber = @p2;",
                    unit, treeNumber) > 0;
            }
        }

        public IEnumerable<Tree> GetTreesByUnitCode(string unitCode)
        {
            return QueryTree_Base()
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where("CuttingUnit.Code = @p1 AND Plot_CN IS NULL")
                .Query(unitCode).ToArray();
        }

        public TreeStub GetTreeStub(string tree_GUID)
        {
            return QueryTreeStub()
                .Where("Tree_GUID = @p1")
                .Query(tree_GUID).FirstOrDefault();
        }

        public void UpdateTreeInitials(string tree_guid, string value)
        {
            Database.Execute("UPDATE Tree SET " +
                "Initials = @p1 " +
                "WHERE Tree_GUID = @p2",
                value, tree_guid);
        }

        public void DeleteTree(string tree_guid)
        {
            Database.Execute("Delete FROM Tree WHERE Tree_GUID = @p1", tree_guid);
        }

        public void DeleteTree(Tree tree)
        {
            DeleteTree(tree.Tree_GUID);
        }

        private IQuerryAcceptsJoin<TreeStub> QueryTreeStub()
        {
            return Database.From<TreeStub>()
                .Join("Stratum", "USING (Stratum_CN)")
                .Join("SampleGroup", "USING (SampleGroup_CN)");
        }

        public IEnumerable<TreeStub> GetTreeStubsByUnitCode(string unitCode)
        {
            return QueryTreeStub()
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where("CuttingUnit.Code = @p1 AND Plot_CN IS NULL")
                .Query(unitCode);
        }

        public IEnumerable<TreeStub_Plot> GetPlotTreeProxies(string unitCode, int plotNumber)
        {
            return Database.Query<TreeStub_Plot>("SELECT Tree_GUID, CuttingUnit.Code AS CuttingUnitCode, TreeNumber, Plot.PlotNumber AS PlotNumber, " +
                "Stratum.Code AS StratumCode, SampleGroup.Code AS SgCode, Tree.Species, Tree.LiveDead, Tree.TreeCount, Tree.STM, Tree.KPI, " +
                "max(TotalHeight, MerchHeightPrimary, UpperStemHeight) AS Height, " +
                "max(DBH, DRC, DBHDoubleBarkThickness) AS Diameter, " +
                "Tree.CountOrMeasure " +
                "FROM Tree " +
                "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                "JOIN Stratum USING (Stratum_CN) " +
                "LEFT JOIN SampleGroup USING (SampleGroup_CN) " +
                "JOIN Plot USING (Plot_CN) " +
                "WHERE CuttingUnit.Code = @p1 " +
                "AND PlotNumber = @p2;", new object[] { unitCode, plotNumber });
        }

        public Tree GetTree(string unitCode, int treeNumber)
        {
            return QueryTree_Base()
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where("CuttingUnit.Code = @p1 AND TreeNumber = @p2")
                .Query(unitCode, treeNumber).FirstOrDefault();
        }

        public Tree GetTree(string tree_GUID)
        {
            return QueryTree_Base()
                .Where("Tree_GUID = @p1")
                .Query(tree_GUID).FirstOrDefault();
        }

        private IQuerryAcceptsJoin<Tree> QueryTree_Base()
        {
            return Database.From<Tree>()
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
               .Join("Stratum", "USING (Stratum_CN)")
               .Join("SampleGroup", "USING (SampleGroup_CN)")
               .LeftJoin("Plot", "USING (Plot_CN)");
        }

        public string CreateTree(string unitCode, string stratumCode, string sampleGroupCode = null, string species = null, string liveDead = "L", string countMeasure = "M", int treeCount = 1, int kpi = 0, bool stm = false)
        {
            var tree_guid = Guid.NewGuid().ToString();
            CreateTree(tree_guid, unitCode, stratumCode, sampleGroupCode, species, liveDead, countMeasure, treeCount, kpi, stm);
            return tree_guid;
        }

        public string CreatePlotTree(string unitCode, int plotNumber, string stratumCode, string sampleGroupCode, string species = null, string liveDead = "L", string countMeasure = "M", int treeCount = 1, int kpi = 0, bool stm = false)
        {
            var tree_guid = Guid.NewGuid().ToString();
            CreatePlotTree(tree_guid, unitCode, plotNumber, stratumCode, sampleGroupCode, species, liveDead, countMeasure, treeCount, kpi, stm);
            return tree_guid;
        }

        public void CreatePlotTree(string tree_guid, string unitCode, int plotNumber, string stratumCode, string sampleGroupCode, string species = null, string liveDead = "L", string countMeasure = "M", int treeCount = 1, int kpi = 0, bool stm = false)
        {
            var plot_cn = Database.ExecuteScalar<int?>("SELECT Plot_CN FROM Plot " +
                "JOIN CuttingUnit_CN USING (CuttingUnit_CN) " +
                "JOIN Stratum USING (Stratum_CN) " +
                "WHERE CuttingUnit.Code = @p1 " +
                "AND Stratum.Code = @p2 " +
                "AND PlotNumber = @p3;", new object[] { unitCode, stratumCode, plotNumber });

            if (plot_cn == null) { throw new InvalidOperationException($"Plot # {plotNumber} could not be found"); }

            Database.Execute("INSERT INTO Tree " +
                "(Tree_GUID, TreeNumber, CuttingUnit_CN, Plot_CN, Stratum_CN, SampleGroup_CN, TreeDefaultValue_CN, Species, LiveDead, CountOrMeasure, TreeCount, KPI, STM) " +
                "VALUES (@p1,\r\n " +
                "(SELECT ifnull(max(TreeNumber), 0) + 1  FROM Tree JOIN CuttingUnit USING (CuttingUnit_CN) WHERE CuttingUnit.Code = @p2 AND Tree.Plot_CN = @p3),\r\n " +
                "(SELECT CuttingUnit_CN FROM CuttingUnit WHERE Code = @p2),\r\n " +
                "@p3,\r\n " +
                "(SELECT Stratum_CN FROM Stratum WHERE Code = @p4),\r\n " +
                "(SELECT SampleGroup_CN FROM SampleGroup JOIN Stratum USING (Stratum_CN) WHERE SampleGroup.Code = @p5 AND Stratum.Code = @p4),\r\n " +
                "(SELECT TreeDefaultValue_CN FROM SampleGroupTreeDefaultValue JOIN SampleGroup USING (SampleGroup_CN) JOIN Stratum USING (Stratum_CN) JOIN TreeDefaultValue USING (TreeDefaultValue_CN) WHERE SampleGroup.Code = @p5 AND Stratum.Code = @p4 AND TreeDefaultValue.Species = @p6 AND TreeDefaultValue.LiveDead = @p7),\r\n " +
                "@p6,\r\n" + //species
                "@p7,\r\n" + //liveDead
                "@p8,\r\n " + //countMeasure
                "@p9,\r\n  " + //treecount
                "@p10,\r\n " + //kpi
                "@p11);\r\n " //stm
                , new object[]
                {
                    tree_guid
                    , unitCode
                    , plot_cn
                    , stratumCode
                    , sampleGroupCode
                    , species ?? ""
                    , liveDead ?? ""
                    , countMeasure
                    , treeCount
                    , kpi
                    , (stm) ? "Y" : "N"
                }
            );
        }

        protected void CreateTree(string tree_guid, string unitCode, string stratumCode, string sampleGroupCode = null, string species = null, string liveDead = "L", string countMeasure = "M", int treeCount = 1, int kpi = 0, bool stm = false)
        {
            Database.Execute(CREATE_TREE_COMMAND
                , tree_guid,
                unitCode,
                stratumCode,
                sampleGroupCode,
                species ?? "",
                liveDead ?? "",
                countMeasure,
                treeCount,
                kpi,
                (stm) ? "Y" : "N");
        }

        public int GetNextPlotTreeNumber(string unitCode, string stratumCode, int plotNumber, bool isRecon)
        {
            if (!isRecon)
            {
                return Database.ExecuteScalar<int>("SELECT ifnull(max(TreeNumber), 0) + 1  FROM Tree " +
                    "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                    "JOIN Plot USING (Plot_CN) " +
                    "WHERE CuttingUnit.Code = @p1 AND Plot.PlotNumber = @p2;"
                    , unitCode, plotNumber);
            }
            else
            {
                return Database.ExecuteScalar<int>("SELECT ifnull(max(TreeNumber), 0) + 1  FROM Tree " +
                    "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                    "JOIN Plot USING (Plot_CN) " +
                    "JOIN Stratum USING (Stratum_CN) " +
                    "WHERE CuttingUnit.Code = @p1 AND PlotNumber = @p2 AND Stratum.Code = @p3;"
                    , unitCode, plotNumber, stratumCode);
            }
        }

        public void InsertTree(TreeStub_Plot tree)
        {
            var tree_guid = tree.Tree_GUID ?? Guid.NewGuid().ToString();

            int? plot_cn = Database.ExecuteScalar<int?>("SELECT Plot_CN FROM Plot " +
                    "JOIN Stratum USING (Stratum_CN) " +
                    "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                    "WHERE PlotNumber = @p1 " +
                    "AND Stratum.Code = @p2 " +
                    "AND CuttingUnit.Code = @p3;", tree.PlotNumber, tree.StratumCode, tree.CuttingUnitCode);

            if (plot_cn == null) { throw new InvalidOperationException($"Plot_CN value not found for Plot {tree.PlotNumber} and Stratum {tree.StratumCode}"); }

            Database.Execute("INSERT INTO TREE " +
                "(Tree_GUID, TreeNumber, CuttingUnit_CN, Plot_CN, Stratum_CN, SampleGroup_CN, TreeDefaultValue_CN, Species, LiveDead, CountOrMeasure, TreeCount, KPI, STM) " +
                "VALUES (@p1,\r\n " +
                "@p2,\r\n " +
                "(SELECT CuttingUnit_CN FROM CuttingUnit WHERE Code = @p3),\r\n " +
                "@p4,\r\n " +
                "(SELECT Stratum_CN FROM Stratum WHERE Code = @p5),\r\n " +
                "(SELECT SampleGroup_CN FROM SampleGroup JOIN Stratum USING (Stratum_CN) WHERE SampleGroup.Code = @p6 AND Stratum.Code = @p5),\r\n " +
                "(SELECT TreeDefaultValue_CN FROM SampleGroupTreeDefaultValue JOIN SampleGroup USING (SampleGroup_CN) JOIN Stratum USING (Stratum_CN) JOIN TreeDefaultValue USING (TreeDefaultValue_CN) WHERE SampleGroup.Code = @p6 AND Stratum.Code = @p5 AND TreeDefaultValue.Species = @p6 AND TreeDefaultValue.LiveDead = @p8),\r\n " +
                "@p7,\r\n" + //species
                "@p8,\r\n" + //liveDead
                "@p9,\r\n " + //countMeasure
                "@p10,\r\n  " + //treecount
                "@p11,\r\n " + //kpi
                "@p12);" //stm
                , new object[]
                {
                    tree_guid
                    , tree.TreeNumber
                    , tree.CuttingUnitCode
                    , plot_cn
                    , tree.StratumCode
                    , tree.SampleGroupCode
                    , tree.Species ?? ""
                    , tree.LiveDead ?? ""
                    , tree.CountOrMeasure
                    , tree.TreeCount
                    , tree.KPI
                    , tree.STM
                });
        }

        public void UpdateTree(Tree tree)
        {
            //if (tree.IsPersisted == false) { throw new InvalidOperationException("tree is not persisted before calling update"); }
            //Database.Update(tree);

            Database.Execute("UPDATE Tree SET \r\n" +
                "Stratum_CN = (SELECT Stratum_CN FROM Stratum WHERE Code = @p1), " +
                "SampleGroup_CN = (SELECT SampleGroup_CN FROM SampleGroup JOIN Stratum USING (Stratum_CN) WHERE Stratum.Code = @p1 AND SampleGroup.Code = @p2), " +
                "TreeDefaultValue_CN = " +
                    "(SELECT TreeDefaultValue_CN FROM TreeDefaultValue AS [tdv] " +
                    "JOIN SampleGroupTreeDefaultValue AS [sgtdv] USING (TreeDefaultValue_CN) " +
                    "JOIN SampleGroup USING (SampleGroup_CN) " +
                    "JOIN Stratum USING (Stratum_CN) " +
                    "WHERE Stratum.Code = @p1 AND SampleGroup.Code = @p2 AND [tdv].Species = @p3 AND [tdv].LiveDead = @p4), " +
                "Species = @p3," +
                "LiveDead = @p4, " +
                "CountOrMeasure = @p5, " +
                "TreeCount = @p6, " +
                "KPI = @p7, " +
                "STM = @p8, " +
                "SeenDefectPrimary = @p9, " +
                "SeenDefectSecondary = @p10, " +
                "RecoverablePrimary = @p11, " +
                "HiddenPrimary = @p12, " +
                "Initials = @p13, " +
                "Grade = @p14, " +
                "HeightToFirstLiveLimb = @p15, " +
                "PoleLength = @p16, " +
                "ClearFace = @p17, " +
                "CrownRatio = @p18, " +
                "DBH = @p19, " +
                "DRC = @p20, " +
                "TotalHeight = @p21, " +
                "MerchHeightPrimary = @p22, " +
                "MerchHeightSecondary = @p23, " +
                "FormClass = @p24, " +
                "UpperStemDiameter = @p25, " +
                "UpperStemHeight = @p26, " +
                "DBHDoubleBarkThickness = @p27, " +
                "TopDIBPrimary = @p28, " +
                "TopDIBSecondary = @p29, " +
                "DefectCode = @p30, " +
                "DiameterAtDefect = @p31, " +
                "VoidPercent = @p32, " +
                "Slope = @p33, " +
                "Aspect = @p34, " +
                "Remarks = @p35, " +
                "IsFallBuckScale = @p36, " +
                "ModifiedBy = @p37, " +
                "ModifiedDate = @p38 " +
                "WHERE Tree_GUID = @p39;",
                tree.StratumCode,
                tree.SampleGroupCode,
                tree.Species ?? "",
                tree.LiveDead ?? "",
                tree.CountOrMeasure ?? "",
                tree.TreeCount,
                tree.KPI,
                tree.STM,
                tree.SeenDefectPrimary,
                tree.SeenDefectSecondary,
                tree.RecoverablePrimary,
                tree.HiddenPrimary,
                tree.Initials,
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
                "mobile user ",
                DateTime.Now,
                tree.Tree_GUID);
        }

        public Task UpdateTreeAsync(Tree tree)
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

        public IEnumerable<TreeAuditRule> GetTreeAuditRules(string stratum, string sampleGroup, string species, string livedead)
        {
            return Database.Query<TreeAuditRule>("SELECT * FROM TreeAuditValue " +
                "JOIN TreeDefaultValueTreeAuditValue USING (TreeAuditValue_CN) " +
                "JOIN TreeDefaultValue AS TDV USING (TreeDefaultValue_CN) " +
                "JOIN SampleGroup ON TDV.PrimaryProduct = SampleGroup.PrimaryProduct " +
                "JOIN Stratum USING (Stratum_CN) " +
                "WHERE Stratum.Code = @p1 " +
                "AND SampleGroup.Code = @p2 " +
                "AND TDV.Species = @p3 " +
                "AND TDV.LiveDead = @p4;", new object[] { stratum, sampleGroup, species, livedead });
        }

        public void UpdateTreeErrors(string tree_GUID, IEnumerable<ValidationError> errors)
        {
            Database.Execute("DELETE FROM ErrorLog WHERE TableName = 'Tree' " +
                "AND CN_Number = (SELECT Tree_CN FROM Tree WHERE Tree_GUID = @p1) " +
                "AND Suppress = 0;", tree_GUID);

            foreach (var error in errors)
            {
                Database.Execute("INSERT OR IGNORE INTO ErrorLog (TableName, CN_Number, ColumnName, Level, Message, Program) " +
                    "VALUES ('Tree', (SELECT Tree_CN FROM Tree WHERE Tree_GUID = @p1), @p2, @p3, @p4, 'FScruiser');",
                    tree_GUID,
                    error.Property,
                    error.Level.ToString(),
                    error.Message);
            }
        }

        #endregion Tree Audits and ErrorLog

        #region Tally Entry

        public IEnumerable<TallyEntry> GetTallyEntriesByUnitCode(string unitCode)
        {
            return Database.From<TallyEntry>()
                .LeftJoin("Tree", "USING (Tree_GUID)")
                //.Where("UnitCode = @p1 AND PlotNumber IS NULL ")
                .Where("UnitCode = @p1")
                .OrderBy("TimeStamp DESC")
                .Limit(NUMBER_OF_TALLY_ENTRIES_PERPAGE, 0 * NUMBER_OF_TALLY_ENTRIES_PERPAGE)
                .Query(unitCode);
        }

        public IEnumerable<TallyEntry> GetTallyEntries(string unitCode, int plotNumber)
        {
            return Database.From<TallyEntry>()
                .LeftJoin("Tree", "USING (Tree_GUID)")
                .Where("UnitCode = @p1 AND PlotNumber = @p2 ")
                .OrderBy("TimeStamp DESC")
                .Query(unitCode, plotNumber);
        }

        public void InsertTallyEntry(TallyEntry entry)
        {
            Database.BeginTransaction();
            try
            {
                if (entry.HasTree)
                {
                    CreateTree(entry.Tree_GUID, entry.UnitCode, entry.StratumCode, entry.SampleGroupCode, entry.Species, entry.LiveDead, entry.CountOrMeasure, entry.TreeCount, entry.KPI, entry.IsSTM);
                    entry.TreeNumber = Database.ExecuteScalar<long>("SELECT TreeNumber FROM Tree WHERE Tree_GUID = @p1;", entry.Tree_GUID);
                }

                entry.TallyLedgerID = Guid.NewGuid().ToString();
                entry.TimeStamp = DateTime.Now;

                Database.Insert(entry);

                var countTree_CN = Database.ExecuteScalar<int?>("SELECT CountTree_CN FROM CountTree " +
                    "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                    "JOIN SampleGroup USING (SampleGroup_CN) " +
                    "JOIN Stratum USING (Stratum_CN) " +
                    "LEFT JOIN TreeDefaultValue as TDV USING (TreeDefaultValue_CN) " +
                    "WHERE CuttingUnit.Code = @p1 " +
                    "AND Stratum.Code = @p2 " +
                    "AND SampleGroup.Code = @p3 " +
                    "AND ifnull(TDV.Species,'') = @p4 " +
                    "AND ifnull(TDV.LiveDead, '') = @p5; "
                    , entry.UnitCode
                    , entry.StratumCode
                    , entry.SampleGroupCode
                    , entry.Species ?? ""
                    , entry.LiveDead ?? "");

                Database.Execute("UPDATE CountTree " +
                    "SET TreeCount = TreeCount + @p1, " +
                    "SumKPI = SumKPI + @p2 " +
                    "WHERE CountTree_CN = @p3;",
                    entry.TreeCount,
                    entry.KPI,
                    countTree_CN);

                //Database.Execute(
                //    "UPDATE CountTree SET " +
                //    "TreeCount = TreeCount + @p1, " +
                //    "SumKPI = SumKPI + @p2 " +
                //    "WHERE CountTree_CN IN (" +
                //    "SELECT CountTree_CN FROM CountTree " +
                //    "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                //    "JOIN SampleGroup USING (SampleGroup_CN) " +
                //    "JOIN Stratum USING (Stratum_CN) " +
                //    "LEFT JOIN TreeDefaultValue as TDV USING (TreeDefaultValue_CN) " +
                //    "WHERE CuttingUnit.Code = @p3 " +
                //    "AND Stratum.Code = @p4 " +
                //    "AND SampleGroup.Code = @p5 " +
                //    "AND ifnull(TDV.Species,'') = ifnull(@p6, '') " +
                //    "AND ifnull(TDV.LiveDead, '') = ifnull(@p7, ''); "
                //    , entry.TreeCount
                //    , entry.KPI
                //    , entry.UnitCode
                //    , entry.StratumCode
                //    , entry.SGCode
                //    , entry.Species
                //    , entry.LiveDead);

                Database.CommitTransaction();
            }
            catch
            {
                Database.RollbackTransaction();
                throw;
            }
        }

        public void DeleteTally(TallyEntry entry)
        {
            Database.BeginTransaction();
            try
            {
                Database.Execute("DELETE FROM TallyLedger WHERE TallyLedgerID = @p1;", entry.TallyLedgerID);

                if (entry.HasTree)
                {
                    Database.Execute("DELETE FROM Tree WHERE Tree_GUID = @p1;", entry.Tree_GUID);
                }

                Database.Execute(
                        "UPDATE CountTree SET " +
                        "TreeCount = TreeCount - @p1, " +
                        "SumKPI = SumKPI - @p2 " +
                        "WHERE CountTree_CN IN (" +
                        "SELECT CountTree_CN FROM CountTree " +
                        "JOIN CuttingUnit USING (CuttingUnit_CN) " +
                        "JOIN SampleGroup USING (SampleGroup_CN) " +
                        "JOIN Stratum USING (Stratum_CN) " +
                        "LEFT JOIN TreeDefaultValue as TDV USING (TreeDefaultValue_CN) " +
                        "WHERE CuttingUnit.Code = @p3 " +
                        "AND Stratum.Code = @p4 " +
                        "AND SampleGroup.Code = @p5 " +
                        "AND ifnull(TDV.Species,'') = ifnull(@p6, '') " +
                        "AND ifnull(TDV.LiveDead, '') = ifnull(@p7, '') " +
                        "AND ifnull(Component_CN, 0) == 0); "
                        , entry.TreeCount
                        , entry.KPI
                        , entry.UnitCode
                        , entry.StratumCode
                        , entry.SampleGroupCode
                        , entry.Species
                        , entry.LiveDead);

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

        public IEnumerable<Log> GetLogs(string tree_guid)
        {
            return Database.From<Log>()
                .Join("Tree", "USING (Tree_CN)")
                .Where("Tree.Tree_GUID = @p1")
                .Query(tree_guid).ToArray();
        }

        public Log GetLog(string log_guid)
        {
            return Database.From<Log>()
                .Join("Tree", "USING (Tree_CN)")
                .Where("Log_GUID = @p1")
                .Query(log_guid).FirstOrDefault();
        }

        public Log GetLog(string tree_guid, int logNumber)
        {
            return Database.From<Log>()
                .Join("Tree", "USING (Tree_CN)")
                .Where("Tree.Tree_GUID = @p1 AND LogNumber = @p2")
                .Query(tree_guid, logNumber).FirstOrDefault();
        }

        public void InsertLog(Log log)
        {
            var log_guid = Guid.NewGuid().ToString();
            var createdBy = log.CreatedBy ?? "Android Device";

            var tree_cn = Database.ExecuteScalar<long>("SELECT Tree_CN FROM Tree WHERE Tree_GUID = @p1", log.Tree_GUID);
            var logNumber = Database.ExecuteScalar<int>("SELECT ifnull(max(LogNumber), 0) + 1 FROM Log WHERE Tree_CN = @p1", tree_cn);

            Database.Execute("INSERT INTO Log " +
                "(Log_GUID, " +
                "Tree_CN, " +
                "LogNumber, " + //
                "Grade, " +
                "SeenDefect, " +
                "PercentRecoverable, " + //
                "Length, " +
                "ExportGrade, " +
                "SmallEndDiameter, " + //
                "LargeEndDiameter, " +
                "GrossBoardFoot, " +
                "NetBoardFoot, " + //
                "GrossCubicFoot, " +
                "NetCubicFoot, " +
                "BoardFootRemoved, " + //
                "CubicFootRemoved, " +
                "DIBClass, " +
                "BarkThickness, " + //
                "CreatedBy) " +
                "VALUES " +
                "(@p1, @p2, @p3," +
                "@p4, @p5, @p6, " +
                "@p7, @p8, @p9, " +
                "@p10, @p11, @p12, " +
                "@p13, @p14, @p15, " +
                "@p16, @p17, @p18, @p19);",
                log_guid, tree_cn, logNumber,
                log.Grade, log.SeenDefect, log.PercentRecoverable,
                log.Length, log.ExportGrade, log.SmallEndDiameter,
                log.LargeEndDiameter, log.GrossBoardFoot, log.NetBoardFoot,
                log.GrossCubicFoot, log.NetCubicFoot, log.BoardFootRemoved,
                log.CubicFootRemoved, log.DIBClass, log.BarkThickness,
                createdBy);

            log.LogNumber = logNumber;
            log.CreatedBy = createdBy;
            log.Log_GUID = log_guid;
        }

        public void UpdateLog(Log log)
        {
            Database.Execute("UPDATE Log SET " +
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
                "WHERE Log_GUID = @p18;",
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
                log.ModifiedBy,
                log.Log_GUID);
        }

        public void DeleteLog(string log_guid)
        {
            Database.Execute("DELETE FROM Log WHERE Log_GUID = @p1;", log_guid);
        }

        private static readonly LogFieldSetup[] DEFAULT_LOG_FIELDS = new LogFieldSetup[]{
            new LogFieldSetup(){
                Field = CruiseDAL.Schema.LOG.LOGNUMBER, Heading = "LogNum"},
            new LogFieldSetup(){
                Field = CruiseDAL.Schema.LOG.GRADE, Heading = "Grade"},
            new LogFieldSetup() {
                Field = CruiseDAL.Schema.LOG.SEENDEFECT, Heading = "PctSeenDef"}
        };

        public IEnumerable<LogFieldSetup> GetLogFields(string tree_guid)
        {
            var tree_stratum_CN = Database.ExecuteScalar<int>("SELECT Stratum_CN FROM Tree WHERE Tree_GUID = @p1;", tree_guid);

            var fields = Database.From<LogFieldSetup>()
                .Join("Stratum", "USING (Stratum_CN)")
                .Where("Stratum_CN = @p1")
                .OrderBy("FieldOrder")
                .Query(tree_stratum_CN).ToArray();

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
            var msg = new MessageLogDO
            {
                Message = message,
                Level = level
            };

            Database.Insert(msg);
        }
    }
}