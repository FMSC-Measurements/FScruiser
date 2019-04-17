using CruiseDAL;
using FluentAssertions;
using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FScruiser.Util;

namespace FScruiser.Core.Test.Services
{
    public class CuttingUnitDatastore_Test_Plot_Methods
    {
        void InitializeDatabase(DAL database, string[] units, string[][] strata,
            string[][] unit_strata, dynamic[] sampleGroups,
            string[] species, string[][] tdvs, string[][] subPops)
        {
            //Cutting Units
            foreach (var unit in units.OrEmpty())
            {
                database.Execute(
                    "INSERT INTO CuttingUnit (" +
                    "Code" +
                    ") VALUES " +
                    $"('{unit}');");
            }

            //Strata
            foreach (var st in strata.OrEmpty())
            {
                database.Execute($"INSERT INTO Stratum (Code, Method) VALUES ('{st[0]}', '{st[1]}');");
            }

            //Unit - Strata
            foreach (var cust in unit_strata.OrEmpty())
            {
                database.Execute(
                    "INSERT INTO CuttingUnit_Stratum " +
                    "(CuttingUnitCode, StratumCode) " +
                    "VALUES " +
                    $"('{cust[0]}','{cust[1]}');");
            }

            //Sample Groups
            foreach (var sg in sampleGroups.OrEmpty())
            {
                database.Execute(
                    "INSERT INTO SampleGroup_V3 (" +
                    "StratumCode, " +
                    "SampleGroupCode," +
                    "SamplingFrequency, " +
                    "TallyBySubPop " +
                    ") VALUES " +
                    $"('{sg.StCode}', '{sg.SgCode}', {sg.Freq}, {sg.TallyBySp}); ");
            }


            //TreeDefaults

            foreach (var sp in species.OrEmpty())
            {
                database.Execute($"INSERT INTO Species (Species) VALUES ('{sp}');");
            }

            foreach (var tdv in tdvs.OrEmpty())
            {
                database.Execute(
                    "INSERT INTO TreeDefaultValue (" +
                    "Species, " +
                    "LiveDead, " +
                    "PrimaryProduct" +
                    ") VALUES " +
                    $"('{tdv[0]}', '{tdv[1]}', '{tdv[2]}');");
            }

            foreach (var sub in subPops.OrEmpty())
            {
                database.Execute(
                    "INSERT INTO SubPopulation (" +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead)" +
                    "VALUES " +
                    $"('{sub[0]}', '{sub[1]}', '{sub[2]}', '{sub[3]}');");
            }
        }

        private DAL CreateDatabase()
        {
            var units = new string[] { "u1", "u2" };
            var strata = new string[][]
            {
                new string[] {"st1", "" },
                new string[] {"st2", "" },
            };
            var unit_strata = new string[][]
            {
                new string[] {"u1", "st1" },
                new string[] { "u1", "st2" },
                new string[] { "u2", "st2" },
            };

            var sampleGroups = new[]
            {
                new{StCode = "st1", SgCode = "sg1", Freq = 101, TallyBySp = 1},
                new{StCode = "st2", SgCode = "sg2", Freq = 101, TallyBySp = 0},
            };

            var species = new string[] { "sp1", "sp2", "sp3" };

            var tdvs = new[]
            {
                // sp, L/D, Prod
                new[] { "sp1", "L", "01" },
                new[] { "sp1", "D", "01" },
                new[] { "sp2", "L", "01" },
                new[] { "sp3", "L", "01" },
            };

            var subPops = new string[][]
            {
                // st, sg, sp, ld
                new[] { "st1", "sg1", "sp1", "L" },
                new[] { "st1", "sg1", "sp2", "L" },
                new[] { "st1", "sg1", "sp3", "L" },
            };


            var database = new DAL();

            //HACK: in the current db version there is a foreign key constraint on the Tree view in TreeCalculated values
            //will be removed but for now lets just drop the TCV table
            //database.Execute("Drop table TreeCalculatedValues;");

            InitializeDatabase(database, units, strata, unit_strata, sampleGroups, species, tdvs, subPops);

            return database;
        }

