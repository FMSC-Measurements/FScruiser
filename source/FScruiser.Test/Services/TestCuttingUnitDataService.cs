using Backpack;
using Backpack.SQLite;
using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FScruiser.Test.Services
{
    public class TestCuttingUnitDataService
    {
        [Fact]
        public void TestGetAllTreeProxiesInUnit()
        {
            var dataStore = new SQLiteDatastore(".//TestFiles//MultiTest.cruise");

            var unit = dataStore.From<CuttingUnitModel>().Query().FirstOrDefault();

            Assert.NotNull(unit);

            var dataService = new CuttingUnitDataService(unit, dataStore);

            var trees = dataService.GetAllTreeProxiesInUnit().ToList();

            Assert.True(trees.Count > 0);
            Assert.True(dataService.TreeProxies.ToList().Count > 0);
        }

        [Fact]
        public void TestGetPlot()
        {
            var dataStore = new SQLiteDatastore(".//TestFiles//MultiTest.cruise");

            var unit = dataStore.From<CuttingUnitModel>().Where("Code = '02'").Query().FirstOrDefault();

            Assert.NotNull(unit);

            var dataService = new CuttingUnitDataService(unit, dataStore);

            var plot = dataService.GetPlot("04", 1);

            Assert.NotNull(plot);

            Assert.True(dataService.Plots.Count() > 0);
        }

        [Fact]
        public void TestGetPlotProxiesByStratum()
        {
            var dataStore = new SQLiteDatastore(".//TestFiles//MultiTest.cruise");

            var unit = dataStore.From<CuttingUnitModel>().Where("Code = '02'").Query().FirstOrDefault();

            Assert.NotNull(unit);

            var dataService = new CuttingUnitDataService(unit, dataStore);

            var plots = dataService.GetPlotProxiesByStratum("04").ToList();

            Assert.True(plots.Count > 0);
        }

        [Fact]
        public void TestGetSamplerBySampleGroup()
        {
            var dataStore = new SQLiteDatastore(".//TestFiles//MultiTest.cruise");

            var unit = dataStore.From<CuttingUnitModel>().Where("Code = '02'").Query().FirstOrDefault();

            Assert.NotNull(unit);

            var dataService = new CuttingUnitDataService(unit, dataStore);

            var samplers = dataService.GetSamplerBySampleGroup("02", "03");

            Assert.NotNull(samplers);
        }
    }
}