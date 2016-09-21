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

namespace FScruiser.Models
{
    public class CruiseModel
    {
        FileInfo _fileInfo;

        public CruiseModel(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        public string FileName => _fileInfo?.Name;

        public string Path => _fileInfo?.FullName;
    }
}