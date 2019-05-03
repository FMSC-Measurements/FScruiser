using FScruiser.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface ITreeDatastore
    {
        string CreateMeasureTree(string unitCode, string stratumCode,
            string sampleGroupCode = null, string species = null, string liveDead = "L",
            int treeCount = 1, int kpi = 0, bool stm = false);

        Tree GetTree(string treeID);

        IEnumerable<TreeFieldValue> GetTreeFieldValues(string treeID);

        void UpdateTree(Tree tree);

        void UpdateTree(Tree_Ex tree);

        void UpsertTreeMeasurments(TreeMeasurment mes);

        Task UpdateTreeAsync(Tree_Ex tree);

        void UpdateTreeFieldValue(TreeFieldValue treeFieldValue);

        void DeleteTree(string tree_guid);

        #region util

        IEnumerable<TreeError> GetTreeErrors(string treeID);

        bool IsTreeNumberAvalible(string unit, int treeNumber, int? plotNumber = null);

        void UpdateTreeInitials(string tree_guid, string value);

        #endregion util
    }
}