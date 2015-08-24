using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{

    public class PersonalStatisticsInfo
    {
        private string m_PersonName = "";
        /// <summary>
        /// 人员姓名
        /// </summary>
        public string PersonName
        {
            get { return m_PersonName; }
            set { m_PersonName = value; }
        }
                    
        private string m_PTaskTimes = "";
        /// <summary>
        /// 出车次数
        /// </summary>
        public string PTaskTimes
        {
            get { return m_PTaskTimes; }
            set { m_PTaskTimes = value; }
        }
       
        private string m_PFirstAidTimes = "";
        /// <summary>
        /// 急救次数
        /// </summary>
        public string PFirstAidTimes
        {
            get { return m_PFirstAidTimes; }
            set { m_PFirstAidTimes = value; }
        }
      
        private string m_PTransTimes = "";
        /// <summary>
        /// 转院次数
        /// </summary>
        public string PTransTimes
        {
            get { return m_PTransTimes; }
            set { m_PTransTimes = value; }
        }
      
        private string m_PGoHomeTimes = "";
        /// <summary>
        /// 回家次数
        /// </summary>
        public string PGoHomeTimes
        {
            get { return m_PGoHomeTimes; }
            set { m_PGoHomeTimes = value; }
        }

       
        private string m_PDays = "";
        /// <summary>
        /// 当班数
        /// </summary>
        public string PDays
        {
            get { return m_PDays; }
            set { m_PDays = value; }
        }
      
        private string m_PTaskTimesEveryDay = "";
        /// <summary>
        /// 人均班出车次数
        /// </summary>
        public string PTaskTimesEveryDay
        {
            get { return m_PTaskTimesEveryDay; }
            set { m_PTaskTimesEveryDay = value; }
        }
       
        private string m_PAvgSendTime = "";
        /// <summary>
        /// 平均出车时间
        /// </summary>
        public string PAvgSendTime
        {
            get { return m_PAvgSendTime; }
            set { m_PAvgSendTime = value; }
        }

        private string m_PAvgArriveTime = "";

        /// <summary>
        /// 平均到达时间
        /// </summary>
        public string PAvgArriveTime
        {
            get { return m_PAvgArriveTime; }
            set { m_PAvgArriveTime = value; }
        }

      
        private string m_PAvgLocalAidTime = "";
        /// <summary>
        /// 平均现场抢救时间
        /// </summary>
        public string PAvgLocalAidTime
        {
            get { return m_PAvgLocalAidTime; }
            set { m_PAvgLocalAidTime = value; }
        }

       
        private string m_PAvgTransTime = "";
        /// <summary>
        /// 平均运送时间
        /// </summary>
        public string PAvgTransTime
        {
            get { return m_PAvgTransTime; }
            set { m_PAvgTransTime = value; }
        }

        
        private string m_PAvgHospitalTransTime = "";
        /// <summary>
        /// 平均医院交接时间
        /// </summary>
        public string PAvgHospitalTransTime
        {
            get { return m_PAvgHospitalTransTime; }
            set { m_PAvgHospitalTransTime = value; }
        }

       
        private string m_PAvgReactTime = "";
        /// <summary>
        /// 平均反应时间
        /// </summary>
        public string PAvgReactTime
        {
            get { return m_PAvgReactTime; }
            set { m_PAvgReactTime = value; }
        }

       
        private string m_PAvgTurnTime = "";
        /// <summary>
        /// 平均周转时间
        /// </summary>
        public string PAvgTurnTime
        {
            get { return m_PAvgTurnTime; }
            set { m_PAvgTurnTime = value; }
        }

        
        private string m_PDisposePercent = "";
        /// <summary>
        /// 急救处理率
        /// </summary>
        public string PDisposePercent
        {
            get { return m_PDisposePercent; }
            set { m_PDisposePercent = value; }
        }

     
        private string m_PDisposeNumber = "";
        /// <summary>
        /// 急救治疗数
        /// </summary>
        public string PDisposeNumber
        {
            get { return m_PDisposeNumber; }
            set { m_PDisposeNumber = value; }
        }

       
        private string m_PPRNumberExceptRefuseTreatment = "";
        /// <summary>
        /// 病历数除去拒绝治疗的
        /// </summary>
        public string PPRNumberExceptRefuseTreatment
        {
            get { return m_PPRNumberExceptRefuseTreatment; }
            set { m_PPRNumberExceptRefuseTreatment = value; }
        }

     
        private string m_PCharge = "";
        /// <summary>
        /// 收费金额
        /// </summary>
        public string PCharge
        {
            get { return m_PCharge; }
            set { m_PCharge = value; }
        }

       
        private string m_PKilometer = "";
        /// <summary>
        /// 行驶公里数
        /// </summary>
        public string PKilometer
        {
            get { return m_PKilometer; }
            set { m_PKilometer = value; }
        }

      
        private string m_PChargeCarFee = "";
        /// <summary>
        /// 车费
        /// </summary>
        public string PChargeCarFee
        {
            get { return m_PChargeCarFee; }
            set { m_PChargeCarFee = value; }
        }

       
        private string m_PChargeWaitFee = "";
        /// <summary>
        /// 等候费
        /// </summary>
        public string PChargeWaitFee
        {
            get { return m_PChargeWaitFee; }
            set { m_PChargeWaitFee = value; }
        }

        /// <summary>
        /// 治疗费
        /// </summary>
        private string m_PChargeAidFee = "";

        public string PChargeAidFee
        {
            get { return m_PChargeAidFee; }
            set { m_PChargeAidFee = value; }
        }

      
        private string m_PVeinPercent = "";
        /// <summary>
        /// 静脉开通率
        /// </summary>
        public string PVeinPercent
        {
            get { return m_PVeinPercent; }
            set { m_PVeinPercent = value; }
        }

  
        private string m_PVeinNumber = "";
        /// <summary>
        /// 静脉开通数
        /// </summary>
        public string PVeinNumber
        {
            get { return m_PVeinNumber; }
            set { m_PVeinNumber = value; }
        }


        private string m_PROSCPercent = "";
        /// <summary>
        /// 复苏成功率
        /// </summary>
        public string PROSCPercent
        {
            get { return m_PROSCPercent; }
            set { m_PROSCPercent = value; }
        }

       
        private string m_PROSCNumber = "";
        /// <summary>
        /// 复苏成功数
        /// </summary>
        public string PROSCNumber
        {
            get { return m_PROSCNumber; }
            set { m_PROSCNumber = value; }
        }

    
        private string m_PCPRNumber = "";
        /// <summary>
        /// 心肺复苏数
        /// </summary>
        public string PCPRNumber
        {
            get { return m_PCPRNumber; }
            set { m_PCPRNumber = value; }
        }
     
        private string m_PPRNumber = "";
        /// <summary>
        /// 病历数
        /// </summary>
        public string PPRNumber
        {
            get { return m_PPRNumber; }
            set { m_PPRNumber = value; }
        }
    }
}
