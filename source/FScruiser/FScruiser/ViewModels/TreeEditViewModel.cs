using Backpack;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.ViewModels
{
    public class TreeEditViewModel : FreshMvvm.FreshBasePageModel
    {
        public DatastoreRedux Datastore { get; set; }

        public IEnumerable<TreeField> TreeFields { get; set; }

        public Tree Tree { get; set; }

        public TreeEditViewModel(DatastoreRedux datastore)
        {
            Datastore = datastore;
        }

        public override void Init(object initData)
        {
            base.Init(initData);
            var treeProxy = initData as TreeProxy;

            TreeFields = Datastore.From<TreeField>()
                .Where($"Stratum_CN = {treeProxy.Stratum_CN}")
                .OrderBy("FieldOrder").Read().ToList();

            Tree = Datastore.From<Tree>().Where($"Tree_GUID = ?1").Read(treeProxy.Tree_GUID).FirstOrDefault();
        }
    }
}