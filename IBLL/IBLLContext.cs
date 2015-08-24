
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anke.SHManage.IBLL
{
	public partial interface IBLLContext
    {
		II_ApplyBLL II_ApplyBLL{get;set;}
		II_ApplyDetailBLL II_ApplyDetailBLL{get;set;}
		II_BalanceBLL II_BalanceBLL{get;set;}
		II_DeliveryBLL II_DeliveryBLL{get;set;}
		II_DeliveryDetailBLL II_DeliveryDetailBLL{get;set;}
		II_EntryBLL II_EntryBLL{get;set;}
		II_EntryDetailBLL II_EntryDetailBLL{get;set;}
		II_InventoryRecordBLL II_InventoryRecordBLL{get;set;}
		II_MaterialBLL II_MaterialBLL{get;set;}
		II_StorageBLL II_StorageBLL{get;set;}
		II_StoragePersonBLL II_StoragePersonBLL{get;set;}
		II_StorageRoleBLL II_StorageRoleBLL{get;set;}
		II_SurplusBLL II_SurplusBLL{get;set;}
		IM_DictionaryBLL IM_DictionaryBLL{get;set;}
		IM_DictionaryTypeBLL IM_DictionaryTypeBLL{get;set;}
		IM_PatientChargeBLL IM_PatientChargeBLL{get;set;}
		IM_PatientRecordBLL IM_PatientRecordBLL{get;set;}
		IM_PatientRecordAppendBLL IM_PatientRecordAppendBLL{get;set;}
		IM_PatientRecordCPRBLL IM_PatientRecordCPRBLL{get;set;}
		IM_PatientRecordDiagBLL IM_PatientRecordDiagBLL{get;set;}
		IM_PatientRecordDrugBLL IM_PatientRecordDrugBLL{get;set;}
		IM_PatientRecordECGImpressionsBLL IM_PatientRecordECGImpressionsBLL{get;set;}
		IM_PatientRecordLossDrugBLL IM_PatientRecordLossDrugBLL{get;set;}
		IM_PatientRecordLossSanitationBLL IM_PatientRecordLossSanitationBLL{get;set;}
		IM_PatientRecordMeasureBLL IM_PatientRecordMeasureBLL{get;set;}
		IM_PatientRecordRescueBLL IM_PatientRecordRescueBLL{get;set;}
		IM_PatientRecordSanitationBLL IM_PatientRecordSanitationBLL{get;set;}
		IM_PatientRecordTraceBLL IM_PatientRecordTraceBLL{get;set;}
		IM_ZCaseTemplateBLL IM_ZCaseTemplateBLL{get;set;}
		IM_ZECGImpressionBLL IM_ZECGImpressionBLL{get;set;}
		IM_ZICDNewBLL IM_ZICDNewBLL{get;set;}
		IM_ZPatientStateBLL IM_ZPatientStateBLL{get;set;}
		IM_ZSymptomPendingInvestigationBLL IM_ZSymptomPendingInvestigationBLL{get;set;}
		IP_DepartmentBLL IP_DepartmentBLL{get;set;}
		IP_PermissionBLL IP_PermissionBLL{get;set;}
		IP_RoleBLL IP_RoleBLL{get;set;}
		IP_RolePermissionBLL IP_RolePermissionBLL{get;set;}
		IP_SpeicalPermissionBLL IP_SpeicalPermissionBLL{get;set;}
		IP_UserBLL IP_UserBLL{get;set;}
		IP_UserRoleBLL IP_UserRoleBLL{get;set;}
		ITDictionaryBLL ITDictionaryBLL{get;set;}
		ITDictionaryTypeBLL ITDictionaryTypeBLL{get;set;}
    }

}