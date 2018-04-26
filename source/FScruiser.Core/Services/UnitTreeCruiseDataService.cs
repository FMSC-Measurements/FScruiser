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
        public DAL Datastore { get; set; }

        public CuttingUnit Unit { get; protected set; }

        public IEnumerable<StratumDO> Strata { get; protected set; }
        public Dictionary<long?, TreeDefaultValueDO> TreeDefaultValues { get; private set; }
        public IEnumerable<SampleGroup> SampleGroups { get; protected set; }

        public IEnumerable<TallyPopulation> TallyPopulations { get; protected set; }

        //public IList<Tree> Trees { get; set; }

        public CuttingUnitDataService(CuttingUnit unit)
        { Unit = unit; }

        public CuttingUnitDataService(string path, CuttingUnit unit) : this(unit)
        {
            if (path == null) { throw new ArgumentNullException(path); }

            Datastore = new DAL(path);
        }

        public void RefreshData(bool force = false)
        {
            Strata = QueryStrataByUnitCode(Unit.Code).ToArray();

            var tdv = Datastore.From<TreeDefaultValueDO>()
                .Query().ToArray();

            TreeDefaultValues = tdv.ToDictionary(x => x.TreeDefaultValue_CN);

            SampleGroups = GetSampleGroupsByUnitCode(Unit.Code).ToArray();

            foreach (var sg in SampleGroups)
            {
                sg.Stratum = Strata.Where(x => x.Stratum_CN == sg.Stratum_CN).Single();
                sg.Sampler = SampleSelectorFactory.MakeSampleSelecter(sg);
            }

            TallyPopulations = GetTallyPopulationsByUnitCode(Unit.Code).ToArray();

            foreach (var tally in TallyPopulations)
            {
                tally.CuttingUnit = Unit;
                if (tally.TreeDefaultValue_CN != null)
                {
                    tally.TreeDefaultValue = TreeDefaultValues[tally.TreeDefaultValue_CN];
                }
                tally.SampleGroup = SampleGroups.Where(x => tally.SampleGroup_CN == x.SampleGroup_CN).Single();
            }

            //Trees
        }

        #region query methods

        public IEnumerable<StratumDO> QueryStrataByUnitCode(string unitCode)
        {
            var query = Datastore.From<StratumDO>()
                .Join("CuttingUnitStratum", "USING (Stratum_CN)")
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where("CuttingUnit.Code = @p1");

            return query.Query(unitCode).ToList();
        }

        //public IEnumerable<TreeFieldSetupDO> GetTreeFieldsByStratum(string stratumCode)
        //{
        //    return Datastore.From<TreeFieldSetupDO>()
        //        .Join("Stratum", "USING (Stratum_CN)")
        //        .Where("Stratum.Code = @p1").Query(stratumCode);
        //}

        //public IEnumerable<TreeDefaultValueDO> GetTreeDefaultsBySampleGroup(string sampleGroupCode)
        //{
        //    return Datastore.From<TreeDefaultValueDO>()
        //        .Join("SampleGroup", "USING (SampleGroup_CN)")
        //        .Where("SampleGroup.Code = @p1").Query(sampleGroupCode);
        //}

        public IEnumerable<SampleGroup> GetSampleGroupsByUnitCode(string unitCode)
        {
            return Datastore.From<SampleGroup>()
                .Join("CuttingUnitStratum", "USING (Stratum_CN)")
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where("CuttingUnit.Code = @p1").Query(unitCode).ToArray();
        }

        public IEnumerable<TallyPopulation> GetTallyPopulationsByUnitCode(string unitCode)
        {
            return Datastore.From<TallyPopulation>()
                .Join("Tally", "USING (Tally_CN)")
                .Join("SampleGroup", "USING (SampleGroup_CN)")
                .Join("Stratum", "USING (Stratum_CN)")
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where("CuttingUnit.Code = @p1")
                .Query(unitCode);
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
            if (tallyPopulation.IsPersisted == false) { throw new InvalidOperationException("count is not persisted"); }
            Datastore.Update(tallyPopulation);
        }
    }
}