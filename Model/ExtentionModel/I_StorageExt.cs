using Anke.SHManage.Model.EasyUIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{


    public class I_StorageExt
    {
        public int StorageID { get; set; }
        public string ParentStorageID { get; set; }
        public string Name { get; set; }
        public int LinkSign { get; set; }
        public string LinkCode { get; set; }
        public string Remark { get; set; }
        public int ID { get; set; }
    }


    public partial class I_Storage
    {

        public M_Dictionary ToDictionary()
        {
            //对象的初始化器直接构造
            M_Dictionary dic = new M_Dictionary()
            {
                ID = this.StorageID.ToString(),
                Name = this.Name,
            };
            return dic;
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
                id = this.StorageID.ToString(),
                text = this.Name,
                state = "open",
                Checked = false,
                attributes = new {LevelID = this.LevelCode},
                children = new List<TreeNode>() // 子节点集合
            };

            return treeNode;
        }

        /// <summary>
        /// 将 权限集合 转成 树节点集合
        /// </summary>
        /// <param name="listPer"></param>
        /// <returns></returns>
        public static List<TreeNode> ToTreeNodes(List<I_Storage> listPer)
        {
            List<TreeNode> listNodes = new List<TreeNode>();
            //生成 树节点时，根据 pid=0的根节点 来生成
            LoadTreeNode(listPer, listNodes, "0");
            return listNodes;
        }

        /// <summary>
        /// 递归权限集合 创建 树节点集合
        /// 无级递归
        /// </summary>
        /// <param name="listPer">权限集合</param>
        /// <param name="listChildren">节点集合</param>
        /// <param name="pID">节点父id</param>
        public static void LoadTreeNode(List<I_Storage> listStorage, List<TreeNode> listChildrenNode, string pID)
        {
            foreach (var storage in listStorage)
            {
                //判断权限ParentID 是否和 传入参数相等
                if (storage.ParentStorageID.Equals(pID))
                {
                    //将 权限转成 树节点
                    TreeNode node = storage.ToTreeNode();
                    //将节点加入到 树子节点集合
                    listChildrenNode.Add(node);

                    //递归 为这个新创建的 树节点找 子节点
                    LoadTreeNode(listStorage, node.children, node.id);
                }
            }
        }



    }
}
