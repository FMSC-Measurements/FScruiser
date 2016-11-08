using FScruiser.Models;
using FScruiser.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FScruiser.Test.Services
{
    public class TestCuttingUnitDataService
    {
        //[Fact]
        //public void TestGetAllTreeProxiesInUnit()
        //{
        //    var unit = new CuttingUnit { CuttingUnit_CN = 2 };
        //    var cruiseFile = new CruiseFile(".//TestFiles//MultiTest.cruise");
        //    var dataService = new CuttingUnitDataService(unit, cruiseFile);

        //    var trees = dataService.GetAllTreeProxiesInUnit().ToList();

        //    Assert.True(trees.Count > 0);
        //    Assert.True(dataService.TreeProxies.ToList().Count > 0);
        //}

        [Fact]
        public void TestGetPlot()
        {
            var unit = new CuttingUnit { CuttingUnit_CN = 2 };
            var cruiseFile = new CruiseFile(".//TestFiles//MultiTest.cruise");
            var dataService = new CuttingUnitDataService(unit, cruiseFile);

            var serviceProvider = dataService.GetInfrastructure<IServiceProvider>();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new MyLoggerProvider());

            var plot = dataService.GetPlot("04", 1);

            Assert.NotNull(plot);

            Assert.True(dataService.Plots.Count() > 0);
        }

        //[Fact]
        //public void TestGetSamplerBySampleGroup()
        //{
        //    var unit = new CuttingUnit { CuttingUnit_CN = 2 };
        //    var cruiseFile = new CruiseFile(".//TestFiles//MultiTest.cruise");
        //    var dataService = new CuttingUnitDataService(unit, cruiseFile);

        //    var samplers = dataService.GetSamplerBySampleGroup("02", "03");

        //    Assert.NotNull(samplers);
        //    Assert.NotNull(samplers.Selector);
        //}

        [Fact]
        public void TestGetStrata()
        {
            var unit = new CuttingUnit { CuttingUnit_CN = 2 };
            var cruiseFile = new CruiseFile(".//TestFiles//MultiTest.cruise");
            var dataService = new CuttingUnitDataService(unit, cruiseFile);

            var strata = dataService.GetStrata().ToList();

            Assert.NotEmpty(strata);

            Assert.NotEmpty(dataService.UnitStrata);
        }

        [Fact]
        public void TestGetTallyPopulationByStratum()
        {
            var unit = new CuttingUnit { CuttingUnit_CN = 2 };
            var cruiseFile = new CruiseFile(".//TestFiles//MultiTest.cruise");
            var dataService = new CuttingUnitDataService(unit, cruiseFile);

            var serviceProvider = dataService.GetInfrastructure<IServiceProvider>();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new MyLoggerProvider());

            var tallyPop = dataService.GetTallyPopulationByStratum("02");

            Assert.NotEmpty(tallyPop);

            Assert.NotEmpty(dataService.TallyPopulations);
        }

        //[Fact]
        //public void TestGetTree()
        //{
        //    var unit = new CuttingUnit { CuttingUnit_CN = 2 };
        //    var cruiseFile = new CruiseFile(".//TestFiles//MultiTest.cruise");
        //    var dataService = new CuttingUnitDataService(unit, cruiseFile);

        //    var trees = dataService.GetTreeProxiesByStratum("02");

        //    foreach (var tree in trees)
        //    {
        //        var t = dataService.GetTree(tree.Tree_GUID);
        //        Assert.NotNull(t);
        //    }

        //    Assert.NotEmpty(dataService.Trees);
        //}

        [Fact]
        public void TestGetTreeFieldsByStratum()
        {
            var unit = new CuttingUnit { CuttingUnit_CN = 2 };
            var cruiseFile = new CruiseFile(".//TestFiles//MultiTest.cruise");
            var dataService = new CuttingUnitDataService(unit, cruiseFile);

            var treeFields = dataService.GetTreeFieldsByStratum("02");

            Assert.NotEmpty(treeFields);
        }

        //[Fact]
        //public void TestGetTreeProxiesByStratum()
        //{
        //    var unit = new CuttingUnit { CuttingUnit_CN = 2 };
        //    var cruiseFile = new CruiseFile(".//TestFiles//MultiTest.cruise");
        //    var dataService = new CuttingUnitDataService(unit, cruiseFile);

        //    var trees = dataService.GetTreeProxiesByStratum("02");

        //    Assert.NotEmpty(trees);

        //    Assert.NotEmpty(dataService.TreeProxies);
        //}
    }

    public class MyLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new MyLogger();
        }

        public void Dispose()
        { }

        private class MyLogger : ILogger
        {
            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                File.AppendAllText(@"C:\temp\log.txt", formatter(state, exception));
                Console.WriteLine(formatter(state, exception));
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        }
    }
}