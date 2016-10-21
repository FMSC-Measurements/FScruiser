using Backpack;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.ViewModels
{
    public class UnitLevelTreeListViewModel : FreshMvvm.FreshBasePageModel
    {
        public DatastoreRedux Datastore { get; set; }

        public CuttingUnitModel Unit { get; set; }

        public IList<Tree> Trees { get; protected set; }

        public ICommand EditTreeCommand =>
            new Command<Tree>((x) => EditTree(x));

        public UnitLevelTreeListViewModel(DatastoreRedux dataStore)
        {
            Datastore = dataStore;
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            Unit = initData as CuttingUnitModel;

            Trees = Datastore.From<Tree>().Where($"CuttingUnit_CN = {Unit.CuttingUnit_CN}")
                .Read().ToList();
        }

        protected void EditTree(Tree tree)
        {
            CoreMethods.PushPageModel<TreeEditViewModel>(tree);
        }
    }
}