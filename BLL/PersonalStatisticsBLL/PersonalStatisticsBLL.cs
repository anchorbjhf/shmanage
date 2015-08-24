using Anke.SHManage.Model;
using Anke.SHManage.MSSQLDAL.TJDAL;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{
    //个人业务统计逻辑
    public class PersonalStatisticsBLL
    {
        TJDAL m_DAL = new TJDAL();
        //private DataTable m_StatisticsDD = null;
       // private DataTable m_StatisticsCharge = null;

        private static Dictionary<string, DataTable> m_cacheTable = new Dictionary<string,DataTable>();

       
        private static Dictionary<string, DataTable> m_cacheTable2 = new Dictionary<string, DataTable>();
        //单实例,解决反复执行GetAllStatisticsDD影响效率。(GetAllStatisticsDD仅执行一遍返回的是Datatable，将结果存进内存， 去GetStatisticsDD方法调用总的Datatable)
        private DataTable GetStatisticsDD(DateTime beginTime, DateTime endTime)
        {
            string cacheKey = beginTime.ToString("yyyy-MM-dd HH:mm:ss") + endTime.ToString("yyyy-MM-dd HH:mm:ss");
            if (!m_cacheTable.ContainsKey(cacheKey))
            {
                m_cacheTable.Clear();
                m_cacheTable[cacheKey] = m_DAL.GetAllStatisticsDD(beginTime, endTime);
            }

            return m_cacheTable[cacheKey];

        }
        private DataTable GetStatisticsCharge(DateTime beginTime, DateTime endTime)
        {
            string cacheKey = beginTime.ToString("yyyy-MM-dd HH:mm:ss") + endTime.ToString("yyyy-MM-dd HH:mm:ss");
            if (!m_cacheTable2.ContainsKey(cacheKey))
            {
                m_cacheTable2.Clear();
                m_cacheTable2[cacheKey] = m_DAL.GetALLStatisticsCharge(beginTime, endTime);
            }

            return m_cacheTable2[cacheKey];
        }

        //private DataTable GetStatisticsCharge(DateTime beginTime, DateTime endTime)
        //{
        //    if (m_StatisticsCharge == null)
        //        m_StatisticsCharge = m_DAL.GetALLStatisticsCharge(beginTime, endTime);
        //    return m_StatisticsCharge;
        //}


        #region 获取个人业务
        public PersonalStatisticsInfo GetPersonalStatisticsDD(DateTime beginTime, DateTime endTime)
        {
         
            string selfWorkCode = UserOperateContext.Current.Session_UsrInfo.WorkCode;
            DataTable tableall = GetStatisticsDD(beginTime, endTime);

            //在表里循环遍历，当某一行的PersonalCode跟session 中获取的ID 相等时，将那一行的数据取出，赋给Model，Return
            PersonalStatisticsInfo AEinfo = new PersonalStatisticsInfo();

            foreach (DataRow dr in tableall.Rows)
            {
                if (Convert.ToString(dr["WorkCode"]) == selfWorkCode)
                {
                    AEinfo.PersonName = DBConvert.ConvertStringToString(dr["Name"]);
                    AEinfo.PTaskTimes = DBConvert.ConvertStringToString(dr["SendCarTimes"]);
                    AEinfo.PFirstAidTimes = DBConvert.ConvertStringToString(dr["FirstAidTimes"]);
                    AEinfo.PTransTimes = DBConvert.ConvertStringToString(dr["TransTimes"]);
                    AEinfo.PGoHomeTimes = DBConvert.ConvertStringToString(dr["GoHomeTimes"]);
                    AEinfo.PDays = DBConvert.ConvertStringToString(dr["Days"]);
                    AEinfo.PTaskTimesEveryDay = DBConvert.ConvertStringToString(dr["AvgDaySendCarTimes"]);
                    AEinfo.PAvgSendTime = DBConvert.ConvertStringToString(dr["AvgSendCarTime"]);
                    AEinfo.PAvgArriveTime = DBConvert.ConvertStringToString(dr["AvgArrivingSceneTime"]);
                    AEinfo.PAvgLocalAidTime = DBConvert.ConvertStringToString(dr["AvgSceneRescueTime"]);
                    AEinfo.PAvgTransTime = DBConvert.ConvertStringToString(dr["AvgTransportTime"]);
                    AEinfo.PAvgHospitalTransTime = DBConvert.ConvertStringToString(dr["AvgHospitalDeliveryTime"]);
                    AEinfo.PAvgReactTime = DBConvert.ConvertStringToString(dr["AvgResponseTime"]);
                    AEinfo.PAvgTurnTime = DBConvert.ConvertStringToString(dr["AvgRevolveTime"]);
                }
            }
            return AEinfo;
        }
        //获取个人管理块收费
        public PersonalStatisticsInfo GetPersonalStatisticsGLCharge(DateTime beginTime, DateTime endTime)
        {
            // long start = DateTime.Now.Ticks;
            List<int> role = UserOperateContext.Current.Session_UsrRole;
            string selfName = UserOperateContext.Current.Session_UsrInfo.Name;
            string selfWorkCode = UserOperateContext.Current.Session_UsrInfo.WorkCode;
            //司机
            if (role.Contains(10))
            {
                PersonalStatisticsInfo AEinfo = new PersonalStatisticsInfo();

                int PKilometer = 0; int PChargeCarFee = 0; int PChargeWaitFee = 0; int PChargeAidFee = 0; int PCharge = 0;
                DataTable tableall = GetStatisticsCharge(beginTime, endTime);
                foreach (DataRow dr in tableall.Rows)
                {
                    if (Convert.ToString(dr["Driver"]) == selfName)
                    {
                        PKilometer += Convert.ToInt32(dr["收费公里数"]);
                        PChargeCarFee += Convert.ToInt32(dr["车费"]);
                        PChargeWaitFee += Convert.ToInt32(dr["等候费"]);
                        PChargeAidFee += Convert.ToInt32(dr["治疗费"]);
                        PCharge += Convert.ToInt32(dr["收费金额"]);

                    }
                }
                AEinfo.PKilometer = Convert.ToString(PKilometer);
                AEinfo.PChargeCarFee = Convert.ToString(PChargeCarFee);
                AEinfo.PChargeWaitFee = Convert.ToString(PChargeWaitFee);
                AEinfo.PChargeAidFee = Convert.ToString(PChargeAidFee);
                AEinfo.PCharge = Convert.ToString(PCharge);
                return AEinfo;

            }
            //医生，护士
            else if (role.Contains(1) || role.Contains(3))
            {
                PersonalStatisticsInfo AEinfo = new PersonalStatisticsInfo();

                int PKilometer = 0; int PChargeCarFee = 0; int PChargeWaitFee = 0; int PChargeAidFee = 0; int PCharge = 0;
                DataTable tableall = GetStatisticsCharge(beginTime, endTime);
                foreach (DataRow dr in tableall.Rows)
                {
                    if (Convert.ToString(dr["DoctorAndNurse"]) == selfName)
                    {
                        PKilometer += Convert.ToInt32(dr["收费公里数"]);
                        PChargeCarFee += Convert.ToInt32(dr["车费"]);
                        PChargeWaitFee += Convert.ToInt32(dr["等候费"]);
                        PChargeAidFee += Convert.ToInt32(dr["治疗费"]);
                        PCharge += Convert.ToInt32(dr["收费金额"]);

                    }
                }
                AEinfo.PKilometer = Convert.ToString(PKilometer);
                AEinfo.PChargeCarFee = Convert.ToString(PChargeCarFee);
                AEinfo.PChargeWaitFee = Convert.ToString(PChargeWaitFee);
                AEinfo.PChargeAidFee = Convert.ToString(PChargeAidFee);
                AEinfo.PCharge = Convert.ToString(PCharge);
                return AEinfo;

            }
            //担架工
            else if (role.Contains(28))
            {
                PersonalStatisticsInfo AEinfo = new PersonalStatisticsInfo();

                int PKilometer = 0; int PChargeCarFee = 0; int PChargeWaitFee = 0; int PChargeAidFee = 0; int PCharge = 0;
                DataTable tableall = GetStatisticsCharge(beginTime, endTime);
                foreach (DataRow dr in tableall.Rows)
                {
                    if (Convert.ToString(dr["StretcherBearersI"]) == selfName)
                    {
                        PKilometer += Convert.ToInt32(dr["收费公里数"]);
                        PChargeCarFee += Convert.ToInt32(dr["车费"]);
                        PChargeWaitFee += Convert.ToInt32(dr["等候费"]);
                        PChargeAidFee += Convert.ToInt32(dr["治疗费"]);
                        PCharge += Convert.ToInt32(dr["收费金额"]);
                    }
                }
                AEinfo.PKilometer = Convert.ToString(PKilometer);
                AEinfo.PChargeCarFee = Convert.ToString(PChargeCarFee);
                AEinfo.PChargeWaitFee = Convert.ToString(PChargeWaitFee);
                AEinfo.PChargeAidFee = Convert.ToString(PChargeAidFee);
                AEinfo.PCharge = Convert.ToString(PCharge);
                return AEinfo;
            }
            else return new PersonalStatisticsInfo();
            // long aaa= DateTime.Now.Ticks - start;



        }
        //获取个人管理块病历
        public PersonalStatisticsInfo GetPersonalStatisticsGLPR(DateTime beginTime, DateTime endTime)
        {
            List<int> role = UserOperateContext.Current.Session_UsrRole;
            string selfName = UserOperateContext.Current.Session_UsrInfo.Name;
            string selfWorkCode = UserOperateContext.Current.Session_UsrInfo.WorkCode;
            return m_DAL.GetPersonalStatisticsGL2(beginTime, endTime, selfName, selfWorkCode, role);
        }
        #endregion

        #region 获取所属分站业务
        //根据个人工号获取最后一次操作的车辆所在的分站的编码
        public string GetstaionCodeByWorkCodeTAmbulance(string selfWorkCode)
        {
            return m_DAL.GetstaionCodeByWorkCodeTAmbulance(selfWorkCode);
        }
        //获取分站调度信息
        public StationStatisticsInfo GetStationStatisticsDD(DateTime beginTime, DateTime endTime)
        {
            string selfWorkCode = UserOperateContext.Current.Session_UsrInfo.WorkCode;
            string selfStationID = m_DAL.GetstaionCodeByWorkCodeTAmbulance(selfWorkCode);

            if (selfStationID != null && selfStationID != "")
            {
                DataTable tableall = GetStatisticsDD(beginTime, endTime);
                //在表里循环遍历，当某一行的station跟session 中获取的selfStationID 相等时，将那一行的数据取出，赋给Model，Return
                StationStatisticsInfo ASinfo = new StationStatisticsInfo();
                int STaskTimes = 0; int SFirstAidTimes = 0; int STransTimes = 0; int a = 0; int SGoHomeTimes = 0; int SDays = 0; int STaskTimesEveryDay = 0;
                int SAvgSendTime = 0; int SAvgArriveTime = 0; int SAvgLocalAidTime = 0; int SAvgTransTime = 0; int SAvgHospitalTransTime = 0;
                int SAvgReactTime = 0; int SAvgTurnTime = 0;
                foreach (DataRow dr in tableall.Rows)
                {
                    if (Convert.ToString(dr["station"]) == selfStationID)
                    {
                        a = a + 1;
                        STaskTimes += Convert.ToInt32(dr["SendCarTimes"]);
                        SFirstAidTimes += Convert.ToInt32(dr["FirstAidTimes"]);
                        STransTimes += Convert.ToInt32(dr["TransTimes"]);
                        SGoHomeTimes += Convert.ToInt32(dr["GoHomeTimes"]);
                        SDays += Convert.ToInt32(dr["Days"]);
                        STaskTimesEveryDay += Convert.ToInt32(dr["AvgDaySendCarTimes"]);
                        SAvgSendTime += Convert.ToInt32(dr["AvgSendCarTime"]);
                        SAvgArriveTime += Convert.ToInt32(dr["AvgArrivingSceneTime"]);
                        SAvgLocalAidTime += Convert.ToInt32(dr["AvgSceneRescueTime"]);
                        SAvgTransTime += Convert.ToInt32(dr["AvgTransportTime"]);
                        SAvgHospitalTransTime += Convert.ToInt32(dr["AvgHospitalDeliveryTime"]);
                        SAvgReactTime += Convert.ToInt32(dr["AvgResponseTime"]);
                        SAvgTurnTime += Convert.ToInt32(dr["AvgRevolveTime"]);

                        ASinfo.StationName = DBConvert.ConvertStringToString(dr["tsName"]);
                    }
                }
                if (a != 0)
                {
                    ASinfo.STaskTimes = Convert.ToString(STaskTimes);
                    ASinfo.SFirstAidTimes = Convert.ToString(SFirstAidTimes);
                    ASinfo.STransTimes = Convert.ToString(STransTimes);
                    ASinfo.SGoHomeTimes = Convert.ToString(SGoHomeTimes);
                    ASinfo.SDays = Convert.ToString(SDays);
                    ASinfo.STaskTimesEveryDay = Convert.ToString(STaskTimesEveryDay / a);
                    ASinfo.SAvgSendTime = Convert.ToString(SAvgSendTime / a);
                    ASinfo.SAvgArriveTime = Convert.ToString(SAvgArriveTime / a);
                    ASinfo.SAvgLocalAidTime = Convert.ToString(SAvgLocalAidTime / a);
                    ASinfo.SAvgTransTime = Convert.ToString(SAvgTransTime / a);
                    ASinfo.SAvgHospitalTransTime = Convert.ToString(SAvgHospitalTransTime / a);
                    ASinfo.SAvgReactTime = Convert.ToString(SAvgReactTime / a);
                    ASinfo.SAvgTurnTime = Convert.ToString(SAvgTurnTime / a);

                    return ASinfo;
                }
                else {
                    return new StationStatisticsInfo();
                }
            }
            else return new StationStatisticsInfo();
        }
        public StationStatisticsInfo GetStationStatisticsGL1(DateTime beginTime, DateTime endTime)
        {
            string selfWorkCode = UserOperateContext.Current.Session_UsrInfo.WorkCode;
            string selfStationID = m_DAL.GetstaionCodeByWorkCodeTAmbulance(selfWorkCode);

            if (selfStationID != null && selfStationID != "")
            {
                //在表里循环遍历，当某一行的CenterID跟session 中获取的selfCenterID 相等时，将那一行的数据取出，赋给Model，Return
                StationStatisticsInfo ASinfo = new StationStatisticsInfo();
                int PKilometer = 0; int PChargeCarFee = 0; int PChargeWaitFee = 0; int PChargeAidFee = 0; int PCharge = 0;
                DataTable tableall = GetStatisticsCharge(beginTime, endTime);
                foreach (DataRow dr in tableall.Rows)
                {
                    if (Convert.ToString(dr["OutStationCode"]) == selfStationID)
                    {
                        PKilometer += Convert.ToInt32(dr["收费公里数"]);
                        PChargeCarFee += Convert.ToInt32(dr["车费"]);
                        PChargeWaitFee += Convert.ToInt32(dr["等候费"]);
                        PChargeAidFee += Convert.ToInt32(dr["治疗费"]);
                        PCharge += Convert.ToInt32(dr["收费金额"]);
                    }
                }
                ASinfo.SKilometer = Convert.ToString(PKilometer);
                ASinfo.SChargeCarFee = Convert.ToString(PChargeCarFee);
                ASinfo.SChargeWaitFee = Convert.ToString(PChargeWaitFee);
                ASinfo.SChargeAidFee = Convert.ToString(PChargeAidFee);
                ASinfo.SCharge = Convert.ToString(PCharge);
                return ASinfo;

            }
            else return new StationStatisticsInfo();
        }
        public StationStatisticsInfo GetStationStatisticsGL2(DateTime beginTime, DateTime endTime)
        {
            string selfWorkCode = UserOperateContext.Current.Session_UsrInfo.WorkCode;
            string selfStationID = m_DAL.GetstaionCodeByWorkCodeTAmbulance(selfWorkCode);

            if (selfStationID != null && selfStationID != "")
            {
                return m_DAL.GetStationStatisticsGL2(beginTime, endTime, selfStationID);
            }
            else return new StationStatisticsInfo();
        }
        #endregion

        #region 获取所属中心业务
        public CenterStatisticsInfo GetCenterStatisticsDD(DateTime beginTime, DateTime endTime)
        {
            string selfCenterID = UserOperateContext.Current.Session_UsrInfo.P_Department.DispatchSubCenterID;
            if (selfCenterID != null && selfCenterID != "")
            {
                DataTable tableall = GetStatisticsDD(beginTime, endTime);

                //在表里循环遍历，当某一行的CenterID跟session 中获取的selfCenterID 相等时，将那一行的数据取出，赋给Model，Return
                CenterStatisticsInfo ACinfo = new CenterStatisticsInfo();
                int STaskTimes = 0; int SFirstAidTimes = 0; int STransTimes = 0; int a = 0; int SGoHomeTimes = 0; int SDays = 0; int STaskTimesEveryDay = 0;
                int SAvgSendTime = 0; int SAvgArriveTime = 0; int SAvgLocalAidTime = 0; int SAvgTransTime = 0; int SAvgHospitalTransTime = 0;
                int SAvgReactTime = 0; int SAvgTurnTime = 0;
                foreach (DataRow dr in tableall.Rows)
                {
                    if (Convert.ToString(dr["center"]) == selfCenterID)
                    {
                        a = a + 1;
                        STaskTimes += Convert.ToInt32(dr["SendCarTimes"]);
                        SFirstAidTimes += Convert.ToInt32(dr["FirstAidTimes"]);
                        STransTimes += Convert.ToInt32(dr["TransTimes"]);
                        SGoHomeTimes += Convert.ToInt32(dr["GoHomeTimes"]);
                        SDays += Convert.ToInt32(dr["Days"]);
                        STaskTimesEveryDay += Convert.ToInt32(dr["AvgDaySendCarTimes"]);
                        SAvgSendTime += Convert.ToInt32(dr["AvgSendCarTime"]);
                        SAvgArriveTime += Convert.ToInt32(dr["AvgArrivingSceneTime"]);
                        SAvgLocalAidTime += Convert.ToInt32(dr["AvgSceneRescueTime"]);
                        SAvgTransTime += Convert.ToInt32(dr["AvgTransportTime"]);
                        SAvgHospitalTransTime += Convert.ToInt32(dr["AvgHospitalDeliveryTime"]);
                        SAvgReactTime += Convert.ToInt32(dr["AvgResponseTime"]);
                        SAvgTurnTime += Convert.ToInt32(dr["AvgRevolveTime"]);

                        ACinfo.CenterName = DBConvert.ConvertStringToString(dr["tcName"]);
                    }
                }
                if (a != 0)
                {
                    ACinfo.CTaskTimes = Convert.ToString(STaskTimes);
                    ACinfo.CFirstAidTimes = Convert.ToString(SFirstAidTimes);
                    ACinfo.CTransTimes = Convert.ToString(STransTimes);
                    ACinfo.CGoHomeTimes = Convert.ToString(SGoHomeTimes);
                    ACinfo.CDays = Convert.ToString(SDays);
                    ACinfo.CTaskTimesEveryDay = Convert.ToString(STaskTimesEveryDay / a);
                    ACinfo.CAvgSendTime = Convert.ToString(SAvgSendTime / a);
                    ACinfo.CAvgArriveTime = Convert.ToString(SAvgArriveTime / a);
                    ACinfo.CAvgLocalAidTime = Convert.ToString(SAvgLocalAidTime / a);
                    ACinfo.CAvgTransTime = Convert.ToString(SAvgTransTime / a);
                    ACinfo.CAvgHospitalTransTime = Convert.ToString(SAvgHospitalTransTime / a);
                    ACinfo.CAvgReactTime = Convert.ToString(SAvgReactTime / a);
                    ACinfo.CAvgTurnTime = Convert.ToString(SAvgTurnTime / a);

                    return ACinfo;
                }
                else {
                    return new CenterStatisticsInfo();
                }
            }
            else return new CenterStatisticsInfo();
        }

        public CenterStatisticsInfo GetCenterStatisticsGL1(DateTime beginTime, DateTime endTime)
        {
            string selfCenterID = UserOperateContext.Current.Session_UsrInfo.P_Department.DispatchSubCenterID;

            if (selfCenterID != null && selfCenterID != "")
            {
                //根据分中心编码获取分站编码(string)
                List<CheckModelExt> lm = m_DAL.GetStationCodeByCenter(selfCenterID);

                string ret = "";
                for (int i = 0; i < lm.Count; i++)
                {
                    ret += lm[i].ID.ToString() + ',';
                }
                ret = ret.TrimEnd(',');

                CenterStatisticsInfo ACinfo = new CenterStatisticsInfo();
                int PKilometer = 0; int PChargeCarFee = 0; int PChargeWaitFee = 0; int PChargeAidFee = 0; int PCharge = 0;
                DataTable tableall = GetStatisticsCharge(beginTime, endTime);
                foreach (DataRow dr in tableall.Rows)
                {
                    if (ret.Contains(Convert.ToString(dr["OutStationCode"])))
                    {
                        PKilometer += Convert.ToInt32(dr["收费公里数"]);
                        PChargeCarFee += Convert.ToInt32(dr["车费"]);
                        PChargeWaitFee += Convert.ToInt32(dr["等候费"]);
                        PChargeAidFee += Convert.ToInt32(dr["治疗费"]);
                        PCharge += Convert.ToInt32(dr["收费金额"]);
                    }
                }
                ACinfo.CKilometer = Convert.ToString(PKilometer);
                ACinfo.CChargeCarFee = Convert.ToString(PChargeCarFee);
                ACinfo.CChargeWaitFee = Convert.ToString(PChargeWaitFee);
                ACinfo.CChargeAidFee = Convert.ToString(PChargeAidFee);
                ACinfo.CCharge = Convert.ToString(PCharge);
                return ACinfo;
              
            }
            else return new CenterStatisticsInfo();
        }
        public CenterStatisticsInfo GetCenterStatisticsGL2(DateTime beginTime, DateTime endTime)
        {
            string selfCenterID = UserOperateContext.Current.Session_UsrInfo.P_Department.DispatchSubCenterID;

            if (selfCenterID != null && selfCenterID != "")
            {
                List<CheckModelExt> lcm = m_DAL.GetStationCodeByCenter(selfCenterID);

                string ret1 = "";
                for (int i = 0; i < lcm.Count; i++)
                {
                    ret1 += lcm[i].ID.ToString() + ',';
                }
                ret1 = ret1.TrimEnd(',');

                return m_DAL.GetCenterStatisticsGL2(beginTime, endTime, ret1);
            }
            else return new CenterStatisticsInfo();
        }
        #endregion

        #region 获取中心业务
        public TotalStatisticsInfo GetTotalStatisticsDD(DateTime beginTime, DateTime endTime)
        {
            DataTable tableall = GetStatisticsDD(beginTime, endTime);

            //在表里循环遍历，当某一行的CenterID跟session 中获取的selfCenterID 相等时，将那一行的数据取出，赋给Model，Return
            TotalStatisticsInfo ATinfo = new TotalStatisticsInfo();
            int STaskTimes = 0; int SFirstAidTimes = 0; int STransTimes = 0; int a = 0; int SGoHomeTimes = 0; int SDays = 0; int STaskTimesEveryDay = 0;
            int SAvgSendTime = 0; int SAvgArriveTime = 0; int SAvgLocalAidTime = 0; int SAvgTransTime = 0; int SAvgHospitalTransTime = 0;
            int SAvgReactTime = 0; int SAvgTurnTime = 0;
            foreach (DataRow dr in tableall.Rows)
            {
                a = a + 1;
                STaskTimes += Convert.ToInt32(dr["SendCarTimes"]);
                SFirstAidTimes += Convert.ToInt32(dr["FirstAidTimes"]);
                STransTimes += Convert.ToInt32(dr["TransTimes"]);
                SGoHomeTimes += Convert.ToInt32(dr["GoHomeTimes"]);
                SDays += Convert.ToInt32(dr["Days"]);
                STaskTimesEveryDay += Convert.ToInt32(dr["AvgDaySendCarTimes"]);
                SAvgSendTime += Convert.ToInt32(dr["AvgSendCarTime"]);
                SAvgArriveTime += Convert.ToInt32(dr["AvgArrivingSceneTime"]);
                SAvgLocalAidTime += Convert.ToInt32(dr["AvgSceneRescueTime"]);
                SAvgTransTime += Convert.ToInt32(dr["AvgTransportTime"]);
                SAvgHospitalTransTime += Convert.ToInt32(dr["AvgHospitalDeliveryTime"]);
                SAvgReactTime += Convert.ToInt32(dr["AvgResponseTime"]);
                SAvgTurnTime += Convert.ToInt32(dr["AvgRevolveTime"]);

            }
            if (a != 0)
            {
                ATinfo.TTaskTimes = Convert.ToString(STaskTimes);
                ATinfo.TFirstAidTimes = Convert.ToString(SFirstAidTimes);
                ATinfo.TTransTimes = Convert.ToString(STransTimes);
                ATinfo.TGoHomeTimes = Convert.ToString(SGoHomeTimes);
                ATinfo.TDays = Convert.ToString(SDays);
                ATinfo.TTaskTimesEveryDay = Convert.ToString(STaskTimesEveryDay / a);
                ATinfo.TAvgSendTime = Convert.ToString(SAvgSendTime / a);
                ATinfo.TAvgArriveTime = Convert.ToString(SAvgArriveTime / a);
                ATinfo.TAvgLocalAidTime = Convert.ToString(SAvgLocalAidTime / a);
                ATinfo.TAvgTransTime = Convert.ToString(SAvgTransTime / a);
                ATinfo.TAvgHospitalTransTime = Convert.ToString(SAvgHospitalTransTime / a);
                ATinfo.TAvgReactTime = Convert.ToString(SAvgReactTime / a);
                ATinfo.TAvgTurnTime = Convert.ToString(SAvgTurnTime / a);

                return ATinfo;
            }
            else {
                return new TotalStatisticsInfo();
            }
        }
        public TotalStatisticsInfo GetTotalStatisticsGL1(DateTime beginTime, DateTime endTime)
        {

            TotalStatisticsInfo ACinfo = new TotalStatisticsInfo();
            int PKilometer = 0; int PChargeCarFee = 0; int PChargeWaitFee = 0; int PChargeAidFee = 0; int PCharge = 0;

            DataTable tableall = GetStatisticsCharge(beginTime, endTime);

            foreach (DataRow dr in tableall.Rows)
            {
                PKilometer += Convert.ToInt32(dr["收费公里数"]);
                PChargeCarFee += Convert.ToInt32(dr["车费"]);
                PChargeWaitFee += Convert.ToInt32(dr["等候费"]);
                PChargeAidFee += Convert.ToInt32(dr["治疗费"]);
                PCharge += Convert.ToInt32(dr["收费金额"]);
            }
            ACinfo.TKilometer = Convert.ToString(PKilometer);
            ACinfo.TChargeCarFee = Convert.ToString(PChargeCarFee);
            ACinfo.TChargeWaitFee = Convert.ToString(PChargeWaitFee);
            ACinfo.TChargeAidFee = Convert.ToString(PChargeAidFee);
            ACinfo.TCharge = Convert.ToString(PCharge);
            return ACinfo;

        }
        public TotalStatisticsInfo GetTotalStatisticsGL2(DateTime beginTime, DateTime endTime)
        {
            return m_DAL.GetTotalStatisticsGL2(beginTime, endTime);
        }

        #endregion

    }
}
