//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Anke.SHManage.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class M_PatientRecordRescue
    {
        public string TaskCode { get; set; }
        public int PatientOrder { get; set; }
        public string RescueRecordCode { get; set; }
        public int DisposeOrder { get; set; }
        public string PeriodOfTime { get; set; }
        public Nullable<int> PeriodOfTimeCode { get; set; }
        public string Measures { get; set; }
        public string MeasureCodes { get; set; }
        public string Drugs { get; set; }
        public string DrugCodes { get; set; }
        public string Sanitations { get; set; }
        public string SanitationCodes { get; set; }
        public string Remark { get; set; }
        public string LossDrugs { get; set; }
        public string LossDrugCodes { get; set; }
        public string LossSanitations { get; set; }
        public string LossSanitationCodes { get; set; }
        public string DiseaseIFChanges { get; set; }
        public string DiseaseChangesSupplement { get; set; }
        public string LastUpdatePerson { get; set; }
        public Nullable<System.DateTime> LastUpdateTime { get; set; }
        public string RescueXFFSMouldChoose { get; set; }
    }
}
