using Anke.SHManage.Model;
using Anke.SHManage.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.IDAL
{
    public partial interface IM_PatientChargeDAL : IBaseDAL<M_PatientCharge>
    {

        /// <summary>
        /// 获取病人收费信息
        /// </summary>
        /// <param name="taskCode"></param>
        /// <param name="PatientOrder"></param>
        /// <returns></returns>
        IEnumerable<PatientChargeInfo> getPatientCharge(string taskCode, string PatientOrder);
    }
}
