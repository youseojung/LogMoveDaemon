using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IIS_LOG_MANAGE
{
    public class XmlLoader<T> where T : class
    {
        public static T LoadFromXml(string fileName)
        {
            T _settings = null;

            if (false == File.Exists(fileName)) return null;

            FileStream fs = null;
            XmlSerializer xs = null;

            try
            {
                xs = new XmlSerializer(typeof(T));
            }
            catch(Exception e)
            {
                Debug.Assert(false);
                e.ToString();
            }

            try
            {
                fs = new FileStream(fileName, FileMode.Open);
                _settings = (T)xs.Deserialize(fs);
            }
            catch(Exception e)
            {
                Debug.Assert(false);
                e.ToString();
            }

            return _settings;
        }
    }
}
