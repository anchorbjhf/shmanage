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
    
    public partial class I_StorageRole
    {
        public int ID { get; set; }
        public int RoleID { get; set; }
        public string MaterialType { get; set; }
    
        public virtual P_Role P_Role { get; set; }
        public virtual TDictionary TDictionary { get; set; }
    }
}
