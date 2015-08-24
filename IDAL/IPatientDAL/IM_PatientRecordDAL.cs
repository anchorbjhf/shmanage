using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anke.SHManage.Model;

namespace Anke.SHManage.IDAL
{
    public partial interface IM_PatientRecordDAL
    {
        object GetTasks(int page, int rows, string order, string sort, DateTime m_BeginTime, DateTime m_EndTime
           , string m_LinkPhone, int m_AlarmEventType, string m_LocalAddr, string m_TaskResult, int m_TaskAbendReason
           , string m_CenterCode, string m_StationCode, string m_AmbCode, string m_Driver, string m_Doctor, string m_Litter, string m_IsCharge
           , string m_IsFill, string m_Nurse, string m_PatientName, string searchBound, string isTest, M_UserLoginInfo loginInfo
           , string m_CPRIFSuccess, string m_PatientState);

        List<M_PatientRecord> GetPatientCommonByTask(string taskCode);

        object GetAllTemplate(string TemplateName);

        M_AttemperData GetAttemperData(string taskCode, string state);

        int GetPatientMaxOrder(string taskCode);
        string GetRescueRecordMaxCode(string taskCode, int patientOrder);//--暂时没用

        void GetPatientInfo(string taskCode, int patientOrder, out object info
            , out M_PatientRecordAppend pra, out M_PatientRecordCPR prCPR);
        void GetPRRescueInfo(string taskCode, int patientOrder, string rescueRecordCode, int disposeOrder
            , out M_PatientRecordRescue prrInfo);
        //void GetPatientRescueInfo(string taskCode, int patientOrder
        //    , out List<M_PatientRecordRescue> prrRescueList
        //    , out List<M_PatientRecordDrug> drugSCO);
        List<M_PatientRecordRescue> GetPRRescueList(int page, int rows, ref int rowCounts, string taskCode, int patientOrder);

        bool Insert(object info, M_PatientRecordAppend pra
            , M_PatientRecordCPR prCPR, List<M_PatientRecordDiag> prDiag
            , List<M_PatientRecordECGImpressions> prECG);
        bool InsertPRRescue(M_PatientRecordRescue info, List<M_PatientRecordMeasure> measureSCO
            , List<M_PatientRecordDrug> drugSCO, List<M_PatientRecordSanitation> sanitationSCO
            , List<M_PatientRecordLossDrug> lossDrugSCO, List<M_PatientRecordLossSanitation> lossSanitationSCO);

        bool Update(object info, M_PatientRecordAppend pra
            , M_PatientRecordCPR prCPR, List<M_PatientRecordDiag> prDiag
            , List<M_PatientRecordECGImpressions> prECG);
        bool UpdatePRRescue(M_PatientRecordRescue prrInfo, List<M_PatientRecordMeasure> measureSCO
            , List<M_PatientRecordDrug> drugSCO, List<M_PatientRecordSanitation> sanitationSCO
            , List<M_PatientRecordLossDrug> lossDrugSCO, List<M_PatientRecordLossSanitation> lossSanitationSCO);
        bool UpdateAuditCPR(M_PatientRecordCPR info);
        bool UpdateSpotChecks(M_PatientRecord info, int orderNumber);
        bool UpdateFollowUp(string TaskCode, int PatientOrder, string DoctorFollowUp);
        bool Delete(string TaskCode, int PatientOrder);
        bool DeletePRRescue(string TaskCode, int PatientOrder, string RescueRecordCode, int DisposeOrder);

        string GetTaskCodeByTaskOrder(string TaskOrder);
    }
}
