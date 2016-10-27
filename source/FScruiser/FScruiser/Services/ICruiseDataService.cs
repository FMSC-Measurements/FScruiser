﻿using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface ICruiseDataService
    {
        Sale Sale { get; }

        IEnumerable<CuttingUnitModel> Units { get; }

        IEnumerable<CuttingUnitModel> GetUnits();
    }
}