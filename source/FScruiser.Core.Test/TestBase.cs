using System.Data.Common;
using System.Diagnostics;
using System.IO;
using Xunit.Abstractions;

namespace Xunit
{
    public class TestBase
    {
        protected ITestOutputHelper Output { get; private set; }
        protected DbProviderFactory DbProvider { get; private set; }
        protected Stopwatch _stopwatch;
        private string _testTempPath;

        public TestBase(ITestOutputHelper output)
        {
            Output = output;
            Output.WriteLine($"CodeBase: {System.Reflection.Assembly.GetExecutingAssembly().CodeBase}");
            var testTempPath = TestTempPath;
            if (!Directory.Exists(testTempPath))
            {
                Directory.CreateDirectory(testTempPath);
            }
        }

        public string TestTempPath
        {
            get
            {
                return _testTempPath ?? (_testTempPath = Path.Combine(Path.GetTempPath(), "TestTemp", this.GetType().FullName));
            }
        }

        public void StartTimer()
        {
            _stopwatch = new Stopwatch();
            Output.WriteLine("Stopwatch Started");
            _stopwatch.Start();
        }

        public void EndTimer()
        {
            _stopwatch.Stop();
            Output.WriteLine("Stopwatch Ended:" + _stopwatch.ElapsedMilliseconds.ToString() + "ms");
        }
    }
}