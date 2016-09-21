using FMSC.ORM.Core;
using FMSC.ORM.SQLite;
using FScruiser.Models;
using FScruiser.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.ViewModels
{
    public class MainViewModel : FreshMvvm.FreshBasePageModel
    {
        public IEnumerable<CruiseModel> CruiseFiles => FindCruiseFiles();

        IEnumerable<CruiseModel> FindCruiseFiles()
        {
            foreach (var path in App.FolderService.CruiseFolders)
            {
                var fi = new FileInfo(path);
                yield return new CruiseModel(fi);
            }
        }

        public ICommand ShowCruiseCommand =>
                            new Command<CruiseModel>(
                file => { ShowCruise(file); });

        void ShowCruise(CruiseModel cruise)
        {
            var datastore = new SQLiteDatastore(cruise.Path);
            FreshMvvm.FreshIOC.Container.Register<DatastoreRedux>(datastore);

            Task t = CoreMethods.PushPageModel<CruiseViewModel>(cruise);
            var ex = t.Exception;
        }
    }
}