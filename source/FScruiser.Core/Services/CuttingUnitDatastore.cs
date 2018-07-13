using CruiseDAL;
using CruiseDAL.DataObjects;
using CruiseDAL.Schema;
using FMSC.ORM.Core.SQL.QueryBuilder;
using FScruiser.Models;
using FScruiser.Util;
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

        public const string CREATE_TABLE_TALLY_LEDGER_COMMAND =
            "CREATE TABLE TallyLedger " +
            "( " +
            "TallyLedgerID TEXT PRIMARY KEY, " +
            "UnitCode TEXT NOT NULL, " +
            "PlotNumber INTEGER, " +
            "StratumCode TEXT NOT NULL, " +
            "SampleGroupCode TEXT NOT NULL, " +
            "Species TEXT, " +
            "LiveDead TEXT, " +
            "TreeCount INTEGER NOT NULL, " +
            "KPI INTEGER DEFAULT 0, " +
            "Tree_GUID TEXT REFERENCES Tree (Tree_GUID) ON DELETE CASCADE, " +
            "TimeStamp TEXT DEFAULT (datetime('now', 'localtime')), " +
            "Signature TEXT " +
            ");";

        public const string REBUILD_TREE_TABLE =
            "ALTER TABLE Tree RENAME TO Tree_Temp; " +
            "CREATE TABLE Tree ( " +
                "Tree_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Tree_GUID TEXT UNIQUE , " +
                "TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue, " +
                "Stratum_CN INTEGER REFERENCES Stratum NOT NULL, " +
                "SampleGroup_CN INTEGER REFERENCES SampleGroup, " +
                "CuttingUnit_CN INTEGER REFERENCES CuttingUnit NOT NULL, " +
                "Plot_CN INTEGER REFERENCES Plot, " +
                "TreeNumber INTEGER NOT NULL, " +
                "Species TEXT, " +
                "CountOrMeasure TEXT, " +
                "TreeCount REAL Default 0.0, " +
                "KPI REAL Default 0.0, " +
                "STM TEXT Default 'N', " +
                "SeenDefectPrimary REAL Default 0.0, " +
                "SeenDefectSecondary REAL Default 0.0, " +
                "RecoverablePrimary REAL Default 0.0, " +
                "HiddenPrimary REAL Default 0.0, " +
                "Initials TEXT, " +
                "LiveDead TEXT, " +
                "Grade TEXT, " +
                "HeightToFirstLiveLimb REAL Default 0.0, " +
                "PoleLength REAL Default 0.0, " +
                "ClearFace TEXT, " +
                "CrownRatio REAL Default 0.0, " +
                "DBH REAL Default 0.0, " +
                "DRC REAL Default 0.0, " +
                "TotalHeight REAL Default 0.0, " +
                "MerchHeightPrimary REAL Default 0.0, " +
                "MerchHeightSecondary REAL Default 0.0, " +
                "FormClass REAL Default 0.0, " +
                "UpperStemDOB REAL Default 0.0, " +
                "UpperStemDiameter REAL Default 0.0, " +
                "UpperStemHeight REAL Default 0.0, " +
                "DBHDoubleBarkThickness REAL Default 0.0, " +
                "TopDIBPrimary REAL Default 0.0, " +
                "TopDIBSecondary REAL Default 0.0, " +
                "DefectCode TEXT, " +
                "DiameterAtDefect REAL Default 0.0, " +
                "VoidPercent REAL Default 0.0, " +
                "Slope REAL Default 0.0, " +
                "Aspect REAL Default 0.0, " +
                "Remarks TEXT, " +
                "XCoordinate DOUBLE Default 0.0, " +
                "YCoordinate DOUBLE Default 0.0, " +
                "ZCoordinate DOUBLE Default 0.0, " +
                "MetaData TEXT, " +
                "IsFallBuckScale INTEGER Default 0, " +
                "ExpansionFactor REAL Default 0.0, " +
                "TreeFactor REAL Default 0.0, " +
                "PointFactor REAL Default 0.0, " +
                "CreatedBy TEXT DEFAULT 'none', " +
                "CreatedDate DateTime DEFAULT (datetime('now', 'localtime')) , " + //date time changed
                "ModifiedBy TEXT, " +
                "ModifiedDate DateTime , " +
                "RowVersion INTEGER DEFAULT 0);" +
            "INSERT INTO Tree " +
            "(Tree_CN, Tree_GUID, TreeDefaultValue_CN, Stratum_CN, SampleGroup_CN, CuttingUnit_CN, Plot_CN, TreeNumber, Species, CountOrMeasure, TreeCount, KPI, STM, SeenDefectPrimary, SeenDefectSecondary, RecoverablePrimary, HiddenPrimary, Initials, LiveDead, Grade, HeightToFirstLiveLimb, PoleLength, ClearFace, CrownRatio, DBH, DRC, TotalHeight, MerchHeightPrimary, MerchHeightSecondary, FormClass, UpperstemDOB, UpperStemDiameter, UpperStemHeight, DBHDoubleBarkThickness, TopDIBPrimary, TopDIBSecondary, DefectCode, DiameterAtDefect, VoidPercent, Slope, Aspect, Remarks, XCoordinate, YCoordinate, ZCoordinate, MetaData) " +
            "SELECT Tree_CN, Tree_GUID, TreeDefaultValue_CN, Stratum_CN, SampleGroup_CN, CuttingUnit_CN, Plot_CN, TreeNumber, Species, CountOrMeasure, TreeCount, KPI, STM, SeenDefectPrimary, SeenDefectSecondary, RecoverablePrimary, HiddenPrimary, Initials, LiveDead, Grade, HeightToFirstLiveLimb, PoleLength, ClearFace, CrownRatio, DBH, DRC, TotalHeight, MerchHeightPrimary, MerchHeightSecondary, FormClass, UpperstemDOB, UpperStemDiameter, UpperStemHeight, DBHDoubleBarkThickness, TopDIBPrimary, TopDIBSecondary, DefectCode, DiameterAtDefect, VoidPercent, Slope, Aspect, Remarks, XCoordinate, YCoordinate, ZCoordinate, MetaData " +
            "FROM Tree_Temp;" +
            "DROP Table Tree_Temp;";

        public const string CREATE_TALLY_POPULATION =
            "CREATE VIEW TallyPopulation " +
            "( UnitCode, StratumCode, SampleGroupCode, Species, LiveDead, Description, HotKey) " +
            "AS " +
            "SELECT CuttingUnit.Code, Stratum.Code, SampleGroup.Code, TDV.Species, TDV.LiveDead, Tally.Description, Tally.HotKey " +
            "FROM CountTree " +
            "JOIN CuttingUnit USING (CuttingUnit_CN) " +
            "JOIN SampleGroup USING (SampleGroup_CN) " +
            "JOIN Stratum USING (Stratum_CN) " +
            "LEFT JOIN TreeDefaultValue AS TDV USING (TreeDefaultValue_CN) " +
            "JOIN Tally USING (Tally_CN) " +
            "GROUP BY CuttingUnit_CN, SampleGroup_CN, ifnull(TreeDefaultValue_CN, 0);";

        public const string INITIALIZE_TALLY_LEDGER_FROM_COUNTTREE =
            "INSERT INTO TallyLedger " +
            "(TallyLedgerID, UnitCode, StratumCode, SampleGroupCode, Species, LiveDead, TreeCount, KPI, Signature)" +
            "VALUES " +
            "(SELECT " +
            "(lower(hex(randomblob(4))) || '-' || lower(hex(randomblob(2))) || '-4' || substr(lower(hex(randomblob(2))),2) || '-' || substr('89ab',abs(random()) % 4 + 1, 1) || substr(lower(hex(randomblob(2))),2) || '-' || lower(hex(randomblob(6)))), " +
            "CuttingUnit.Code AS UnitCode, " +
            "Stratum.Code AS StratumCode, " +
            "SampleGroup.Code AS SampleGroupCode, " +
            "TDV.Species AS Species, " +
            "TDV.LiveDead AS LiveDead, " +
            "Sum(TreeCount) AS TreeCount, " +
            "Sum(SumKPI) AS SumKPI, " +
            "FROM CountTree " +
            "JOIN CuttingUnit USING (CuttingUnit_CN) " +
            "JOIN SampleGroup USING (SampleGroup_CN) " +
            "JOIN Stratum USING (Stratum_CN) " +
            "LEFT JOIN TreeDefaultValue AS TDV USING (TreeDefaultValue_CN) " +
            "GROUP BY CuttingUnit.Code, Stratum.Code, SampleGroup.Code, ifnull(TDV.Species, 0), ifnull(TDV.LiveDead, 0), Component_CN;";

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
            var databaseversion = Database.DatabaseVersion;

            if (databaseversion.StartsWith("2.2."))
            {
                database.BeginTransaction();
                try
                {
                    database.Execute(CREATE_TALLY_POPULATION);
                    database.Execute(REBUILD_TREE_TABLE);
                    database.Execute(CREATE_TABLE_TALLY_LEDGER_COMMAND);
                    database.SetDatabaseVersion("2.3.0");
                    database.CommitTransaction();
                }
                catch
                {
                    database.RollbackTransaction();
                    throw;
                }
            }
        }

        #region strata

        public IEnumerable<Stratum> GetStrataByUnitCode(string unitCode)
        {
            return Database.From<Stratum>()
                .Join("CuttingUnitStratum", "USING (Stratum_CN)")
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where("CuttingUnit.Code = @p1")
                .Query(unitCode).ToArray();
        }

        public IEnumerable<StratumProxy> GetStrataProxiesByUnitCode(string unitCode)
        {
            return Database.From<StratumProxy>()
                .Join("CuttingUnitStratum", "USING (Stratum_CN)")
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where("CuttingUnit.Code = @p1")
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
                .Where("CuttingUnit.Code = @p1")
                .Query(unitCode).ToArray();
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
                .Where($"CuttingUnit.Code = @p1 AND Stratum.Method NOT IN ({string.Join(",", CruiseMethods.PLOT_METHODS.Select(s => "'" + s + "'").ToArray())})")
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
               .Join("Stratum", "USING (Stratum_CN)")
               .Join("SampleGroup", "USING (SampleGroup_CN)");
        }

        public string CreateTree(string unitCode, string stratumCode, string sampleGroupCode = null, string species = null, string liveDead = "L", string countMeasure = "M", int treeCount = 1, int kpi = 0, bool stm = false)
        {
            var tree_guid = Guid.NewGuid().ToString();
            CreateTree(tree_guid, unitCode, stratumCode, sampleGroupCode, species, liveDead, countMeasure, treeCount, kpi, stm);
            return tree_guid;
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

        #region Tally Entry

        public IEnumerable<TallyEntry> GetTallyEntriesByUnitCode(string unitCode)
        {
            return Database.From<TallyEntry>()
                .LeftJoin("Tree", "USING (Tree_GUID)")
                .Where("UnitCode = @p1")
                .OrderBy("TimeStamp DESC")
                .Limit(NUMBER_OF_TALLY_ENTRIES_PERPAGE, 0 * NUMBER_OF_TALLY_ENTRIES_PERPAGE)
                .Query(unitCode);
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