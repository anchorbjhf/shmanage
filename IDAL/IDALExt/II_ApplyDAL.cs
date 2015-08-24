using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.IDAL
{
    public partial interface II_ApplyDAL : IBaseDAL<I_Apply>
    {
        bool ApplyOperate(I_Apply apply, List<I_ApplyDetail> applyDetail, ref string errorMsg);
        object GetApplyList(int page, int rows, ref int rowCounts, DateTime startTime, DateTime endTime, string ApplyUserID, string ApplyType);
        List<I_ApplyDetailExt> getApplyDetailListBy(string strApplyCode);
        bool ApproveApplyOperate(I_Apply apply, List<I_ApplyDetail> applyDetail, I_Delivery deliveryInfo, List<I_DeliveryDetail> deliveryDetailInfos, ref string errorMsg);
    }
}
