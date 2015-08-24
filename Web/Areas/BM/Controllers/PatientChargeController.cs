using Anke.SHManage.BLL;
using Anke.SHManage.Model;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.BM.Controllers
{
    [SkipLoginAttribute]
    public class PatientChargeController : Controller
    {
        /// <summary>
        /// 保存病历收费信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SavePatientCharge()
        {
            string chargeStr = Request.Form["charge"].ToString();
            M_PatientCharge charge = JsonHelper.GetJsonInfoBy<M_PatientCharge>(chargeStr);
            if (charge == null)
                return Json(new { result = "ERROR", msg = "解析实体为空！" }, "appliction/json", JsonRequestBehavior.AllowGet);

            M_PatientCharge info = new M_PatientChargeBLL().GetListBy(pc => pc.TaskCode.Equals(charge.TaskCode) && pc.PatientOrder == 1).FirstOrDefault();
            if (info != null)
            {
                if (new M_PatientChargeBLL().Del(info) <= 0)  // 删除旧收费信息
                {
                    return Json(new { result = "ERROR", msg = "删除旧收费信息失败！" }, "appliction/json", JsonRequestBehavior.AllowGet);
                }
            }

            //查询是否已经填写病历
            M_PatientRecord prinfo = new M_PatientRecordBLL().GetListBy(pr => pr.TaskCode.Equals(charge.TaskCode) && pr.PatientOrder == 1).FirstOrDefault();
            if (prinfo == null)
            {
                M_PatientRecordBLL bll = new M_PatientRecordBLL();
                M_PatientRecord pinfo = null;//病历主表信息
                M_PatientRecordAppend pra = null;//病历附表
                M_PatientRecordCPR prCPR = null;//病历附表--心肺复苏
                List<M_PatientRecordDiag> prDiag = null;//病历子表--初步印象
                List<M_PatientRecordECGImpressions> prECG = null;//病历子表--心电图印象
                GetPatientRecordInfo(charge.TaskCode,charge.ChargePerson, out pinfo, out pra, out prCPR);
                bll.Insert(pinfo, pra, prCPR, prDiag, prECG);//新增病历主表、附表、子表
            }

            charge.PatientOrder = 1;  //收费序号目前永远为1
            if (new M_PatientChargeBLL().Add(charge) > 0)
                return Json(new { result = "OK", msg = "收费成功！" }, "appliction/json", JsonRequestBehavior.AllowGet);
            else
                return Json(new { result = "ERROR", msg = "收费失败！" }, "appliction/json", JsonRequestBehavior.AllowGet);
        }

        //获取病历信息
        public void GetPatientRecordInfo(string TaskCode, string WorkID, out M_PatientRecord info, out M_PatientRecordAppend pra, out M_PatientRecordCPR prCPR)
        {
            M_PatientRecordBLL prBLL = new M_PatientRecordBLL();
            M_AttemperData result = prBLL.GetAttemperData(TaskCode,"");//根据任务编码获取调度信息

            info = new M_PatientRecord();//病历主表信息
            info.TaskCode = TaskCode;
            info.PatientOrder = 1;
            info.CallOrder = result.CallOrder;
            info.Name = result.Name;
            info.Sex = result.Sex;
            info.AgeType = "岁";
            info.ForHelpTelephone = result.ForHelpPhone;
            info.ContactTelephone = result.ContactTelephone;
            info.PatientVersion = result.AlarmType;
            info.OriginalTaskType = result.AlarmType;
            info.ForArea = result.Area;
            info.LocalAddress = result.LocalAddress;
            info.OutStationCode = result.StationCode;
            info.Station = result.Station;
            info.SendAddress = result.SendAddress;
            info.DrivingTime = result.DrivingTime;
            info.ArriveSceneTime = result.ArriveSceneTime;
            info.LeaveSceneTime = result.LeaveSceneTime;
            info.ArriveDestinationTime = result.ArriveDestinationTime;
            info.Driver = result.Driver;
            info.StretcherBearersI = result.StretcherBearers;
            info.DoctorAndNurse = result.Doctor + result.Nurse;
            P_UserBLL bll = new P_UserBLL();
            P_User user = bll.GetListBy(u => u.WorkCode == WorkID).Select(u => u.ToExtModle()).FirstOrDefault(); //查找用户名 密码
            info.AgentCode = user.ID.ToString();
            info.AgentWorkID = WorkID;
            info.AgentName = user.Name;
            info.BeginFillPatientTime = DateTime.Now;
            info.MedicalRecordGenerationTime = DateTime.Now;
            info.FormCompleteLogo = false;
            //obj.FormCompleteTime = null;
            info.ChargeOrder = 0;
            info.SubCenterIFSpotChecks = false;
            info.CenterIFSpotChecks = false;
            info.SubmitLogo = false;
            info.SubmitTime = null;
            info.MedicalStateCode = 0;
            info.LastUpdatePerson = user.Name;//填写病历的人员
            info.LastUpdateTime = DateTime.Now;
            info.CPRIFSuccess = "";//心肺复苏选择
            info.IFRefuseTreatment = "治疗记录";//是否拒绝治疗（救治记录）
            info.RescueType = "";//抢救类型(救治记录)
            info.IMEI = "PAD";//从PAD填写


            pra = new M_PatientRecordAppend();//
            pra.TaskCode = TaskCode;
            pra.PatientOrder = 1;

            prCPR = new M_PatientRecordCPR();//
            prCPR.TaskCode = TaskCode;
            prCPR.PatientOrder = 1;
            prCPR.CenterIFAuditForXFFS = false;
        }
    }
}
