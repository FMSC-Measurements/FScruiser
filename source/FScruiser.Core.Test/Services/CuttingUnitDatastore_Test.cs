using Bogus;
using CruiseDAL;
using CruiseDAL.DataObjects;
using FluentAssertions;
using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.Core.Test.Services
{
    public class CuttingUnitDatastore_Test : TestBase
    {
        public CuttingUnitDatastore_Test(ITestOutputHelper output) : base(output)
        {
        }

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

        #region plot

        [Fact]
        public void GetNextPlotNumber()
        {
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                datastore.GetNextPlotNumber("u1").Should().Be(1, "unit with no plots, should return 1 for first plot number");

                database.Insert(new PlotDO
                {
                    CuttingUnit_CN = 1,
                    Stratum_CN = 1,
                    CreatedBy = "someone",
                    PlotNumber = 1
                });

                datastore.GetNextPlotNumber("u1").Should().Be(2, "unit with a plot, should return max plot number + 1");
            }
        }

        [Fact]
        public void IsPlotNumberAvalible()
        {
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                datastore.IsPlotNumberAvalible("u1", 1).Should().BeTrue("no plots in unit yet");

                database.Insert(new PlotDO
                {
                    CuttingUnit_CN = 1,
                    Stratum_CN = 1,
                    CreatedBy = "someone",
                    PlotNumber = 1
                });

                datastore.IsPlotNumberAvalible("u1", 1).Should().BeFalse("we just inserted a plot");
            }
        }

        [Fact]
        public void GetPlotsByUnitCode()
        {
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                datastore.GetPlotsByUnitCode("u1").Should().BeEmpty("we havn't added any plots yet");

                database.Insert(new PlotDO
                {
                    CuttingUnit_CN = 1,
                    Stratum_CN = 1,
                    CreatedBy = "someone",
                    PlotNumber = 1
                });

                datastore.GetPlotsByUnitCode("u1").Should().ContainSingle();
            }
        }

        [Theory]
        [InlineData("FIX")]
        [InlineData("PCM")]
        [InlineData("FCM")]
        public void GetPlotStrataProxies(string method)
        {
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                datastore.GetPlotStrataProxies("u1").Should().HaveCount(0);

                database.Insert(new StratumDO() { Stratum_CN = 3, Code = "03", Method = CruiseDAL.Schema.CruiseMethods.FIX });
                database.Insert(new CuttingUnitStratumDO() { Stratum_CN = 3, CuttingUnit_CN = 1 });

                var result = datastore.GetPlotStrataProxies("u1").ToArray();

                result.Should().HaveCount(1);
            }
        }

        [Fact]
        public void InsertStratumPlot()
        {
            var plotNumber = 1;
            var stratumCode = "st1";
            var unitCode = "u1";
            var isEmpty = "True";
            var kpi = 101;
            var remarks = "something";

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var stratumPlot = new StratumPlot()
                {
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    IsEmpty = isEmpty,
                    KPI = kpi,
                    Remarks = remarks
                };

                datastore.InsertStratumPlot(unitCode, stratumPlot);

                var plot_guid = stratumPlot.Plot_GUID;
                plot_guid.Should().NotBeNullOrEmpty();

                datastore.IsPlotNumberAvalible(unitCode, plotNumber).Should().BeFalse("we just took that plot number");

                var ourStratumPlot = datastore.GetStratumPlot(unitCode, stratumCode, plotNumber);
                ourStratumPlot.Should().NotBeNull();
                ourStratumPlot.Plot_GUID.Should().Be(plot_guid);
                ourStratumPlot.PlotNumber.Should().Be(plotNumber);
                ourStratumPlot.Remarks.Should().Be(remarks);
                ourStratumPlot.KPI.Should().Be(kpi);
                ourStratumPlot.IsEmpty.Should().Be(isEmpty);
                ourStratumPlot.StratumCode.Should().Be(stratumCode);
            }
        }

        [Fact]
        public void UpdateStratumPlot()
        {
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var stratumPlot = new StratumPlot()
                {
                    PlotNumber = 1,
                    StratumCode = "st1"
                };

                datastore.InsertStratumPlot("u1", stratumPlot);

                stratumPlot.Remarks = "hey";
                datastore.UpdateStratumPlot(stratumPlot);

                var ourStratumPlot = datastore.GetStratumPlot("u1", "st1", 1);

                ourStratumPlot.Remarks.Should().Be("hey");
            }
        }

        [Fact]
        public void DeleteStratumPlot()
        {
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var stratumPlot = new StratumPlot()
                {
                    PlotNumber = 1,
                    StratumCode = "st1"
                };

                datastore.InsertStratumPlot("u1", stratumPlot);

                var echo = datastore.GetStratumPlot("u1", "st1", 1);
                echo.Should().NotBeNull("where's my echo");

                datastore.DeleteStratumPlot(echo.Plot_GUID);
            }
        }

        [Fact]
        public void GetPlotTallyPopulationsByUnitCode_PNT_FIX_no_tally_setup_noPlot()
        {
            var method = CruiseDAL.Schema.CruiseMethods.PNT;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                database.Insert(new CuttingUnitDO
                {
                    Code = "u3",
                    Area = 0
                });

                database.Insert(new StratumDO
                {
                    Code = "st3",
                    Method = method
                });

                database.Insert(new StratumDO
                {
                    Code = "st4",
                    Method = CruiseDAL.Schema.CruiseMethods.PCM
                });

                database.Insert(new CuttingUnitStratumDO
                {
                    CuttingUnit_CN = 3,
                    Stratum_CN = 3
                });

                database.Insert(new CuttingUnitStratumDO
                {
                    CuttingUnit_CN = 3,
                    Stratum_CN = 4
                });

                database.Insert(new SampleGroupDO
                {
                    Stratum_CN = 3,
                    Code = "sg3",
                    CutLeave = "C",
                    UOM = "01",
                    PrimaryProduct = "01"
                });

                database.Insert(new SampleGroupTreeDefaultValueDO
                {
                    SampleGroup_CN = 3,
                    TreeDefaultValue_CN = 1
                });

                {
                    //we are going to check that the tally population returned is vallid for a
                    //tally population with no count tree record associated
                    //it should return one tally pop per sample group in the unit, that is associated with a FIX or PNT stratum
                    var unit3tallyPops = datastore.GetPlotTallyPopulationsByUnitCode("u3", 1);
                    unit3tallyPops.Should().HaveCount(1);
                    var unit3tallyPop = unit3tallyPops.Single();
                    unit3tallyPop.CountTree_CN.Should().BeNull("countTree_CN");
                    unit3tallyPop.Species.Should().BeNull("Species");
                    unit3tallyPop.LiveDead.Should().BeNull("liveDead");
                    unit3tallyPop.StratumCode.Should().Be("st3");
                    unit3tallyPop.SampleGroupCode.Should().Be("sg3");
                    unit3tallyPop.InCruise.Should().BeFalse();
                }
            }
        }

        [Fact]
        public void GetPlotTallyPopulationsByUnitCode_noPlot()
        {
            var method = CruiseDAL.Schema.CruiseMethods.PCM;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                //set up two cutting units one (u3) will be given a count tree record
                //the other (u4) will not,
                //so that we can test situations where count tree records are missing for a unit
                database.Insert(new CuttingUnitDO
                {
                    Code = "u3",
                    Area = 0,
                });

                database.Insert(new CuttingUnitDO
                {
                    Code = "u4",
                    Area = 0,
                });

                database.Insert(new StratumDO
                {
                    Code = "st3",
                    Method = method,
                });

                database.Insert(new CuttingUnitStratumDO
                {
                    CuttingUnit_CN = 3,
                    Stratum_CN = 3,
                });

                database.Insert(new CuttingUnitStratumDO
                {
                    CuttingUnit_CN = 4,
                    Stratum_CN = 3,
                });

                database.Insert(new SampleGroupDO
                {
                    Stratum_CN = 3,
                    Code = "sg3",
                    CutLeave = "C",
                    UOM = "01",
                    PrimaryProduct = "01"
                });

                database.Insert(new SampleGroupTreeDefaultValueDO
                {
                    SampleGroup_CN = 3,
                    TreeDefaultValue_CN = 1
                });

                database.Insert(new CountTreeDO()
                {
                    CuttingUnit_CN = 3,
                    SampleGroup_CN = 3,
                    Tally_CN = 1,
                    TreeDefaultValue_CN = 1
                });

                //tally population should
                var unit3tallyPops = datastore.GetPlotTallyPopulationsByUnitCode("u3", 1);
                unit3tallyPops.Should().HaveCount(1);
                var unit3tallyPop = unit3tallyPops.Single();
                unit3tallyPop.CountTree_CN.Should().NotBeNull();
                unit3tallyPop.Species.Should().Be("sp1");
                unit3tallyPop.StratumCode.Should().Be("st3");
                unit3tallyPop.SampleGroupCode.Should().Be("sg3");
                unit3tallyPop.InCruise.Should().BeFalse();

                var unit4tallyPops = datastore.GetPlotTallyPopulationsByUnitCode("u4", 1);
                unit4tallyPops.Should().HaveCount(1);
                var unit4tallyPop = unit4tallyPops.Single();
                unit4tallyPop.CountTree_CN.Should().BeNull();
                unit4tallyPop.InCruise.Should().BeFalse();

                database.Insert(new PlotDO
                {
                    CuttingUnit_CN = 3,
                    Stratum_CN = 3,
                    PlotNumber = 1
                });

                //tally population should
                var unit3tallyPops = datastore.GetPlotTallyPopulationsByUnitCode("u3", 1);
                unit3tallyPops.Should().HaveCount(1);
                var unit3tallyPop = unit3tallyPops.Single();
                unit3tallyPop.CountTree_CN.Should().NotBeNull();
                unit3tallyPop.Species.Should().Be("sp1");
                unit3tallyPop.StratumCode.Should().Be("st3");
                unit3tallyPop.SampleGroupCode.Should().Be("sg3");
                unit3tallyPop.InCruise.Should().BeTrue();

                var unit4tallyPops = datastore.GetPlotTallyPopulationsByUnitCode("u4", 1);
                unit4tallyPops.Should().HaveCount(1);
                var unit4tallyPop = unit4tallyPops.Single();
                unit4tallyPop.CountTree_CN.Should().BeNull();
                unit4tallyPop.InCruise.Should().BeFalse();
            }
        }

        #endregion plot

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

        #region tree

        [Theory]
        [InlineData("u1", "st1", null, "", "", null)]
        [InlineData("u1", "st1", "sg1", "sp1", "L", "C")]
        public void GetTreeStub(string unitCode, string stratumCode, string sgCode, string species, string liveDead, string countMeasure)
        {
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var tree_GUID = datastore.CreateTree(unitCode, stratumCode, sgCode, species, liveDead, countMeasure);

                var tree = datastore.GetTreeStub(tree_GUID);
                tree.Should().NotBeNull();

                tree.Tree_GUID.Should().Be(tree_GUID);
                tree.StratumCode.Should().Be(stratumCode);
                tree.SampleGroupCode.Should().Be(sgCode);
                tree.Species.Should().Be(species);
                tree.CountOrMeasure.Should().Be(countMeasure);
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

        [Fact]
        public void DeleteTree()
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

                datastore.DeleteTree(tree_GUID);

                tree = datastore.GetTree(tree_GUID);
                tree.Should().BeNull();
            }
        }

        #endregion tree

        #region tally entry

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("sp1", "L")]
        public void InsertTallyEntry(string species, string liveDead)
        {
            var unitCode = "u1";
            var stratumCode = "st1";
            var sgCode = "sg1";
            var countMeasure = "C";
            var treeCount = 50;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var pop = datastore.GetTallyPopulationsByUnitCode(unitCode)
                    .Where(x => (x.Species ?? "") == (species ?? ""))
                    .FirstOrDefault();

                pop.Should().NotBeNull();

                var tallyEntry = new TallyEntry(unitCode, pop)
                {
                    Tree_GUID = Guid.NewGuid().ToString(),
                    //UnitCode = unitCode,
                    //StratumCode = stratumCode,
                    //SGCode = sgCode,
                    //Species = species,
                    //LiveDead = liveDead,
                    CountOrMeasure = countMeasure,
                    TreeCount = treeCount
                };

                datastore.InsertTallyEntry(tallyEntry);

                tallyEntry.TallyLedgerID.Should().NotBeEmpty();
                tallyEntry.TreeNumber.Should().NotBeNull();

                var resultTallyEntry = database.From<TallyEntry>()
                    .LeftJoin("Tree", "USING (Tree_GUID)")
                    .Where("TallyLedgerID = @p1")
                    .Query(tallyEntry.TallyLedgerID)
                    .FirstOrDefault();

                resultTallyEntry.Should().NotBeNull();
                resultTallyEntry.TreeNumber.Should().NotBeNull();

                var tree = database.From<TreeDO>().Where("Tree_GUID = @p1").Query(tallyEntry.Tree_GUID).FirstOrDefault();
                var stratum_CN = database.ExecuteScalar<long?>("SELECT Stratum_CN FROM Stratum WHERE Code = @p1;", stratumCode);
                var sg_CN = database.ExecuteScalar<long?>("SELECT SampleGroup_CN FROM SampleGroup WHERE Code = @p1;", sgCode);

                tree.Should().NotBeNull();

                tree.Tree_GUID.Should().Be(tallyEntry.Tree_GUID);
                tree.Stratum_CN.Should().Be(stratum_CN);
                tree.SampleGroup_CN.Should().Be(sg_CN);
                tree.Species.Should().Be(species ?? "");
                tree.LiveDead.Should().Be(liveDead ?? "");
                tree.CountOrMeasure.Should().Be(countMeasure);
                tree.TreeCount.Should().Be(treeCount);

                if (string.IsNullOrWhiteSpace(species))
                {
                    tree.TreeDefaultValue_CN.Should().BeNull();
                }
                else
                {
                    tree.TreeDefaultValue_CN.Should().NotBeNull();
                }

                var countTree = database.From<CountTreeDO>().LeftJoin("TreeDefaultValue AS TDV", "USING (TreeDefaultValue_CN)")
                    .Where("ifnull(TDV.Species, '') = ifnull(@p1, '')").Query(species).Single();

                countTree.TreeCount.Should().Be(treeCount);

                var tallyPopulate = datastore.GetTallyPopulationsByUnitCode(unitCode).Where(x => (x.Species ?? "") == (species ?? "")).Single();

                tallyPopulate.TreeCount.Should().Be(treeCount);
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

                var tallyEntry = new TallyEntry()
                {
                    TallyLedgerID = tallyLedgerID,
                    UnitCode = unitCode,
                    SampleGroupCode = sgCode,
                    StratumCode = stratumCode,
                    Species = species,
                    LiveDead = liveDead,
                    Tree_GUID = tree_guid,
                    TreeCount = treeCount
                };

                datastore.InsertTallyEntry(tallyEntry);

                datastore.DeleteTally(tallyEntry);

                var tallyPop = datastore.GetTallyPopulationsByUnitCode(unitCode).Where(x => x.StratumCode == stratumCode
                && x.SampleGroupCode == sgCode
                && x.Species == species).Single();

                tallyPop.Should().NotBeNull("tallyPop");

                tallyPop.TreeCount.Should().Be(0, "TreeCount");
                tallyPop.SumKPI.Should().Be(0, "SumKPI");

                database.ExecuteScalar<int>("SELECT count(*) FROM Tree WHERE Tree_GUID = @p1", tree_guid).Should().Be(0, "tree should be deleted");
            }
        }

        #endregion tally entry

        #region logs

        [Fact]
        public void InsertLog_test()
        {
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var tree_guid = datastore.CreateTree("u1", "st1");

                var log = new Log() { Tree_GUID = tree_guid };

                var randomizer = new Randomizer(8675309);

                log.BarkThickness = randomizer.Double();
                log.BoardFootRemoved = randomizer.Double();
                log.CubicFootRemoved = randomizer.Double();
                log.DIBClass = randomizer.Double();
                log.ExportGrade = randomizer.String();
                log.Grade = randomizer.String();
                log.GrossBoardFoot = randomizer.Double();
                log.GrossCubicFoot = randomizer.Double();
                log.LargeEndDiameter = randomizer.Double();
                log.Length = randomizer.Int();
                log.LogNumber = randomizer.Int();
                log.ModifiedBy = randomizer.String();
                log.NetBoardFoot = randomizer.Double();
                log.NetCubicFoot = randomizer.Double();
                log.PercentRecoverable = randomizer.Double();
                log.SeenDefect = randomizer.Double();
                log.SmallEndDiameter = randomizer.Double();

                datastore.InsertLog(log);

                log.Log_GUID.Should().NotBeNullOrWhiteSpace();
                Guid.TryParse(log.Log_GUID, out Guid log_guid).Should().BeTrue();
                log_guid.Should().NotBe(Guid.Empty);
            }
        }

        [Fact]
        public void UpdateLog_test()
        {
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var tree_guid = datastore.CreateTree("u1", "st1");


                var log = new Log() { Tree_GUID = tree_guid, LogNumber = 1 };
                datastore.InsertLog(log);

                var randomizer = new Randomizer(8675309);

                log.BarkThickness = randomizer.Double();
                log.BoardFootRemoved = randomizer.Double();
                log.CubicFootRemoved = randomizer.Double();
                log.DIBClass = randomizer.Double();
                log.ExportGrade = randomizer.String();
                log.Grade = randomizer.String();
                log.GrossBoardFoot = randomizer.Double();
                log.GrossCubicFoot = randomizer.Double();
                log.LargeEndDiameter = randomizer.Double();
                log.Length = randomizer.Int();
                log.LogNumber = randomizer.Int();
                log.ModifiedBy = randomizer.String(10);
                log.NetBoardFoot = randomizer.Double();
                log.NetCubicFoot = randomizer.Double();
                log.PercentRecoverable = randomizer.Double();
                log.SeenDefect = randomizer.Double();
                log.SmallEndDiameter = randomizer.Double();

                datastore.UpdateLog(log);

                var logAgain = datastore.GetLog(log.Log_GUID);

                logAgain.Should().BeEquivalentTo(log);
            }
        }

        [Fact]
        public void DeleteLog_test()
        {
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var tree_guid = datastore.CreateTree("u1", "st1");


                var log = new Log() { Tree_GUID = tree_guid, LogNumber = 1 };
                datastore.InsertLog(log);

                datastore.DeleteLog(log.Log_GUID); 

                var logAgain = datastore.GetLog(log.Log_GUID);
                logAgain.Should().BeNull();
            }
        }

        #endregion
    }
}