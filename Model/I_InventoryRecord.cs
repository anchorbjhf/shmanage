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
    
    public partial class I_InventoryRecord
    {
        public int ID { get; set; }
        public int MaterialID { get; set; }
        public bool Is_Entry { get; set; }
        public string EntryDetailCode { get; set; }
        public string DeliveryDetailCode { get; set; }
        public System.DateTime OperatorDateTime { get; set; }
        public int StorageCode { get; set; }
        public double OriginalSurplus { get; set; }
        public double ChangeSurplus { get; set; }
        public double Surplus { get; set; }
        public decimal SurplusPrice { get; set; }
        public string BatchNo { get; set; }
        public string RealBatchNo { get; set; }
        public string DeliveryEntryType { get; set; }
        public int OperatorCode { get; set; }
    
        public virtual I_DeliveryDetail I_DeliveryDetail { get; set; }
        public virtual I_EntryDetail I_EntryDetail { get; set; }
        public virtual P_User P_User { get; set; }
        public virtual I_Storage I_Storage { get; set; }
        public virtual I_Material I_Material { get; set; }
    }
}