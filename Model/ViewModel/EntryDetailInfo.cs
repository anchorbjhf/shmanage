using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model.ViewModel
{
    public class EntryDetailInfo
    {
        public string MaterialName { get; set; }
        public int MaterialID { get; set; }
        public string BatchNo { get; set; }
        public string RealBatchNo { get; set; }
        public System.DateTime EntryDate { get; set; }
        public double EntryCounts { get; set; }
        public decimal TotalPrice { get; set; }
        public string ValidityDateStr { get; set; }
        public int StorageCode { get; set; }
        public string Storage { get; set; }
        public string Remark { get; set; }
        public string Specification { get; set; }
        public string Unit { get; set; }
        public string RelatedOrderNum { get; set; }
        public string RedEntryDetailCode { get; set; }
        public string EntryCode { get; set; }

    }
}
