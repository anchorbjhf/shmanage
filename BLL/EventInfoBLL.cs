using Anke.SHManage.MSSQLDAL;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{
    public class EventInfoBLL
    {
        private EventInfoDAL dal = new EventInfoDAL();

        //查询事件信息列表
        public object GetEventInfoList(int pageSize, int pageIndex, DateTime start, DateTime end, string mainSuit, string telephoneNumber, string localAddress, string patientName, string sendAddress,
                       string dispatcher, string driver, string doctor, string nurse,string stretcher, string eventType, string illnessJudgment, string eventCode, string station, string ambulanceCode,
                       string eventSource, E_StatisticsPermisson em, string selfWorkCode, string selfCenterID, string selfStationID)
        {
            return dal.GetEventInfoList(pageSize, pageIndex, start, end, mainSuit, telephoneNumber, localAddress, patientName, sendAddress,
                               dispatcher, driver, doctor, nurse, stretcher, eventType, illnessJudgment, eventCode, station, ambulanceCode, 
                               eventSource, em,selfWorkCode,selfCenterID,selfStationID);
        }
        //事件类型
        public object GetEventTypeList()
        {
            return dal.GetEventTypeList();
        }
        //事件来源
        public object GetEventSourceList()
        {
            return dal.GetEventSourceList();
        }
        //病情判断
        public object GetIllnessStateList()
        {
            return dal.GetIllnessStateList();
        }
        //车辆分站
        public object GetStationList()
        {
            return dal.GetStationList();
        }
        //车辆编码
        public object GetAmbulanceCodeList()
        {
            return dal.GetAmbulanceCodeList();
        }
        //调度员
        public object GetDispatcherList()
        {
            return dal.GetDispatcherList();
        }
        //司机
        public object GetDriverList()
        {
            return dal.GetDriverList();
        }
        //医生
        public object GetDoctorList()
        {
            return dal.GetDoctorList();
        }
        //护士
        public object GetNurseList()
        {
            return dal.GetNurseList();
        }
        //担架员
        public object GetStretcherList()
        {
            return dal.GetStretcherList();
        }

    }
}
