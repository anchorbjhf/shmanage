using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
    public class M_AddPatientRecordRescue
    {
        public string TaskCode { get; set; }

        public int PatientOrder { get; set; }
        public string state { get; set; }

        public M_PatientRecordRescue prRescue { get; set; }

        public List<M_PatientRecordMeasure> prMeasure { get; set; }

        public List<M_PatientRecordDrug> prDrug { get; set; }
        public List<M_PatientRecordSanitation> prSanitation { get; set; }
        public List<M_PatientRecordLossDrug> prLossDrug { get; set; }
        public List<M_PatientRecordLossSanitation> prLossSanitation { get; set; }
    }
}
