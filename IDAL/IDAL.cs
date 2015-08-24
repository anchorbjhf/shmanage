
 
 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anke.SHManage.Model;


namespace Anke.SHManage.IDAL
{
	public partial interface II_ApplyDAL : IBaseDAL<I_Apply>
    {
    }

	public partial interface II_ApplyDetailDAL : IBaseDAL<I_ApplyDetail>
    {
    }

	public partial interface II_BalanceDAL : IBaseDAL<I_Balance>
    {
    }
    public partial interface II_BalanceListDAL : IBaseDAL<I_BalanceList>
    {
    }

	public partial interface II_DeliveryDAL : IBaseDAL<I_Delivery>
    {
    }

	public partial interface II_DeliveryDetailDAL : IBaseDAL<I_DeliveryDetail>
    {
    }

	public partial interface II_EntryDAL : IBaseDAL<I_Entry>
    {
    }

	public partial interface II_EntryDetailDAL : IBaseDAL<I_EntryDetail>
    {
    }

	public partial interface II_InventoryRecordDAL : IBaseDAL<I_InventoryRecord>
    {
    }

	public partial interface II_MaterialDAL : IBaseDAL<I_Material>
    {
    }

	public partial interface II_StorageDAL : IBaseDAL<I_Storage>
    {
    }

	public partial interface II_StoragePersonDAL : IBaseDAL<I_StoragePerson>
    {
    }

	public partial interface II_StorageRoleDAL : IBaseDAL<I_StorageRole>
    {
    }

	public partial interface II_SurplusDAL : IBaseDAL<I_Surplus>
    {
    }

	public partial interface IM_DictionaryDAL : IBaseDAL<M_Dictionary>
    {
    }

	public partial interface IM_DictionaryTypeDAL : IBaseDAL<M_DictionaryType>
    {
    }

	public partial interface IM_PatientChargeDAL : IBaseDAL<M_PatientCharge>
    {
    }

	public partial interface IM_PatientRecordDAL : IBaseDAL<M_PatientRecord>
    {
    }

	public partial interface IM_PatientRecordAppendDAL : IBaseDAL<M_PatientRecordAppend>
    {
    }

	public partial interface IM_PatientRecordCPRDAL : IBaseDAL<M_PatientRecordCPR>
    {
    }

	public partial interface IM_PatientRecordDiagDAL : IBaseDAL<M_PatientRecordDiag>
    {
    }

	public partial interface IM_PatientRecordDrugDAL : IBaseDAL<M_PatientRecordDrug>
    {
    }

	public partial interface IM_PatientRecordECGImpressionsDAL : IBaseDAL<M_PatientRecordECGImpressions>
    {
    }

	public partial interface IM_PatientRecordLossDrugDAL : IBaseDAL<M_PatientRecordLossDrug>
    {
    }

	public partial interface IM_PatientRecordLossSanitationDAL : IBaseDAL<M_PatientRecordLossSanitation>
    {
    }

	public partial interface IM_PatientRecordMeasureDAL : IBaseDAL<M_PatientRecordMeasure>
    {
    }

	public partial interface IM_PatientRecordRescueDAL : IBaseDAL<M_PatientRecordRescue>
    {
    }

	public partial interface IM_PatientRecordSanitationDAL : IBaseDAL<M_PatientRecordSanitation>
    {
    }

	public partial interface IM_PatientRecordTraceDAL : IBaseDAL<M_PatientRecordTrace>
    {
    }

	public partial interface IM_ZCaseTemplateDAL : IBaseDAL<M_ZCaseTemplate>
    {
    }

	public partial interface IM_ZECGImpressionDAL : IBaseDAL<M_ZECGImpression>
    {
    }

	public partial interface IM_ZICDNewDAL : IBaseDAL<M_ZICDNew>
    {
    }

	public partial interface IM_ZPatientStateDAL : IBaseDAL<M_ZPatientState>
    {
    }

	public partial interface IM_ZSymptomPendingInvestigationDAL : IBaseDAL<M_ZSymptomPendingInvestigation>
    {
    }

	public partial interface IP_DepartmentDAL : IBaseDAL<P_Department>
    {
    }

	public partial interface IP_PermissionDAL : IBaseDAL<P_Permission>
    {
    }

	public partial interface IP_RoleDAL : IBaseDAL<P_Role>
    {
    }

	public partial interface IP_RolePermissionDAL : IBaseDAL<P_RolePermission>
    {
    }

	public partial interface IP_SpeicalPermissionDAL : IBaseDAL<P_SpeicalPermission>
    {
    }

	public partial interface IP_UserDAL : IBaseDAL<P_User>
    {
    }

	public partial interface IP_UserRoleDAL : IBaseDAL<P_UserRole>
    {
    }

	public partial interface ITDictionaryDAL : IBaseDAL<TDictionary>
    {
    }

	public partial interface ITDictionaryTypeDAL : IBaseDAL<TDictionaryType>
    {
    }


}