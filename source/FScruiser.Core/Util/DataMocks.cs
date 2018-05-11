using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Util
{
    public static class DataMocks
    {
        public static IEnumerable<TallyFeedItem> TenTallyFeedItems
        {
            get
            {
                var count = new TallyPopulation()
                {
                    SampleGroupCode = "lg",
                    StratumCode = "01"
                    //,Species = "PP"
                };

                for(int i = 0; i< 10; i++)
                {
                    yield return new TallyFeedItem()
                    {
                        Count = count
                        //,Tree = new Tree() { HasFieldData = (i % 2 == 0) }
                    };

                }
            }

        }
    }
}
