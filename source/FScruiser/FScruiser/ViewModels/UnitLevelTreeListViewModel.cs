using Backpack;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.ViewModels
{
    public class UnitLevelTreeListViewModel : FreshMvvm.FreshBasePageModel
    {
        public DatastoreRedux Datastore { get; set; }

        public IList<Tree> Trees { get; protected set; }

        public UnitLevelTreeListViewModel(DatastoreRedux dataStore)
        {
            Datastore = dataStore;
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            var unit = initData as CuttingUnitModel;

            Trees = Datastore.From<Tree>().Where($"CuttingUnit_CN = {unit.CuttingUnit_CN}")
                .Read().ToList();
        }
    }
}