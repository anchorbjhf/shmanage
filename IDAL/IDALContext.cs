
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anke.SHManage.IDAL
{
	public partial interface IDALContext
    {
		II_ApplyDAL II_ApplyDAL{get;set;}
		II_ApplyDetailDAL II_ApplyDetailDAL{get;set;}
		II_BalanceDAL II_BalanceDAL{get;set;}
		II_DeliveryDAL II_DeliveryDAL{get;set;}
		II_DeliveryDetailDAL II_DeliveryDetailDAL{get;set;}
		II_EntryDAL II_EntryDAL{get;set;}
		II_EntryDetailDAL II_EntryDetailDAL{get;set;}
		II_InventoryRecordDAL II_InventoryRecordDAL{get;set;}
		II_MaterialDAL II_MaterialDAL{get;set;}
		II_StorageDAL II_StorageDAL{get;set;}
		II_StoragePersonDAL II_StoragePersonDAL{get;set;}
		II_StorageRoleDAL II_StorageRoleDAL{get;set;}
		II_SurplusDAL II_SurplusDAL{get;set;}
		IM_DictionaryDAL IM_DictionaryDAL{get;set;}
		IM_DictionaryTypeDAL IM_DictionaryTypeDAL{get;set;}
		IM_PatientChargeDAL IM_PatientChargeDAL{get;set;}
		IM_PatientRecordDAL IM_PatientRecordDAL{get;set;}
		IM_PatientRecordAppendDAL IM_PatientRecordAppendDAL{get;set;}
		IM_PatientRecordCPRDAL IM_PatientRecordCPRDAL{get;set;}
		IM_PatientRecordDiagDAL IM_PatientRecordDiagDAL{get;set;}
		IM_PatientRecordDrugDAL IM_PatientRecordDrugDAL{get;set;}
		IM_PatientRecordECGImpressionsDAL IM_PatientRecordECGImpressionsDAL{get;set;}
		IM_PatientRecordLossDrugDAL IM_PatientRecordLossDrugDAL{get;set;}
		IM_PatientRecordLossSanitationDAL IM_PatientRecordLossSanitationDAL{get;set;}
		IM_PatientRecordMeasureDAL IM_PatientRecordMeasureDAL{get;set;}
		IM_PatientRecordRescueDAL IM_PatientRecordRescueDAL{get;set;}
		IM_PatientRecordSanitationDAL IM_PatientRecordSanitationDAL{get;set;}
		IM_PatientRecordTraceDAL IM_PatientRecordTraceDAL{get;set;}
		IM_ZCaseTemplateDAL IM_ZCaseTemplateDAL{get;set;}
		IM_ZECGImpressionDAL IM_ZECGImpressionDAL{get;set;}
		IM_ZICDNewDAL IM_ZICDNewDAL{get;set;}
		IM_ZPatientStateDAL IM_ZPatientStateDAL{get;set;}
		IM_ZSymptomPendingInvestigationDAL IM_ZSymptomPendingInvestigationDAL{get;set;}
		IP_DepartmentDAL IP_DepartmentDAL{get;set;}
		IP_PermissionDAL IP_PermissionDAL{get;set;}
		IP_RoleDAL IP_RoleDAL{get;set;}
		IP_RolePermissionDAL IP_RolePermissionDAL{get;set;}
		IP_SpeicalPermissionDAL IP_SpeicalPermissionDAL{get;set;}
		IP_UserDAL IP_UserDAL{get;set;}
		IP_UserRoleDAL IP_UserRoleDAL{get;set;}
		ITDictionaryDAL ITDictionaryDAL{get;set;}
		ITDictionaryTypeDAL ITDictionaryTypeDAL{get;set;}
    }

}