
using Anke.SHManage.IDAL;
using Anke.SHManage.Model;
using Anke.SHManage.Model.ViewModel;
using Anke.SHManage.MSSQLDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{
    public partial class M_PatientChargeBLL : BaseBLL<M_PatientCharge>
    {
        /// <summary>
        /// Controller 调用此方法
        /// </summary>
        /// <param name="taskCode"></param>
        /// <param name="PatientOrder"></param>
        /// <returns></returns>
        public void getPatientCharge(string taskCode, string PatientOrder, out List<PatientChargeInfo> listYP, out List<PatientChargeInfo> listCL, out List<PatientChargeInfo> listJC, out List<PatientChargeInfo> listZL, out dynamic pinfo, out dynamic editinfo)
        {
            int pOrder = Convert.ToInt32(PatientOrder);

            //当状态为未收，PatientRecord取内容，只取PatientRecord中的Name，SendAddress,LocalAddress
            pinfo = new M_PatientRecordBLL().DALContext.IM_PatientRecordDAL.GetDynamicListWithOrderBy(p => p.TaskCode == taskCode && p.PatientOrder == pOrder, p => new { CallOrder = p.CallOrder, Name = p.Name, SendAddress = p.SendAddress, LocalAddress = p.LocalAddress }, p => p.TaskCode).FirstOrDefault();

            //当状态为已收，PatientCharge取内容供编辑
            editinfo = new M_PatientChargeBLL().DALContext.IM_PatientChargeDAL.GetDynamicListWithOrderBy(p => p.TaskCode == taskCode && p.PatientOrder == pOrder, p => new
            {
                PatientName = p.PatientName,
                AddressStart = p.AddressStart,
                AddressEnd = p.AddressEnd,
                InvoiceNumber = p.InvoiceNumber,
                OutStationRoadCode = p.OutStationRoadCode,
                PointRoadCode = p.PointRoadCode,
                ArriveHospitalRoadCode = p.ArriveHospitalRoadCode,
                ReturnStationRoadCode = p.ReturnStationRoadCode,
                OneWayKM = p.OneWayKM,
                ChargeKM = p.ChargeKM,
                CarFee = p.CarFee,
                WaitingFee = p.WaitingFee,
                EmergencyFee = p.EmergencyFee,
                DrugFeeTotal = p.DrugFeeTotal,
                ExamineFeeTotal = p.ExamineFeeTotal,
                ConsumableFeeTotal = p.ConsumableFeeTotal,
                MeasureFeeTotal = p.MeasureFeeTotal,
                PaidMoney = p.PaidMoney,
                ReceivableTotal = p.ReceivableTotal
            }, p => p.TaskCode).FirstOrDefault();

            List<PatientChargeInfo> listALL = new M_PatientChargeBLL().DALContext.IM_PatientChargeDAL.getPatientCharge(taskCode, PatientOrder).ToList();

            listYP = listALL.Where(p => p.ChargeType.Equals("药费")).ToList();   //药品
            PatientChargeInfo infoYP = new PatientChargeInfo();
            infoYP.Name = "合计";
            if (listYP.Count() > 0)
                infoYP.TotalPrice = listYP.Sum(p => p.TotalPrice);
            else
                infoYP.TotalPrice = 0;
            listYP.Add(infoYP);

            listCL = listALL.Where(p => p.ChargeType.Equals("材料费")).ToList();  //材料
            PatientChargeInfo infoCL = new PatientChargeInfo();
            infoCL.Name = "合计";
            if (listCL.Count() > 0)
                infoCL.TotalPrice = listCL.Sum(p => p.TotalPrice);
            else
                infoCL.TotalPrice = 0;
            listCL.Add(infoCL);

            listJC = listALL.Where(p => p.ChargeType.Equals("检查费")).ToList();  //检查
            PatientChargeInfo infoJC = new PatientChargeInfo();
            infoJC.Name = "合计";
            if (listJC.Count() > 0)
                infoJC.TotalPrice = listJC.Sum(p => p.TotalPrice);
            else
                infoJC.TotalPrice = 0;
            listJC.Add(infoJC);

            listZL = listALL.Where(p => p.ChargeType.Equals("治疗费")).ToList();  //治疗
            PatientChargeInfo infoZL = new PatientChargeInfo();
            infoZL.Name = "合计";
            if (listZL.Count() > 0)
                infoZL.TotalPrice = listZL.Sum(p => p.TotalPrice);
            else
                infoZL.TotalPrice = 0;
            listZL.Add(infoZL);


        }
    }
}
