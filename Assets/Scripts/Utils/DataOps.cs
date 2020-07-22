using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Utils {
    static class DataOps {
        public static T XmlLoad<T>(string filepath) {
            //string filepath = "filepath";
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            FileStream aFile = new FileStream(filepath, FileMode.Open);
            byte[] buffer = new byte[aFile.Length];
            aFile.Read(buffer, 0, (int)aFile.Length);
            MemoryStream stream = new MemoryStream(buffer);
            return (T)formatter.Deserialize(stream);
        }
    }
}
