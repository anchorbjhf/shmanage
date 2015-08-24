using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Anke.SHManage.Model
{
    public class I_MaterialExt
    {
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
        public Nullable<int> LimitMaxPrice { get; set; }
        public Nullable<int> SN { get; set; }

        /// <summary>
        ///  单位
        /// </summary>
        public string UnitName { get; set; }
    }




    public partial class I_Material
    {
        /// <summary>
        /// 转换成pocos 实体  纯净的I_Material 没有导航属性,避免了自动生成有外键关系的Model
        /// </summary>
        /// <returns></returns>
        public I_Material ToPOCO()
        {
            I_Material poco = new I_Material()
            {
                ID = this.ID,
                Name = this.Name,
                MTypeID = this.MTypeID,
                Manufacturer = this.Manufacturer,
                Vendor = this.Vendor,
                Unit = this.Unit,
                Specification = this.Specification,
                QRCode = this.QRCode,
                Remark = this.Remark,
                CreatorName = this.CreatorName,
                CreatorDate = this.CreatorDate,
                PinYin = this.PinYin,
                IsActive = this.IsActive,
                RealPrice = this.RealPrice,
                AlarmCounts = this.AlarmCounts,
                MCode = this.MCode,
                OtherTypeID = this.OtherTypeID,
                GiveMedicineWay = this.GiveMedicineWay,
                LimitMaxPrice = this.LimitMaxPrice,
                SN = this.SN,
                FeeScale = this.FeeScale

            };
            return poco;
        }
        public ViewModel.MaterialInfo ToViewModel()
        {
            ViewModel.MaterialInfo vModel = new ViewModel.MaterialInfo()
            {

                ID = this.ID,
                Name = this.Name,
                MTypeID = this.MTypeID,
                Manufacturer = this.Manufacturer,
                Vendor = this.Vendor,
                Unit = this.Unit,
                Specification = this.Specification,
                QRCode = this.QRCode,
                Remark = this.Remark,
                CreatorName = this.CreatorName,
                CreatorDate = this.CreatorDate,
                PinYin = this.PinYin,
                IsActive = this.IsActive,
                RealPrice = this.RealPrice,
                AlarmCounts = this.AlarmCounts,
                MCode = this.MCode,
                OtherTypeID = this.OtherTypeID,
                GiveMedicineWay = this.GiveMedicineWay,
                LimitMaxPrice = Convert.ToInt32(this.LimitMaxPrice),
                SN = Convert.ToInt32(this.SN),
                FeeScale = Convert.ToInt32(this.FeeScale)
            };

            return vModel;
        }
      
    }
}
