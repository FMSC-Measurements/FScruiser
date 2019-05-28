using CruiseDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace FScruiser.Core.Test.Services
{
    public class Datastore_TestBase : TestBase
    {
        public Datastore_TestBase(ITestOutputHelper output) : base(output)
        {
        }

        protected virtual DAL CreateDatabase()
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

            InitializeDatabase(database, units, strata, unit_strata, sampleGroups, species, tdvs, subPops);

            return database;
        }

        void InitializeDatabase(DAL database, string[] units, string[][] strata,
    string[][] unit_strata, dynamic[] sampleGroups,
    string[] species, string[][] tdvs, string[][] subPops)
        {
            //Cutting Units
            foreach (var unit in units)
            {
                database.Execute(
                    "INSERT INTO CuttingUnit (" +
                    "Code" +
                    ") VALUES " +
                    $"('{unit}');");
            }

            //Strata
            foreach (var st in strata)
            {
                database.Execute($"INSERT INTO Stratum (Code, Method) VALUES ('{st[0]}', '{st[1]}');");
            }

            //Unit - Strata
            foreach (var cust in unit_strata)
            {
                database.Execute(
                    "INSERT INTO CuttingUnit_Stratum " +
                    "(CuttingUnitCode, StratumCode) " +
                    "VALUES " +
                    $"('{cust[0]}','{cust[1]}');");
            }

            //Sample Groups
            foreach (var sg in sampleGroups)
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

            foreach (var sp in species)
            {
                database.Execute($"INSERT INTO Species (Species) VALUES ('{sp}');");
            }

            foreach (var tdv in tdvs)
            {
                database.Execute(
                    "INSERT INTO TreeDefaultValue (" +
                    "Species, " +
                    "LiveDead, " +
                    "PrimaryProduct" +
                    ") VALUES " +
                    $"('{tdv[0]}', '{tdv[1]}', '{tdv[2]}');");
            }

            foreach (var sub in subPops)
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
    }
}
