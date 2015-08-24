using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
    /// <summary>
    /// 调度信息表
    /// </summary>
    public class M_AttemperData
    {
        public string TaskCode { get; set; }

        public string CallOrder { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string Age { get; set; }

        public string AgeType { get; set; }
        public string ForHelpPhone { get; set; }
        public string LinkPerson { get; set; }
        public string ContactTelephone { get; set; }
        public string AlarmTypeCode { get; set; }
        public string AlarmType { get; set; }
        public string Area { get; set; }
        public string XCoordinate { get; set; }
        public string YCoordinate { get; set; }
        public string AlarmReason { get; set; }
        public string LocalAddress { get; set; }
        public string SendAddress { get; set; }
        public string StationCode { get; set; }
        public string Station { get; set; }
        public string AmbulanceCode { get; set; }
        public Nullable<System.DateTime> TelephoneRingingTime { get; set; }
        public Nullable<System.DateTime> GenerationTaskTime { get; set; }
        public Nullable<System.DateTime> DrivingTime { get; set; }
        public Nullable<System.DateTime> ArriveSceneTime { get; set; }
        public Nullable<System.DateTime> LeaveSceneTime { get; set; }
        public Nullable<System.DateTime> ArriveDestinationTime { get; set; }
        public Nullable<System.DateTime> CompleteTime { get; set; }
        public Nullable<System.DateTime> BackTime { get; set; }
        public string OutResult { get; set; }
        public string Driver { get; set; }
        public string Doctor { get; set; }
        public string Nurse { get; set; }
        public string StretcherBearers { get; set; }
        public string FirstAider { get; set; }
        public string Disptcher { get; set; }
    }
}
