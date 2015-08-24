using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Anke.SHManage.IDAL;
using Anke.SHManage.Utility;
using Anke.SHManage.Model;

namespace Anke.SHManage.BLL
{
    public class M_DictionaryTreeBLL
    {
        private readonly IM_DictionaryTreeDAL m_DAL = DataAccess.CreateM_DictionaryTreeDAL();
        private const string m_M_DictionaryTreeKey = "M_DictionaryTreeKey";
        private static object m_SyncRoot = new object();
        private List<CommonTree> m_ModelList;
        private List<M_ChargeItemInfo> m_ModelChargeItemList;

        public List<CommonTree> GetModelList(string tableName, string ParentID, string TypeID)
        {
            List<CommonTree> list = m_DAL.GetMSDictionaryTreeInfoList(tableName, "", TypeID);
            m_ModelList = list;
            List<CommonTree> listM = m_DAL.GetMSDictionaryTreeInfoList(tableName, ParentID, TypeID);
            List<CommonTree> listNew = new List<CommonTree>();
            CommonTree treeInfo = new CommonTree();
            foreach (CommonTree info in listM)
            {
                treeInfo = new CommonTree();
                treeInfo.id = info.id.ToString();
                treeInfo.text = info.Name.ToString();
                treeInfo.ParentID = ParentID;
                treeInfo.state = "closed";
                treeInfo.children = GetChildren(treeInfo.id, list);
                if (treeInfo.children.Count > 0)
                    treeInfo.state = "closed";//"closed"
                else
                    treeInfo.state = "open";
                treeInfo.PinYin = info.PinYin;
                listNew.Add(treeInfo);
            }

            return listNew;
        }
        private List<CommonTree> GetChildren(string parentID, List<CommonTree> list)
        {
            List<CommonTree> children = new List<CommonTree>();
            CommonTree treeInfo = new CommonTree();
            List<CommonTree> newlist = list.Where(t => t.ParentID == parentID).ToList();
            foreach (CommonTree info in newlist)
            {
                treeInfo = new CommonTree();
                treeInfo.id = info.id.ToString();
                treeInfo.text = info.Name;
                treeInfo.ParentID = parentID;
                treeInfo.children = GetChildren(treeInfo.id, m_ModelList);
                if (treeInfo.children.Count > 0)
                {
                    treeInfo.state = "closed";
                }
                else
                {
                    treeInfo.state = "open";
                }
                treeInfo.PinYin = info.PinYin;
                children.Add(treeInfo);
            }
            return children;
        }

        //获取药品、耗材、救治措施树形结构
        public List<CommonTree> GetModelChargeItemList(string ParentID, string TypeID, string treeState)
        {
            List<M_ChargeItemInfo> list = m_DAL.GetMSChargeItemTreeInfoList("", TypeID);
            m_ModelChargeItemList = list;
            List<M_ChargeItemInfo> listM = m_DAL.GetMSChargeItemTreeInfoList(ParentID, TypeID);
            List<CommonTree> listNew = new List<CommonTree>();
            CommonTree treeInfo = new CommonTree();
            foreach (M_ChargeItemInfo info in listM)
            {
                treeInfo = new CommonTree();
                treeInfo.id = info.ID.ToString();
                treeInfo.text = info.Name;
                treeInfo.ParentID = "-1";
                treeInfo.children = GetChargeItemChildren(treeInfo.id, list);
                if (treeInfo.children.Count > 0)
                    treeInfo.state = "closed";//"closed"
                else
                    treeInfo.state = "open";//"closed"
                treeInfo.Name = info.Name;
                treeInfo.Number = info.Number.ToString();
                treeInfo.PinYin = info.PinYin;
                treeInfo.Specification = info.Specification;
                treeInfo.Unit = info.Unit;
                treeInfo.GiveMedicineWay = info.GiveMedicineWay;//药品--给药方式
                listNew.Add(treeInfo);
            }
            return listNew;
        }
        private List<CommonTree> GetChargeItemChildren(string parentID, List<M_ChargeItemInfo> list)
        {
            List<CommonTree> children = new List<CommonTree>();
            CommonTree treeInfo = new CommonTree();
            List<M_ChargeItemInfo> newlist = list.Where(t => t.ParentID == parentID).ToList();
            foreach (M_ChargeItemInfo info in newlist)
            {
                treeInfo = new CommonTree();
                treeInfo.id = info.ID.ToString();
                treeInfo.text = info.Name;
                treeInfo.ParentID = parentID;
                treeInfo.children = GetChargeItemChildren(treeInfo.id, m_ModelChargeItemList);
                if (treeInfo.children.Count > 0)
                {
                    treeInfo.state = "closed";
                }
                else
                {
                    treeInfo.state = "open";
                }
                treeInfo.Name = info.Name;
                treeInfo.Number = info.Number.ToString();
                treeInfo.PinYin = info.PinYin;
                treeInfo.Specification = info.Specification;
                treeInfo.Unit = info.Unit;
                treeInfo.GiveMedicineWay = info.GiveMedicineWay;//药品--给药方式
                children.Add(treeInfo);
            }
            return children;
        }
    }
}
