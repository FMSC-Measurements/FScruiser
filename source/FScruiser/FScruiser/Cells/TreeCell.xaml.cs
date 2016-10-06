using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FScruiser.Cells
{
    public partial class TreeCell : ViewCell
    {
        public TreeCell()
        {
            InitializeComponent();

            //LiveDead.Items.Add(String.Empty);
            //LiveDead.Items.Add("L");
            //LiveDead.Items.Add("D");
        }

        //protected override void OnBindingContextChanged()
        //{
        //    base.OnBindingContextChanged();

        //    var tree = BindingContext as Tree;
        //    if (tree != null)
        //    {
        //        foreach (var sp in tree.SpeciesOptions)
        //        { this.Species.Items.Add(sp.SpeciesCode); }

        //        foreach (var sg in tree.SampleGroupOptions)
        //        { this.SampleGroup.Items.Add(sg.SampleGroupCode); }

        //        LiveDead.SelectedIndex = LiveDead.Items.IndexOf(tree.LiveDead);
        //    }
        //    else
        //    {
        //        Species.Items.Clear();

        //        SampleGroup.Items.Clear();
        //    }
        //}
    }
}