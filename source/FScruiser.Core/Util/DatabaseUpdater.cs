using CruiseDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Util
{
    public static class DatabaseUpdater
    {
        public const string CREATE_TABLE_TALLY_LEDGER_COMMAND =
            "CREATE TABLE TallyLedger " +
            "( " +
            "TallyLedgerID TEXT PRIMARY KEY, " +
            "UnitCode TEXT NOT NULL, " +
            //"PlotNumber INTEGER, " + // no plot number, currently we are only using tally ledger for tree based
            "StratumCode TEXT NOT NULL, " +
            "SampleGroupCode TEXT NOT NULL, " +
            "Species TEXT, " +
            "LiveDead TEXT, " +
            "TreeCount INTEGER NOT NULL, " +
            "KPI INTEGER DEFAULT 0, " +
            "ThreePRandomValue INTEGER DEFAULT 0, " +
            "Tree_GUID TEXT REFERENCES Tree (Tree_GUID) ON DELETE CASCADE, " +
            "TimeStamp TEXT DEFAULT (datetime('now', 'localtime')), " +
            "Signature TEXT " +
            ");";

        public const string REBUILD_TREE_TABLE =
            //"CREATE TEMP TABLE sqlite_master_temp AS SELECT * FROM sqlite_master WHERE Name = 'Tree';\r\n" +
            "CREATE TABLE new_Tree ( " +
                "Tree_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Tree_GUID TEXT UNIQUE , " + //added unique constraint 
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
                "RowVersion INTEGER DEFAULT 0);\r\n" +
            "INSERT INTO new_Tree SELECT * FROM Tree;\r\n" +
            "DROP Table Tree;\r\n" +
            "ALTER Table new_Tree RENAME TO Tree;\r\n";

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

        

        public static void Update(DAL database)
        {
            var databaseversion = database.DatabaseVersion;

            if (databaseversion.StartsWith("2.2."))
            {

                database.Execute("PRAGMA foreign_keys=OFF;");
                database.BeginTransaction();
                try
                {
                    database.Execute(CREATE_TALLY_POPULATION);
                    database.Execute(REBUILD_TREE_TABLE);
                    database.Execute(CREATE_TABLE_TALLY_LEDGER_COMMAND);
                    database.CommitTransaction();
                    database.Execute("PRAGMA foreign_keys=ON;");

                }
                catch
                {
                    database.RollbackTransaction();
                    throw;
                }
                database.SetDatabaseVersion("2.3.0");
            }
        }
    }
}
