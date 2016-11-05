using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.ViewModels
{
    public class TreeEditViewModel : FreshMvvm.FreshBasePageModel
    {
        public ICuttingUnitDataService Dataservice { get; set; }

        public IEnumerable<TreeField> TreeFields { get; set; }

        public Tree Tree { get; set; }

        public TreeEditViewModel(ICuttingUnitDataService ds)
        {
            Dataservice = ds;
        }

        public override void Init(object initData)
        {
            base.Init(initData);
            var treeProxy = initData as TreeProxy;

            TreeFields = Dataservice.GetTreeFieldsByStratum(treeProxy.Stratum.Code);

            Tree = Dataservice.GetTree(treeProxy.Tree_GUID);
        }
    }
}