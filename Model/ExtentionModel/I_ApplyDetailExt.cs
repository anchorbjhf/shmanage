using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{

    /// <summary>
    /// 申请从表
    /// </summary>
    public class I_ApplyDetailExt
    {
        public string ApplyDetailCode { get; set; }
        public string ApplyCode { get; set; }
        public int MaterialID { get; set; }
        public string RealBatchNo { get; set; }
        public string BatchNo { get; set; }
        public System.DateTime ApplyTime { get; set; }
        public double ApplyCounts { get; set; }
        public int ApplyUserID { get; set; }
        public int SelfStorageCode { get; set; }
        public int ApplyTargetStorageCode { get; set; }
        public string Remark { get; set; }
        public double ApprovalCounts { get; set; }
        public double Surplus { get; set; }
        public System.DateTime ValidityDate { get; set; }
        public string MaterialName { get; set; }
        public string ApplyUserName { get; set; }
        public string SelfStorageName { get; set; }
        public string ApplyTargetStorageName { get; set; }
    }
}
