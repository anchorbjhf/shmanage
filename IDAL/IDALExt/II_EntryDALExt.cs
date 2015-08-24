using Anke.SHManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.IDAL
{
    public partial interface II_EntryDAL
    {

        /// <summary>
        /// 获取入库主表信息
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="EntryCode"></param>
        /// <param name="EntryType"></param>
        /// <returns></returns>
        object GetEntryList(int page, int rows, ref int rowCounts, DateTime startTime, DateTime endTime, string EntryCode, string EntryType,List<int> listEntryStorageIDs);



        /// <summary>
        /// 入库操作
        /// </summary>
        /// <param name="entryInfo">入库主表信息</param>
        /// <param name="entryDetailInfos">入库从表信息</param>
        /// <param name="errorMsg">错误消息</param>
        /// <returns></returns>
        bool EntryOperate(I_Entry entryInfo, List<I_EntryDetail> entryDetailInfos, ref string errorMsg);


        /// <summary>
        /// 红充入库
        /// </summary>
        /// <param name="newEntryInfo">新入库主</param>
        /// <param name="orientryDetailInfo">原入库从</param>
        /// <param name="newEntryDetailInfo">新入库从</param>
        /// <param name="errorMsg">错误消息</param>
        /// <returns></returns>
        bool EntryRedOperate(I_Entry newEntryInfo, I_EntryDetail orientryDetailInfo,I_EntryDetail newEntryDetailInfo,ref string errorMsg);
    }
}
