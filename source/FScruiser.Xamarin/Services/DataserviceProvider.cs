using CruiseDAL;
using FScruiser.Data;
using FScruiser.Services;
using System;
using Xamarin.Forms;

namespace FScruiser.XF.Services
{
    public interface IDataserviceProvider
    {
        string CruisePath { get; set; }

        TResult Get<TResult>() where TResult : class;

        object Get(Type type);

        //void Register<T>(T instance);
    }

    public class DataserviceProvider : IDataserviceProvider
    {
        private string _cruisePath;
        ICruisersDataservice _cruisersDataservice;

        public DataserviceProvider(Application application)
        {
            Application = application ?? throw new ArgumentNullException(nameof(application));

        }

        public Xamarin.Forms.Application Application { get; }

        public ICuttingUnitDatastore CuttingUnitDatastore { get; set; }
        public ISampleSelectorDataService SampleSelectorDataService { get; set; }

        public ICruisersDataservice CruisersDataService { get; set; }

        public CruiseDatastore_V3 CruiseDatastore { get; set; }

        public string CruisePath
        {
            get => _cruisePath;
            set
            {
                if (value != null)
                {
                    var datastore = new CruiseDatastore_V3(value);
                    CruiseDatastore = datastore;
                    CuttingUnitDatastore = new CuttingUnitDatastore(datastore);
                    SampleSelectorDataService = new SampleSelectorRepository(CuttingUnitDatastore);
                }
                _cruisePath = value;
            }
        }

        public TResult Get<TResult>() where TResult : class
        {
            return (TResult)Get(typeof(TResult));
        }

        public object Get(Type type)
        {
            if (type.IsAssignableFrom(typeof(ICuttingUnitDatastore)))
            { return CuttingUnitDatastore; }
            if (type.IsAssignableFrom(typeof(ISampleSelectorDataService)))
            { return SampleSelectorDataService; }
            if (type.IsAssignableFrom(typeof(ICruisersDataservice)))
            { return _cruisersDataservice ?? (_cruisersDataservice = new CruisersDataservice(Application)); }
            if (type.IsAssignableFrom(typeof(ISaleDataservice)))
            {
                return new SaleDataservice(CruisePath);
            }
            if (type.IsAssignableFrom(typeof(IFixCNTDataservice)))
            {
                return new FixCNTDataservice(CruisePath);
            }
            else
            { return null; }
        }

        //public void Register<T>(T instance)
        //{
        //    var type = typeof(T);

        //    if (type.IsAssignableFrom(typeof(ICuttingUnitDatastore)))
        //    { CuttingUnitDatastore = (ICuttingUnitDatastore)instance; }

        //    if (type.IsAssignableFrom(typeof(ISampleSelectorDataService)))
        //    { SampleSelectorDataService = (ISampleSelectorDataService)instance; }
        //}
    }
}