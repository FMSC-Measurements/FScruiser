using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.ViewModels
{
    [EntitySource(SourceName = "CuttingUnit")]
    public class CuttingUnitModel
    {
        [Field(Name = "CuttingUnit_CN")]
        public long? CuttingUnit_CN { get; set; }

        [Field(Name = "Code")]
        public string CuttingUnitCode
        {
            get;
            set;
        }
    }

    public class CuttingUnitListViewModel : Xamarin.Forms.BindableObject
    {
        FMSC.ORM.Core.DatastoreRedux Datastore { get; set; }

        public string Test { get; set; } = "Test";

        public CuttingUnitListViewModel()
        {
            CuttingUnits = new CuttingUnitModel[]
            {
                new CuttingUnitModel() {CuttingUnitCode = "01" },
                new CuttingUnitModel() {CuttingUnitCode = "02" }
            };

            //CuttingUnits = datastore.From<CuttingUnitModel>().Query();
        }

        public IEnumerable<CuttingUnitModel> CuttingUnits { get; set; }
    }
}