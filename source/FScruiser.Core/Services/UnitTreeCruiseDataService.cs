using CruiseDAL;
using CruiseDAL.DataObjects;
using FScruiser.Logic;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FScruiser.Services
{
    public class CuttingUnitDataService : ICuttingUnitDataService
    {
        private IEnumerable<UnitStratum> _strata;

        protected DAL Datastore { get; set; }

        public CuttingUnit Unit { get; set; }

        public CuttingUnitDataService(string path)
        {
            if (path == null) { throw new ArgumentNullException(path); }

            Datastore = new DAL(path);
        }

        #region query methods

        public IEnumerable<UnitStratum> QueryStrataByUnitCode(string unitCode)
        {
            var query = Datastore.From<UnitStratum>()
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Join("Stratum", "USING (Stratum_CN)")
                .Where("CuttingUnit.Code = @p1");

            return query.Query(unitCode).ToList();
        }

        public IEnumerable<TreeFieldSetupDO> GetTreeFieldsByStratum(string stratumCode)
        {
            return Datastore.From<TreeFieldSetupDO>()
                .Join("Stratum", "USING (Stratum_CN)")
                .Where("Stratum.Code = @p1").Query(stratumCode);
        }

        public IEnumerable<SampleGroup> GetSampleGroupsByStratum(string stratumCode)
        {
            var sampleGroups = Datastore.From<SampleGroup>()
                .Join("Stratum", "USING (Stratum_CN)")
                .Where("Stratum.Code = @p1").Query(stratumCode).ToArray();

            foreach (var sg in sampleGroups)
            {
                sg.Sampler = SampleSelectorFactory.DeserializeSamplerState(sg);
            }

            return sampleGroups;
        }

        public IEnumerable<TreeDefaultValueDO> GetTreeDefaultsBySampleGroup(string sampleGroupCode)
        {
            return Datastore.From<TreeDefaultValueDO>()
                .Join("SampleGroup", "USING (SampleGroup_CN)")
                .Where("SampleGroup.Code = @p1").Query(sampleGroupCode);
        }

        public IEnumerable<TallyPopulation> GetTalliesByStratum(string stratumCode)
        {
            return Datastore.From<TallyPopulation>()
                .Join("Tally", "USING (Tally_CN)")
                .Join("SampleGroup", "USING (SampleGroup_CN)")
                .Join("Stratum", "USING (Stratum_CN)")
                .Where("Stratum.Code = @p1")
                .Query(stratumCode);
        }

        public TallyPopulation GetCount(long countCN)
        {
            return Datastore.From<TallyPopulation>()
                .Join("Tally", "USING (Tally_CN)")
                .Join("SampleGroup", "USING (SampleGroup_CN)")
                .Join("Stratum", "USING (Stratum_CN)")
                .Where("CountTree_CN = @p1")
                .Query(countCN).FirstOrDefault();
        }

        public Tree GetTree(long treeCN)
        {
            return Datastore.From<Tree>()
                .Where("Tree.Tree_CN = @p1")
                .Query(treeCN).FirstOrDefault();
        }

        public TreeEstimateDO GetTreeEstimate(long treeEstimateCN)
        {
            return Datastore.From<TreeEstimateDO>()
                .Where("TreeEstimate_CN = @p1")
                .Query(treeEstimateCN).FirstOrDefault();
        }

        #endregion query methods

        public TreeEstimateDO LogTreeEstimate(CountTreeDO count, int kpi)
        {
            var treeEstimate = new TreeEstimateDO
            {
                CountTree_CN = count.CountTree_CN,
                KPI = kpi
            };

            InsertTreeEstimate(treeEstimate);
            return treeEstimate;
        }

        //just a helper method, does this belong here?
        public Tree CreateTree(TallyPopulation population)
        {
            return new Tree()
            {
                TreeDefaultValue = population.TreeDefaultValue,
                SampleGroup = population.SampleGroup,
                Stratum = population.SampleGroup.Stratum,
                CuttingUnit = population.CuttingUnit
            };
        }

        public void UpdateTree(Tree tree)
        {
            if (tree.IsPersisted == false) { throw new InvalidOperationException("tree is not persisted before calling update"); }
            Datastore.Update(tree);
        }

        public void InsertTree(Tree tree)
        {
            if (tree.IsPersisted == true) { throw new InvalidOperationException("tree is persisted, should be calling update instead of insert"); }
            Datastore.Insert(tree);
        }

        public void InsertTreeEstimate(TreeEstimateDO treeEstimate)
        {
            if (treeEstimate.IsPersisted == true) { throw new InvalidOperationException("treeEstimate is persisted, should be calling update instead of insert"); }
            Datastore.Insert(treeEstimate);
        }

        public void UpdateCount(TallyPopulation tallyPopulation)
        {
            if(tallyPopulation.IsPersisted == false) { throw new InvalidOperationException("count is not persisted"); }
            Datastore.Update(tallyPopulation);
        }


    }
}