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
	public partial class I_ApplyDetail
	{
		public I_ApplyDetail ToPOCO(bool isPOCO = true){
			return new I_ApplyDetail(){
				ApplyDetailCode = this.ApplyDetailCode,
				ApplyCode = this.ApplyCode,
				MaterialID = this.MaterialID,
				RealBatchNo = this.RealBatchNo,
				BatchNo = this.BatchNo,
				ApplyTime = this.ApplyTime,
				ApplyCounts = this.ApplyCounts,
				ApplyUserID = this.ApplyUserID,
				SelfStorageCode = this.SelfStorageCode,
				ApplyTargetStorageCode = this.ApplyTargetStorageCode,
				Remark = this.Remark,
				ApprovalCounts = this.ApprovalCounts,
			};
		}
	}
}
