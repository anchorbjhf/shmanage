using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
  public  class StationStatisticsInfo
    {
     
      
        private string m_StationName = "";
        /// <summary>
        /// 分站名
        /// </summary>
        public string StationName
        {
            get { return m_StationName; }
            set { m_StationName = value; }
        }

      
        private string m_STaskTimes = "";
        /// <summary>
        /// 出车次数
        /// </summary>
        public string STaskTimes
        {
            get { return m_STaskTimes; }
            set { m_STaskTimes = value; }
        }
     
        private string m_SFirstAidTimes = "";
        /// <summary>
        /// 急救次数
        /// </summary>
        public string SFirstAidTimes
        {
            get { return m_SFirstAidTimes; }
            set { m_SFirstAidTimes = value; }
        }
      
        private string m_STransTimes = "";
        /// <summary>
        /// 转院次数
        /// </summary>
        public string STransTimes
        {
            get { return m_STransTimes; }
            set { m_STransTimes = value; }
        }
      
        private string m_SGoHomeTimes = "";
        /// <summary>
        /// 回家次数
        /// </summary>
        public string SGoHomeTimes
        {
            get { return m_SGoHomeTimes; }
            set { m_SGoHomeTimes = value; }
        }
     

        private string m_SDays = "";
        /// <summary>
        /// 当班数
        /// </summary>
        public string SDays
        {
            get { return m_SDays; }
            set { m_SDays = value; }
        }
      
        private string m_STaskTimesEveryDay = "";
        /// <summary>
        /// 人均班出车次数
        /// </summary>
        public string STaskTimesEveryDay
        {
            get { return m_STaskTimesEveryDay; }
            set { m_STaskTimesEveryDay = value; }
        }

    
        private string m_SAvgSendTime = "";
        /// <summary>
        /// 平均出车时间
        /// </summary>
        public string SAvgSendTime
        {
            get { return m_SAvgSendTime; }
            set { m_SAvgSendTime = value; }
        }

      
        private string m_SAvgArriveTime = "";
        /// <summary>
        /// 平均到达时间
        /// </summary>
        public string SAvgArriveTime
        {
            get { return m_SAvgArriveTime; }
            set { m_SAvgArriveTime = value; }
        }

     
        private string m_SAvgLocalAidTime = "";
        /// <summary>
        /// 平均现场抢救时间
        /// </summary>
        public string SAvgLocalAidTime
        {
            get { return m_SAvgLocalAidTime; }
            set { m_SAvgLocalAidTime = value; }
        }

      
        private string m_SAvgTransTime = "";
        /// <summary>
        /// 平均运送时间
        /// </summary>
        public string SAvgTransTime
        {
            get { return m_SAvgTransTime; }
            set { m_SAvgTransTime = value; }
        }

      
        private string m_SAvgHospitalTransTime = "";
        /// <summary>
        /// 平均医院交接时间
        /// </summary>
        public string SAvgHospitalTransTime
        {
            get { return m_SAvgHospitalTransTime; }
            set { m_SAvgHospitalTransTime = value; }
        }

       
        private string m_SAvgReactTime = "";
        /// <summary>
        /// 平均反应时间
        /// </summary>
        public string SAvgReactTime
        {
            get { return m_SAvgReactTime; }
            set { m_SAvgReactTime = value; }
        }

     
        private string m_SAvgTurnTime = "";
        /// <summary>
        /// 平均周转时间
        /// </summary>
        public string SAvgTurnTime
        {
            get { return m_SAvgTurnTime; }
            set { m_SAvgTurnTime = value; }
        }

     
        private string m_SDisposePercent = "";
        /// <summary>
        /// 急救处理率
        /// </summary>
        public string SDisposePercent
        {
            get { return m_SDisposePercent; }
            set { m_SDisposePercent = value; }
        }

       
        private string m_SDisposeNumber = "";
        /// <summary>
        /// 急救治疗数
        /// </summary>
        public string SDisposeNumber
        {
            get { return m_SDisposeNumber; }
            set { m_SDisposeNumber = value; }
        }

    
        private string m_SPRNumberExceptRefuseTreatment = "";
        /// <summary>
        /// 病历数除去拒绝治疗的
        /// </summary>
        public string SPRNumberExceptRefuseTreatment
        {
            get { return m_SPRNumberExceptRefuseTreatment; }
            set { m_SPRNumberExceptRefuseTreatment = value; }
        }

    
        private string m_SCharge = "";
        /// <summary>
        /// 收费金额
        /// </summary>
        public string SCharge
        {
            get { return m_SCharge; }
            set { m_SCharge = value; }
        }

    
        private string m_SKilometer = "";
        /// <summary>
        /// 行驶公里数
        /// </summary>
        public string SKilometer
        {
            get { return m_SKilometer; }
            set { m_SKilometer = value; }
        }

    
        private string m_SChargeCarFee = "";
        /// <summary>
        /// 车费
        /// </summary>
        public string SChargeCarFee
        {
            get { return m_SChargeCarFee; }
            set { m_SChargeCarFee = value; }
        }

     
        private string m_SChargeWaitFee = "";
        /// <summary>
        /// 等候费
        /// </summary>
        public string SChargeWaitFee
        {
            get { return m_SChargeWaitFee; }
            set { m_SChargeWaitFee = value; }
        }

    
        private string m_SChargeAidFee = "";
        /// <summary>
        /// 治疗费
        /// </summary>
        public string SChargeAidFee
        {
            get { return m_SChargeAidFee; }
            set { m_SChargeAidFee = value; }
        }

        private string m_SVeinPercent = "";
        /// <summary>
        /// 静脉开通率
        /// </summary>
        public string SVeinPercent
        {
            get { return m_SVeinPercent; }
            set { m_SVeinPercent = value; }
        }

    
        private string m_SVeinNumber = "";
        /// <summary>
        /// 静脉开通数
        /// </summary>
        public string SVeinNumber
        {
            get { return m_SVeinNumber; }
            set { m_SVeinNumber = value; }
        }

      
        private string m_SROSCPercent = "";
        /// <summary>
        /// 复苏成功率
        /// </summary>
        public string SROSCPercent
        {
            get { return m_SROSCPercent; }
            set { m_SROSCPercent = value; }
        }

      
        private string m_SROSCNumber = "";
        /// <summary>
        /// 复苏成功数
        /// </summary>
        public string SROSCNumber
        {
            get { return m_SROSCNumber; }
            set { m_SROSCNumber = value; }
        }

    
        private string m_SCPRNumber = "";
        /// <summary>
        /// 心肺复苏数
        /// </summary>
        public string SCPRNumber
        {
            get { return m_SCPRNumber; }
            set { m_SCPRNumber = value; }
        }
     
        private string m_SPRNumber = "";
        /// <summary>
        /// 病历数
        /// </summary>
        public string SPRNumber
        {
            get { return m_SPRNumber; }
            set { m_SPRNumber = value; }
        }
    }
}
