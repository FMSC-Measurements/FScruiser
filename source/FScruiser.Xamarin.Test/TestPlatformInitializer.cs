using FScruiser.Services;
using Prism.Ioc;
using System;
using Xunit.Abstractions;

namespace FScruiser.XF
{
    public class TestPlatformInitializer : Prism.IPlatformInitializer
    {

        public Xunit.Abstractions.ITestOutputHelper TestOutput { get; set; }


        public TestPlatformInitializer(ITestOutputHelper testOutput)
        {
            TestOutput = testOutput ?? throw new ArgumentNullException(nameof(testOutput));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {


            containerRegistry.RegisterInstance<Prism.Logging.ILoggerFacade>(new TestLogger(TestOutput));
        }
    }
}