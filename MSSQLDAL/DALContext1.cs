
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anke.SHManage.IDAL;

namespace Anke.SHManage.MSSQLDAL
{
	public partial class DALContext:IDALContext
    {

		II_ApplyDAL iI_ApplyDAL;
		public II_ApplyDAL II_ApplyDAL
		{
			get
			{
				if(iI_ApplyDAL==null)
					iI_ApplyDAL= new I_ApplyDAL();
				return iI_ApplyDAL;
			}
			set
			{
				iI_ApplyDAL= value;
			}
		}


		II_ApplyDetailDAL iI_ApplyDetailDAL;
		public II_ApplyDetailDAL II_ApplyDetailDAL
		{
			get
			{
				if(iI_ApplyDetailDAL==null)
					iI_ApplyDetailDAL= new I_ApplyDetailDAL();
				return iI_ApplyDetailDAL;
			}
			set
			{
				iI_ApplyDetailDAL= value;
			}
		}


		II_BalanceDAL iI_BalanceDAL;
		public II_BalanceDAL II_BalanceDAL
		{
			get
			{
				if(iI_BalanceDAL==null)
					iI_BalanceDAL= new I_BalanceDAL();
				return iI_BalanceDAL;
			}
			set
			{
				iI_BalanceDAL= value;
			}
		}


		II_DeliveryDAL iI_DeliveryDAL;
		public II_DeliveryDAL II_DeliveryDAL
		{
			get
			{
				if(iI_DeliveryDAL==null)
					iI_DeliveryDAL= new I_DeliveryDAL();
				return iI_DeliveryDAL;
			}
			set
			{
				iI_DeliveryDAL= value;
			}
		}


		II_DeliveryDetailDAL iI_DeliveryDetailDAL;
		public II_DeliveryDetailDAL II_DeliveryDetailDAL
		{
			get
			{
				if(iI_DeliveryDetailDAL==null)
					iI_DeliveryDetailDAL= new I_DeliveryDetailDAL();
				return iI_DeliveryDetailDAL;
			}
			set
			{
				iI_DeliveryDetailDAL= value;
			}
		}


		II_EntryDAL iI_EntryDAL;
		public II_EntryDAL II_EntryDAL
		{
			get
			{
				if(iI_EntryDAL==null)
					iI_EntryDAL= new I_EntryDAL();
				return iI_EntryDAL;
			}
			set
			{
				iI_EntryDAL= value;
			}
		}


		II_EntryDetailDAL iI_EntryDetailDAL;
		public II_EntryDetailDAL II_EntryDetailDAL
		{
			get
			{
				if(iI_EntryDetailDAL==null)
					iI_EntryDetailDAL= new I_EntryDetailDAL();
				return iI_EntryDetailDAL;
			}
			set
			{
				iI_EntryDetailDAL= value;
			}
		}


		II_InventoryRecordDAL iI_InventoryRecordDAL;
		public II_InventoryRecordDAL II_InventoryRecordDAL
		{
			get
			{
				if(iI_InventoryRecordDAL==null)
					iI_InventoryRecordDAL= new I_InventoryRecordDAL();
				return iI_InventoryRecordDAL;
			}
			set
			{
				iI_InventoryRecordDAL= value;
			}
		}


		II_MaterialDAL iI_MaterialDAL;
		public II_MaterialDAL II_MaterialDAL
		{
			get
			{
				if(iI_MaterialDAL==null)
					iI_MaterialDAL= new I_MaterialDAL();
				return iI_MaterialDAL;
			}
			set
			{
				iI_MaterialDAL= value;
			}
		}


		II_StorageDAL iI_StorageDAL;
		public II_StorageDAL II_StorageDAL
		{
			get
			{
				if(iI_StorageDAL==null)
					iI_StorageDAL= new I_StorageDAL();
				return iI_StorageDAL;
			}
			set
			{
				iI_StorageDAL= value;
			}
		}


		II_StoragePersonDAL iI_StoragePersonDAL;
		public II_StoragePersonDAL II_StoragePersonDAL
		{
			get
			{
				if(iI_StoragePersonDAL==null)
					iI_StoragePersonDAL= new I_StoragePersonDAL();
				return iI_StoragePersonDAL;
			}
			set
			{
				iI_StoragePersonDAL= value;
			}
		}


