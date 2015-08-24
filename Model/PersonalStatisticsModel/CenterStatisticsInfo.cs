using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
   public class CenterStatisticsInfo
    {
     
        private string m_CenterName = "";
        /// <summary>
        /// 分中心名称
        /// </summary>
        public string CenterName
        {
            get { return m_CenterName; }
            set { m_CenterName = value; }
        }
        
        private string m_CTaskTimes = "";
        /// <summary>
        /// 出车次数
        /// </summary>
        public string CTaskTimes
        {
            get { return m_CTaskTimes; }
            set { m_CTaskTimes = value; }
        }
       
        private string m_CFirstAidTimes = "";
        /// <summary>
        /// 急救次数
        /// </summary>

        public string CFirstAidTimes
        {
            get { return m_CFirstAidTimes; }
            set { m_CFirstAidTimes = value; }
        }

        private string m_CTransTimes = "";
        /// <summary>
        /// 转院次数
        /// </summary>
        public string CTransTimes
        {
            get { return m_CTransTimes; }
            set { m_CTransTimes = value; }
        }
     
        private string m_CGoHomeTimes = "";
        /// <summary>
        /// 回家次数
        /// </summary>
        public string CGoHomeTimes
        {
            get { return m_CGoHomeTimes; }
            set { m_CGoHomeTimes = value; }
        }

       
        private string m_CDays = "";
        ///
       ///当班数
        ///
        public string CDays
        {
            get { return m_CDays; }
            set { m_CDays = value; }
        }
     
        private string m_CTaskTimesEveryDay = "";
        /// <summary>
        /// 人均班出车次数
        /// </summary>
        public string CTaskTimesEveryDay
        {
            get { return m_CTaskTimesEveryDay; }
            set { m_CTaskTimesEveryDay = value; }
        }

        
        private string m_CAvgSendTime = "";
        /// <summary>
        /// 平均出车时间
        /// </summary>
        public string CAvgSendTime
        {
            get { return m_CAvgSendTime; }
            set { m_CAvgSendTime = value; }
        }

     
        private string m_CAvgArriveTime = "";
        /// <summary>
        /// 平均到达时间
        /// </summary>
        public string CAvgArriveTime
        {
            get { return m_CAvgArriveTime; }
            set { m_CAvgArriveTime = value; }
        }

      
        private string m_CAvgLocalAidTime = "";
        /// <summary>
        /// 平均现场抢救时间
        /// </summary>
        public string CAvgLocalAidTime
        {
            get { return m_CAvgLocalAidTime; }
            set { m_CAvgLocalAidTime = value; }
        }

       
        private string m_CAvgTransTime = "";
        /// <summary>
        /// 平均运送时间
        /// </summary>
        public string CAvgTransTime
        {
            get { return m_CAvgTransTime; }
            set { m_CAvgTransTime = value; }
        }

      
        private string m_CAvgHospitalTransTime = "";
        /// <summary>
        /// 平均医院交接时间
        /// </summary>
        public string CAvgHospitalTransTime
        {
            get { return m_CAvgHospitalTransTime; }
            set { m_CAvgHospitalTransTime = value; }
        }

        
        private string m_CAvgReactTime = "";
        /// <summary>
        /// 平均反应时间
        /// </summary>
        public string CAvgReactTime
        {
            get { return m_CAvgReactTime; }
            set { m_CAvgReactTime = value; }
        }

     
        private string m_CAvgTurnTime = "";
        /// <summary>
        /// 平均周转时间
        /// </summary>
        public string CAvgTurnTime
        {
            get { return m_CAvgTurnTime; }
            set { m_CAvgTurnTime = value; }
        }

     
        private string m_CDisposePercent = "";
        /// <summary>
        /// 急救处理率
        /// </summary>
        public string CDisposePercent
        {
            get { return m_CDisposePercent; }
            set { m_CDisposePercent = value; }
        }

       
        private string m_CDisposeNumber = "";
        /// <summary>
        /// 急救治疗数
        /// </summary>
        public string CDisposeNumber
        {
            get { return m_CDisposeNumber; }
            set { m_CDisposeNumber = value; }
        }

       
        private string m_CPRNumberExceptRefuseTreatment = "";
        /// <summary>
        /// 病历数除去拒绝治疗的
        /// </summary>
        public string CPRNumberExceptRefuseTreatment
        {
            get { return m_CPRNumberExceptRefuseTreatment; }
            set { m_CPRNumberExceptRefuseTreatment = value; }
        }

       
        private string m_CCharge = "";
        /// <summary>
        /// 收费金额
        /// </summary>
        public string CCharge
        {
            get { return m_CCharge; }
            set { m_CCharge = value; }
        }

       
        private string m_CKilometer = "";
        /// <summary>
        /// 行驶公里数
        /// </summary>
        public string CKilometer
        {
            get { return m_CKilometer; }
            set { m_CKilometer = value; }
        }

       
        private string m_CChargeCarFee = "";
        /// <summary>
        /// 车费
        /// </summary>
        public string CChargeCarFee
        {
            get { return m_CChargeCarFee; }
            set { m_CChargeCarFee = value; }
        }

       
        private string m_CChargeWaitFee = "";
        /// <summary>
        /// 等候费
        /// </summary>
        public string CChargeWaitFee
        {
            get { return m_CChargeWaitFee; }
            set { m_CChargeWaitFee = value; }
        }

     
        private string m_CChargeAidFee = "";
        /// <summary>
        /// 治疗费
        /// </summary>
        public string CChargeAidFee
        {
            get { return m_CChargeAidFee; }
            set { m_CChargeAidFee = value; }
        }

     
        private string m_CVeinPercent = "";
        /// <summary>
        /// 静脉开通率
        /// </summary>
        public string CVeinPercent
        {
            get { return m_CVeinPercent; }
            set { m_CVeinPercent = value; }
        }

      
        private string m_CVeinNumber = "";
        /// <summary>
        /// 静脉开通数
        /// </summary>
        public string CVeinNumber
        {
            get { return m_CVeinNumber; }
            set { m_CVeinNumber = value; }
        }

     
        private string m_CROSCPercent = "";
        /// <summary>
        /// 复苏成功率
        /// </summary>
        public string CROSCPercent
        {
            get { return m_CROSCPercent; }
            set { m_CROSCPercent = value; }
        }

       
        private string m_CROSCNumber = "";
        /// <summary>
        /// 复苏成功数
        /// </summary>
        public string CROSCNumber
        {
            get { return m_CROSCNumber; }
            set { m_CROSCNumber = value; }
        }

       
        private string m_CCPRNumber = "";
        /// <summary>
        /// 心肺复苏数
        /// </summary>
        public string CCPRNumber
        {
            get { return m_CCPRNumber; }
            set { m_CCPRNumber = value; }
        }
       
        private string m_CPRNumber = "";
        /// <summary>
        /// 病历数
        /// </summary>
        public string CPRNumber
        {
            get { return m_CPRNumber; }
            set { m_CPRNumber = value; }
        }
    }
}
