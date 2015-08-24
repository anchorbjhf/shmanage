 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anke.SHManage.Model;

namespace Anke.SHManage.BLL
{
	public partial class I_ApplyBLL : BaseBLL<I_Apply>
    {
		public override void SetDAL()
		{
			idal = DALContext.II_ApplyDAL;
		}
    }

	public partial class I_ApplyDetailBLL : BaseBLL<I_ApplyDetail>
    {
		public override void SetDAL()
		{
			idal = DALContext.II_ApplyDetailDAL;
		}
    }

	public partial class I_BalanceBLL : BaseBLL<I_Balance>
    {
		public override void SetDAL()
		{
			idal = DALContext.II_BalanceDAL;
		}
    }

	public partial class I_DeliveryBLL : BaseBLL<I_Delivery>
    {
		public override void SetDAL()
		{
			idal = DALContext.II_DeliveryDAL;
		}
    }

	public partial class I_DeliveryDetailBLL : BaseBLL<I_DeliveryDetail>
    {
		public override void SetDAL()
		{
			idal = DALContext.II_DeliveryDetailDAL;
		}
    }

	public partial class I_EntryBLL : BaseBLL<I_Entry>
    {
		public override void SetDAL()
		{
			idal = DALContext.II_EntryDAL;
		}
    }

	public partial class I_EntryDetailBLL : BaseBLL<I_EntryDetail>
    {
		public override void SetDAL()
		{
			idal = DALContext.II_EntryDetailDAL;
		}
    }

	public partial class I_InventoryRecordBLL : BaseBLL<I_InventoryRecord>
    {
		public override void SetDAL()
		{
			idal = DALContext.II_InventoryRecordDAL;
		}
    }

	public partial class I_MaterialBLL : BaseBLL<I_Material>
    {
		public override void SetDAL()
		{
			idal = DALContext.II_MaterialDAL;
		}
    }

	public partial class I_StorageBLL : BaseBLL<I_Storage>
    {
		public override void SetDAL()
		{
			idal = DALContext.II_StorageDAL;
		}
    }

	public partial class I_StoragePersonBLL : BaseBLL<I_StoragePerson>
    {
		public override void SetDAL()
		{
			idal = DALContext.II_StoragePersonDAL;
		}
    }

	public partial class I_StorageRoleBLL : BaseBLL<I_StorageRole>
    {
		public override void SetDAL()
		{
			idal = DALContext.II_StorageRoleDAL;
		}
    }

	public partial class I_SurplusBLL : BaseBLL<I_Surplus>
    {
		public override void SetDAL()
		{
			idal = DALContext.II_SurplusDAL;
		}
    }

	public partial class M_DictionaryBLL : BaseBLL<M_Dictionary>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_DictionaryDAL;
		}
    }

	public partial class M_DictionaryTypeBLL : BaseBLL<M_DictionaryType>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_DictionaryTypeDAL;
		}
    }

	public partial class M_PatientChargeBLL : BaseBLL<M_PatientCharge>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_PatientChargeDAL;
		}
    }

	public partial class M_PatientRecordBLL : BaseBLL<M_PatientRecord>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_PatientRecordDAL;
		}
    }

	public partial class M_PatientRecordAppendBLL : BaseBLL<M_PatientRecordAppend>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_PatientRecordAppendDAL;
		}
    }

	public partial class M_PatientRecordCPRBLL : BaseBLL<M_PatientRecordCPR>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_PatientRecordCPRDAL;
		}
    }

	public partial class M_PatientRecordDiagBLL : BaseBLL<M_PatientRecordDiag>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_PatientRecordDiagDAL;
		}
    }

	public partial class M_PatientRecordDrugBLL : BaseBLL<M_PatientRecordDrug>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_PatientRecordDrugDAL;
		}
    }

	public partial class M_PatientRecordECGImpressionsBLL : BaseBLL<M_PatientRecordECGImpressions>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_PatientRecordECGImpressionsDAL;
		}
    }

	public partial class M_PatientRecordLossDrugBLL : BaseBLL<M_PatientRecordLossDrug>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_PatientRecordLossDrugDAL;
		}
    }

	public partial class M_PatientRecordLossSanitationBLL : BaseBLL<M_PatientRecordLossSanitation>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_PatientRecordLossSanitationDAL;
		}
    }

	public partial class M_PatientRecordMeasureBLL : BaseBLL<M_PatientRecordMeasure>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_PatientRecordMeasureDAL;
		}
    }

	public partial class M_PatientRecordRescueBLL : BaseBLL<M_PatientRecordRescue>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_PatientRecordRescueDAL;
		}
    }

	public partial class M_PatientRecordSanitationBLL : BaseBLL<M_PatientRecordSanitation>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_PatientRecordSanitationDAL;
		}
    }

	public partial class M_PatientRecordTraceBLL : BaseBLL<M_PatientRecordTrace>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_PatientRecordTraceDAL;
		}
    }

	public partial class M_ZCaseTemplateBLL : BaseBLL<M_ZCaseTemplate>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_ZCaseTemplateDAL;
		}
    }

	public partial class M_ZECGImpressionBLL : BaseBLL<M_ZECGImpression>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_ZECGImpressionDAL;
		}
    }

	public partial class M_ZICDNewBLL : BaseBLL<M_ZICDNew>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_ZICDNewDAL;
		}
    }

	public partial class M_ZPatientStateBLL : BaseBLL<M_ZPatientState>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_ZPatientStateDAL;
		}
    }

	public partial class M_ZSymptomPendingInvestigationBLL : BaseBLL<M_ZSymptomPendingInvestigation>
    {
		public override void SetDAL()
		{
			idal = DALContext.IM_ZSymptomPendingInvestigationDAL;
		}
    }

	public partial class P_DepartmentBLL : BaseBLL<P_Department>
    {
		public override void SetDAL()
		{
			idal = DALContext.IP_DepartmentDAL;
		}
    }

	public partial class P_PermissionBLL : BaseBLL<P_Permission>
    {
		public override void SetDAL()
		{
			idal = DALContext.IP_PermissionDAL;
		}
    }

	public partial class P_RoleBLL : BaseBLL<P_Role>
    {
		public override void SetDAL()
		{
			idal = DALContext.IP_RoleDAL;
		}
    }

	public partial class P_RolePermissionBLL : BaseBLL<P_RolePermission>
    {
		public override void SetDAL()
		{
			idal = DALContext.IP_RolePermissionDAL;
		}
    }

	public partial class P_SpeicalPermissionBLL : BaseBLL<P_SpeicalPermission>
    {
		public override void SetDAL()
		{
			idal = DALContext.IP_SpeicalPermissionDAL;
		}
    }

	public partial class P_UserBLL : BaseBLL<P_User>
    {
		public override void SetDAL()
		{
			idal = DALContext.IP_UserDAL;
		}
    }

	public partial class P_UserRoleBLL : BaseBLL<P_UserRole>
    {
		public override void SetDAL()
		{
			idal = DALContext.IP_UserRoleDAL;
		}
    }

	public partial class TDictionaryBLL : BaseBLL<TDictionary>
    {
		public override void SetDAL()
		{
			idal = DALContext.ITDictionaryDAL;
		}
    }

	public partial class TDictionaryTypeBLL : BaseBLL<TDictionaryType>
    {
		public override void SetDAL()
		{
			idal = DALContext.ITDictionaryTypeDAL;
		}
    }

}