using Xunit.Abstractions;

namespace FScruiser.XF.Test
{
    public class TestBase
    {
        protected App App { get; }
        protected ITestOutputHelper Output { get; }

        protected Prism.Ioc.IContainerExtension Container => (Prism.Ioc.IContainerExtension)App.Container;

        public TestBase(ITestOutputHelper output)
        {
            Output = output;
            Xamarin.Forms.Mocks.MockForms.Init();

            App = new App(null);
        }
    }
}