using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IIS_LOG_MANAGE
{

    public class Logger
    {
        public static string LOG_DIR = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string LOG_FILE = "Log";
        private const string DateTimeFormat = "yyyyMMdd";
        
        
        public Logger()
        {

        }        

        public static void SetLogPath(string strDir)
        {
            LOG_DIR = strDir;
        }

        public static void SetLogFile(string strFile)
        {
            LOG_FILE = strFile;
        }

        public static string GetLogPath()
        {
            return LOG_DIR;
        }

        public static void WriteLog(string strLog)
        {
            WriteLog(strLog, LogCode.Infomation, LOG_FILE);
        }

        public static void WriteLog(string strLog, string strFileName)
        {
            WriteLog(strLog, LogCode.Infomation, strFileName, LOG_DIR);
        }

        public static void WriteLog(string strLog, string strFileName, string strPath)
        {
            WriteLog(strLog, LogCode.Infomation, strFileName, strPath);
        }

        public static void WriteLog(string strLog, LogCode logCode)
        {
            WriteLog(strLog, logCode, LOG_FILE);
        }

        public static void WriteLog(string strLog, LogCode logCode, string strFileName)
        {
            WriteLog(strLog, logCode, strFileName, LOG_DIR);
        }

        public static void WriteLog(string strLog, LogCode logCode, string strFileName, string strPath)
        {
            string strFullName;

            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }

            if (strPath.EndsWith(@"\") == false || strPath.EndsWith("/") == false)
            {
                strPath = strPath + @"\";
            }

            strFullName = strPath + strFileName + "_" + DateTime.Now.ToString(DateTimeFormat) + ".txt";

            string strFullLog = DateTime.Now.ToString("HH:mm:ss") + " (" + logCode.ToString() + ")" + " : " + strLog;

            using (StreamWriter sw = new StreamWriter(strFullName, true, System.Text.Encoding.UTF8, 4096))
            {
                sw.WriteLine(strFullLog);
                sw.Close();
            }
        }

        public static void WriteLog(string strLog, bool timeDisplay)
        {
            string strFullName;

            if (!Directory.Exists(LOG_DIR))
            {
                Directory.CreateDirectory(LOG_DIR);
            }

            if (LOG_DIR.EndsWith(@"\") == false || LOG_DIR.EndsWith("/") == false)
            {
                LOG_DIR = LOG_DIR + @"\";
            }
            strFullName = LOG_DIR + LOG_FILE + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

            string strFullLog = string.Empty;

            if (timeDisplay)
                strFullLog = DateTime.Now.ToString("HH:mm:ss") + " " + strLog;
            else
                strFullLog = strLog;

            using (StreamWriter sw = new StreamWriter(strFullName, true, System.Text.Encoding.UTF8, 4096))
            {
                sw.WriteLine(strFullLog);
                sw.Close();
                sw.Dispose();
            }
        }

        public static void WriteLogForOnlyText(string strLog, string LOG_FILEName)
        {
            using (StreamWriter sw = new StreamWriter(LOG_FILEName, true, System.Text.Encoding.UTF8, 4096))
            {
                sw.WriteLine(strLog);
                sw.Close();
                sw.Dispose();
            }
        }

        public static void AppendPathSeparator(ref string dir)
        {
            if (!dir.EndsWith("\\") && !dir.EndsWith("/"))
                dir += "\\";
        }
    }

    public enum LogCode
    {
        Infomation = 0,
        Success = 1,
        Error = -1,
        Failure = -2,
        Warning = -10,
        SystemError = -101,
        ApplicationError = -201
    }
}
