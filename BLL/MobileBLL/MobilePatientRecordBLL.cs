using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anke.SHManage.MSSQLDAL;
using Anke.SHManage.Utility;
using Anke.SHManage.Model;

namespace Anke.SHManage.BLL.MobileBLL
{
    public class MobilePatientRecordBLL
    {
        private M_PatientRecordDAL m_DAL = new M_PatientRecordDAL();
        public bool PADAddPatientRecord(string TaskCode, int PatientOrder, string EvenType, string TaskOrder, M_PatientRecord pr, M_PatientRecordAppend pra, string WorkCode, string PersonName, ref string errorMsg)
        {
            try
            {
                if (pr == null)
                    pr = new M_PatientRecord();
                if (pra == null)
                    pra = new M_PatientRecordAppend();
                
                if (TaskCode == "")
                {
                    TaskCode = m_DAL.GetTaskCodeByTaskOrder(TaskOrder);
                }
                if (TaskCode=="")
                {
                    errorMsg = "任务流水号错误！未在任务中找到该流水号。";
                    return false;
                }
                pr.TaskCode = TaskCode;
                pr.PatientOrder = PatientOrder;
                pr.AgentWorkID = WorkCode;
                pr.AgentName = PersonName;
                pr.PatientVersion = EvenType;
                pra.TaskCode = TaskCode;
                pra.PatientOrder = PatientOrder;
                object objpr = null;
                M_PatientRecordAppend mpra = null;
                M_PatientRecordCPR mprCPR = new M_PatientRecordCPR();;
                M_PatientRecord mpr = null;
                mprCPR.TaskCode = TaskCode;
                mprCPR.PatientOrder = PatientOrder;
                mprCPR.CenterIFAuditForXFFS = false;
                pr.MedicalRecordGenerationTime = DateTime.Now;
                pr.IMEI = "PAD";
                m_DAL.GetPatientInfo(TaskCode, PatientOrder, out objpr, out mpra);
                if (objpr != null)
                {
                    mpr = (M_PatientRecord)objpr;
                    ReflectionUtility.UpdateObjectProperty(mpr, pr);
                    ReflectionUtility.UpdateObjectProperty(mpra, pra);
                    m_DAL.Update(pr, pra);
                }
                else
                {
                    string HJTel = "";
                    string Area = "";
                    string EventType = "";
                    new MobileTaskDAL().GetMobileTaskInfo(TaskCode, out HJTel, out Area, out EventType);
                    pr.ForHelpTelephone = HJTel;
                    pr.OriginalTaskType = EventType;
                    pr.ForArea = Area;
                    pr.MedicalStateCode = 0;
                    pr.FormCompleteLogo = false;
                    pr.SubmitLogo = false;
                    pr.SubCenterIFSpotChecks = false;
                    pr.CenterIFSpotChecks = false;
                    pr.LastUpdatePerson = PersonName;
                    pr.LastUpdateTime = DateTime.Now;
                    m_DAL.Insert(pr, pra, mprCPR, null, null);
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }
        public void GetPatientInfo(string TaskCode, int PatientOrder, out M_PatientRecord info
            , out M_PatientRecordAppend pra)
        {
            info = null;
            object objpr = null;
            M_PatientRecordCPR mprCPR = null;
            m_DAL.GetPatientInfo(TaskCode, PatientOrder, out objpr, out pra, out mprCPR);
            if (objpr != null)
            {
                info = (M_PatientRecord)objpr;
            }
        }
    }
}
