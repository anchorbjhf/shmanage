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
    
    public partial class I_Material
    {
        public I_Material()
        {
            this.I_ApplyDetail = new HashSet<I_ApplyDetail>();
            this.I_DeliveryDetail = new HashSet<I_DeliveryDetail>();
            this.I_EntryDetail = new HashSet<I_EntryDetail>();
            this.I_InventoryRecord = new HashSet<I_InventoryRecord>();
            this.I_Surplus = new HashSet<I_Surplus>();
            this.I_Balance = new HashSet<I_Balance>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string MTypeID { get; set; }
        public string OtherTypeID { get; set; }
        public string Manufacturer { get; set; }
        public string Vendor { get; set; }
        public string Unit { get; set; }
        public string Specification { get; set; }
        public string QRCode { get; set; }
        public string Remark { get; set; }
        public string CreatorName { get; set; }
        public Nullable<System.DateTime> CreatorDate { get; set; }
        public string PinYin { get; set; }
        public bool IsActive { get; set; }
        public Nullable<decimal> RealPrice { get; set; }
        public int AlarmCounts { get; set; }
        public string MCode { get; set; }
        public Nullable<int> FeeScale { get; set; }
        public string GiveMedicineWay { get; set; }
        public Nullable<int> SN { get; set; }
        public Nullable<int> LimitMaxPrice { get; set; }
        public string Other1 { get; set; }
        public string Other2 { get; set; }
    
        public virtual ICollection<I_ApplyDetail> I_ApplyDetail { get; set; }
        public virtual ICollection<I_DeliveryDetail> I_DeliveryDetail { get; set; }
        public virtual ICollection<I_EntryDetail> I_EntryDetail { get; set; }
        public virtual ICollection<I_InventoryRecord> I_InventoryRecord { get; set; }
        public virtual TDictionary TDictionary { get; set; }
        public virtual TDictionary TDictionary1 { get; set; }
        public virtual TDictionary TDictionary2 { get; set; }
        public virtual ICollection<I_Surplus> I_Surplus { get; set; }
        public virtual ICollection<I_Balance> I_Balance { get; set; }
    }
}