		II_StorageRoleDAL iI_StorageRoleDAL;
		public II_StorageRoleDAL II_StorageRoleDAL
		{
			get
			{
				if(iI_StorageRoleDAL==null)
					iI_StorageRoleDAL= new I_StorageRoleDAL();
				return iI_StorageRoleDAL;
			}
			set
			{
				iI_StorageRoleDAL= value;
			}
		}


		II_SurplusDAL iI_SurplusDAL;
		public II_SurplusDAL II_SurplusDAL
		{
			get
			{
				if(iI_SurplusDAL==null)
					iI_SurplusDAL= new I_SurplusDAL();
				return iI_SurplusDAL;
			}
			set
			{
				iI_SurplusDAL= value;
			}
		}


		IM_DictionaryDAL iM_DictionaryDAL;
		public IM_DictionaryDAL IM_DictionaryDAL
		{
			get
			{
				if(iM_DictionaryDAL==null)
					iM_DictionaryDAL= new M_DictionaryDAL();
				return iM_DictionaryDAL;
			}
			set
			{
				iM_DictionaryDAL= value;
			}
		}


		IM_DictionaryTypeDAL iM_DictionaryTypeDAL;
		public IM_DictionaryTypeDAL IM_DictionaryTypeDAL
		{
			get
			{
				if(iM_DictionaryTypeDAL==null)
					iM_DictionaryTypeDAL= new M_DictionaryTypeDAL();
				return iM_DictionaryTypeDAL;
			}
			set
			{
				iM_DictionaryTypeDAL= value;
			}
		}


		IM_PatientChargeDAL iM_PatientChargeDAL;
		public IM_PatientChargeDAL IM_PatientChargeDAL
		{
			get
			{
				if(iM_PatientChargeDAL==null)
					iM_PatientChargeDAL= new M_PatientChargeDAL();
				return iM_PatientChargeDAL;
			}
			set
			{
				iM_PatientChargeDAL= value;
			}
		}


		IM_PatientRecordDAL iM_PatientRecordDAL;
		public IM_PatientRecordDAL IM_PatientRecordDAL
		{
			get
			{
				if(iM_PatientRecordDAL==null)
					iM_PatientRecordDAL= new M_PatientRecordDAL();
				return iM_PatientRecordDAL;
			}
			set
			{
				iM_PatientRecordDAL= value;
			}
		}


		IM_PatientRecordAppendDAL iM_PatientRecordAppendDAL;
		public IM_PatientRecordAppendDAL IM_PatientRecordAppendDAL
		{
			get
			{
				if(iM_PatientRecordAppendDAL==null)
					iM_PatientRecordAppendDAL= new M_PatientRecordAppendDAL();
				return iM_PatientRecordAppendDAL;
			}
			set
			{
				iM_PatientRecordAppendDAL= value;
			}
		}


		IM_PatientRecordCPRDAL iM_PatientRecordCPRDAL;
		public IM_PatientRecordCPRDAL IM_PatientRecordCPRDAL
		{
			get
			{
				if(iM_PatientRecordCPRDAL==null)
					iM_PatientRecordCPRDAL= new M_PatientRecordCPRDAL();
				return iM_PatientRecordCPRDAL;
			}
			set
			{
				iM_PatientRecordCPRDAL= value;
			}
		}


		IM_PatientRecordDiagDAL iM_PatientRecordDiagDAL;
		public IM_PatientRecordDiagDAL IM_PatientRecordDiagDAL
		{
			get
			{
				if(iM_PatientRecordDiagDAL==null)
					iM_PatientRecordDiagDAL= new M_PatientRecordDiagDAL();
				return iM_PatientRecordDiagDAL;
			}
			set
			{
				iM_PatientRecordDiagDAL= value;
			}
		}


		IM_PatientRecordDrugDAL iM_PatientRecordDrugDAL;
		public IM_PatientRecordDrugDAL IM_PatientRecordDrugDAL
		{
			get
			{
				if(iM_PatientRecordDrugDAL==null)
					iM_PatientRecordDrugDAL= new M_PatientRecordDrugDAL();
				return iM_PatientRecordDrugDAL;
			}
			set
			{
				iM_PatientRecordDrugDAL= value;
			}
		}


		IM_PatientRecordECGImpressionsDAL iM_PatientRecordECGImpressionsDAL;
		public IM_PatientRecordECGImpressionsDAL IM_PatientRecordECGImpressionsDAL
		{
			get
			{
				if(iM_PatientRecordECGImpressionsDAL==null)
					iM_PatientRecordECGImpressionsDAL= new M_PatientRecordECGImpressionsDAL();
				return iM_PatientRecordECGImpressionsDAL;
			}
			set
			{
				iM_PatientRecordECGImpressionsDAL= value;
			}
		}


