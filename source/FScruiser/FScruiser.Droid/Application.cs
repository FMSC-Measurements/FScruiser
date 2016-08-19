using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Caliburn.Micro;
using FScruiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FScruiser.Droid
{
    [Application]
    public class Application : CaliburnApplication
    {
        public Application(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            //register assemblies that our IoC will look for Views/ViewModels

            return new[]
            {
                GetType().Assembly,                 //executing assembly
                typeof(MainPageViewModel).Assembly  //ViewModel assembly
            };
        }

        private SimpleContainer container;

        public override void OnCreate()
        {
            base.OnCreate();

            Initialize();
        }

        protected override void Configure()
        {
            container = new SimpleContainer();
            container.Instance(container);

            container.Singleton<App>();
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }
    }
}