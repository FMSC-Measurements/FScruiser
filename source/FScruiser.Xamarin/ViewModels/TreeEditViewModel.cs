﻿using CruiseDAL.DataObjects;
using FScruiser.Models;
using FScruiser.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class TreeEditViewModel : ViewModelBase
    {
        private Tree _tree;
        private IEnumerable<TreeFieldSetupDO> _treeFields;

        public ICuttingUnitDataService Dataservice => ServiceService.CuttingUnitDataSercie;

        public IEnumerable<TreeFieldSetupDO> TreeFields
        {
            get
            {
                return _treeFields;
            }
            set
            {
                SetValue(ref _treeFields, value);
            }
        }

        public IEnumerable<SampleGroupDO> SampleGroups { get; protected set; }

        public IEnumerable<TreeDefaultValueDO> TreeDefaults { get; protected set; }

        public IEnumerable<string> SpeciesOptions { get; protected set; }

        public Tree Tree
        {
            get { return _tree; }
            set
            {
                SetValue(ref _tree, value);
            }
        }

        public TreeEditViewModel(Tree tree)
        {
            Tree = tree;
        }

        public override Task InitAsync()
        {
            var strata = Dataservice.Strata;
            var sampleGroups = Dataservice.SampleGroups;
            //var speciesOptions = Dataservice.

            var stratum = Tree.Stratum;
            TreeFields = Dataservice.TreeFields;
            return Task.CompletedTask;
        }
    }
}