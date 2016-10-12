using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.Cells
{
    public partial class TreeCell : ViewCell
    {
        public static readonly BindableProperty EditCommandProperty = BindableProperty.Create(nameof(EditCommand), typeof(ICommand), typeof(TreeCell), null, propertyChanged: (bo, o, n) => ((TreeCell)bo).OnEditCommandChanged());
        public static readonly BindableProperty DeleteCommandProperty = BindableProperty.Create(nameof(DeleteCommand), typeof(ICommand), typeof(TreeCell), null);

        public TreeCell()
        {
            InitializeComponent();

            //LiveDead.Items.Add(String.Empty);
            //LiveDead.Items.Add("L");
            //LiveDead.Items.Add("D");
        }

        public ICommand EditCommand
        {
            get { return Edit.Command; }
            set { Edit.Command = value; }
        }

        public ICommand DeleteCommand
        {
            get { return Delete.Command; }
            set { Delete.Command = value; }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            Edit.CommandParameter = BindingContext;
            Delete.CommandParameter = BindingContext;
        }

        protected void OnEditCommandChanged()
        {
            Edit.Command = EditCommand;
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