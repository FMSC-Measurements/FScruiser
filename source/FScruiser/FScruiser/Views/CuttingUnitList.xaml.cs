﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FScruiser.Views
{
    public partial class CuttingUnitList : ContentPage
    {
        public CuttingUnitList()
        {
            InitializeComponent();
            BindingContext = App.Locator.CuttingUnitList;
        }
    }
}