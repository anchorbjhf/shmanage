using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
    /// <summary>
    /// OverdueMaterial Model
    /// </summary>
   public class I_OverdueMaterial
    {
       /// <summary>
        /// 分类
       /// </summary>
       public string MTypeID { get; set; }
       /// <summary>
       /// 名称
       /// </summary>
       public string Name { get; set; }
       /// <summary>
       /// 代码
       /// </summary>
       public string MCode { get; set; }
       /// <summary>
       /// 有效期
       /// </summary>
       public string ValidDate { get; set; }
       /// <summary>
       /// 入库编码
       /// </summary>
       public string EntryCode { get; set; }
       /// <summary>
       /// 入库日期
       /// </summary>
       public string EntryData { get; set; }
       /// <summary>
       /// 生产厂家
       /// </summary>
       public string Manufacturer { get; set; }
       /// <summary>
       /// 供应商
       /// </summary>
       public string Vendor { get; set; }
       /// <summary>
       /// 度量单位
       /// </summary>
       public string Unit { get; set; }
       /// <summary>
       /// 规格
       /// </summary>
       public string Specification { get; set; }

    /// <summary>
       /// 所属仓库
    /// </summary>
       public string Storage { get; set; }
       /// <summary>
       /// 库存量
       /// </summary>
       public string Stocks { get; set; }



    }
}
