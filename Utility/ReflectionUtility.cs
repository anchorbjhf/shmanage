using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.Utility
{
   public  class ReflectionUtility
    {
       public static void CopyObjectProperty<T>(T tSource, T tDestination) where T : class
       {
           //获得所有property的信息
           PropertyInfo[] properties = tSource.GetType().GetProperties();
           foreach (PropertyInfo p in properties)
           {
               p.SetValue(tDestination, p.GetValue(tSource, null), null);//设置tDestination的属性值              
           }
       }
       /// <summary>
       /// 对照两个实体 为空的从A赋值到B
       /// </summary>
       /// <typeparam name="T"><peparam>
       /// <param name="tSource"></param>
       /// <param name="tDestination"></param>
       public static void UpdateObjectProperty<T>(T tSource, T tDestination) where T : class
       {
           //获得所有property的信息
           PropertyInfo[] properties = tSource.GetType().GetProperties();
           foreach (PropertyInfo p in properties)
           {
               if (p.GetValue(tDestination, null) == null || p.GetValue(tDestination, null).ToString() == "")
                   p.SetValue(tDestination, p.GetValue(tSource, null), null);
           }
       }  

    }
}
