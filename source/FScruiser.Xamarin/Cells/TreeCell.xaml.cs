﻿using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.Cells
{
    public partial class TreeCell : ViewCell
    {
        #region Command

        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(TextCell), default(ICommand),
            propertyChanging: (bindable, oldvalue, newvalue) =>
            {
                var cell = (TreeCell)bindable;
                var oldcommand = (ICommand)oldvalue;
                if (oldcommand != null)
                    oldcommand.CanExecuteChanged -= cell.OnCommandCanExecuteChanged;
            }, propertyChanged: (bindable, oldvalue, newvalue) =>
            {
                var cell = (TreeCell)bindable;
                var newcommand = (ICommand)newvalue;
                if (newcommand != null)
                {
                    newcommand.CanExecuteChanged += cell.OnCommandCanExecuteChanged;
                    cell.OnCommandCanExecuteChanged(cell, EventArgs.Empty);
                }
            });

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            IsEnabled = Command?.CanExecute(CommandParameter) ?? false;
        }

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(TextCell), default(object),
            propertyChanged: (bindable, oldvalue, newvalue) =>
            {
                var cell = (TreeCell)bindable;
                cell.OnCommandCanExecuteChanged(cell, EventArgs.Empty);
            });

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        #endregion Command

        //private Tree _tree;

        //Tree Tree
        //{
        //    get
        //    {
        //        return _tree;
        //    }
        //    set
        //    {
        //        if (_tree == value) { return; }
        //        OnTreeChanging(_tree);
        //        _tree = value;
        //        OnTreeChanged(_tree);
        //    }
        //}

        //private void OnTreeChanged(Tree tree)
        //{
        //    if (tree != null)
        //    {
        //        tree.PropertyChanged += Tree_PropertyChanged;
        //    }
        //}

        //private void OnTreeChanging(Tree tree)
        //{
        //    if (tree != null)
        //    {
        //        tree.PropertyChanged -= Tree_PropertyChanged;
        //    }
        //}

        //private void Tree_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    switch (e.PropertyName)
        //    {
        //        case nameof(Tree.SampleGroup):
        //            {
        //                break;
        //            }
        //        case nameof(Tree.TreeDefaultValue):
        //            {
        //                break;
        //            }
        //    }
        //}

        public TreeCell()
        {
            InitializeComponent();
            base.Tapped += TreeCell_Tapped;
        }

        private void TreeCell_Tapped(object sender, EventArgs e)
        {
            Command?.Execute(CommandParameter);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }
    }
}