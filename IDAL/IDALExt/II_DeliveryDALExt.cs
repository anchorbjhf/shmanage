using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.IDAL
{
    public partial interface II_DeliveryDAL : IBaseDAL<I_Delivery>
    {

        /// <summary>
        /// 出库操作
        /// </summary>
        /// <param name="deliveryInfo">出库主消息</param>
        /// <param name="deliveryDetailInfos">从表信息</param>
        /// <param name="errorMsg">错误消息</param>
        /// <returns></returns>
        bool DeliveryOperate(I_Delivery deliveryInfo, List<I_DeliveryDetail> deliveryDetailInfos, ref string errorMsg);
        

        /// <summary>
        /// 出库查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="rowCounts"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="DeliveryCode"></param>
        /// <param name="DeliveryType"></param>
        /// <param name="listSourceStorageID"></param>
        /// <param name="listTargetStorageID"></param>
        /// <param name="listMaterialType"></param>
        /// <param name="MaterialID"></param>
        /// <returns></returns>
        object GetDeliveryList(int page, int rows, ref int rowCounts, DateTime startTime, DateTime endTime, string DeliveryCode, string DeliveryType, List<int> listSourceStorageID, List<int> listTargetStorageID, List<string> listMaterialType, string MaterialID);
    }
}
