using FScruiser.Models;
using FScruiser.Services;
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
        public ICuttingUnitDataService DataService { get; set; }

        public IList<Tree> Trees { get; protected set; }

        public ICommand EditTreeCommand =>
            new Command<TreeProxy>((x) => EditTree(x));

        public UnitLevelTreeListViewModel(ICuttingUnitDataService dataService)
        {
            DataService = dataService;
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            Trees = DataService.GetAllTrees().ToList();
        }

        protected void EditTree(TreeProxy tree)
        {
            CoreMethods.PushPageModel<TreeEditViewModel>(tree);
        }
    }
}