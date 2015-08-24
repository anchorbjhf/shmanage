using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Anke.SHManage.Model.ViewModel
{
    public class MaterialInfo
    {
        [DisplayName("ID")]
        public int ID { get; set; }

        [Required]
        [DisplayName("物资名称")]
        public string Name { get; set; }

        [Required]
        [DisplayName("分类")]
        public string MTypeID { get; set; }

        [DisplayName("生产厂家")]
        public string Manufacturer { get; set; }

        [DisplayName("供应商")]
        public string Vendor { get; set; }

        [DisplayName("度量单位")]
        public string Unit { get; set; }

        [DisplayName("规格")]
        public string Specification { get; set; }

        [DisplayName("二维码")]
        public string QRCode { get; set; }
        [MaxLength()]
        [DisplayName("备注")]
        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }
        [DisplayName("创建人")]
        public string CreatorName { get; set; }
        [DisplayName("创建日期")]
        public Nullable<System.DateTime> CreatorDate { get; set; }
        [DisplayName("拼音")]
        public string PinYin { get; set; }

        [Required]
        [DisplayName("是否有效?")]
        public bool IsActive { get; set; }

        [DisplayName("实际出手价格")]
        public Nullable<decimal> RealPrice { get; set; }

        [DisplayName("转库价格")]
        public Nullable<decimal> TransferPrice { get; set; }

        [Required]
        [DisplayName("警示基数")]
        public int AlarmCounts { get; set; }

        [DisplayName("物资代码")]
        public string MCode { get; set; }

        [DisplayName("其他类型")]
        public string OtherTypeID { get; set; }

        [DisplayName("收费刻度")]
        public int FeeScale { get; set; }

        [DisplayName("最大限制金额")]
        public int LimitMaxPrice { get; set; }

        [DisplayName("给药方式")]
        public string GiveMedicineWay { get; set; }

        [DisplayName("顺序号")]
        public int SN { get; set; }
    }
}
