using Anke.SHManage.Model.EasyUIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{

    public partial class P_Department
    {

        public P_Department ToExt(bool isPOCO = true)
        {
            return new P_Department()
            {
                ID = this.ID,
                ParentID = this.ParentID,
                Name = this.Name,
                Remark = this.Remark,
                IsActive = this.IsActive,
                SN = this.SN,
            };
        }


        /// <summary>
        /// 生成TreeNode
        /// </summary>
        /// <returns></returns>
        private TreeNode ToTreeNode()
        {
            //对象的初始化器直接构造
            TreeNode treeNode = new TreeNode()
            {
                id = this.ID.ToString(),
                text = this.Name,
                state = "open",
                Checked = false,
                attributes = null,
                children = new List<TreeNode>() // 子节点集合
            };

            return treeNode;
        }

        /// <summary>
        /// 将 权限集合 转成 树节点集合
        /// </summary>
        /// <param name="listPer"></param>
        /// <returns></returns>
        public static List<TreeNode> ToTreeNodes(List<P_Department> list)
        {
            List<TreeNode> listNodes = new List<TreeNode>();

            if (list.Count>0)
            {
                if (list[0].ID == list[0].ParentID)
                {
                    list[0].ParentID = -1;
                }
            }

            //生成 树节点时，根据 pid=0的根节点 来生成
            LoadTreeNode(list, listNodes, "-1");
            return listNodes;
        }

        /// <summary>
        /// 递归权限集合 创建 树节点集合
        /// 无级递归
        /// </summary>
        /// <param name="listPer">权限集合</param>
        /// <param name="listChildren">节点集合</param>
        /// <param name="pID">节点父id</param>
        public static void LoadTreeNode(List<P_Department> list, List<TreeNode> listChildrenNode, string pID)
        {
            foreach (var dep in list)
            {
                //判断权限ParentID 是否和 传入参数相等
                if (dep.ParentID.ToString().Equals(pID))
                {
                    //将 权限转成 树节点
                    TreeNode node = dep.ToTreeNode();
                    //将节点加入到 树子节点集合
                    listChildrenNode.Add(node);

                    //递归 为这个新创建的 树节点找 子节点
                    LoadTreeNode(list, node.children, node.id);

                }
            }
        }




    }
}