		IM_PatientRecordLossDrugDAL iM_PatientRecordLossDrugDAL;
		public IM_PatientRecordLossDrugDAL IM_PatientRecordLossDrugDAL
		{
			get
			{
				if(iM_PatientRecordLossDrugDAL==null)
					iM_PatientRecordLossDrugDAL= new M_PatientRecordLossDrugDAL();
				return iM_PatientRecordLossDrugDAL;
			}
			set
			{
				iM_PatientRecordLossDrugDAL= value;
			}
		}


		IM_PatientRecordLossSanitationDAL iM_PatientRecordLossSanitationDAL;
		public IM_PatientRecordLossSanitationDAL IM_PatientRecordLossSanitationDAL
		{
			get
			{
				if(iM_PatientRecordLossSanitationDAL==null)
					iM_PatientRecordLossSanitationDAL= new M_PatientRecordLossSanitationDAL();
				return iM_PatientRecordLossSanitationDAL;
			}
			set
			{
				iM_PatientRecordLossSanitationDAL= value;
			}
		}


		IM_PatientRecordMeasureDAL iM_PatientRecordMeasureDAL;
		public IM_PatientRecordMeasureDAL IM_PatientRecordMeasureDAL
		{
			get
			{
				if(iM_PatientRecordMeasureDAL==null)
					iM_PatientRecordMeasureDAL= new M_PatientRecordMeasureDAL();
				return iM_PatientRecordMeasureDAL;
			}
			set
			{
				iM_PatientRecordMeasureDAL= value;
			}
		}


		IM_PatientRecordRescueDAL iM_PatientRecordRescueDAL;
		public IM_PatientRecordRescueDAL IM_PatientRecordRescueDAL
		{
			get
			{
				if(iM_PatientRecordRescueDAL==null)
					iM_PatientRecordRescueDAL= new M_PatientRecordRescueDAL();
				return iM_PatientRecordRescueDAL;
			}
			set
			{
				iM_PatientRecordRescueDAL= value;
			}
		}


		IM_PatientRecordSanitationDAL iM_PatientRecordSanitationDAL;
		public IM_PatientRecordSanitationDAL IM_PatientRecordSanitationDAL
		{
			get
			{
				if(iM_PatientRecordSanitationDAL==null)
					iM_PatientRecordSanitationDAL= new M_PatientRecordSanitationDAL();
				return iM_PatientRecordSanitationDAL;
			}
			set
			{
				iM_PatientRecordSanitationDAL= value;
			}
		}


		IM_PatientRecordTraceDAL iM_PatientRecordTraceDAL;
		public IM_PatientRecordTraceDAL IM_PatientRecordTraceDAL
		{
			get
			{
				if(iM_PatientRecordTraceDAL==null)
					iM_PatientRecordTraceDAL= new M_PatientRecordTraceDAL();
				return iM_PatientRecordTraceDAL;
			}
			set
			{
				iM_PatientRecordTraceDAL= value;
			}
		}


		IM_ZCaseTemplateDAL iM_ZCaseTemplateDAL;
		public IM_ZCaseTemplateDAL IM_ZCaseTemplateDAL
		{
			get
			{
				if(iM_ZCaseTemplateDAL==null)
					iM_ZCaseTemplateDAL= new M_ZCaseTemplateDAL();
				return iM_ZCaseTemplateDAL;
			}
			set
			{
				iM_ZCaseTemplateDAL= value;
			}
		}


		IM_ZECGImpressionDAL iM_ZECGImpressionDAL;
		public IM_ZECGImpressionDAL IM_ZECGImpressionDAL
		{
			get
			{
				if(iM_ZECGImpressionDAL==null)
					iM_ZECGImpressionDAL= new M_ZECGImpressionDAL();
				return iM_ZECGImpressionDAL;
			}
			set
			{
				iM_ZECGImpressionDAL= value;
			}
		}


		IM_ZICDNewDAL iM_ZICDNewDAL;
		public IM_ZICDNewDAL IM_ZICDNewDAL
		{
			get
			{
				if(iM_ZICDNewDAL==null)
					iM_ZICDNewDAL= new M_ZICDNewDAL();
				return iM_ZICDNewDAL;
			}
			set
			{
				iM_ZICDNewDAL= value;
			}
		}


