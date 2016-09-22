using FMSC.ORM.Core;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.ViewModels
{
    public class DataEntryViewModel : FreshMvvm.FreshBasePageModel
    {
        public CuttingUnitModel Unit { get; protected set; }

        public IList<StratumModel> Strata { get; set; }

        public DatastoreRedux Datastore { get; set; }

        public DataEntryViewModel(DatastoreRedux dataStore)
        {
            Datastore = dataStore;
        }

        public override void Init(object initData)
        {
            Unit = initData as CuttingUnitModel;

            Strata = Datastore.From<StratumModel>()
                .Join("CuttingUnitStratum", "USING (Stratum_CN)")
                .Where("CuttingUnit_CN = ?1")
                .Read(Unit.CuttingUnit_CN).ToList();

            base.Init(initData);
        }

        public override void ReverseInit(object returnedData)
        {
            base.ReverseInit(returnedData);
        }
    }
}