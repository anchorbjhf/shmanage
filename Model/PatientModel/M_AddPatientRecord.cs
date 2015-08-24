using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
    public class M_AddPatientRecord
    {
        public string TaskCode { get; set; }

        public int PatientOrder { get; set; }
        public string state { get; set; }

        public M_PatientRecord info { get; set; }

        public M_PatientRecordAppend pra { get; set; }

        public M_PatientRecordCPR prCPR { get; set; }

        public List<M_PatientRecordDiag> prDiag { get; set; }

        public List<M_PatientRecordECGImpressions> prECG { get; set; }

    }
}
