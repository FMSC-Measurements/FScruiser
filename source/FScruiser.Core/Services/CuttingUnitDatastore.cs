using CruiseDAL;
using CruiseDAL.DataObjects;
using CruiseDAL.Schema;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public class CuttingUnitDatastore : ICuttingUnitDatastore
    {
        public DAL Database { get; }

        public CuttingUnitDatastore(string path)
        {
            var database = new DAL(path);

            Database = database;
        }

        public CuttingUnitDatastore(DAL database)
        {
            Database = database;
        }

        public void DeleteTree(Tree tree)
        {
            Database.Delete(tree);
        }

        //public int GetHighestTreeNumberInUnit(string unit)
        //{
        //    throw new NotImplementedException();
        //}

        public IEnumerable<CountTree> GetCountTreeByUnitCode(string unitCode)
        {
            return Database.From<CountTree>()
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Join("SampleGroup", "USING (SampleGroup_CN)")
                .Join("Stratum", "USING (Stratum_CN)")
                .LeftJoin("TreeDefaultValue", "USING (TreeDefaultValue_CN)")
                .Where("CuttingUnit.Code = @p1")
                .Query(unitCode);
        }

        public IEnumerable<SampleGroup> GetSampleGroupsByUnitCode(string unitCode)
        {
            return Database.From<SampleGroup>()
                .Join("Stratum", "USING (Stratum_CN)")
                .Join("CuttingUnitStratum", "USING (Stratum_CN)")
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where("CuttingUnit.Code = @p1").Query(unitCode).ToArray();
        }

        public IEnumerable<Stratum> GetStrataByUnitCode(string unitCode)
        {
            return Database.From<Stratum>()
                .Join("CuttingUnitStratum", "USING (Stratum_CN)")
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where("CuttingUnit.Code = @p1")
                .Query(unitCode).ToArray();
        }

        public IEnumerable<TallyEntry> GetTallyEntriesByUnitCode(string unitCode)
        {
            //TODO implement method
            return Enumerable.Empty<TallyEntry>();
        }

        public TreeEstimateDO GetTreeEstimate(long treeEstimateCN)
        {
            return Database.From<TreeEstimateDO>()
                .Where("TreeEstimate_CN = @p1")
                .Query(treeEstimateCN).FirstOrDefault();
        }

        public IEnumerable<TallyPopulation> GetTallyPopulationsByUnitCode(string unitCode)
        {
            return Database.From<TallyPopulation>()
                .Join("Tally", "USING (Tally_CN)")
                .Join("SampleGroup", "USING (SampleGroup_CN)")
                .LeftJoin("TreeDefaultValue", "USING (TreeDefaultValue_CN)")
                .Join("Stratum", "USING (Stratum_CN)")
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where("CuttingUnit.Code = @p1")
                .Query(unitCode).ToArray();
        }

        public IEnumerable<TreeDefaultValueDO> GetTreeDefaultsByUnitCode(string unitCode)
        {
            return Database.From<TreeDefaultValueDO>()
                .Join("SampleGroupTreeDefaultValue", "USING (TreeDefaultValue_CN)")
                .Join("SampleGroup", "USING (SampleGroup_CN)")
                .Join("CuttingUnitStratum", "USING (Stratum_CN)")
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where("CuttingUnit.Code = @p1")
                .GroupBy("TreeDefaultValue_CN")
                .Query(unitCode).ToArray();
        }

        public IEnumerable<TreeDefaultValueDO> GetTreeDefaultsBySampleGroup(string sgCode)
        {
            return Database.From<TreeDefaultValueDO>()
                .Join("SampleGroupTreeDefaultValue", "USING (TreeDefaultValue_CN)")
                .Join("SampleGroup", "USING (SampleGroup_CN)")
                .Where("SampleGroup.Code = @p1")
                .Query(sgCode).ToArray();
        }

        public IEnumerable<SampleGroupTreeDefaultValueDO> GetSampleGroupTreeDefaultMaps(string stratumCode, string sgCode)
        {
            return Database.From<SampleGroupTreeDefaultValueDO>()
                .Join("SampleGroup", "USING (SampleGroup_CN)")
                .Join("Stratum", "USING (Stratum_CN)")
                .Where("Stratum.Code = @p1 AND SampleGroup.Code = @p2")
                .Query(stratumCode, sgCode).ToArray();
        }

        public IEnumerable<TreeFieldSetupDO> GetTreeFieldsByUnitCode(string unitCode)
        {
            return Database.From<TreeFieldSetupDO>()
                .Join("CuttingUnitStratum", "USING (Stratum_CN)")
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Join("Stratum", "USING (Stratum_CN)")
                .Where($"CuttingUnit.Code = @p1 AND Stratum.Method NOT IN ({string.Join(",", CruiseMethods.PLOT_METHODS.Select(s => "'" + s + "'").ToArray())})")
                .GroupBy("Field")
                .OrderBy("FieldOrder")
                .Query(unitCode).ToArray();
        }

        public IEnumerable<Tree> GetTreesByUnitCode(string unitCode)
        {
            return Database.From<Tree>()
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where("CuttingUnit.Code = @p1 AND Plot_CN IS NULL")
                .Query(unitCode).ToArray();
        }

        public void UpdateCount(CountTree count)
        {
            //if (count.IsPersisted == true) { throw new InvalidOperationException("counttree is persisted, should be calling update instead of insert"); }
            Database.Update(count);
        }

        public void InsertTree(Tree tree)
        {
            if (tree.IsPersisted == true) { throw new InvalidOperationException("tree is persisted, should be calling update instead of insert"); }
            Database.Insert(tree);
        }

        public Task InsertTreeAsync(Tree tree)
        {
            return Task.Run(() => InsertTree(tree));
        }

        public void InsertTreeEstimate(TreeEstimateDO treeEstimate)
        {
            if (treeEstimate.IsPersisted == true) { throw new InvalidOperationException("treeEstimate is persisted, should be calling update instead of insert"); }
            Database.Insert(treeEstimate);
        }

        public void UpdateTree(Tree tree)
        {
            if (tree.IsPersisted == false) { throw new InvalidOperationException("tree is not persisted before calling update"); }
            Database.Update(tree);
        }

        public Task UpdateTreeAsync(Tree tree)
        {
            return Task.Run(() => UpdateTree(tree));
        }

        public void LogMessage(string message, string level)
        {
            var msg = new MessageLogDO
            {
                Message = message,
                Level = level
            };

            Database.Insert(msg);
        }
    }
}