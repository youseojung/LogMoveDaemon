using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IIS_LOG_MANAGE
{
    class Program
    {
        static string _configFileName = @"SettingConfig.xml";
        static SettingModel _settingModel;        

        static void Main(string[] args)
        {
            _settingModel = XmlLoader<SettingModel>.LoadFromXml(_configFileName);

            //_settingModel.SourceFolderList = _settingModel.SourceFolderList.Where(m => m.Src.Contains("NAVERM3")).ToList();
            //_settingModel.DestinationFolderList = _settingModel.DestinationFolderList.Where(m => m.Dest.Contains("52.83")).ToList();

            if (_settingModel == null)
            {
                Logger.WriteLog("SettingConfig.xml is not found.", LogCode.Error); //path is application location;
                Console.WriteLine("SettingConfig.xml is not found.");
                return;
            }

            Logger.LOG_DIR = _settingModel.DaemonLogPath;
            Logger.LOG_FILE = _settingModel.DaemonLogFileName;

            var srcs = _settingModel.SourceFolderList;
            var dest = _settingModel.DestinationFolderList;

            if(srcs.Count != dest.Count)
            {
                Logger.WriteLog("src path length not equal dest path length.", LogCode.Error);
                Console.WriteLine("src path length not equal dest path length.");
                return;
            }

            foreach(var folder in dest)
            {
                if (!Directory.Exists(folder.Dest))
                {
                    Console.WriteLine($"folder not exists {folder.Dest}. Create folder {folder.Dest}");
                    Logger.WriteLog($"folder not exists {folder.Dest}. Create folder {folder.Dest}", LogCode.Infomation);
                    Directory.CreateDirectory(folder.Dest);
                }
            }

            LogFileMove();
            LogFileCompress();
            LogFileDelete();

            Console.Write("Completed IIS LOG MANAGE");
        } 
        
        private static void LogFileMove()
        {
            DateTime baseCopyDate = DateTime.Now.AddDays((double)Convert.ToInt32(_settingModel.MoveDays));

            for (int i = 0; i < _settingModel.SourceFolderList.Count; i++)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                FileDir.MoveFile(_settingModel.DaemonLogPath,
                    _settingModel.DaemonLogFileName,
                    _settingModel.SourceFolderList[i].Src,
                    baseCopyDate,
                    _settingModel.DestinationFolderList[i].Dest);                

                stopwatch.Stop();

                Logger.WriteLog($"Log Move Execution Time : {stopwatch.ElapsedMilliseconds},", LogCode.Infomation);
            }
        }       

        private static void LogFileCompress()
        {
            for (int i = 0; i < _settingModel.SourceFolderList.Count; i++)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                FileDir.Compress(_settingModel.DestinationFolderList[i].Dest);

                stopwatch.Stop();
                Logger.WriteLog($"Compress Execution Time : {stopwatch.ElapsedMilliseconds},", LogCode.Infomation);
            }
        }

        private static void LogFileDelete()
        {
            DateTime delDate = DateTime.Now.AddMonths(Convert.ToInt32(_settingModel.DeleteMonths));

            Logger.LOG_FILE = _settingModel.DaemonLogFileNameDelete;

            for (int i = 0; i < _settingModel.DestinationFolderList.Count; i++)
            {
                FileDir.DeleteFile(_settingModel.DaemonLogPath,
                    _settingModel.DaemonLogFileNameDelete,
                    _settingModel.DestinationFolderList[i].Dest,
                    delDate);
            }
        }
    }
}
