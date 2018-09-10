using CruiseDAL;
using CruiseDAL.DataObjects;
using FluentAssertions;
using FScruiser.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FScruiser.Core.Test.Util
{
    public class DatabaseUpdater_Test
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

        [Fact]
        public void Update()
        {
            using (var database = CreateDatabase())
            {
                DatabaseUpdater.Update(database);

                database.CheckTableExists("TallyLedger").Should().BeTrue("TallyLedger");
                database.Invoking(x => x.Execute("EXPLAIN SELECT * FROM TallyPopulation;")).Should().NotThrow("TallyPopulation is a view");
            }
        }
    }
}
