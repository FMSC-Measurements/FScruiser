using CruiseDAL;
using CruiseDAL.DataObjects;
using FluentAssertions;
using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Linq;
using Xunit;

namespace FScruiser.Core.Test.Services
{
    public class CuttingUnitDatastore_Test
    {
        private DAL CreateDatabase()
        {
            var database = new DAL();

            //Cutting Units
            database.Insert(new CuttingUnitDO
            {
                Code = "u1",
                Area = 0,
            });

            database.Insert(new CuttingUnitDO
            {
                Code = "u2",
                Area = 0,
            });

            //Strata
            database.Insert(new StratumDO
            {
                Code = "st1",
                Method = "something",
            });

            database.Insert(new StratumDO
            {
                Code = "st2",
                Method = "something",
            });

            //Unit - Strata
            database.Insert(new CuttingUnitStratumDO
            {
                CuttingUnit_CN = 1,
                Stratum_CN = 1,
            });

            database.Insert(new CuttingUnitStratumDO
            {
                CuttingUnit_CN = 1,
                Stratum_CN = 2,
            });

            database.Insert(new CuttingUnitStratumDO
            {
                CuttingUnit_CN = 2,
                Stratum_CN = 2,
            });

            //Sample Groups
            database.Insert(new SampleGroupDO
            {
                Stratum_CN = 1,
                Code = "sg1",
                CutLeave = "C",
                UOM = "01",
                PrimaryProduct = "01"
            });

            database.Insert(new SampleGroupDO
            {
                Stratum_CN = 2,
                Code = "sg2",
                CutLeave = "C",
                UOM = "01",
                PrimaryProduct = "01"
            });

            //TreeDefaults

            database.Insert(new TreeDefaultValueDO
            {
                PrimaryProduct = "01",
                Species = "sp1",
                LiveDead = "L"
            });

            database.Insert(new TreeDefaultValueDO
            {
                PrimaryProduct = "01",
                Species = "sp1",
                LiveDead = "D"
            });

            database.Insert(new TreeDefaultValueDO
            {
                PrimaryProduct = "01",
                Species = "sp2",
                LiveDead = "L"
            });

            //samplegroup - TreeDefaults
            database.Insert(new SampleGroupTreeDefaultValueDO
            {
                SampleGroup_CN = 1,
                TreeDefaultValue_CN = 1
            });

            database.Insert(new SampleGroupTreeDefaultValueDO
            {
                SampleGroup_CN = 1,
                TreeDefaultValue_CN = 2
            });

            database.Insert(new SampleGroupTreeDefaultValueDO
            {
                SampleGroup_CN = 1,
                TreeDefaultValue_CN = 3
            });

            database.Insert(new TallyDO { Hotkey = "A", Description = "something" });

            database.Insert(new CountTreeDO()
            {
                CuttingUnit_CN = 1,
                SampleGroup_CN = 1,
                Tally_CN = 1
            });

            database.Insert(new CountTreeDO()
            {
                CuttingUnit_CN = 1,
                SampleGroup_CN = 1,
                Tally_CN = 1,
                TreeDefaultValue_CN = 1

            });

            return database;
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

        [Fact]
        public void GetTallyPopulationsByUnitCode_Test()
        {
            var unitCode = "u1";

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var results = datastore.GetTallyPopulationsByUnitCode(unitCode);
                results.Should().HaveCount(2);


                foreach (var pop in results)
                {
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
            result.TallyDescription.Should().NotBeNullOrWhiteSpace();
            result.TallyHotKey.Should().NotBeNullOrWhiteSpace();
            result.Method.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GetTallyPopulationsByUnitCode_with_species_Test()
        {
            var unitCode = "u1";
            var species = "specialSepecies";

            using (var database = CreateDatabase())
            {
                var treeDefaultValue_CN = (long)database.Insert(new TreeDefaultValueDO()
                {
                    PrimaryProduct = "01",
                    LiveDead = "L",
                    Species = species
                });

                database.Insert(new CountTreeDO
                {
                    Tally_CN = 1,
                    CuttingUnit_CN = 1,
                    SampleGroup_CN = 1,
                    TreeDefaultValue_CN = treeDefaultValue_CN
                });

                var datastore = new CuttingUnitDatastore(database);

                var results = datastore.GetTallyPopulationsByUnitCode(unitCode);

                var expectedTallyPop = results.Where(x => x.Species == species).Single();

                VerifyTallyPopulation(expectedTallyPop, species);
            }
        }

        [Fact]
        public void CreateTree()
        {
            var unitCode = "u1";
            var stratumCode = "st1";
            var sgCode = "sg1";
            var species = "sp1";
            var liveDead = "L";
            var countMeasure = "C";
            var treeCount = 1;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var tree_GUID = datastore.CreateTree(unitCode, stratumCode, sgCode, species, liveDead, countMeasure, treeCount);

                var tree = datastore.GetTree(tree_GUID);
                tree.Should().NotBeNull();

                //tree.CuttingUnit_CN.Should().Be(1);
                tree.Tree_GUID.Should().Be(tree_GUID);
                tree.StratumCode.Should().Be(stratumCode);
                tree.SampleGroupCode.Should().Be(sgCode);
                tree.Species.Should().Be(species);
                tree.LiveDead.Should().Be(liveDead);
                tree.CountOrMeasure.Should().Be(countMeasure);
                tree.TreeCount.Should().Be(treeCount);

                database.ExecuteScalar<int>("SELECT Stratum_CN FROM Tree WHERE Tree_GUID = @p1", tree_GUID.ToString()).Should().Be(1);
                database.ExecuteScalar<int>("SELECT SampleGroup_CN FROM Tree WHERE Tree_GUID = @p1", tree_GUID.ToString()).Should().Be(1);
                database.ExecuteScalar<int>("SELECT TreeDefaultValue_CN FROM Tree WHERE Tree_GUID = @p1", tree_GUID.ToString()).Should().Be(1);
            }
        }

        [Fact]
        public void UpdateTree()
        {
            var unitCode = "u1";
            var stratumCode = "st1";
            var sgCode = "sg1";
            var species = "sp1";
            var liveDead = "L";
            var countMeasure = "C";
            var treeCount = 1;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var tree_GUID = datastore.CreateTree(unitCode, stratumCode, sgCode, species, liveDead, countMeasure, treeCount);

                var tree = datastore.GetTree(tree_GUID);
                tree.Should().NotBeNull();

                tree.DBH = 100;

                datastore.UpdateTree(tree);

                var treeAgain = datastore.GetTree(tree_GUID);

                treeAgain.DBH.Should().Be(tree.DBH);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("sp1")]
        public void InsertTallyEntry(string species)
        {
            var unitCode = "u1";
            var stratumCode = "st1";
            var sgCode = "sg1";
            var liveDead = "L";
            var countMeasure = "C";
            var treeCount = 50;

            var tree_guid = Guid.NewGuid().ToString();

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var pop = datastore.GetTallyPopulationsByUnitCode(unitCode)
                    .Where(x=> (x.Species ?? "") == (species ?? ""))
                    .FirstOrDefault();

                pop.Should().NotBeNull();

                var tallyEntry = new TallyEntry(unitCode, pop)
                {
                    Tree_GUID = tree_guid,
                    //UnitCode = unitCode,
                    //StratumCode = stratumCode,
                    //SGCode = sgCode,
                    //Species = species,
                    //LiveDead = liveDead,
                    CountOrMeasure = countMeasure,
                    TreeCount = treeCount
                };

                datastore.InsertTallyEntry(tallyEntry);

                tallyEntry.TallyEntryID.Should().NotBeEmpty();
                tallyEntry.TreeNumber.Should().NotBeNull();

                var resultTallyEntry = database.From<TallyEntry>()
                    .LeftJoin("Tree", "USING (Tree_GUID)")
                    .Where("TallyEntryID = @p1")
                    .Query(tallyEntry.TallyEntryID)
                    .FirstOrDefault();

                resultTallyEntry.Should().NotBeNull();
                resultTallyEntry.TreeNumber.Should().NotBeNull();

                var tree = datastore.GetTreesByUnitCode(unitCode).FirstOrDefault();
                tree.Should().NotBeNull();

                tree.Tree_GUID.Should().Be(tallyEntry.Tree_GUID);
                tree.StratumCode.Should().Be(stratumCode);
                tree.SampleGroupCode.Should().Be(sgCode);
                //tree.Species.Should().Be(species);
                //tree.LiveDead.Should().Be(liveDead);
                tree.CountOrMeasure.Should().Be(countMeasure);
                //tree.TreeCount.Should().Be(treeCount);

                var countTree = database.From<CountTreeDO>().LeftJoin("TreeDefaultValue AS TDV", "USING (TreeDefaultValue_CN)")
                    .Where("ifnull(TDV.Species, '') = ifnull(@p1, '')").Query(species).Single();

                countTree.TreeCount.Should().Be(treeCount);

                var tallyPopulate = datastore.GetTallyPopulationsByUnitCode(unitCode).Where(x => (x.Species ?? "") == (species ?? "")).Single();

                tallyPopulate.TreeCount.Should().Be(treeCount);

                datastore.InsertTallyEntry(tallyEntry);

                var countTree2 = database.From<CountTreeDO>().LeftJoin("TreeDefaultValue AS TDV", "USING (TreeDefaultValue_CN)")
                    .Where("ifnull(TDV.Species, '') = ifnull(@p1, '')").Query(species).Single();

                countTree2.TreeCount.Should().Be(treeCount * 2);

                var tallyPopulate2 = datastore.GetTallyPopulationsByUnitCode(unitCode).Where(x => (x.Species ?? "") == (species ?? "")).Single();

                tallyPopulate2.TreeCount.Should().Be(treeCount * 2);
            }
        }
    }
}