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
	public partial class I_Delivery
	{
		public I_Delivery ToPOCO(bool isPOCO = true){
			return new I_Delivery(){
				DeliveryCode = this.DeliveryCode,
				ConsigneeID = this.ConsigneeID,
				DeliveryTime = this.DeliveryTime,
				Remark = this.Remark,
				OperatorCode = this.OperatorCode,
				OperationTime = this.OperationTime,
				ReceivingStoreID = this.ReceivingStoreID,
				DeliveryType = this.DeliveryType,
			};
		}
	}
}