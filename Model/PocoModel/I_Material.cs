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
	public partial class I_Material
	{
		public I_Material ToPOCO(bool isPOCO = true){
			return new I_Material(){
				ID = this.ID,
				Name = this.Name,
				MTypeID = this.MTypeID,
				OtherTypeID = this.OtherTypeID,
				Manufacturer = this.Manufacturer,
				Vendor = this.Vendor,
				Unit = this.Unit,
				Specification = this.Specification,
				QRCode = this.QRCode,
				Remark = this.Remark,
				CreatorName = this.CreatorName,
				CreatorDate = this.CreatorDate,
				PinYin = this.PinYin,
				IsActive = this.IsActive,
				RealPrice = this.RealPrice,
				AlarmCounts = this.AlarmCounts,
				MCode = this.MCode,
				FeeScale = this.FeeScale,
				GiveMedicineWay = this.GiveMedicineWay,
				SN = this.SN,
				LimitMaxPrice = this.LimitMaxPrice,
				Other1 = this.Other1,
				Other2 = this.Other2,
			};
		}
	}
}
