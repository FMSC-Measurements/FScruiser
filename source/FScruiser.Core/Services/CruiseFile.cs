using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public class CruiseFile
    {
        public CruiseFile()
        {
        }

        public CruiseFile(string path)
        {
            Path = path;
        }

        public string Path { get; set; }
    }
}