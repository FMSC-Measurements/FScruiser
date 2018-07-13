using CruiseDAL.DataObjects;
using FMSC.Sampling;
using FScruiser.Logic;
using FScruiser.Models;
using FScruiser.Validation;
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

        private Dictionary<string, SampleSelecter[]> _sampleSelectors = new Dictionary<string, SampleSelecter[]>();

        public CuttingUnit Unit { get; protected set; }

        #region ctor

        public CuttingUnitDataService(CuttingUnit unit)
        { Unit = unit ?? throw new ArgumentNullException(nameof(unit)); }

        public CuttingUnitDataService(string path, CuttingUnit unit)
            : this(new CuttingUnitDatastore(path)
                , unit)
        {
        }

        public CuttingUnitDataService(ICuttingUnitDatastore datastore, CuttingUnit unit) : this(unit)
        {
            Datastore = datastore ?? throw new ArgumentNullException(nameof(datastore));
        }

        #endregion ctor

        public IEnumerable<StratumProxy> GetStratumProxies()
        {
            return Datastore.GetStrataProxiesByUnitCode(Unit.Code);
        }

        public IEnumerable<TallyEntry> GetTallyEntries()
        {
            return Datastore.GetTallyEntriesByUnitCode(Unit.Code);
        }

        public IEnumerable<TallyPopulation> GetTallyPopulations()
        {
            return Datastore.GetTallyPopulationsByUnitCode(Unit.Code);
        }

        public IEnumerable<SampleGroupProxy> GetSampleGroupProxies(string stratumCode)
        {
            return Datastore.GetSampleGroupProxies(stratumCode);
        }

        public IEnumerable<SampleSelecter> GetSamplersBySampleGroupCode(string stratumCode, string sgCode)
        {
            var key = stratumCode + "/" + sgCode;

            if (_sampleSelectors.ContainsKey(key) == false)
            {
                var sampleGroup = Datastore.GetSampleGroup(stratumCode, sgCode);
                var samplers = new SampleSelecter[2];

                samplers[0] = SampleSelectorFactory.MakeSampleSelecter(sampleGroup);
                if (sampleGroup.Method == CruiseDAL.Schema.CruiseMethods.S3P)
                {
                    samplers[1] = SampleSelectorFactory.MakeSystematicSampleSelector(sampleGroup);
                }

                _sampleSelectors.Add(key, samplers);
            }

            return _sampleSelectors[key];
        }

        #region tree

        public Tree GetTree(string tree_guid)
        {
            return Datastore.GetTree(tree_guid);
        }

        public IEnumerable<Tree> GetTrees()
        {
            return Datastore.GetTreesByUnitCode(Unit.Code).ToList();
        }

        public TreeStub GetTreeStub(string tree_guid)
        {
            return Datastore.GetTreeStub(tree_guid);
        }

        public IEnumerable<TreeStub> GetTreeStubs()
        {
            return Datastore.GetTreeStubsByUnitCode(Unit.Code);
        }

        public string CreateTree(string stratumCode)
        {
            return Datastore.CreateTree(Unit.Code, stratumCode);
        }

        public string CreateTree(TallyPopulation population)
        {
            return Datastore.CreateTree(Unit.Code, population.StratumCode, population.SampleGroupCode, population.Species, population.LiveDead);
        }

        public void UpdateTree(Tree tree)
        {
            Datastore.UpdateTree(tree);
        }

        public void UpdateTreeInitials(string tree_guid, string value)
        {
            Datastore.UpdateTreeInitials(tree_guid, value);
        }

        public Task UpdateTreeAsync(Tree tree)
        {
            return Datastore.UpdateTreeAsync(tree);
        }

        #endregion tree

        public IEnumerable<TreeDefaultProxy> GetTreeDefaultProxies(string stratumCode, string sampleGroupCode)
        {
            return Datastore.GetTreeDefaultProxies(stratumCode, sampleGroupCode);
        }

        public TallyEntry CreateTally(TallyPopulation population, int treeCount = 1, int kpi = 0, bool stm = false)
        {
            var tallyEntry = new TallyEntry(Unit.Code, population)
            {
                CountOrMeasure = "C",
                TreeCount = treeCount,
                KPI = kpi,
                IsSTM = stm
            };

            Datastore.InsertTallyEntry(tallyEntry);

            return tallyEntry;
        }

        public TallyEntry CreateTallyWithTree(TallyPopulation population, string countOrMeasure, int treeCount = 1, int kpi = 0, bool stm = false)
        {
            var tallyEntry = new TallyEntry(Unit.Code, population)
            {
                CountOrMeasure = countOrMeasure,
                TreeCount = treeCount,
                KPI = kpi,
                IsSTM = stm,
                Tree_GUID = Guid.NewGuid().ToString()
            };

            Datastore.InsertTallyEntry(tallyEntry);

            return tallyEntry;
        }

        public void DeleteTally(TallyEntry tallyEntry)
        {
            Datastore.DeleteTally(tallyEntry);
        }

        public void LogMessage(string message, string level)
        {
            Datastore.LogMessage(message, level);
        }

        public IEnumerable<TreeFieldSetupDO> GetTreeFields()
        {
            var unitCode = Unit.Code;

            var fields = Datastore.GetTreeFieldsByUnitCode(unitCode).ToList();

            if (fields.Count == 0)
            {
                fields.AddRange(Constants.DEFAULT_TREE_FIELDS);
            }

            var strata = Datastore.GetStrataByUnitCode(unitCode);

            //if unit has multiple tree strata
            //but stratum column is missing
            if (strata.Count() > 1
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

            if (strata.Any(st => st.Is3P)
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

            var strata = Datastore.GetStrataByUnitCode(Unit.Code);

            //if unit has multiple tree strata
            //but stratum column is missing
            if (strata.Count() > 1
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

            var stratum = strata.Where(x => x.Code == stratumCode).Single();
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

        public IEnumerable<TreeAuditRule> GetTreeAuditRules(string stratum, string sampleGroup, string species, string livedead)
        {
            return Datastore.GetTreeAuditRules(stratum, sampleGroup, species, livedead);
        }

        public void UpdateTreeErrors(string tree_GUID, IEnumerable<ValidationError> errors)
        {
            Datastore.UpdateTreeErrors(tree_GUID, errors);
        }
    }
}