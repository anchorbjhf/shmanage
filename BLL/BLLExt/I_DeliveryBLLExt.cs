using Anke.SHManage.Model;
using Anke.SHManage.MSSQLDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{
    public partial class I_DeliveryBLL : BaseBLL<I_Delivery>
    {
        private I_DeliveryDAL dal = new I_DeliveryDAL();

        /// <summary>
        ///  出库操作
        /// </summary>
        /// <param name="storageCode">出库</param>
        /// <param name="BatchNo">批次号</param>
        /// <param name="counts">出库数量</param>
        /// <param name="errorMsg">错误消息</param>
        /// <returns></returns>
        public bool DeliveryOerate(I_Delivery deliveryInfo, List<I_DeliveryDetail> deliveryDetailInfos, ref string errorMsg)
        {
            int ind = 0;
            foreach (var item in deliveryDetailInfos)
            {
                ind++;
                item.DeliveryCode = deliveryInfo.DeliveryCode;
                item.DeliveryDetailCode = item.DeliveryCode + ind;
                item.DeliveryTime = deliveryInfo.DeliveryTime;
                item.OperatorCode = deliveryInfo.OperatorCode;
                item.TargetEntryDetailCode = item.DeliveryDetailCode;
            }

            return dal.DeliveryOperate(deliveryInfo, deliveryDetailInfos, ref errorMsg);
        }

        /// <summary>
        /// 出库单查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="rowCounts"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="DeliveryCode"></param>
        /// <param name="DeliveryType"></param>
        /// <param name="listSourceStorageIDs"></param>
        /// <param name="listTargetStorageIDs"></param>
        /// <returns></returns>
        public object GetDeliveryList(int page, int rows, ref int rowCounts, DateTime startTime, DateTime endTime, string DeliveryCode, string DeliveryType, List<int> listSourceStorageID, List<int> listTargetStorageID, List<string> listMaterialType, string MaterialID)
        {
            return dal.GetDeliveryList(page, rows, ref  rowCounts, startTime, endTime, DeliveryCode, DeliveryType, listSourceStorageID, listTargetStorageID, listMaterialType, MaterialID);
        }
    }
}
