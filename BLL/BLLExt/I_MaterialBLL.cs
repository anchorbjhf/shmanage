using Anke.SHManage.IDAL;
using Anke.SHManage.Model;
using Anke.SHManage.MSSQLDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{
    public partial class I_MaterialBLL : BaseBLL<I_Material>
    {
        private I_MaterialDAL dal = new I_MaterialDAL();
        /// <summary>
        /// 根据父级编码获取所有孩子的名称
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public IList<M_CheckModel> GetCheckBoxModelByparentID(string parentID)
        {
            return dal.GetCheckBoxModelByparentID(parentID);
        }

        /// <summary>
        /// 根据类型获取 物资流水   尤浩
        /// </summary>
        /// <param name="mtype"></param>
        /// <returns></returns>
        public List<I_MaterialExt> GetMaterialListBy(string mtype)
        {
            return dal.GetMaterialListBy(mtype);
        }

        /// <summary>
        /// 根据类型获取 措施流水   尤浩
        /// </summary>
        /// <param name="mtype"></param>
        /// <returns></returns>
        public List<I_MaterialExt> GetMeasureListBy(string measuretype)
        {
            return dal.GetMeasureListBy(measuretype);
        }

        /// <summary>
        /// 获取所有措施流水  尤浩
        /// </summary>
        /// <param name="mtype"></param>
        /// <returns></returns>
        public List<I_MaterialExt> GetMeasureList()
        {
            return dal.GetMeasureList();
        }

        /// <summary>
        /// 获取所有物资流水  尤浩
        /// </summary>
        /// <param name="mtype"></param>
        /// <returns></returns>
        public List<I_MaterialExt> GetMaterialList()
        {
            return dal.GetMaterialList();
        }

        /// <summary>
        /// 获取仓库的ID 和Name
        /// </summary>
        /// <returns></returns>
        public IList<CheckModelExt> GetStorage()
        {
            return dal.GetStorage();
        }
        /// <summary>
        /// 获取出库单
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="order"></param>
        /// <param name="sort"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="deliveryType"></param>
        /// <param name="deliveryCode"></param>
        /// <param name="entryStorageCode"></param>
        /// <param name="operatorName"></param>
        /// <param name="mName"></param>
        /// <param name="receivingStoreID"></param>
        /// <param name="consigneeName"></param>
        /// <param name="mCode"></param>
        /// <returns></returns>
        public object GetDeliveryOrder(int page, int rows, DateTime startTime, DateTime endTime, string deliveryType,
  string deliveryCode, string entryStorageCode, string operatorName, string mName, string receivingStoreID, string consigneeName, string mCode)
        {
            return dal.GetDeliveryOrder(page, rows, startTime, endTime, deliveryType,
        deliveryCode, entryStorageCode, operatorName, mName, receivingStoreID, consigneeName, mCode);
        }
        /// <summary>
        /// 物资基本信息查询  尤浩
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="vender"></param>
        /// <param name="mTypeId"></param>
        /// <param name="mName"></param>
        /// <param name="mCode"></param>
        /// <returns></returns>
        public object GetMaterialList(int page, int rows, DateTime startTime, DateTime endTime, string vender, string isActive, string mTypeId, string mName, string mCode)
        {
            return dal.GetMaterialList(page, rows, startTime, endTime, vender, isActive, mTypeId, mName, mCode);
        }
        public object GetMaterialList(int page, int rows, ref int rowCounts, string strIsActive, string measureType,string measureID)
        {
            return dal.GetMaterialList(page, rows, ref rowCounts, strIsActive, measureType, measureID);
        }

        /// <summary>
        /// 物资基本信息查询
        /// 单成
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="rowCounts"></param>
        /// <param name="manufacturer"></param>
        /// <param name="vender"></param>
        /// <param name="strIsActive"></param>
        /// <param name="listmTypeId"></param>
        /// <param name="mCode"></param>
        /// <returns></returns>
        public object GetMaterialList(int page, int rows, ref int rowCounts, string manufacturer, string vender, string strIsActive, List<string> listmTypeId, string mCode)
        {
            return dal.GetMaterialList(page, rows, ref rowCounts, manufacturer, vender, strIsActive, listmTypeId, mCode);
        }

        public object GetOverdue(int page, int rows, DateTime startTime, DateTime endTime, string mTypeId, string mName, int remainTime, string mCode)
        {
            return dal.GetOverdue(page, rows, startTime, endTime, mTypeId, mName, remainTime, mCode);
        }
    }
}
