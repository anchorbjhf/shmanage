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
	public partial class M_PatientRecordCPR
	{
		public M_PatientRecordCPR ToPOCO(bool isPOCO = true){
			return new M_PatientRecordCPR(){
				TaskCode = this.TaskCode,
				PatientOrder = this.PatientOrder,
				Witness = this.Witness,
				CarToBeforeCPR = this.CarToBeforeCPR,
				CarToBeforeDefibrillation = this.CarToBeforeDefibrillation,
				EmergencyTime = this.EmergencyTime,
				ECGMonitoringTime = this.ECGMonitoringTime,
				CardiacArrestReason = this.CardiacArrestReason,
				CardiacArrestReasonSupplement = this.CardiacArrestReasonSupplement,
				BeforeResuscitationECGDiagnosis = this.BeforeResuscitationECGDiagnosis,
				BeforeResuscitationSaO2 = this.BeforeResuscitationSaO2,
				AfterResuscitationBP = this.AfterResuscitationBP,
				AfterResuscitationSaO2 = this.AfterResuscitationSaO2,
				PulsationAppearTime = this.PulsationAppearTime,
				BreatheAppearTime = this.BreatheAppearTime,
				AfterResuscitationECGDiagnosis = this.AfterResuscitationECGDiagnosis,
				IFAdmittedToHospital = this.IFAdmittedToHospital,
				IFLeaveHospital = this.IFLeaveHospital,
				LeaveHospitalDate = this.LeaveHospitalDate,
				LeaveHospitalCPC = this.LeaveHospitalCPC,
				VerifyResults = this.VerifyResults,
				VerifyPerson = this.VerifyPerson,
				VerifyTime = this.VerifyTime,
				RegistrationPerson = this.RegistrationPerson,
				RegistrationTime = this.RegistrationTime,
				DoctorFollowUp = this.DoctorFollowUp,
				CenterIFAuditForXFFS = this.CenterIFAuditForXFFS,
				CenterAuditPerson = this.CenterAuditPerson,
				CenterAuditTime = this.CenterAuditTime,
				CenterAuditResult = this.CenterAuditResult,
				CenterNotThroughReason = this.CenterNotThroughReason,
			};
		}
	}
}
