﻿using CruiseDAL.DataObjects;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface ICuttingUnitDatastore
    {

        IEnumerable<Stratum> GetStrataByUnitCode(string unitCode);

        IEnumerable<SampleGroup> GetSampleGroupsByUnitCode(string unitCode);

        IEnumerable<TallyPopulation> GetTallyPopulationsByUnitCode(string unitCode);

        IEnumerable<TreeFieldSetupDO> GetTreeFieldsByUnitCode(string unitCode);

        IEnumerable<TreeDefaultValueDO> GetTreeDefaultsByUnitCode(string unitCode);

        IEnumerable<TreeDefaultValueDO> GetTreeDefaultsBySampleGroup(string sgCode);

        IEnumerable<Tree> GetTreesByUnitCode(string unitCode);

        IEnumerable<CountTree> GetCountTreeByUnitCode(string unitCode);

        IEnumerable<TallyEntry> GetTallyEntriesByUnitCode(string unitCode);

        #region Tree
        //int GetHighestTreeNumberInUnit(string unit);        

        void InsertTree(Tree tree);

        Task InsertTreeAsync(Tree tree);

        void UpdateTree(Tree tree);

        Task UpdateTreeAsync(Tree tree);

        void DeleteTree(Tree tree);
        #endregion

        TreeEstimateDO GetTreeEstimate(long treeEstimateCN);

        void InsertTreeEstimate(TreeEstimateDO treeEstimate);

        void UpdateCount(CountTree count);

        void LogMessage(string message, string level);


    }
}