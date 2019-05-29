using Bogus;
using CruiseDAL;
using FluentAssertions;
using FluentAssertions.Equivalency;
using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.Core.Test.Services
{
    public class CuttingUnitDatastore_Test : Datastore_TestBase
    {
        public CuttingUnitDatastore_Test(ITestOutputHelper output) : base(output)
        {
        }

        
        [Theory]
        [InlineData("u1", "st1", "st2")]
        [InlineData("u2", "st2")]
        public void GetStrataByUnitCode_Test(string unitCode, params string[] expectedStrataCodes)
        {
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var results = datastore.GetStrataByUnitCode(unitCode);

                var strata_codes = results.Select(x => x.Code);
                strata_codes.Should().Contain(expectedStrataCodes);
                strata_codes.Should().HaveSameCount(expectedStrataCodes);
            }
        }

        [Theory]
        [InlineData("u1", "st1", "sg1", "sp1", "L", true)]
        [InlineData("u1", "st1", "sg1", null, null, false)]
        public void GetTallyPopulation(string unitCode, string stratum, string sampleGroup, string species, string liveDead, bool tallyBySubpop)
        {
            var tallyDescription = $"{stratum} {sampleGroup} {species} {liveDead}";
            var hotKey = "A";
            var method = CruiseDAL.Schema.CruiseMethods.FIX;

            using (var database = new DAL())
            {
                database.Execute($"INSERT INTO CuttingUnit (Code) VALUES ('{unitCode}');");

                database.Execute($"INSERT INTO Stratum (Code, Method) VALUES ('{stratum}', '{method}');");

                database.Execute($"INSERT INTO CuttingUnit_Stratum (CuttingUnitCode, StratumCode) VALUES " +
                    $"('{unitCode}','{stratum}');");

                database.Execute($"INSERT INTO SampleGroup_V3 (StratumCode, SampleGroupCode, SamplingFrequency, TallyBySubPop ) VALUES " +
                    $"('{stratum}', '{sampleGroup}', 101, {tallyBySubpop});");

                database.Execute($"INSERT INTO Species (Species) VALUES ('{((species == null || species == "") ? "dummy" : species)}');");

                database.Execute(
                "INSERT INTO SubPopulation (" +
                "StratumCode, " +
                "SampleGroupCode, " +
                "Species, " +
                "LiveDead)" +
                "VALUES " +
                $"('{stratum}', '{sampleGroup}', " +
                $"'{((species == null || species == "") ? "dummy" : species)}', " +
                $"'{((liveDead == null || liveDead == "") ? "L" : liveDead)}');");

                database.Execute("INSERT INTO TallyDescription (StratumCode, SampleGroupCode, Species, LiveDead, Description) VALUES " +
                    "(@p1, @p2, @p3, @p4, @p5);", new object[] { stratum, sampleGroup, species, liveDead, tallyDescription });

                database.Execute("INSERT INTO TallyHotKey (StratumCode, SampleGroupCode, Species, LiveDead, HotKey) VALUES " +
                    "(@p1, @p2, @p3, @p4, @p5);", new object[] { stratum, sampleGroup, species, liveDead, hotKey });

                var datastore = new CuttingUnitDatastore(database);

                var pop = datastore.GetTallyPopulation(unitCode, stratum, sampleGroup, species, liveDead);
                pop.Should().NotBeNull();

                VerifyTallyPopulation(pop);

                pop.TallyDescription.Should().NotBeNullOrWhiteSpace();
                pop.TallyHotKey.Should().NotBeNullOrWhiteSpace();
                pop.Method.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Fact]
        public void GetTallyPopulationsByUnitCode_with_tallybysubpop_Test()
        {
            string unitCode = "u1";
            string stratum = "st1";
            string sampleGroup = "sg1";
            string[] species = new string[] { "sp1", "sp2" };
            string liveDead = "L";

            var tallyBySubpop = true;
            //var method = CruiseDAL.Schema.CruiseMethods.FIX;

            using (var database = new DAL())
            {
                database.Execute($"INSERT INTO CuttingUnit (Code) VALUES ('{unitCode}');");

                database.Execute($"INSERT INTO Stratum (Code) VALUES ('{stratum}');");

                database.Execute($"INSERT INTO CuttingUnit_Stratum (CuttingUnitCode, StratumCode) VALUES " +
                    $"('{unitCode}','{stratum}');");

                database.Execute($"INSERT INTO SampleGroup_V3 (StratumCode, SampleGroupCode, SamplingFrequency, TallyBySubPop ) VALUES " +
                    $"('{stratum}', '{sampleGroup}', 101, {tallyBySubpop});");

                foreach (var sp in species)
                {
                    database.Execute($"INSERT INTO Species (Species) VALUES ('{sp}');");

                    database.Execute(
                        "INSERT INTO SubPopulation (" +
                        "StratumCode, " +
                        "SampleGroupCode, " +
                        "Species, " +
                        "LiveDead)" +
                        "VALUES " +
                        $"('{stratum}', '{sampleGroup}', '{sp}', '{liveDead}');");
                }

                var datastore = new CuttingUnitDatastore(database);

                var results = datastore.GetTallyPopulationsByUnitCode(unitCode);
                results.Should().HaveCount(species.Count());

                foreach (var pop in results)
                {
                    VerifyTallyPopulation(pop);
                }
            }
        }

        [Fact]
        public void GetTallyPopulationsByUnitCode_with_TallyBySG_Test()
        {
            string unitCode = "u1";
            string stratum = "st1";
            string sampleGroup = "sg1";
            string[] species = new string[] { "sp1", "sp2" };
            string liveDead = "L";

            var tallyBySubpop = false;
            //var method = CruiseDAL.Schema.CruiseMethods.FIX;

            using (var database = new DAL())
            {
                database.Execute($"INSERT INTO CuttingUnit (Code) VALUES ('{unitCode}');");

                database.Execute($"INSERT INTO Stratum (Code) VALUES ('{stratum}');");

                database.Execute($"INSERT INTO CuttingUnit_Stratum (CuttingUnitCode, StratumCode) VALUES " +
                    $"('{unitCode}','{stratum}');");

                database.Execute($"INSERT INTO SampleGroup_V3 (StratumCode, SampleGroupCode, SamplingFrequency, TallyBySubPop ) VALUES " +
                    $"('{stratum}', '{sampleGroup}', 101, {tallyBySubpop});");

                foreach (var sp in species)
                {
                    database.Execute($"INSERT INTO Species (Species) VALUES ('{sp}');");

                    database.Execute(
                        "INSERT INTO SubPopulation (" +
                        "StratumCode, " +
                        "SampleGroupCode, " +
                        "Species, " +
                        "LiveDead)" +
                        "VALUES " +
                        $"('{stratum}', '{sampleGroup}', '{sp}', '{liveDead}');");
                }

                var datastore = new CuttingUnitDatastore(database);

                var results = datastore.GetTallyPopulationsByUnitCode(unitCode);
                results.Should().HaveCount(1);

                foreach (var pop in results)
                {
                    VerifyTallyPopulation(pop);
                }
            }
        }

        [Fact]
        public void GetTallyPopulationsByUnitCode_Test_with_clicker_tally()
        {
            string unitCode = "u1";
            string stratum = "st1";
            string sampleGroup = "sg1";
            string[] species = new string[] { "sp1", "sp2" };
            string liveDead = "L";

            var tallyBySubpop = false;

            using (var database = new DAL())
            {
                var datastore = new CuttingUnitDatastore(database);

                database.Execute($"INSERT INTO CuttingUnit (Code) VALUES ('{unitCode}');");

                database.Execute($"INSERT INTO Stratum (Code) VALUES ('{stratum}');");

                database.Execute($"INSERT INTO CuttingUnit_Stratum (CuttingUnitCode, StratumCode) VALUES " +
                    $"('{unitCode}','{stratum}');");

                database.Execute($"INSERT INTO SampleGroup_V3 (StratumCode, SampleGroupCode, SamplingFrequency, TallyBySubPop ) VALUES " +
                    $"('{stratum}', '{sampleGroup}', 101, {tallyBySubpop});");

                foreach (var sp in species)
                {
                    database.Execute($"INSERT INTO Species (Species) VALUES ('{sp}');");

                    database.Execute(
                        "INSERT INTO SubPopulation (" +
                        "StratumCode, " +
                        "SampleGroupCode, " +
                        "Species, " +
                        "LiveDead)" +
                        "VALUES " +
                        $"('{stratum}', '{sampleGroup}', '{sp}', '{liveDead}');");
                }

                database.Execute($"INSERT INTO SamplerState (StratumCode, SampleGroupCode, SampleSelectorType) " +
                    $"SELECT StratumCode, SampleGroupCode, '{CruiseDAL.Schema.CruiseMethods.CLICKER_SAMPLER_TYPE}' AS SampleSelectorType FROM SampleGroup_V3;");

                var results = datastore.GetTallyPopulationsByUnitCode(unitCode);
                //results.Should().HaveCount(2);

                foreach (var pop in results)
                {
                    pop.IsClickerTally.Should().BeTrue();

                    VerifyTallyPopulation(pop);
                }
            }
        }

        private static void VerifyTallyPopulation(Models.TallyPopulation result, string species = null)
        {
            if (species != null)
            {
                result.Species.Should().Be(species);
            }

            result.SampleGroupCode.Should().NotBeNullOrEmpty();
            result.StratumCode.Should().NotBeNullOrEmpty();

            result.Frequency.Should().BeGreaterThan(0);
        }

       

        #region tally entry

        [Fact]
        public void GetTallyEntriesByUnitCode()
        {
            var unit = "u1";
            var stratum = "st1";
            var sampleGroup = "sg1";
            var species = "sp1";
            var liveDead = "L";

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var pop = datastore.GetTallyPopulation(unit, stratum, sampleGroup, species, liveDead);

                datastore.InsertTallyAction(new TallyAction(unit, pop));

                var tallyEntries = datastore.GetTallyEntriesByUnitCode(unit);

                tallyEntries.Should().HaveCount(1);

                datastore.InsertTallyLedger(new TallyLedger(unit, pop));

                tallyEntries = datastore.GetTallyEntriesByUnitCode(unit);

                tallyEntries.Should().HaveCount(2);
            }
        }

        [Theory]
        [InlineData("st2", "sg2", null, null, false)]
        [InlineData("st2", "sg2", "", "", false)]// not tally by subpop
        [InlineData("st1", "sg1", "sp1", "L", false)]// tally by subpop
        public void InsertTallyEntry(string stratumCode, string sgCode, string species, string liveDead, bool isSample)
        {
            var unitCode = "u1";
            var treeCount = 50;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var pop = datastore.GetTallyPopulation(unitCode, stratumCode, sgCode, species, liveDead);

                pop.Should().NotBeNull();

                var tallyAction = new TallyAction(unitCode, pop)
                {
                    IsSample = isSample,
                    TreeCount = treeCount,
                };

                var entry = datastore.InsertTallyAction(tallyAction);

                entry.TallyLedgerID.Should().NotBeEmpty();

                ValidateTallyEntry(entry, isSample);

                var entryAgain = datastore.GetTallyEntry(entry.TallyLedgerID);

                ValidateTallyEntry(entryAgain, isSample);

                //var tree = database.From<Tree>().Where("TreeID = @p1").Query(entry.TreeID).FirstOrDefault();

                if (isSample)
                {
                    var tree = datastore.GetTree(entry.TreeID);

                    tree.Should().NotBeNull();

                    tree.TreeID.Should().Be(entry.TreeID);
                    tree.StratumCode.Should().Be(stratumCode);
                    tree.SampleGroupCode.Should().Be(sgCode);
                    tree.Species.Should().Be(species ?? "");
                    tree.LiveDead.Should().Be(liveDead ?? "");
                    tree.CountOrMeasure.Should().Be(isSample ? "M" : "C");
                }

                var tallyPopulate = datastore.GetTallyPopulationsByUnitCode(unitCode).Where(x => (x.Species ?? "") == (species ?? "")).Single();

                tallyPopulate.TreeCount.Should().Be(treeCount);
            }
        }

        private void ValidateTallyEntry(TallyEntry entry, bool isSample, string entryType = "tally")
        {
            entry.Should().NotBeNull();
            entry.TallyLedgerID.Should().NotBeNull();

            if (isSample)
            {
                entry.TreeNumber.Should().NotBeNull();
            }
            else
            {
                entry.TreeNumber.Should().BeNull();
            }

            entry.EntryType.Should().BeEquivalentTo(entryType);
        }

        [Fact]
        public void InsertTallyLedger()
        {
            string unitCode = "u1";
            string stratum = "st1";
            string sampleGroup = "sg1";
            string species = "sp1";
            string liveDead = "L";

            int treeCountDiff = 1;
            int kpi = 1;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var pop = datastore.GetTallyPopulation(unitCode, stratum, sampleGroup, species, liveDead);
                pop.Should().NotBeNull();
                VerifyTallyPopulation(pop);
                pop.TreeCount.Should().Be(0);
                pop.SumKPI.Should().Be(0);

                var tallyLedger = new TallyLedger(unitCode, pop);
                tallyLedger.TreeCount = treeCountDiff;
                tallyLedger.KPI = 1;

                datastore.InsertTallyLedger(tallyLedger);

                database.ExecuteScalar<int>("SELECT count(*) FROM TallyLedger;").Should().Be(1);
                database.ExecuteScalar<int>("SELECT sum(TreeCount) FROM TallyLedger;").Should().Be(treeCountDiff);

                var popAfter = datastore.GetTallyPopulation(unitCode, stratum, sampleGroup, species, liveDead);
                popAfter.TreeCount.Should().Be(treeCountDiff);
                popAfter.SumKPI.Should().Be(kpi);
            }
        }

        [Fact]
        public void DeleteTallyEntry()
        {
            var unitCode = "u1";
            var stratumCode = "st1";
            var sgCode = "sg1";
            var species = "sp1";
            var liveDead = "L";
            var tree_guid = Guid.NewGuid().ToString();
            var tallyLedgerID = Guid.NewGuid().ToString();
            var treeCount = 1;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var tallyPop = datastore.GetTallyPopulation(unitCode, stratumCode, sgCode, species, liveDead);

                tallyPop.Should().NotBeNull("tallyPop");

                var tallyEntry = new TallyAction(unitCode, tallyPop)
                {
                    TreeCount = treeCount
                };

                var entry = datastore.InsertTallyAction(tallyEntry);

                datastore.DeleteTallyEntry(entry.TallyLedgerID);

                var tallyPopAgain = datastore.GetTallyPopulationsByUnitCode(unitCode).Where(x => x.StratumCode == stratumCode
                && x.SampleGroupCode == sgCode
                && x.Species == species).Single();

                tallyPopAgain.TreeCount.Should().Be(0, "TreeCount");
                tallyPopAgain.SumKPI.Should().Be(0, "SumKPI");

                database.ExecuteScalar<int>("SELECT count(*) FROM Tree_V3 WHERE TreeID = @p1", entry.TreeID).Should().Be(0, "tree should be deleted");
            }
        }

        #endregion tally entry


    }
}