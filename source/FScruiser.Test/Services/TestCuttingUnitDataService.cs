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
            Assert.NotNull(samplers.Selector);
        }

        [Fact]
        public void TestGetStrata()
        {
            var dataStore = new SQLiteDatastore(".//TestFiles//MultiTest.cruise");

            var unit = dataStore.From<CuttingUnitModel>().Where("Code = '02'").Query().FirstOrDefault();

            Assert.NotNull(unit);

            var dataService = new CuttingUnitDataService(unit, dataStore);

            var strata = dataService.GetStrata().ToList();

            Assert.NotEmpty(strata);

            Assert.NotEmpty(dataService.Strata);
        }

        [Fact]
        public void TestGetTallyPopulationByStratum()
        {
            var dataStore = new SQLiteDatastore(".//TestFiles//MultiTest.cruise");

            var unit = dataStore.From<CuttingUnitModel>().Where("Code = '02'").Query().FirstOrDefault();

            Assert.NotNull(unit);

            var dataService = new CuttingUnitDataService(unit, dataStore);

            var tallyPop = dataService.GetTallyPopulationByStratum("02");

            Assert.NotEmpty(tallyPop);

            Assert.NotEmpty(dataService.TallyPopulations);
        }

        [Fact]
        public void TestGetTree()
        {
            var dataStore = new SQLiteDatastore(".//TestFiles//MultiTest.cruise");

            var unit = dataStore.From<CuttingUnitModel>().Where("Code = '02'").Query().FirstOrDefault();

            Assert.NotNull(unit);

            var dataService = new CuttingUnitDataService(unit, dataStore);

            var trees = dataService.GetTreeProxiesByStratum("02");

            foreach (var tree in trees)
            {
                var t = dataService.GetTree(tree.Tree_GUID);
                Assert.NotNull(t);
            }

            Assert.NotEmpty(dataService.Trees);
        }

        [Fact]
        public void TestGetTreeFieldsByStratum()
        {
            var dataStore = new SQLiteDatastore(".//TestFiles//MultiTest.cruise");

            var unit = dataStore.From<CuttingUnitModel>().Where("Code = '02'").Query().FirstOrDefault();

            Assert.NotNull(unit);

            var dataService = new CuttingUnitDataService(unit, dataStore);

            var treeFields = dataService.GetTreeFieldsByStratum("02");

            Assert.NotEmpty(treeFields);
        }

        [Fact]
        public void TestGetTreeProxiesByStratum()
        {
            var dataStore = new SQLiteDatastore(".//TestFiles//MultiTest.cruise");

            var unit = dataStore.From<CuttingUnitModel>().Where("Code = '02'").Query().FirstOrDefault();

            Assert.NotNull(unit);

            var dataService = new CuttingUnitDataService(unit, dataStore);

            var trees = dataService.GetTreeProxiesByStratum("02");

            Assert.NotEmpty(trees);

            Assert.NotEmpty(dataService.TreeProxies);
        }
    }
}