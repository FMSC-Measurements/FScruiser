using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FixCntTallyPage : ContentPage
    {
        public FixCntTallyPage()
        {
            InitializeComponent();

            var viewMode = new ViewModels.FixCNTViewModel()
            {
                
                TallyPopulations = new Models.FixCntTallyPopulation[]
                {
                    new Models.FixCntTallyPopulation
                    {
                        FieldName = "DBH",
                        Buckets = new List<Models.FixCNTTallyBucket>()
                        {
                            new Models.FixCNTTallyBucket { Value = 1.0 },
                            new Models.FixCNTTallyBucket { Value = 2.0 },
                            new Models.FixCNTTallyBucket { Value = 3.0 },
                            new Models.FixCNTTallyBucket { Value = 4.0 },
                            new Models.FixCNTTallyBucket { Value = 5.0 },
                        }
                    },
                }
            };

            BindingContext = viewMode;

        }

    }
}