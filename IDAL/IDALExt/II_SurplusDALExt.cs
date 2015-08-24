using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.IDAL
{
    public partial interface II_SurplusDAL : IBaseDAL<I_Surplus>
    {
        /// <summary>
        /// 保存入库结余信息  同时记录入库流水
        /// </summary>
        /// <param name="info"></param>
        void SaveEntrySurplusInfo(I_EntryDetail info, string strEntryType, ref string errorMsg);

        /// <summary>
        /// 保存出库结余信息说  同时及记录出库流水
        /// </summary>
        /// <param name="info"></param>
        /// <param name="strDeliveryType"></param>
        /// <returns></returns>
        void SaveDeliverySurplusInfo(I_DeliveryDetail info, string strDeliveryType, ref string errorMsg);


        /// <summary>
        /// 获取剩余列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="materialID"></param>
        /// <param name="storageCode"></param>
        /// <returns></returns>
        object GetSurplusList(int page, int rows, ref int rowCounts, string materialID, List<int> listStorageCode, List<string> listMaterialType, int alarmCounts = -1, int overDays = -1,bool IsShowOver = true);



        /// <summary>
        /// 获取物资剩余分组列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="rowCounts"></param>
        /// <param name="materialID"></param>
        /// <param name="listStorageCode"></param>
        /// <param name="listMaterialType"></param>
        /// <returns></returns>
        object GetSurplusListGroupBy(int page, int rows, ref int rowCounts, string materialID, List<int> listStorageCode, List<string> listMaterialType);
    }
}
