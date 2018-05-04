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
        [InlineData(1, 0, "M", false, false)]
        //TODO it would be nice to have a better way to guarntee a insurance sample
        [InlineData(2, 1, "I", false, false)]//if freq is 1 sampler wont do insurance
        [InlineData(1, 0, "M", true, true)]
        //[InlineData(0,0, "C", false, false)]//frequency of 0 is not allowed and breaks the OnTally
        public void OnTallyTest_STR(int frequency, int insuranceFreq, string resultCountMeasure, bool enableCruiserPopup, bool enterMeasureTreeData)
        {

            using (var ds = CreateDatastore(CruiseDAL.Schema.CruiseMethods.STR, frequency, insuranceFreq))
            {
                var unit = ds.From<CuttingUnit>().Query().First();

                var dataService = new CuttingUnitDataService(unit) { Datastore = ds };
                dataService.RefreshData();

                var count = dataService.TallyPopulations.FirstOrDefault();

                var tallyFeed = new List<TallyFeedItem>();

                var appSettingsMock = new Mock<ITallySettingsDataService>();
                appSettingsMock.Setup(x => x.EnableCruiserPopup).Returns(enableCruiserPopup);
                appSettingsMock.Setup(x => x.EnableAskEnterTreeData).Returns(enterMeasureTreeData);

                var dialogServiceMock = new Mock<IDialogService>();
                dialogServiceMock.Setup(x => x.AskYesNoAsync(It.Is<string>(s => s == "Would you like to enter tree data now?"), It.IsAny<string>(), It.IsAny<bool>()))
                    .Returns(Task.FromResult(enterMeasureTreeData));
                dialogServiceMock.Setup(x => x.ShowEditTreeAsync(It.IsAny<Tree>()));

                var soundServiceMock = new Mock<ISoundService>();


                TreeBasedTallyLogic.OnTallyAsync(count, dataService, tallyFeed,
                    appSettingsMock.Object,
                    dialogServiceMock.Object,
                    soundServiceMock.Object).Wait();



                tallyFeed.Should().HaveCount(1);
                var tallyAction = tallyFeed.Single();

                var treeCount = ds.ExecuteScalar<int>("SELECT Sum(TreeCount) FROM CountTree;");
                treeCount.Should().Be(1);

                soundServiceMock.Verify(x => x.SignalTally(It.IsAny<bool>()));

                if (resultCountMeasure == "M" || resultCountMeasure == "I")
                {
                    tallyAction.Tree.Should().NotBeNull();

                    var tree = ds.From<Tree>().Read().Single();
                    tree.Should().NotBeNull();
                    tree.CountOrMeasure.Should().Be(resultCountMeasure);

                    if (resultCountMeasure == "M")
                    {
                        soundServiceMock.Verify(x => x.SignalMeasureTree());
                    }
                    else
                    {
                        soundServiceMock.Verify(x => x.SignalInsuranceTree());
                    }

                    if (enterMeasureTreeData)
                    {
                        dialogServiceMock.Verify(x => x.ShowEditTreeAsync(It.IsNotNull<Tree>()));
                    }

                    if (enableCruiserPopup)
                    {
                        dialogServiceMock.Verify(x => x.AskCruiserAsync(It.IsNotNull<Tree>()));
                    }
                    else
                    {
                        dialogServiceMock.Verify(x => x.ShowMessageAsync(It.Is<string>(s => s.Contains("Tree #")), It.IsAny<string>()));
                    }
                }
            }
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
