using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace Anke.AnkeOAService
{
    public class XmlHelper
    {
        /// <summary>
        /// XML转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T XmlConvert<T>(string xml)
        {
            var xmlSer = new XmlSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                return (T)xmlSer.Deserialize(ms);
        }
    }
}