//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Anke.SHManage.Model
{
	public partial class M_ZECGImpression
	{
		public M_ZECGImpression ToPOCO(bool isPOCO = true){
			return new M_ZECGImpression(){
				ID = this.ID,
				Name = this.Name,
				ParentID = this.ParentID,
				SN = this.SN,
				IsActive = this.IsActive,
				PinYin = this.PinYin,
			};
		}
	}
}