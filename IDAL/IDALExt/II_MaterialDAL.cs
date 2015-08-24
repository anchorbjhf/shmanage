using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.IDAL
{
    public partial interface II_MaterialDAL : IBaseDAL<I_Material>
    {
        IList<M_CheckModel> GetCheckBoxModelByparentID(string parentID);

        IList<CheckModelExt> GetStorage();

        object GetDeliveryOrder(int page, int rows, DateTime startTime, DateTime endTime, string deliveryType,
      string deliveryCode, string entryStorageCode, string operatorName, string mName, string receivingStoreID, string consigneeName, string mCode);

        object GetMaterialList(int page, int rows, DateTime startTime, DateTime endTime, string vender, string isActive, string mTypeId, string mName, string mCode);

        object GetMaterialList(int page, int rows, ref int rowCounts, string manufacturer, string vender, string strIsActive, List<string> listmTypeId, string mCode);
        List<I_MaterialExt> GetMaterialListBy(string mtype);
    }

}
