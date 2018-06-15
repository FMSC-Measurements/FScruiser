using CruiseDAL.DataObjects;
using FScruiser.Core.Util;
using FScruiser.Logic;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public class CuttingUnitDataService : ICuttingUnitDataService
    {
        private object _dataLoadingLock = new Object();
        private bool _dataLoaded;

        protected ICuttingUnitDatastore Datastore { get; }

        private Dictionary<long, SampleGroup> _sampleGroups;
        private Dictionary<long, Stratum> _strata;
        private Dictionary<long, TreeDefaultValueDO> _treeDefaultValues;
        private Dictionary<string, IEnumerable<TreeDefaultValueDO>> _treeDefaultSampleGroupLookup = new Dictionary<string, IEnumerable<TreeDefaultValueDO>>();
        private Dictionary<int, CountTree> _counts;

        private TreeFieldSetupDO[] _treeFields;
        private IList<TallyEntry> _tallyFeed;

        public CuttingUnit Unit { get; protected set; }

        public IEnumerable<Stratum> Strata => _strata?.Values;
        public IEnumerable<TreeDefaultValueDO> TreeDefaultValues => _treeDefaultValues?.Values;
        public Dictionary<string, IEnumerable<TreeDefaultValueDO>> TreeDefaultSampleGroupLookup => _treeDefaultSampleGroupLookup;
        public IEnumerable<SampleGroup> SampleGroups => _sampleGroups?.Values;

        public IEnumerable<CountTree> Counts => _counts?.Values;

        public IEnumerable<TreeFieldSetupDO> TreeFields => _treeFields;
        public IEnumerable<TallyEntry> TallyFeed => _tallyFeed;

        public IEnumerable<TallyPopulation> TallyPopulations { get; set; }

        #region ctor

        public CuttingUnitDataService(CuttingUnit unit)
        { Unit = unit; }

        public CuttingUnitDataService(string path, CuttingUnit unit) : this(unit)
        {
            if (path == null) { throw new ArgumentNullException(path); }

            Datastore = new CuttingUnitDatastore(path);
        }

        public CuttingUnitDataService(ICuttingUnitDatastore datastore, CuttingUnit unit) : this(unit)
        {
            Datastore = datastore ?? throw new ArgumentNullException(nameof(datastore));
        }

        #endregion ctor

        public Task RefreshDataAsync(bool force = false)
        {
            return Task.Run(() => RefreshData(force));
        }

        public void RefreshData(bool force = false)
        {
            if (_dataLoaded == true) { return; }
            lock (_dataLoadingLock)
            {
                var unitCode = Unit.Code;

                _strata = Datastore.GetStrataByUnitCode(unitCode).ToDictionary(x => x.Stratum_CN.Value);

                _sampleGroups = Datastore.GetSampleGroupsByUnitCode(unitCode).ToDictionary(x => x.SampleGroup_CN.Value);

                _treeDefaultValues = Datastore.GetTreeDefaultsByUnitCode(unitCode).ToDictionary(x => x.TreeDefaultValue_CN.Value);

                foreach (var sg in SampleGroups)
                {
                    sg.Stratum = _strata.Where(x => x.Key == sg.Stratum_CN.Value).Single().Value;
                    sg.Sampler = SampleSelectorFactory.MakeSampleSelecter(sg);

                    var sgCode = sg.Code;
                    var treeDefaultMaps = Datastore.GetSampleGroupTreeDefaultMaps(sg.StratumCode, sgCode);

                    var treeDefaults = treeDefaultMaps.Select(x => _treeDefaultValues[x.TreeDefaultValue_CN.Value]).ToArray();

                    _treeDefaultSampleGroupLookup.Add(sg.Code, treeDefaults);
                }

                _counts = Datastore.GetCountTreeByUnitCode(unitCode).ToDictionary(x => x.CountTree_CN);
                TallyPopulations = Datastore.GetTallyPopulationsByUnitCode(unitCode);

                foreach (var tally in TallyPopulations)
                {
                    tally.Count = _counts[tally.CountTree_CN];
                    tally.SampleGroup = _sampleGroups[tally.SampleGroup_CN];
                }

                _treeFields = Datastore.GetTreeFieldsByUnitCode(unitCode).ToArray();

                _tallyFeed = Datastore.GetTallyEntriesByUnitCode(unitCode).ToObservableCollection();

                _dataLoaded = true;
            }
        }

        private void HydrateTree(Tree tree)
        {
            tree.CuttingUnit = Unit;
            tree.Stratum = _strata[tree.Stratum_CN];
            tree.SampleGroup = _sampleGroups.Where(x => tree.SampleGroup_CN.HasValue && x.Key == tree.SampleGroup_CN.Value)
                .Select(x => x.Value).SingleOrDefault();
            tree.TreeDefaultValue = _treeDefaultValues.Where(x => tree.TreeDefaultValue_CN.HasValue && x.Key == tree.TreeDefaultValue_CN.Value)
                .Select(x => x.Value).SingleOrDefault();
        }

        #region inflate tallyFeed item methods

        public TallyPopulation GetCount(long countCN)
        {
            return TallyPopulations.Where(x => x.CountTree_CN == countCN).SingleOrDefault();
        }

        public Tree GetTree(int treeNumber)
        {
            var tree = Datastore.GetTree(Unit.Code, treeNumber);
            HydrateTree(tree);
            return tree;
        }

        public IEnumerable<Tree> GetTrees()
        {
            var trees = Datastore.GetTreesByUnitCode(Unit.Code).ToList();

            foreach(var tree in trees)
            {
                HydrateTree(tree);
            }

            return trees;

        }

        public TreeEstimateDO GetTreeEstimate(long treeEstimateCN)
        {
            return Datastore.GetTreeEstimate(treeEstimateCN);
        }

        #endregion inflate tallyFeed item methods

        public TreeEstimateDO LogTreeEstimate(CountTreeDO count, int kpi)
        {
            var treeEstimate = new TreeEstimateDO
            {
                CountTree_CN = count.CountTree_CN,
                KPI = kpi
            };

            Datastore.InsertTreeEstimate(treeEstimate);
            return treeEstimate;
        }

        public int GetNextTreeNumber()
        {
            return Datastore.GetNextTreeNumber(Unit.Code);
        }

        public Tree CreateTree(string stratumCode)
        {
            var treeNumber = GetNextTreeNumber();
            var stratum = Strata.Where(x => x.Code == stratumCode).Single();

            var newTree = new Tree()
            {
                CuttingUnit_CN = Unit.CuttingUnit_CN.Value,
                TreeNumber = treeNumber,
                Stratum = stratum
            };

            return newTree;
        }

        //just a helper method, does this belong here?
        public Tree CreateTree(TallyPopulation population)
        {
            var count = population.Count;
            var treeNumber = GetNextTreeNumber();

            return new Tree()
            {
                TreeNumber = treeNumber,
                SampleGroup = population.SampleGroup,
                Stratum = population.SampleGroup.Stratum,
                CuttingUnit_CN = count.CuttingUnit_CN,
                TreeDefaultValue_CN = count.TreeDefaultValue_CN,
                Species = count.Species
            };
        }

        public void AddTallyEntry(TallyEntry tallyEntry)
        {
            var unitCode = tallyEntry.UnitCode;
            var stCode = tallyEntry.StratumCode;
            var sgCode = tallyEntry.SGCode;

            var count = Counts.Where(x => x.UnitCode == unitCode
                            && x.StratumCode == stCode
                            && x.SampleGroupCode == sgCode).Single();

            var treeCount = count.TreeCount;
            var sumKPI = count.SumKPI;

            count.TreeCount = treeCount + tallyEntry.TreeCount;
            count.SumKPI = sumKPI + tallyEntry.KPI;

            var tree = tallyEntry.Tree;

            if (tree != null)
            {
                Datastore.InsertTree(tree);
            }
            Datastore.UpdateCount(count);
            _tallyFeed.Add(tallyEntry);

            //TODO compleate implemtentation
        }

        public void AddTree(Tree tree)
        {
            Datastore.InsertTree(tree);
        }

        public void UpdateTree(Tree tree)
        {
            Datastore.UpdateTree(tree);
        }

        public Task UpdateTreeAsync(Tree tree)
        {
            return Datastore.UpdateTreeAsync(tree);
        }

        public void InsertTree(Tree tree)
        {
            Datastore.InsertTree(tree);
        }

        public void UpdateCount(CountTree tallyPopulation)
        {
            Datastore.UpdateCount(tallyPopulation);
        }

        public void LogMessage(string message, string level)
        {
            Datastore.LogMessage(message, level);
        }

        public IEnumerable<TreeFieldSetupDO> GetTreeFieldsByUnitCode(string unitCode)
        {
            var fields = Datastore.GetTreeFieldsByUnitCode(unitCode).ToList();

            if (fields.Count == 0)
            {
                fields.AddRange(Constants.DEFAULT_TREE_FIELDS);
            }

            //if unit has multiple tree strata
            //but stratum column is missing
            if (Strata.Count() > 1
                && !fields.Any(x => x.Field == "Stratum"))
            {
                //find the location of the tree number field
                int indexOfTreeNum = fields.FindIndex(x => x.Field == CruiseDAL.Schema.TREE.TREENUMBER);
                //if user doesn't have a tree number field, fall back to the last field index
                if (indexOfTreeNum == -1) { indexOfTreeNum = fields.Count - 1; }//last item index
                                                                                //add the stratum field to the filed list
                TreeFieldSetupDO tfs = new TreeFieldSetupDO() { Field = "Stratum", Heading = "St", Format = "[Code]" };
                fields.Insert(indexOfTreeNum + 1, tfs);
            }

            if (Strata.Any(st => st.Is3P)
                && !fields.Any(f => f.Field == "STM"))
            {
                fields.Add(new TreeFieldSetupDO() { Field = "STM", Heading = "STM" });
            }

            return fields;
        }

        public IEnumerable<TreeFieldSetupDO> GetTreeFieldsByStratumCode(string stratumCode)
        {
            var fields = Datastore.GetTreeFieldsByStratumCode(stratumCode).ToList();

            if (fields.Count == 0)
            {
                fields.AddRange(Constants.DEFAULT_TREE_FIELDS);
            }

            //if unit has multiple tree strata
            //but stratum column is missing
            if (Strata.Count() > 1
                && !fields.Any(x => x.Field == "Stratum"))
            {
                //find the location of the tree number field
                int indexOfTreeNum = fields.FindIndex(x => x.Field == CruiseDAL.Schema.TREE.TREENUMBER);
                //if user doesn't have a tree number field, fall back to the last field index
                if (indexOfTreeNum == -1) { indexOfTreeNum = fields.Count - 1; }//last item index
                                                                                //add the stratum field to the filed list
                TreeFieldSetupDO tfs = new TreeFieldSetupDO() { Field = "Stratum", Heading = "St", Format = "[Code]" };
                fields.Insert(indexOfTreeNum + 1, tfs);
            }

            var stratum = Strata.Where(x => x.Code == stratumCode).Single();
            if (stratum.Is3P)
            {
                fields.Add(new TreeFieldSetupDO() { Field = "STM", Heading = "STM" });
            }

            return fields;
        }

        public IEnumerable<TreeFieldSetupDO> GetSimplifiedTreeFieldsByStratumCode(string stratumCode)
        {
            var fields = Datastore.GetTreeFieldsByStratumCode(stratumCode);
            return fields.Where(x => Constants.LESS_IMPORTANT_TREE_FIELDS.Contains(x.Field) == false).ToArray();
        }
    }
}