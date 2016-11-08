using FluentAssertions;
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
        CuttingUnitDataService CreateDataService(long unit_CN = 2)
        {
            var unit = new CuttingUnit { CuttingUnit_CN = unit_CN };
            var cruiseFile = new CruiseFile(".//TestFiles//MultiTest.cruise");
            var dataService = new CuttingUnitDataService(unit, cruiseFile);

            var serviceProvider = dataService.GetInfrastructure<IServiceProvider>();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new MyLoggerProvider());

            return dataService;
        }

        [Fact]
        public void TestGetPlot()
        {
            var dataService = CreateDataService();

            var plot = dataService.GetPlot("04", 1);

            Assert.NotNull(plot);

            Assert.True(dataService.Plots.Count() > 0);
        }

        [Fact]
        public void TestGetUnitStrata()
        {
            var dataService = CreateDataService();

            var strata = dataService.GetAllUnitStrata().ToList();

            Assert.NotEmpty(strata);

            foreach (var unitStratum in strata)
            {
                ValidateUnitStratum(unitStratum);
            }
        }

        [Fact]
        public void TestGetUnitStratum()
        {
            var dataService = CreateDataService();

            var unitStratum = dataService.GetUnitStratum("04");

            ValidateUnitStratum(unitStratum);
        }

        void ValidateUnitStratum(UnitStratum unitStratum)
        {
            unitStratum.Stratum.Should().NotBeNull();
            if (unitStratum.Stratum.IsPlotStratum)
            {
                unitStratum.Plots.Should().NotBeEmpty();
                unitStratum.Plots[0].Stratum.Should().NotBeNull();
            }
        }

        [Fact]
        public void TestGetTallyPopulationByStratum()
        {
            var dataService = CreateDataService();

            var tallyPop = dataService.GetTallyPopulationByStratum("02");

            Assert.NotEmpty(tallyPop);

            foreach (var tp in tallyPop)
            {
                tp.Tally.Should().NotBeNull();
                tp.SampleGroup.Should().NotBeNull();
                tp.SampleGroup.Stratum.Should().NotBeNull();
            }
        }

        [Fact]
        public void TestGetTreeFieldsByStratum()
        {
            var dataService = CreateDataService();

            var treeFields = dataService.GetTreeFieldsByStratum("02");

            Assert.NotEmpty(treeFields);
        }
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