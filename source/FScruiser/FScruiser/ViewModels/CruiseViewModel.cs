using FMSC.ORM.Core;
using FMSC.ORM.SQLite;
using FScruiser.Models;
using FScruiser.ViewModels;
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
    public class CruiseViewModel : FreshMvvm.FreshBasePageModel
    {
        IList<CuttingUnitModel> _cuttingUnits;
        FileInfo _fileInfo;

        public CruiseViewModel(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        public IList<CuttingUnitModel> CuttingUnits
        {
            get
            {
                if (_cuttingUnits == null)
                {
                    _cuttingUnits = Datastore?.From<CuttingUnitModel>().Read().ToList();
                }
                return _cuttingUnits;
            }
            set
            {
                if (_cuttingUnits == value) { return; }
                _cuttingUnits = value;
                RaisePropertyChanged();
            }
        }

        public DatastoreRedux Datastore { get; protected set; }
        public string FileName => _fileInfo?.Name;

        public string Path => _fileInfo?.FullName;

        public ICommand ShowCruiseCommand =>
                            new Command<CruiseViewModel>(
                x => { ShowCruise(); },
                x =>
                {
                    return _fileInfo.Exists && _fileInfo.Extension.ToLower() == ".cruise";
                });

        void ShowCruise()
        {
            Datastore = new SQLiteDatastore(this.Path);
            FreshMvvm.FreshIOC.Container.Register<DatastoreRedux>(Datastore);

            CoreMethods.PushPageModel<CruiseViewModel>();
        }
    }
}