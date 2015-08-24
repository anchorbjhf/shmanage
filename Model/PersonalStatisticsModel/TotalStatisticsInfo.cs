using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
  public  class TotalStatisticsInfo
  {

     
      private string m_TTaskTimes = "";
      /// <summary>
      /// 出车次数
      /// </summary>
      public string TTaskTimes
      {
          get { return m_TTaskTimes; }
          set { m_TTaskTimes = value; }
      }
     
      private string m_TFirstAidTimes = "";
      /// <summary>
      /// 急救次数
      /// </summary>
      public string TFirstAidTimes
      {
          get { return m_TFirstAidTimes; }
          set { m_TFirstAidTimes = value; }
      }
     
      private string m_TTransTimes = "";
      /// <summary>
      /// 转院次数
      /// </summary>
      public string TTransTimes
      {
          get { return m_TTransTimes; }
          set { m_TTransTimes = value; }
      }
     
      private string m_TGoHomeTimes = "";
      /// <summary>
      /// 回家次数
      /// </summary>
      public string TGoHomeTimes
      {
          get { return m_TGoHomeTimes; }
          set { m_TGoHomeTimes = value; }
      }
     
      private string m_TDays = "";
      /// <summary>
      /// 当班数
      /// </summary>
      public string TDays
      {
          get { return m_TDays; }
          set { m_TDays = value; }
      }

     
      private string m_TTaskTimesEveryDay = "";
      /// <summary>
      /// 人均班出车次数
      /// </summary>
      public string TTaskTimesEveryDay
      {
          get { return m_TTaskTimesEveryDay; }
          set { m_TTaskTimesEveryDay = value; }
      }

   
      private string m_TAvgSendTime = "";
      /// <summary>
      /// 平均出车时间
      /// </summary>
      public string TAvgSendTime
      {
          get { return m_TAvgSendTime; }
          set { m_TAvgSendTime = value; }
      }

     
      private string m_TAvgArriveTime = "";
      /// <summary>
      /// 平均到达时间
      /// </summary>
      public string TAvgArriveTime
      {
          get { return m_TAvgArriveTime; }
          set { m_TAvgArriveTime = value; }
      }

   
      private string m_TAvgLocalAidTime = "";
      /// <summary>
      /// 平均现场抢救时间
      /// </summary>
      public string TAvgLocalAidTime
      {
          get { return m_TAvgLocalAidTime; }
          set { m_TAvgLocalAidTime = value; }
      }


      private string m_TAvgTransTime = "";
      /// <summary>
      /// 平均运送时间
      /// </summary>
      public string TAvgTransTime
      {
          get { return m_TAvgTransTime; }
          set { m_TAvgTransTime = value; }
      }

    
      private string m_TAvgHospitalTransTime = "";
      /// <summary>
      /// 平均医院交接时间
      /// </summary>
      public string TAvgHospitalTransTime
      {
          get { return m_TAvgHospitalTransTime; }
          set { m_TAvgHospitalTransTime = value; }
      }

   
      private string m_TAvgReactTime = "";
      /// <summary>
      /// 平均反应时间
      /// </summary>
      public string TAvgReactTime
      {
          get { return m_TAvgReactTime; }
          set { m_TAvgReactTime = value; }
      }

     
      private string m_TAvgTurnTime = "";
      /// <summary>
      /// 平均周转时间
      /// </summary>
      public string TAvgTurnTime
      {
          get { return m_TAvgTurnTime; }
          set { m_TAvgTurnTime = value; }
      }

     
      private string m_TDisposePercent = "";
      /// <summary>
      /// 急救处理率
      /// </summary>
      public string TDisposePercent
      {
          get { return m_TDisposePercent; }
          set { m_TDisposePercent = value; }
      }

     
      private string m_TDisposeNumber = "";
      /// <summary>
      /// 急救治疗数
      /// </summary>
      public string TDisposeNumber
      {
          get { return m_TDisposeNumber; }
          set { m_TDisposeNumber = value; }
      }

     
      private string m_TPRNumberExceptRefuseTreatment = "";
      /// <summary>
      /// 病历数除去拒绝治疗的
      /// </summary>
      public string TPRNumberExceptRefuseTreatment
      {
          get { return m_TPRNumberExceptRefuseTreatment; }
          set { m_TPRNumberExceptRefuseTreatment = value; }
      }

   
      private string m_TCharge = "";
      /// <summary>
      /// 收费金额
      /// </summary>
      public string TCharge
      {
          get { return m_TCharge; }
          set { m_TCharge = value; }
      }

    
      private string m_TKilometer = "";
      /// <summary>
      /// 行驶公里数
      /// </summary>
      public string TKilometer
      {
          get { return m_TKilometer; }
          set { m_TKilometer = value; }
      }

    
      private string m_TChargeCarFee = "";
      /// <summary>
      /// 车费
      /// </summary>
      public string TChargeCarFee
      {
          get { return m_TChargeCarFee; }
          set { m_TChargeCarFee = value; }
      }

  
      private string m_TChargeWaitFee = "";
      /// <summary>
      /// 等候费
      /// </summary>
      public string TChargeWaitFee
      {
          get { return m_TChargeWaitFee; }
          set { m_TChargeWaitFee = value; }
      }

     
      private string m_TChargeAidFee = "";
      /// <summary>
      /// 治疗费
      /// </summary>
      public string TChargeAidFee
      {
          get { return m_TChargeAidFee; }
          set { m_TChargeAidFee = value; }
      }

     
      private string m_TVeinPercent = "";
      /// <summary>
      /// 静脉开通率
      /// </summary>
      public string TVeinPercent
      {
          get { return m_TVeinPercent; }
          set { m_TVeinPercent = value; }
      }

   
      private string m_TVeinNumber = "";
      /// <summary>
      /// 静脉开通数
      /// </summary>
      public string TVeinNumber
      {
          get { return m_TVeinNumber; }
          set { m_TVeinNumber = value; }
      }

   
      private string m_TROSCPercent = "";
      /// <summary>
      /// 复苏成功率
      /// </summary>
      public string TROSCPercent
      {
          get { return m_TROSCPercent; }
          set { m_TROSCPercent = value; }
      }

   
      private string m_TROSCNumber = "";
      /// <summary>
      /// 复苏成功数
      /// </summary>
      public string TROSCNumber
      {
          get { return m_TROSCNumber; }
          set { m_TROSCNumber = value; }
      }

     
      private string m_TCPRNumber = "";
      /// <summary>
      /// 心肺复苏数
      /// </summary>
      public string TCPRNumber
      {
          get { return m_TCPRNumber; }
          set { m_TCPRNumber = value; }
      }
     
      private string m_TPRNumber = "";
      /// <summary>
      /// 病历数
      /// </summary>
      public string TPRNumber
      {
          get { return m_TPRNumber; }
          set { m_TPRNumber = value; }
      }
    }
}
