using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model.ViewModel
{
    public class PatientChargeInfo
    {
        public string Name { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Counts { get; set; }
        public Nullable<int> ChargeCounts { get; set; }
        public string ChargeWay { get; set; }
        public Nullable<int> FeeScale { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public string ChargeType { get; set; }


    }
}
