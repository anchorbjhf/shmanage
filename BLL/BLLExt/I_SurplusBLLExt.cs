using Anke.SHManage.Model;
using Anke.SHManage.MSSQLDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{
    public partial class I_SurplusBLL : BaseBLL<I_Surplus>
    {
        private I_SurplusDAL dal = new I_SurplusDAL();
        public object GetSurplusList(int page, int rows, ref int rowCounts, string materialID, List<int> listStorageCode, List<string> listMaterialType, int alarmCounts = -1, int overDays = 0, bool isShowOver = true)
        {
            return dal.GetSurplusList(page, rows, ref rowCounts, materialID, listStorageCode, listMaterialType, alarmCounts, overDays, isShowOver);
        }
        public object GetLastDaySurplusList(int page, int rows, ref int rowCounts, string materialID, List<int> listStorageCode, List<string> listMaterialType)
        {
            return dal.GetLastDaySurplusList(page, rows, ref rowCounts, materialID, listStorageCode, listMaterialType);
        }

        public object GetSurplusListGroupBy(int page, int rows, ref int rowCounts, string materialID, List<int> listStorageCode, List<string> listMaterialType)
        {
            return dal.GetSurplusListGroupBy(page, rows, ref rowCounts, materialID, listStorageCode, listMaterialType);
        }

    }
}
