using FMSC.ORM.Core;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.ViewModels
{
    public class StratumTalliesViewModel : FreshMvvm.FreshBasePageModel
    {
        public DatastoreRedux Datastore { get; set; }

        public UnitStratum Stratum { get; set; }

        public IList<CountTree> Counts { get; set; }

        public StratumTalliesViewModel(DatastoreRedux datastore)
        {
            Datastore = datastore;
        }

        public override void Init(object initData)
        {
            Stratum = initData as UnitStratum;

            Counts = Datastore.From<CountTree>()
                .Where($"CuttingUnit_CN = {Stratum.CuttingUnit_CN} AND Stratum_CN = {Stratum.Stratum_CN}")
                .Read().ToList();

            base.Init(initData);
        }
    }
}