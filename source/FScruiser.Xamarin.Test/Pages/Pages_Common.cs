using FluentAssertions;
using FScruiser.XF.Test;
using System;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.XF.Pages
{
    public class Pages_Common : TestBase
    {
        public Pages_Common(ITestOutputHelper output) : base(output)
        {
        }

        /// <summary>
        /// verify that all page classes initialize and there are no silly mistakes in the xaml
        /// </summary>
        [Fact]
        public void InitializePages()
        {
            var assembly = Assembly.GetAssembly(typeof(MainPage));

            var pageTypes = assembly.GetTypes()
                .Where(t => t.Namespace == "FScruiser.XF.Pages" && t.IsSubclassOf(typeof(Xamarin.Forms.Page)));

            foreach (var t in pageTypes)
            {
                Output.WriteLine(t.Name);
                var page = Activator.CreateInstance(t) as Xamarin.Forms.Page;

                page.Should().NotBeNull();
            }
        }
    }
}