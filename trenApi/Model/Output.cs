using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace trenApi.Model
{
    public class Output
    {
        public bool RezervasyonYapilabilir { get; set; }
        public List<YerlesimAyrinti> YerlesimAyrinti { get; set; }  
    }
}
