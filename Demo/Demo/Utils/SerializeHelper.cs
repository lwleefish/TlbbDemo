using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Demo.Utils
{
    public class SerializeHelper
    {
        /// <summary>
        /// 文件XML序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="filename">文件路径</param>
        public static void XMLSerialize(object obj, string filename)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(fs, obj);
            }
            catch (Exception ex)
            {
                throw new Exception("文件XML序列化失败", ex);
            }
            finally
            {
                fs?.Close();
            }
        }

        /// <summary>
        /// 文件XML反序列化
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="filename">文件路径</param>
        public static object XMLDeSerialize<T>(Type type, string filename)
        {
            FileStream fs = null;
            try
            {
                if (!File.Exists(filename)) { return null; }
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(fs);
            }
            catch
            {
                return default(T);
                //throw new Exception("文件XML反序列化失败");
            }
            finally
            {
                fs?.Close();
            }
        }

    }
}
