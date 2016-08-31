using Caliburn.Micro;
using FMSC.ORM.Core.SQL;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.ORM.SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.ViewModels
{
    [EntitySource(SourceName = "Names")]
    public class Names
    {
        [Field(Name = "Name")]
        public string Name { get; set; }
    }

    public class MainPageViewModel : PropertyChangedBase
    {
        string name;

        public MainPageViewModel()
        {
            var ds = new SQLiteDatastore();
            ds.CreateTable("Names",
                new ColumnInfo[] { new ColumnInfo("Name", "TEXT", false, false, null) }, false);

            ds.Insert(new Names() { Name = "ben" }, OnConflictOption.Fail);

            var name = ds.From<Names>().Query().FirstOrDefault();

            Name = name?.Name;
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyOfPropertyChange(() => Name);
                NotifyOfPropertyChange(() => CanSayHello);
            }
        }

        public bool CanSayHello
        {
            get { return !string.IsNullOrWhiteSpace(Name); }
        }

        public void SayHello(Page view)
        {
            view?.DisplayAlert("", string.Format("Hello {0}!", Name), "OK");
        }
    }
}