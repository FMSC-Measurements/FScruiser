using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface IDataService
    {
        int SaveChanges();

        //Task<int> SaveChangesAsync();
    }
}