using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{

    /// <summary>
    /// 入库明细
    /// </summary>
    public partial class I_EntryDetail
    {
        public object ToExtModel()
        {
            return new
            {
                EntryDetailCode = this.EntryDetailCode,
                EntryCode = this.EntryCode,
                MaterialID = this.MaterialID,
                BatchNo = this.BatchNo,
                RealBatchNo = this.RealBatchNo,
                EntryDate = this.EntryDate,
                EntryCounts = this.EntryCounts,
                TotalPrice = this.TotalPrice,
                ValidityDate = this.ValidityDate,
                OperatorCode = this.OperatorCode,
                StorageCode = this.StorageCode,
                Remark = this.Remark,
                Specification = this.Specification,
                Unit = this.Unit,
                RelatedOrderNum = this.RelatedOrderNum,
                RedEntryDetailCode = this.RedEntryDetailCode,

                StorageName = this.I_Storage.Name, //翻译仓库名称
                MaterialName= this.I_Material.Name //翻译物资名称
            };

        }
    }
}
