using CruiseDAL;
using CruiseDAL.DataObjects;
using FluentAssertions;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FScruiser.Core.Test.Services
{
    public class CuttingUnitDatastore_Test
    {
        DAL CreateDatabase()
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
                database.Insert(new TallyDO
                {
                    Hotkey = "A",
                    Description = "something"
                });

                database.Insert(new CountTreeDO
                {
                    Tally_CN = 1,
                    CuttingUnit_CN = 1,
                    SampleGroup_CN = 1
                });

                var datastore = new CuttingUnitDatastore(database);

                var results = datastore.GetTallyPopulationsByUnitCode(unitCode);
                results.Should().HaveCount(1);

                var result = results.Single();

                VerifyTallyPopulation(result);
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
            var species = "pp";

            using (var database = CreateDatabase())
            {
                database.Insert(new TreeDefaultValueDO()
                {
                    PrimaryProduct = "01",
                    LiveDead = "L",
                    Species = species
                });

                database.Insert(new TallyDO
                {
                    Hotkey = "A",
                    Description = "something"
                });

                database.Insert(new CountTreeDO
                {
                    Tally_CN = 1,
                    CuttingUnit_CN = 1,
                    SampleGroup_CN = 1,
                    TreeDefaultValue_CN = 1
                });

                var datastore = new CuttingUnitDatastore(database);

                var results = datastore.GetTallyPopulationsByUnitCode(unitCode);
                results.Should().HaveCount(1);

                var result = results.Single();
                VerifyTallyPopulation(result, species);
            }
        }

    }
}
