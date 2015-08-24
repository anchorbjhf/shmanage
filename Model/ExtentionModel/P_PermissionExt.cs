
using Anke.SHManage.Model.EasyUIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{
    /// <summary>
    /// 权限类扩展类
    /// </summary>
    public partial class P_Permission
    {
        /// <summary>
        /// 转换成pocos 实体 纯净的P_Permission 没有导航属性   
        /// </summary>
        /// <returns></returns>
        //public P_Permission ToPOCO()
        //{
        //    P_Permission poco = new P_Permission()
        //       {
        //           ID = this.ID,
        //           ParentID = this.ParentID,
        //           Name = this.Name,
        //           AreaName = this.AreaName,
        //           ControllerName = this.ControllerName,
        //           ActionName = this.ActionName,
        //           FormMethod = this.FormMethod,
        //           IsLink = this.IsLink,
        //           LinkURL = this.LinkURL,
        //           IsShow = this.IsShow,
        //           ICO = this.ICO,
        //           Remark = this.Remark,
        //           IsActive = this.IsActive,
        //           SN = this.SN
        //       };
        //    return poco;
        //}



        /// <summary>
        /// 转换成相对应的实体对象
        /// </summary>
        /// <returns></returns>
        public ViewModel.Permission ToViewModel()
        {
            ViewModel.Permission vModel = new ViewModel.Permission()
            {
                ID = this.ID,
                ParentID = this.ParentID,
                ParentName = "无",
                Name = this.Name,
                AreaName = this.AreaName,
                ControllerName = this.ControllerName,
                ActionName = this.ActionName,
                FormMethod = this.FormMethod,
                SN = this.SN,
                IsShow = this.IsShow,
                Remark = this.Remark
            };

            return vModel;
        }

        #region 构建EazyUI TreeNode
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
                iconCls= this.ICO,
                Checked = false,
                attributes = new { url = this.GetUrl(this.AreaName, this.ControllerName, this.ActionName)},  //把生成的URL存到
                children = new List<TreeNode>() // 子节点集合
            };

            return treeNode;
        }

        /// <summary>
        /// 生成URL
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        protected string GetUrl(string areaName, string controllerName, string actionName)
        {
            string tempUrl=GetURLPart(areaName) + GetURLPart(controllerName) +GetURLPart(actionName);
            if (tempUrl.Length > 0)
                tempUrl = tempUrl.Substring(1);
            return tempUrl;
        }

        /// <summary>
        /// 生成 URL 部分字符串
        /// </summary>
        /// <param name="urlPart"></param>
        /// <returns></returns>
        protected string GetURLPart(string urlPart)
        {
            return string.IsNullOrEmpty(urlPart) ? "" : "/" + urlPart;
        }
        #endregion

        /// <summary>
        /// 将 权限集合 转成 树节点集合
        /// </summary>
        /// <param name="listPer"></param>
        /// <returns></returns>
        public static List<TreeNode> ToTreeNodes(List<P_Permission> listPer)
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
        public static void LoadTreeNode(List<P_Permission> listPer, List<TreeNode> listChildrenNode, string pID)
        {
            foreach (var permission in listPer)
            {
                //判断权限ParentID 是否和 传入参数相等
                if (permission.ParentID.ToString().Equals(pID))
                {
                    //将 权限转成 树节点
                    TreeNode node = permission.ToTreeNode();
                    //将节点加入到 树子节点集合
                    listChildrenNode.Add(node);

                    //递归 为这个新创建的 树节点找 子节点
                    LoadTreeNode(listPer, node.children, node.id);
                }
            }
        }

    }
}
