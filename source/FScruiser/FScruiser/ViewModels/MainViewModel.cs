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
        public IEnumerable<CruiseViewModel> CruiseFiles => FindCruiseFiles();

        IEnumerable<CruiseViewModel> FindCruiseFiles()
        {
            foreach (var path in App.FolderService.CruiseFolders)
            {
                var fi = new FileInfo(path);
                yield return new CruiseViewModel(fi);
            }
        }
    }
}