using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public class FileGroup
    {
        public string GroupName { get; set; }

        public IEnumerable<FileInfo> CruiseFiles { get; set; }

        public override string ToString()
        {
            return GroupName;
        }
    }


    public interface ICruiseFileService
    {
        IEnumerable<FileGroup> CruiseFilesGroups { get; }
    }
}