		IM_ZPatientStateDAL iM_ZPatientStateDAL;
		public IM_ZPatientStateDAL IM_ZPatientStateDAL
		{
			get
			{
				if(iM_ZPatientStateDAL==null)
					iM_ZPatientStateDAL= new M_ZPatientStateDAL();
				return iM_ZPatientStateDAL;
			}
			set
			{
				iM_ZPatientStateDAL= value;
			}
		}


		IM_ZSymptomPendingInvestigationDAL iM_ZSymptomPendingInvestigationDAL;
		public IM_ZSymptomPendingInvestigationDAL IM_ZSymptomPendingInvestigationDAL
		{
			get
			{
				if(iM_ZSymptomPendingInvestigationDAL==null)
					iM_ZSymptomPendingInvestigationDAL= new M_ZSymptomPendingInvestigationDAL();
				return iM_ZSymptomPendingInvestigationDAL;
			}
			set
			{
				iM_ZSymptomPendingInvestigationDAL= value;
			}
		}


		IP_DepartmentDAL iP_DepartmentDAL;
		public IP_DepartmentDAL IP_DepartmentDAL
		{
			get
			{
				if(iP_DepartmentDAL==null)
					iP_DepartmentDAL= new P_DepartmentDAL();
				return iP_DepartmentDAL;
			}
			set
			{
				iP_DepartmentDAL= value;
			}
		}


		IP_PermissionDAL iP_PermissionDAL;
		public IP_PermissionDAL IP_PermissionDAL
		{
			get
			{
				if(iP_PermissionDAL==null)
					iP_PermissionDAL= new P_PermissionDAL();
				return iP_PermissionDAL;
			}
			set
			{
				iP_PermissionDAL= value;
			}
		}


		IP_RoleDAL iP_RoleDAL;
		public IP_RoleDAL IP_RoleDAL
		{
			get
			{
				if(iP_RoleDAL==null)
					iP_RoleDAL= new P_RoleDAL();
				return iP_RoleDAL;
			}
			set
			{
				iP_RoleDAL= value;
			}
		}


		IP_RolePermissionDAL iP_RolePermissionDAL;
		public IP_RolePermissionDAL IP_RolePermissionDAL
		{
			get
			{
				if(iP_RolePermissionDAL==null)
					iP_RolePermissionDAL= new P_RolePermissionDAL();
				return iP_RolePermissionDAL;
			}
			set
			{
				iP_RolePermissionDAL= value;
			}
		}


		IP_SpeicalPermissionDAL iP_SpeicalPermissionDAL;
		public IP_SpeicalPermissionDAL IP_SpeicalPermissionDAL
		{
			get
			{
				if(iP_SpeicalPermissionDAL==null)
					iP_SpeicalPermissionDAL= new P_SpeicalPermissionDAL();
				return iP_SpeicalPermissionDAL;
			}
			set
			{
				iP_SpeicalPermissionDAL= value;
			}
		}


		IP_UserDAL iP_UserDAL;
		public IP_UserDAL IP_UserDAL
		{
			get
			{
				if(iP_UserDAL==null)
					iP_UserDAL= new P_UserDAL();
				return iP_UserDAL;
			}
			set
			{
				iP_UserDAL= value;
			}
		}


		IP_UserRoleDAL iP_UserRoleDAL;
		public IP_UserRoleDAL IP_UserRoleDAL
		{
			get
			{
				if(iP_UserRoleDAL==null)
					iP_UserRoleDAL= new P_UserRoleDAL();
				return iP_UserRoleDAL;
			}
			set
			{
				iP_UserRoleDAL= value;
			}
		}


		ITDictionaryDAL iTDictionaryDAL;
		public ITDictionaryDAL ITDictionaryDAL
		{
			get
			{
				if(iTDictionaryDAL==null)
					iTDictionaryDAL= new TDictionaryDAL();
				return iTDictionaryDAL;
			}
			set
			{
				iTDictionaryDAL= value;
			}
		}


		ITDictionaryTypeDAL iTDictionaryTypeDAL;
		public ITDictionaryTypeDAL ITDictionaryTypeDAL
		{
			get
			{
				if(iTDictionaryTypeDAL==null)
					iTDictionaryTypeDAL= new TDictionaryTypeDAL();
				return iTDictionaryTypeDAL;
			}
			set
			{
				iTDictionaryTypeDAL= value;
			}
		}

    }

}