        #region plot

        [Fact]
        public void AddNewPlot()
        {
            var unitCode = "u1";
            //var plotNumber = 1;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var plotID = datastore.AddNewPlot(unitCode);

                plotID.Should().NotBeNullOrEmpty();

                

                var plot1 = datastore.GetPlot(plotID);

                var strat_plots = datastore.GetPlot_Strata(unitCode, plot1.PlotNumber);



                var plotID2 = datastore.AddNewPlot(unitCode);

                plotID2.Should().NotBeNullOrEmpty();

                
            }
        }

        [Fact]
        public void CreatePlotTree()
        {
            var unitCode = "u1";
            var plotNumber = 1;
            var stratumCode = "st1";
            var sgCode = "sg1";
            var species = "sp1";
            var liveDead = "L";
            var countMeasure = "C";
            var treeCount = 1;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                database.Execute(
                    $"INSERT INTO Plot_V3 (PlotID, CuttingUnitCode, PlotNumber) VALUES ({Guid.Empty.ToString()}, '{unitCode}', {plotNumber});" +
                    $"INSERT INTO Plot_Stratum (CuttingUnitCode, PlotNumber, StratumCode) VALUES ('{unitCode}', {plotNumber}, '{stratumCode}');");

                var treeID = datastore.CreatePlotTree(unitCode, plotNumber, stratumCode, sgCode, species, liveDead, countMeasure, treeCount);

                database.ExecuteScalar<int>("SELECT count(*) from Tree_V3;").Should().Be(1);

                //var mytree = database.QueryGeneric("SELECT * FROM Tree_V3;")

                var tree = datastore.GetTree(treeID);
                tree.Should().NotBeNull();

                tree.TreeID.Should().Be(treeID);
                tree.StratumCode.Should().Be(stratumCode);
                tree.SampleGroupCode.Should().Be(sgCode);
                tree.Species.Should().Be(species);
                tree.LiveDead.Should().Be(liveDead);
                tree.CountOrMeasure.Should().Be(countMeasure);
                //tree.TreeCount.Should().Be(treeCount);
            }
        }

        [Fact]
        public void GetNextPlotNumber()
        {
            var unitCode = "u1";
            var plotNumber = 1;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                datastore.GetNextPlotNumber(unitCode).Should().Be(1, "unit with no plots, should return 1 for first plot number");

                database.Execute($"INSERT INTO Plot_V3 (PlotID, CuttingUnitCode, PlotNumber) VALUES ('plotID1', '{unitCode}', {plotNumber});");

                datastore.GetNextPlotNumber(unitCode).Should().Be(plotNumber + 1, "unit with a plot, should return max plot number + 1");
            }
        }

        [Fact]
        public void IsPlotNumberAvalible()
        {
            var unitCode = "u1";
            var plotNumber = 1;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                datastore.IsPlotNumberAvalible(unitCode, plotNumber).Should().BeTrue("no plots in unit yet");

                database.Execute($"INSERT INTO Plot_V3 (PlotID, CuttingUnitCode, PlotNumber) VALUES ('plotID1', '{unitCode}', {plotNumber});");

                datastore.IsPlotNumberAvalible(unitCode, plotNumber).Should().BeFalse("we just inserted a plot");
            }
        }

        [Fact]
        public void GetPlotsByUnitCode()
        {
            var unitCode = "u1";
            var plotNumber = 1;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                datastore.GetPlotsByUnitCode(unitCode).Should().BeEmpty("we havn't added any plots yet");

                database.Execute($"INSERT INTO Plot_V3 (PlotID, CuttingUnitCode, PlotNumber) VALUES ('plotID1', '{unitCode}', {plotNumber});");

                datastore.GetPlotsByUnitCode(unitCode).Should().ContainSingle();
            }
        }

        [Fact]
        public void GetPlot_Strata()
        {
            var unitCode = "u1";
            var strata = new string[] { "st1", "st2" };
            var plotNumber = 1;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                datastore.GetPlotsByUnitCode(unitCode).Should().BeEmpty("we havn't added any plots yet");

                database.Execute($"INSERT INTO Plot_V3 (PlotID, CuttingUnitCode, PlotNumber) VALUES ('plotID1', '{unitCode}', {plotNumber});");

                foreach (var st in strata)
                {
                    database.Execute($"INSERT INTO Plot_Stratum (CuttingUnitCode, PlotNumber, StratumCode) VALUES " +
                        $"('{unitCode}', {plotNumber}, '{st}');");
                }

                var stPlots = datastore.GetPlot_Strata(unitCode, plotNumber);

                stPlots.Should().HaveCount(strata.Count());

                foreach (var ps in stPlots)
                {
                    ValidatePlot_Stratum(ps, true);
                }
            }
        }

        [Fact]
        public void GetPlot_Stratum()
        {
            var units = new string[] { "u1" };
            var strata = new string[][]
            {
                new string[] {"st1", "" },
                new string[] {"st2", "" },
                new string[] {"st3", "" },
            };
            var unit_strata = new string[][]
            {
                new string[] {"u1", "st1" },
                new string[] { "u1", "st2" },
                new string[] { "u1", "st3" },
            };

            var plotNumber = 1;

            using (var database = new DAL())
            {
                InitializeDatabase(database, units, strata, unit_strata, null, null, null, null);

                var datastore = new CuttingUnitDatastore(database);

                datastore.GetPlotsByUnitCode(units[0]).Should().BeEmpty("we havn't added any plots yet");

                database.Execute($"INSERT INTO Plot_V3 (PlotID, CuttingUnitCode, PlotNumber) VALUES ('plotID1', '{units[0]}', {plotNumber});");

                foreach (var st in strata)
                {
                    if(st[0] == "st3") { continue; }

                    database.Execute($"INSERT INTO Plot_Stratum (CuttingUnitCode, PlotNumber, StratumCode) VALUES " +
                        $"('{units[0]}', {plotNumber}, '{st[0]}');");
                }

                foreach (var st in strata)
                {
                    var plotStratum = datastore.GetPlot_Stratum(units[0], st[0], plotNumber);
                    ValidatePlot_Stratum(plotStratum, st[0] != "st3");
                }

                var nonExistantPS = datastore.GetPlot_Stratum(units[0], "st3", plotNumber);
                ValidatePlot_Stratum(nonExistantPS, false);
            }
        }

        void ValidatePlot_Stratum(Plot_Stratum ps, bool inCruise)
        {
            ps.CuttingUnitCode.Should().NotBeNullOrWhiteSpace();
            ps.StratumCode.Should().NotBeNullOrWhiteSpace();
            ps.PlotNumber.Should().BeGreaterThan(0);
            ps.InCruise.Should().Be(inCruise);
        }

        [Fact]
        public void GetPlot_ByPlotID()
        {
            var unitCode = "u1";
            var plotNumber = 1;
            var plotID = "plotID1";

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                database.Execute($"INSERT INTO Plot_V3 (PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('{plotID}', '{unitCode}', {plotNumber})");

                var plot = datastore.GetPlot(plotID);

                plot.Should().NotBeNull();
            }
        }

        [Fact]
        public void GetPlot_ByUnitPlotNumber()
        {
            var unitCode = "u1";
            var plotNumber = 1;
            var plotID = "plotID1";

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                database.Execute($"INSERT INTO Plot_V3 (PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('{plotID}', '{unitCode}', {plotNumber})");

                var plot = datastore.GetPlot(unitCode, plotNumber);

                plot.Should().NotBeNull();
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

                database.Execute($"INSERT INTO Stratum (Code, Method) VALUES ('03', '{method}');");
                database.Execute("INSERT INTO CuttingUnit_Stratum (CuttingUnitCode, StratumCode) VALUES ('u1', '03')");

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
            var isEmpty = true;
            var kpi = 101;
            //var remarks = "something";

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var stratumPlot = new Plot_Stratum()
                {
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    IsEmpty = isEmpty,
                    KPI = kpi,
                    //Remarks = remarks
                };

                database.Execute($"INSERT INTO Plot_V3 (PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('plotID1', '{unitCode}', {plotNumber})");

                datastore.InsertPlot_Stratum(stratumPlot);

                datastore.IsPlotNumberAvalible(unitCode, plotNumber).Should().BeFalse("we just took that plot number");

                var plotStratumAgain = datastore.GetPlot_Stratum(unitCode, stratumCode, plotNumber);
                plotStratumAgain.Should().NotBeNull();
                plotStratumAgain.PlotNumber.Should().Be(plotNumber);
                //ourStratumPlot.Remarks.Should().Be(remarks);
                plotStratumAgain.KPI.Should().Be(kpi);
                plotStratumAgain.IsEmpty.Should().Be(isEmpty);
                plotStratumAgain.StratumCode.Should().Be(stratumCode);
            }
        }

        [Fact]
        public void UpdatePlot_Stratum()
        {
            var unitCode = "u1";
            var stratumCode = "st1";
            var plotNumber = 1;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var stratumPlot = new Plot_Stratum()
                {
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                    IsEmpty = false,
                };

                database.Execute($"INSERT INTO Plot_V3 (PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('plotID1', '{unitCode}', {plotNumber})");

                datastore.InsertPlot_Stratum(stratumPlot);

                stratumPlot.IsEmpty = true;
                datastore.UpdatePlot_Stratum(stratumPlot);

                var ourStratumPlot = datastore.GetPlot_Stratum(unitCode, stratumCode, plotNumber);

                ourStratumPlot.IsEmpty.Should().Be(true);
            }
        }

        [Fact]
        public void DeletePlot_Stratum()
        {
            var unitCode = "u1";
            var stratumCode = "st1";
            var plotNumber = 1;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var stratumPlot = new Plot_Stratum()
                {
                    CuttingUnitCode = unitCode,
                    PlotNumber = plotNumber,
                    StratumCode = stratumCode,
                };

                database.Execute("INSERT INTO Plot_V3 (PlotID, CuttingUnitCode, PlotNumber) VALUES " +
                    $"('plotID1', '{unitCode}', {plotNumber});");

                datastore.InsertPlot_Stratum(stratumPlot);

                var echo = datastore.GetPlot_Stratum(unitCode, stratumCode, plotNumber);
                echo.Should().NotBeNull("where's my echo");

                datastore.DeletePlot_Stratum(echo.CuttingUnitCode, echo.StratumCode, echo.PlotNumber);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void GetPlotTallyPopulationsByUnitCode_PNT_FIX_noPlot(int tallyBySp)
        {
            var unitCode = "u3";
            var stCode = "st3";
            var sgCode = "sg4";

            var method = CruiseDAL.Schema.CruiseMethods.PNT;

            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                InitializeDatabase(
                    database,
                    new[] { unitCode },
                    new[]
                    {
                        new[] {stCode, method},
                    },
                    new[]
                    {
                        new [] {unitCode, stCode},
                    },
                    new[]
                    {
                        new {StCode = stCode, SgCode = sgCode, Freq = 101, TallyBySp = tallyBySp}
                    },
                    new[] { "sp4" },
                    new[]
                    {
                        new[] {"sp4", "L", "01"},
                        new[] {"sp4", "D", "01"},
                    },
                    new[]
                    {
                        new [] {stCode, sgCode, "sp4", "L" },
                        new [] {stCode, sgCode, "sp4", "D" },
                    }
                );

                {
                    //we are going to check that the tally population returned is vallid for a
                    //tally population with no count tree record associated
                    //it should return one tally pop per sample group in the unit, that is associated with a FIX or PNT stratum
                    var unit3tallyPops = datastore.GetPlotTallyPopulationsByUnitCode("u3", 1);

                    if(tallyBySp == 0)
                    {
                        unit3tallyPops.Should().HaveCount(1);

                        var tp = unit3tallyPops.Single();
                        tp.Species.Should().BeNull("Species");
                        tp.LiveDead.Should().BeNull("liveDead");

                        ValidateTallyPop(tp);
                    }
                    else
                    {
                        unit3tallyPops.Should().HaveCount(2);

                        foreach(var tp in unit3tallyPops)
                        {
                            tp.Species.Should().NotBeNullOrWhiteSpace();
                            tp.LiveDead.Should().NotBeNullOrWhiteSpace();
                        }
                    }

                    void ValidateTallyPop(TallyPopulation_Plot tp)
                    {
                        tp.StratumCode.Should().Be("st3");
                        tp.SampleGroupCode.Should().Be("sg4");
                        tp.InCruise.Should().BeFalse();
                    }
                }
            }
        }

        //[Fact]
        //public void GetPlotTallyPopulationsByUnitCode_PNT_FIX_no_tally_setup()
        //{
        //    var method = CruiseDAL.Schema.CruiseMethods.PNT;

        //    using (var database = CreateDatabase())
        //    {
        //        var datastore = new CuttingUnitDatastore(database);

        //        database.Insert(new CuttingUnitDO
        //        {
        //            Code = "u3",
        //            Area = 0
        //        });

        //        database.Insert(new StratumDO
        //        {
        //            Code = "st3",
        //            Method = method
        //        });

        //        database.Insert(new StratumDO
        //        {
        //            Code = "st4",
        //            Method = CruiseDAL.Schema.CruiseMethods.PCM
        //        });

        //        database.Insert(new CuttingUnitStratumDO
        //        {
        //            CuttingUnit_CN = 3,
        //            Stratum_CN = 3
        //        });

        //        database.Insert(new CuttingUnitStratumDO
        //        {
        //            CuttingUnit_CN = 3,
        //            Stratum_CN = 4
        //        });

        //        database.Insert(new SampleGroupDO
        //        {
        //            Stratum_CN = 3,
        //            Code = "sg3",
        //            CutLeave = "C",
        //            UOM = "01",
        //            PrimaryProduct = "01"
        //        });

        //        database.Insert(new SampleGroupTreeDefaultValueDO
        //        {
        //            SampleGroup_CN = 3,
        //            TreeDefaultValue_CN = 1
        //        });

        //        database.Insert(new PlotDO
        //        {
        //            CuttingUnit_CN = 3,
        //            Stratum_CN = 3,
        //            PlotNumber = 1
        //        });

        //        {
        //            //we are going to check that the tally population returned is vallid for a
        //            //tally population with no count tree record associated
        //            //it should return one tally pop per sample group in the unit, that is associated with a FIX or PNT stratum
        //            var unit3tallyPops = datastore.GetPlotTallyPopulationsByUnitCode("u3", 1);
        //            unit3tallyPops.Should().HaveCount(1);
        //            var unit3tallyPop = unit3tallyPops.Single();
        //            unit3tallyPop.CountTree_CN.Should().BeNull("countTree_CN");
        //            unit3tallyPop.Species.Should().BeNull("Species");
        //            unit3tallyPop.LiveDead.Should().BeNull("liveDead");
        //            unit3tallyPop.StratumCode.Should().Be("st3");
        //            unit3tallyPop.SampleGroupCode.Should().Be("sg3");
        //            unit3tallyPop.InCruise.Should().BeTrue();
        //        }
        //    }
        //}

        //[Fact]
        //public void GetPlotTallyPopulationsByUnitCode_noPlot()
        //{
        //    var method = CruiseDAL.Schema.CruiseMethods.PCM;

        //    using (var database = CreateDatabase())
        //    {
        //        var datastore = new CuttingUnitDatastore(database);

        //        //set up two cutting units one (u3) will be given a count tree record
        //        //the other (u4) will not,
        //        //so that we can test situations where count tree records are missing for a unit
        //        database.Insert(new CuttingUnitDO
        //        {
        //            Code = "u3",
        //            Area = 0,
        //        });

        //        database.Insert(new CuttingUnitDO
        //        {
        //            Code = "u4",
        //            Area = 0,
        //        });

        //        database.Insert(new StratumDO
        //        {
        //            Code = "st3",
        //            Method = method,
        //        });

        //        database.Insert(new CuttingUnitStratumDO
        //        {
        //            CuttingUnit_CN = 3,
        //            Stratum_CN = 3,
        //        });

        //        database.Insert(new CuttingUnitStratumDO
        //        {
        //            CuttingUnit_CN = 4,
        //            Stratum_CN = 3,
        //        });

        //        database.Insert(new SampleGroupDO
        //        {
        //            Stratum_CN = 3,
        //            Code = "sg3",
        //            CutLeave = "C",
        //            UOM = "01",
        //            PrimaryProduct = "01"
        //        });

        //        database.Insert(new SampleGroupTreeDefaultValueDO
        //        {
        //            SampleGroup_CN = 3,
        //            TreeDefaultValue_CN = 1
        //        });

        //        database.Insert(new CountTreeDO()
        //        {
        //            CuttingUnit_CN = 3,
        //            SampleGroup_CN = 3,
        //            Tally_CN = 1,
        //            TreeDefaultValue_CN = 1
        //        });

        //        //tally population should
        //        var unit3tallyPops = datastore.GetPlotTallyPopulationsByUnitCode("u3", 1);
        //        unit3tallyPops.Should().HaveCount(1);
        //        var unit3tallyPop = unit3tallyPops.Single();
        //        unit3tallyPop.CountTree_CN.Should().NotBeNull();
        //        unit3tallyPop.Species.Should().Be("sp1");
        //        unit3tallyPop.StratumCode.Should().Be("st3");
        //        unit3tallyPop.SampleGroupCode.Should().Be("sg3");
        //        unit3tallyPop.InCruise.Should().BeFalse();

        //        var unit4tallyPops = datastore.GetPlotTallyPopulationsByUnitCode("u4", 1);
        //        unit4tallyPops.Should().HaveCount(1);
        //        var unit4tallyPop = unit4tallyPops.Single();
        //        unit4tallyPop.CountTree_CN.Should().BeNull();
        //        unit4tallyPop.InCruise.Should().BeFalse();
        //    }
        //}

        //[Fact]
        //public void GetPlotTallyPopulationsByUnitCode()
        //{
        //    var method = CruiseDAL.Schema.CruiseMethods.PCM;

        //    using (var database = CreateDatabase())
        //    {
        //        var datastore = new CuttingUnitDatastore(database);

        //        //set up two cutting units one (u3) will be given a count tree record
        //        //the other (u4) will not,
        //        //so that we can test situations where count tree records are missing for a unit
        //        database.Insert(new CuttingUnitDO
        //        {
        //            Code = "u3",
        //            Area = 0,
        //        });

        //        database.Insert(new CuttingUnitDO
        //        {
        //            Code = "u4",
        //            Area = 0,
        //        });

        //        database.Insert(new StratumDO
        //        {
        //            Code = "st3",
        //            Method = method,
        //        });

        //        database.Insert(new CuttingUnitStratumDO
        //        {
        //            CuttingUnit_CN = 3,
        //            Stratum_CN = 3,
        //        });

        //        database.Insert(new CuttingUnitStratumDO
        //        {
        //            CuttingUnit_CN = 4,
        //            Stratum_CN = 3,
        //        });

        //        database.Insert(new SampleGroupDO
        //        {
        //            Stratum_CN = 3,
        //            Code = "sg3",
        //            CutLeave = "C",
        //            UOM = "01",
        //            PrimaryProduct = "01"
        //        });

        //        database.Insert(new SampleGroupTreeDefaultValueDO
        //        {
        //            SampleGroup_CN = 3,
        //            TreeDefaultValue_CN = 1
        //        });

        //        database.Insert(new CountTreeDO()
        //        {
        //            CuttingUnit_CN = 3,
        //            SampleGroup_CN = 3,
        //            Tally_CN = 1,
        //            TreeDefaultValue_CN = 1
        //        });

        //        database.Insert(new PlotDO
        //        {
        //            CuttingUnit_CN = 3,
        //            Stratum_CN = 3,
        //            PlotNumber = 1
        //        });

        //        //tally population should
        //        var unit3tallyPops = datastore.GetPlotTallyPopulationsByUnitCode("u3", 1);
        //        unit3tallyPops.Should().HaveCount(1);
        //        var unit3tallyPop = unit3tallyPops.Single();
        //        unit3tallyPop.CountTree_CN.Should().NotBeNull();
        //        unit3tallyPop.Species.Should().Be("sp1");
        //        unit3tallyPop.StratumCode.Should().Be("st3");
        //        unit3tallyPop.SampleGroupCode.Should().Be("sg3");
        //        unit3tallyPop.InCruise.Should().BeTrue();

        //        var unit4tallyPops = datastore.GetPlotTallyPopulationsByUnitCode("u4", 1);
        //        unit4tallyPops.Should().HaveCount(1);
        //        var unit4tallyPop = unit4tallyPops.Single();
        //        unit4tallyPop.CountTree_CN.Should().BeNull();
        //        unit4tallyPop.InCruise.Should().BeFalse();
        //    }
        //}

        //[Fact]
        //public void GetFixCNTTallyPopulations()
        //{
        //    using (var database = CreateDatabase())
        //    {
        //        var stratum = new StratumDO()
        //        {
        //            Code = "fixCnt1",
        //            Method = CruiseDAL.Schema.CruiseMethods.FIXCNT
        //        };
        //        database.Insert(stratum);

        //        var sg = new SampleGroupDO()
        //        {
        //            Code = "sgFixCnt",
        //            CutLeave = "C",
        //            UOM = "01",
        //            PrimaryProduct = "01",
        //            Stratum_CN = stratum.Stratum_CN
        //        };
        //        database.Insert(sg);

        //        var tdv = new TreeDefaultValueDO()
        //        {
        //            Species = "someSpecies",
        //            LiveDead = "L",
        //            PrimaryProduct = "01"
        //        };
        //        database.Insert(tdv);

        //        var sgTdv = new SampleGroupTreeDefaultValueDO()
        //        {
        //            SampleGroup_CN = sg.SampleGroup_CN,
        //            TreeDefaultValue_CN = tdv.TreeDefaultValue_CN
        //        };
        //        database.Insert(sgTdv);

        //        var fixCntTallyClass = new FixCNTTallyClassDO()
        //        {
        //            FieldName = (int)FixCNTTallyField.DBH,
        //            Stratum_CN = stratum.Stratum_CN
        //        };
        //        database.Insert(fixCntTallyClass);
        //        //database.Execute($"Update FixCNTTallyClass set FieldName = 'DBH';");

        //        var fixCntTallyPop = new FixCNTTallyPopulationDO()
        //        {
        //            FixCNTTallyClass_CN = fixCntTallyClass.FixCNTTallyClass_CN,
        //            SampleGroup_CN = sg.SampleGroup_CN,
        //            TreeDefaultValue_CN = tdv.TreeDefaultValue_CN,
        //            IntervalSize = 101,
        //            Min = 102,
        //            Max = 103
        //        };
        //        database.Insert(fixCntTallyPop);

        //        var datastore = new CuttingUnitDatastore(database);

        //        var result = datastore.GetFixCNTTallyPopulations(stratum.Code).ToArray();
        //        result.Should().HaveCount(1);

        //        var firstResult = result.First();
        //        firstResult.IntervalSize.Should().Be(101);
        //        firstResult.Min.Should().Be(102);
        //        firstResult.Max.Should().Be(103);

        //        firstResult.SGCode.Should().Be(sg.Code);
        //        firstResult.StratumCode.Should().Be(stratum.Code);
        //        firstResult.Species.Should().Be(tdv.Species);
        //        firstResult.FieldName.Should().Be(FixCNTTallyField.DBH);
        //        //firstResult.LiveDead.Should().Be(tdv.LiveDead);
        //    }
        //}

        public void CreateFixCNTTallyTree()
        {
        }

        #endregion plot
    }
}
