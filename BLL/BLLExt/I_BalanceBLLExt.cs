using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anke.SHManage.Model;

using Anke.SHManage.MSSQLDAL;

namespace Anke.SHManage.BLL
{
    public partial class I_BalanceBLLExt
    {
        private I_BalanceDALExt dal = new I_BalanceDALExt();

        public object GetBalanceList(int page, int rows, string monthtime, string Type, ref int rowCounts)
        {
            return dal.GetBalanceList(page, rows, monthtime, Type, ref rowCounts);
        }
        /// <summary>
        /// 根据物资类型查询产生的最大统计报表月份
        /// </summary>
        /// <param name="MTypeID"></param>
        /// <returns></returns>
        public bool GetBalanceMax(string MTypeID, ref string maxstr)
        {
            string temp = dal.GetBalanceMax(MTypeID);
            if (temp != "" && temp != null)
            {
                int mm = Convert.ToInt32(temp.Substring(4, 2));
                int year = Convert.ToInt32(temp.Substring(0, 4));
                if (mm == 12)
                {
                    maxstr = (year + 1).ToString() + "01";
                }
                else
                {
                    int tempMM = mm + 1;
                    if (tempMM < 10)
                        maxstr = year.ToString() + "0" + tempMM.ToString();
                    else
                        maxstr = year.ToString() + tempMM.ToString();
                }
                return true;
            }
            else
            {
                int mm = DateTime.Now.Month;
                int year = DateTime.Now.Year;
                if (mm == 1)
                {
                    maxstr = (year - 1).ToString() + "12";
                }
                else
                {
                    int tempMM = mm - 1;
                    if (tempMM < 10)
                        maxstr = year.ToString() + "0" + tempMM.ToString();
                    else
                        maxstr = year.ToString() + tempMM.ToString();
                }
                return false;
            }
        }
        /// <summary>
        /// 根据月份查询财务统计报表
        /// </summary>
        /// <param name="reportTime"></param>
        /// <param name="MTypeID"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public List<I_Balance> GetI_BalanceNewList(string reportTime, string MTypeID, ref string errorMsg)
        {
            int mm = Convert.ToInt32(reportTime.Substring(4, 2));
            int year = Convert.ToInt32(reportTime.Substring(0, 4));
            int nextMM, nextYear, lastYear, lastMM;
            if (mm == 12)
            {
                nextMM = 1;
                nextYear = year + 1;
            }
            else
            {
                nextMM = mm + 1;
                nextYear = year;
            }
            if (mm == 1)
            {
                lastMM = 12;
                lastYear = year - 1;
            }
            else
            {
                lastMM = mm - 1;
                lastYear = year;
            }
            string tempReportTime = "";
            if (lastMM < 10)
                tempReportTime = lastYear.ToString() + "0" + lastMM.ToString();
            else
                tempReportTime = lastYear.ToString() + lastMM.ToString();

            DateTime beginTime = Convert.ToDateTime(year.ToString() + "-" + mm.ToString() + "-1 00:00:00");
            DateTime endTime = Convert.ToDateTime(nextYear.ToString() + "-" + nextMM.ToString() + "-1 00:00:00");
            return dal.GetI_BalanceNewList(tempReportTime, reportTime, MTypeID, beginTime, endTime, ref errorMsg);
        }
        /// <summary>
        /// 插入I_Balance数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool AddBalance(List<I_Balance> list, string mType, string reportTime, ref string errorMsg)
        {
            I_BalanceList blInfo = new I_BalanceList();
            blInfo.MaterialTypeID = mType;
            blInfo.ReportTime = reportTime;
            return dal.AddBalance(list, blInfo, ref errorMsg);
        }
    }


}


