using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anke.SHManage.IDAL;
using Anke.SHManage.Utility;
using Anke.SHManage.Model;

namespace Anke.SHManage.BLL
{
    public partial class M_PatientRecordBLL : BaseBLL<M_PatientRecord>
    {
        private readonly IM_PatientRecordDAL m_DAL = DataAccess.CreateM_PatientRecordDAL();
        private const string m_M_PatientRecordKey = "M_PatientRecordKey";
        private static object m_SyncRoot = new object();

        #region 出车信息--获取填写病历列表
        public object GetTasks(int page, int rows, string order, string sort, DateTime m_BeginTime, DateTime m_EndTime
           , string m_LinkPhone, int m_AlarmEventType, string m_LocalAddr, string m_TaskResult, int m_TaskAbendReason
           , string m_CenterCode, string m_StationCode, string m_AmbCode, string m_Driver, string m_Doctor, string m_Litter, string m_IsCharge
           , string m_IsFill, string m_Nurse, string m_PatientName, string searchBound, string isTest, M_UserLoginInfo loginInfo
           , string m_CPRIFSuccess, string m_PatientState)
        {
            return m_DAL.GetTasks(page, rows, order, sort, m_BeginTime, m_EndTime, m_LinkPhone, m_AlarmEventType, m_LocalAddr
               , m_TaskResult, m_TaskAbendReason, m_CenterCode, m_StationCode, m_AmbCode, m_Driver, m_Doctor, m_Litter, m_IsCharge, m_IsFill
               , m_Nurse, m_PatientName, searchBound, isTest, loginInfo, m_CPRIFSuccess, m_PatientState); //任务列表
        }
        #endregion

        #region 出车信息-根据任务编码查询病历列表
        public List<M_PatientRecord> GetPatientCommonByTask(string taskCode)
        {
            return m_DAL.GetPatientCommonByTask(taskCode);
        }
        #endregion

        #region 根据病种分类的名称串来获取主诉和现病史
        public object GetAllTemplate(string TemplateName)
        {
            return m_DAL.GetAllTemplate(TemplateName);
        }
        #endregion

        #region 根据任务编码获取调度信息
        public M_AttemperData GetAttemperData(string taskCode, string state)
        {
            return m_DAL.GetAttemperData(taskCode, state);
        }
        #endregion

        #region 获取病历最大序号
        public int GetPatientMaxOrder(string taskCode)
        {
            return m_DAL.GetPatientMaxOrder(taskCode);
        }
        #endregion

        #region 获取最大救治记录编码--暂时没用
        public string GetRescueRecordMaxCode(string taskCode, int patientOrder)
        {
            return m_DAL.GetRescueRecordMaxCode(taskCode, patientOrder);
        }
        #endregion

        #region 获取病历信息及子表
        public void GetPatientInfo(string taskCode, int patientOrder, out object info
            , out M_PatientRecordAppend pra, out M_PatientRecordCPR prCPR)
        {
            m_DAL.GetPatientInfo(taskCode, patientOrder, out info, out pra, out prCPR);
        }
        public List<M_PatientRecordRescue> GetPRRescueList(int page, int rows, ref int rowCounts, string taskCode, int patientOrder)
        {
           return  m_DAL.GetPRRescueList(page, rows, ref rowCounts, taskCode, patientOrder);
        }
        public void GetPRRescueInfo(string taskCode, int patientOrder, string rescueRecordCode, int disposeOrder
            , out M_PatientRecordRescue prrInfo)
        {
             m_DAL.GetPRRescueInfo(taskCode,  patientOrder,  rescueRecordCode,  disposeOrder, out  prrInfo);
        }
        //public void GetPatientRescueInfo(string taskCode, int patientOrder
        //    , out List<M_PatientRecordRescue> prrRescueList
        //    , out List<M_PatientRecordDrug> drugSCO)
        //{
        //    m_DAL.GetPatientRescueInfo(taskCode, patientOrder, out prrRescueList, out drugSCO);
        //}
        #endregion

        #region 新增病历
        /// <summary>
        /// 新增病历
        /// </summary>
        /// <param name="info"></param>
        /// <param name="pra"></param>
        /// <param name="prCPR"></param>
        /// <param name="prDiag"></param>
        /// <param name="prECG"></param>
        /// <param name="prrRescueList"></param>
        /// <param name="measureSCO"></param>
        /// <param name="drugSCO"></param>
        /// <param name="sanitationSCO"></param>
        /// <param name="lossDrugSCO"></param>
        /// <param name="lossSanitationSCO"></param>
        /// <returns></returns>
        public bool Insert(object info, M_PatientRecordAppend pra
            , M_PatientRecordCPR prCPR, List<M_PatientRecordDiag> prDiag
            , List<M_PatientRecordECGImpressions> prECG)
        {
            return m_DAL.Insert(info, pra, prCPR, prDiag, prECG);
        }
        public bool InsertPRRescue(M_PatientRecordRescue info , List<M_PatientRecordMeasure> measureSCO
            , List<M_PatientRecordDrug> drugSCO , List<M_PatientRecordSanitation> sanitationSCO
            , List<M_PatientRecordLossDrug> lossDrugSCO, List<M_PatientRecordLossSanitation> lossSanitationSCO)
        {
            return m_DAL.InsertPRRescue(info, measureSCO, drugSCO, sanitationSCO, lossDrugSCO, lossSanitationSCO);
        }
        #endregion

        #region 修改病历主表和附表
        public bool Update(object info, M_PatientRecordAppend pra
            , M_PatientRecordCPR prCPR, List<M_PatientRecordDiag> prDiag
            , List<M_PatientRecordECGImpressions> prECG)
        {
            return m_DAL.Update(info, pra, prCPR, prDiag, prECG);
        }
        #endregion

        #region 修改病历救治记录主表和子表
        public bool UpdatePRRescue(M_PatientRecordRescue prrInfo,List<M_PatientRecordMeasure> measureSCO
            , List<M_PatientRecordDrug> drugSCO, List<M_PatientRecordSanitation> sanitationSCO
            , List<M_PatientRecordLossDrug> lossDrugSCO, List<M_PatientRecordLossSanitation> lossSanitationSCO)
        {
            return m_DAL.UpdatePRRescue(prrInfo, measureSCO, drugSCO, sanitationSCO, lossDrugSCO, lossSanitationSCO);
        }
        #endregion

        #region 中心心肺复苏审核
        public bool UpdateAuditCPR(M_PatientRecordCPR info)
        {
            return m_DAL.UpdateAuditCPR(info);
        }
        #endregion
        
        #region 更新抽查病历
        public bool UpdateSpotChecks(M_PatientRecord info, int orderNumber)
        {
            return m_DAL.UpdateSpotChecks(info, orderNumber);
        }
        #endregion

        #region 更新医生回访
        public bool UpdateFollowUp(string TaskCode, int PatientOrder, string DoctorFollowUp)
        {
            return m_DAL.UpdateFollowUp(TaskCode, PatientOrder, DoctorFollowUp);
        }
        #endregion

        #region 删除病历主表和附表
        public bool Delete(string TaskCode, int PatientOrder)
        {
            return m_DAL.Delete(TaskCode, PatientOrder);
        }
        #endregion

        #region 删除救治记录主表和子表
        public bool DeletePRRescue(string TaskCode, int PatientOrder, string RescueRecordCode, int DisposeOrder)
        {
            return m_DAL.DeletePRRescue(TaskCode, PatientOrder, RescueRecordCode, DisposeOrder);
        }
        #endregion
        

    }
}
