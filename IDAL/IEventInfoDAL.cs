//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Anke.SHManage.Utility;

//namespace Anke.SHManage.IDAL
//{
//    public interface IEventInfoDAL
//    {
//        //事件信息列表
//        object GetEventInfoList(int pageSize, int pageIndex, DateTime start, DateTime end, string mainSuit, string telephoneNumber, string localAddress, string patientName, string sendAddress,
//               string dispatcher, string driver, string doctor, string nurse,string stretcher, string eventType, string illnessJudgment, string eventCode, string station, string ambulanceCode,
//               string eventSource, E_StatisticsPermisson em, string selfWorkCode, string selfCenterID, string selfStationID);

//        //取事件类型
//        object GetEventTypeList();

//        //取事件来源
//        object GetEventSourceList();
        
//        //取病情判断
//        object GetIllnessStateList();
        
//        //取车辆分站
//        object GetstationList();

//        //取车辆编码
//        object GetAmubulanceCodeList();

//        //取调度员
//        object GetDispatcherList();

//        //取司机
//        object GetDriverList();

//        //取医生
//        object GetDoctorList();

//        //取护士
//        object GetNurseList();

//        //取担架员
//        object GetStretcherList();

//    }
//}
