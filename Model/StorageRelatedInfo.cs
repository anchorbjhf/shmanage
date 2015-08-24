using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Model
{

    /// <summary>
    ///  仓储相关
    /// </summary>
    public class StorageRelatedInfo
    {
        /// <summary>
        /// 用户仓储集合
        /// </summary>
       public List<int> listUserStorage { get; set; }


       /// <summary>
       /// 用户仓储类型集合
       /// </summary>
       public List<string> listUserStorageMaterialType { get; set; }
    }
}
