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
	public partial class TDictionary
	{
		public TDictionary ToPOCO(bool isPOCO = true){
			return new TDictionary(){
				ID = this.ID,
				Name = this.Name,
				TypeID = this.TypeID,
				SN = this.SN,
				IsActive = this.IsActive,
				ParentID = this.ParentID,
			};
		}
	}
}
