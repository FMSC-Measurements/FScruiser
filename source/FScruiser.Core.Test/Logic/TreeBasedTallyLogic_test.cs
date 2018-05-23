using CruiseDAL.DataObjects;
using CruiseDAL.Schema;
using FluentAssertions;
using FScruiser.Logic;
using FScruiser.Models;
using FScruiser.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FScruiser.Core.Test.Logic
{
    public class TreeBasedTallyLogic_test
    {
        [Theory]
        //WHEN frequency 1 in 1 (guarintee sample)
        //AND no insurance trees
        //THEN expect measure tree
        [InlineData(1, 0, "M")]

        //WHEN frequency is 1 in 2 
        //AND insurance frequency is 1 in 1
        [InlineData(2, 1, "I")]//if freq is 1 sampler wont do insurance //TODO it would be nice to have a better way to guarntee a insurance sample

        //[InlineData(0,0, "C", false, false)]//frequency of 0 is not allowed and breaks the OnTally
        public void TallyStandard(int frequency, int insuranceFreq, string resultCountMeasure)
        {

            using (var ds = CreateDatastore(CruiseDAL.Schema.CruiseMethods.STR, frequency, insuranceFreq))
            {
                var unitDs = new CuttingUnitDatastore(ds);

                var unit = ds.From<CuttingUnit>().Query().First();

                var dataService = new CuttingUnitDataService(unitDs, unit);
                dataService.RefreshData();

                var count = dataService.TallyPopulations.FirstOrDefault();

                //var tallyFeed = new List<TallyFeedItem>();

                //var appSettingsMock = new Mock<ITallySettingsDataService>();
                //appSettingsMock.Setup(x => x.EnableCruiserPopup).Returns(enableCruiserPopup);
                //appSettingsMock.Setup(x => x.EnableAskEnterTreeData).Returns(enterMeasureTreeData);

                //var dialogServiceMock = new Mock<IDialogService>();
                //dialogServiceMock.Setup(x => x.AskYesNoAsync(It.Is<string>(s => s == "Would you like to enter tree data now?"), It.IsAny<string>(), It.IsAny<bool>()))
                //    .Returns(Task.FromResult(enterMeasureTreeData));
                //dialogServiceMock.Setup(x => x.ShowEditTreeAsync(It.IsAny<Tree>(), It.IsAny<ICuttingUnitDataService>()));

                //var soundServiceMock = new Mock<ISoundService>();

                var tallyEntry = TreeBasedTallyLogic.TallyStandard(count, dataService);
                tallyEntry.UnitCode.Should().Be(unit.Code);
                tallyEntry.StratumCode.Should().Be(count.StratumCode);
                tallyEntry.SGCode.Should().Be(count.SampleGroupCode);
                tallyEntry.Species.Should().Be(count.Species);
                tallyEntry.TreeCount.Should().Be(1);

                var tree = tallyEntry.Tree;
                if (resultCountMeasure == "M" || resultCountMeasure == "I")
                {
                    
                    tree.Should().NotBeNull();
                    tree.CountOrMeasure.Should().Be(resultCountMeasure);
                    tallyEntry.TreeNumber.Should().NotBeNull();
                    tallyEntry.TreeNumber.Should().Be((int)tree.TreeNumber);
                }
                else
                {
                    tree.Should().BeNull();
                    tallyEntry.TreeNumber.Should().BeNull();
                }
            }
        }

        [Theory]
        [InlineData(2, 0, "M")]
        public void TallyThreeP(int kpi, int insuranceFreq, string resultCountMeasure)
        {
            var unit = new CuttingUnit();

            var sampleGroup = new SampleGroup
            {
                Sampler = new FMSC.Sampling.ThreePSelecter(1, 1, 0)
            };

            var count = new CountTree()
            {
                CuttingUnit_CN = 1
            };

            var pop = new TallyPopulation
            {
                Method = "3P",
                SampleGroup = sampleGroup,
                Count = count
            };


            var dataServiceMock = new Mock<ICuttingUnitDataService>();
            dataServiceMock.Setup(x => x.CreateTree(It.IsAny<TallyPopulation>())).Returns(() => new Tree());

            ICuttingUnitDataService dataService = dataServiceMock.Object;

            var tallyEntry = TreeBasedTallyLogic.TallyThreeP(pop, kpi, dataService);
            var tree = tallyEntry.Tree;
            tree.KPI.Should().Be(kpi);
            tree.CountOrMeasure.Should().Be(resultCountMeasure);
        }

        [Fact]
        public void TallyThreeP_STM()
        {
            int kpi = -1;

            var unit = new CuttingUnit();
            var sampleGroup = new SampleGroup
            {
                Sampler = new FMSC.Sampling.ThreePSelecter(0, 0, 0)
            };
            var count = new CountTree()
            {
                CuttingUnit_CN = 1
            };
            var pop = new TallyPopulation
            {
                Method = "3P",
                SampleGroup = sampleGroup,
                Count = count
            };


            var dataServiceMock = new Mock<ICuttingUnitDataService>();
            dataServiceMock.Setup(x => x.CreateTree(It.IsAny<TallyPopulation>())).Returns(() => new Tree());

            ICuttingUnitDataService dataService = dataServiceMock.Object;

            var tallyEntry = TreeBasedTallyLogic.TallyThreeP(pop, kpi, dataService);
            var tree = tallyEntry.Tree;
            tree.STM.Should().Be("Y");
            tree.KPI.Should().Be(0);
            tree.CountOrMeasure.Should().Be("M");
        }


        private CruiseDAL.DAL CreateDatastore(string cruiseMethod, int freqORkz, int insuranceFreq)
        {
            var ds = new CruiseDAL.DAL();
            try
            {
                var sale = new SaleDO()
                {
                    DAL = ds,
                    SaleNumber = "12345",
                    Region = "1",
                    Forest = "1",
                    District = "1",
                    Purpose = "something",
                    LogGradingEnabled = true
                };
                sale.Save();

                var stratum = new StratumDO()
                {
                    DAL = ds,
                    Code = "01",
                    Method = cruiseMethod
                };
                stratum.Save();

                var cuttingUnit = new CuttingUnitDO()
                {
                    DAL = ds,
                    Code = "01"
                };
                cuttingUnit.Save();

                var cust = new CuttingUnitStratumDO()
                {
                    DAL = ds,
                    CuttingUnit = cuttingUnit,
                    Stratum = stratum
                };
                cust.Save();

                var sampleGroup = new SampleGroupDO()
                {
                    DAL = ds,
                    Stratum = stratum,
                    Code = "01",
                    PrimaryProduct = "01",
                    UOM = "something",
                    CutLeave = "something",
                    InsuranceFrequency = insuranceFreq
                };

                if (CruiseMethods.THREE_P_METHODS.Contains(cruiseMethod))
                {
                    sampleGroup.KZ = freqORkz;
                }
                else
                {
                    sampleGroup.SamplingFrequency = freqORkz;
                }

                sampleGroup.Save();

                var tally = new TallyDO()
                {
                    DAL = ds,
                    Hotkey = "A",
                    Description = "something"
                };
                tally.Save();

                var count = new CountTreeDO()
                {
                    DAL = ds,
                    CuttingUnit = cuttingUnit,
                    SampleGroup = sampleGroup,
                    Tally = tally
                };
                count.Save();

                return ds;
            }
            catch
            {
                ds.Dispose();
                throw;
            }
        }
    }
}
