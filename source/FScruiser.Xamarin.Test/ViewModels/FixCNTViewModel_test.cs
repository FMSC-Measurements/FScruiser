using CruiseDAL;
using CruiseDAL.DataObjects;
using FluentAssertions;
using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Constants;
using Prism.Navigation;
using System.Linq;
using Xunit;

namespace FScruiser.XF.ViewModels
{
    public class FixCNTViewModel_test
    {
        private CruiseDAL.DAL CreateDatabase()
        {
            var database = new DAL();

            database.Insert(new CuttingUnitDO
            {
                Code = "u1",
                Area = 0,
            });

            var stratum = new StratumDO()
            {
                Code = "fixCnt1",
                Method = CruiseDAL.Schema.CruiseMethods.FIXCNT
            };
            database.Insert(stratum);

            var sg = new SampleGroupDO()
            {
                Code = "sgFixCnt",
                CutLeave = "C",
                UOM = "01",
                PrimaryProduct = "01",
                Stratum_CN = stratum.Stratum_CN
            };
            database.Insert(sg);

            var tdv = new TreeDefaultValueDO()
            {
                Species = "someSpecies",
                LiveDead = "L",
                PrimaryProduct = "01"
            };
            database.Insert(tdv);

            var sgTdv = new SampleGroupTreeDefaultValueDO()
            {
                SampleGroup_CN = sg.SampleGroup_CN,
                TreeDefaultValue_CN = tdv.TreeDefaultValue_CN
            };
            database.Insert(sgTdv);

            var fixCntTallyClass = new FixCNTTallyClassDO()
            {
                FieldName = (int)FixCNTTallyField.DBH,
                Stratum_CN = stratum.Stratum_CN
            };
            database.Insert(fixCntTallyClass);
            //database.Execute($"Update FixCNTTallyClass set FieldName = 'DBH';");

            var fixCntTallyPop = new FixCNTTallyPopulationDO()
            {
                FixCNTTallyClass_CN = fixCntTallyClass.FixCNTTallyClass_CN,
                SampleGroup_CN = sg.SampleGroup_CN,
                TreeDefaultValue_CN = tdv.TreeDefaultValue_CN,
                IntervalSize = 101,
                Min = 102,
                Max = 103
            };
            database.Insert(fixCntTallyPop);

            database.Insert(new PlotDO
            {
                CuttingUnit_CN = 1,
                Stratum_CN = 1,
                PlotNumber = 1
            });

            return database;
        }

        [Fact]
        public void Refresh_test()
        {
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var viewModel = new FixCNTViewModel((INavigationService)null,
                    new Services.CuttingUnitDatastoreProvider() { CuttingUnitDatastore = datastore });

                var navParams = new NavigationParameters($"{NavParams.UNIT}=u1&{NavParams.PLOT_NUMBER}=1&{NavParams.STRATUM}=fixCnt1");

                viewModel.OnNavigatedTo(navParams);

                viewModel.TallyPopulations.Should().NotBeEmpty();

                foreach (var tp in viewModel.TallyPopulations)
                {
                    tp.Buckets.Should().NotBeEmpty();
                    foreach(var b in tp.Buckets)
                    {
                        b.Tree.Should().NotBeNull();
                        b.Value.Should().NotBe(0.0);
                    }
                }
            }
        }

        [Fact]
        public void Tally()
        {
            using (var database = CreateDatabase())
            {
                var datastore = new CuttingUnitDatastore(database);

                var viewModel = new FixCNTViewModel((INavigationService)null,
                    new Services.CuttingUnitDatastoreProvider() { CuttingUnitDatastore = datastore });

                var navParams = new NavigationParameters($"{NavParams.UNIT}=u1&{NavParams.PLOT_NUMBER}=1&{NavParams.STRATUM}=fixCnt1");
                viewModel.OnNavigatedTo(navParams);

                var tallyPop = viewModel.TallyPopulations.First();

                viewModel.Tally(tallyPop.Species, tallyPop.IntervalMin + tallyPop.IntervalSize / 2);

                tallyPop.Buckets.First().Tree.TreeCount.Should().Be(1);
            }
        }
    }
}