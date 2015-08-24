using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
    public partial class I_Surplus
    {
        public object ToExtModel()
        {
            return new
            {
                ID = this.ID,
                MaterialID = this.MaterialID,
                BatchNo = this.BatchNo,
                RealBatchNo = this.RealBatchNo,
                EntryCounts = this.EntryCounts,
                Surplus = this.Surplus,
                SurplusPrice = this.SurplusPrice,
                StorageCode = this.StorageCode,
                ValidityDate = this.ValidityDate,

                MaterialName = this.I_Material.Name, //翻译物资名称
                StorageName = this.I_Storage.Name //翻译仓库名称
            };
        }
    }
}
