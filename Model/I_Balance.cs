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
    
    public partial class I_Balance
    {
        public int MaterialID { get; set; }
        public string ReportTime { get; set; }
        public double BeginningCounts { get; set; }
        public decimal BeginningPrice { get; set; }
        public double IncomeCounts { get; set; }
        public decimal IncomePrice { get; set; }
        public double PayCounts { get; set; }
        public decimal PayPrice { get; set; }
        public decimal UpdataPrice { get; set; }
        public double SurplusCounts { get; set; }
        public decimal SurplusPrice { get; set; }
        public System.DateTime SurplusTime { get; set; }
    
        public virtual I_Material I_Material { get; set; }
    }
}