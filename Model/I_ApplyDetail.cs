//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Anke.SHManage.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class I_ApplyDetail
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
    
        public virtual I_Storage I_Storage { get; set; }
        public virtual I_Storage I_Storage1 { get; set; }
        public virtual I_Storage I_Storage2 { get; set; }
        public virtual P_User P_User { get; set; }
        public virtual I_Apply I_Apply { get; set; }
        public virtual I_Material I_Material { get; set; }
    }
}
