using FScruiser.XF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Cells
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TallyFeedCell : ViewCell
	{
		public TallyFeedCell ()
		{
			InitializeComponent ();

            var act = new Core.Legacy.Models.TallyAction();
            act.TreeRecord = new Models.Tree();
            act.Count = new Core.Legacy.Models.CountTree();
            act.Count.SampleGroup = new Models.SampleGroup() { Code = "SG" };
            act.Count.SampleGroup.Stratum = new Models.Stratum() { Code = "St" };

            var value = new TallyFeedEntry();
            value.TallyAction = act;

            BindingContext = value;
		}
	}
}