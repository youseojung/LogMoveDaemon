using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IIS_LOG_MANAGE
{
    public class FileDir
    {
        public static void CopyFiles(string DaemonLogPath, string LogName, string SrcDirPath, DateTime BaseCopyDate, string DestDirPath)
        {
            foreach (FileInfo file in new DirectoryInfo(SrcDirPath).GetFiles())
            {
                if (file.CreationTime.ToString("d").CompareTo(BaseCopyDate.ToString("d")) < 0)
                {
                    try
                    {
                        file.CopyTo(DestDirPath.ToString() + "\\" + file.Name);
                        Console.WriteLine("[Success]\t{0}->{1}", (object)file.FullName, (object)(DestDirPath.ToString() + "\\" + file.Name));
                        Logger.WriteLog(file.FullName + "\t-->\t" + DestDirPath.ToString() + "\\" + file.Name, LogCode.Success);
                        Console.WriteLine();
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("(멀티)파일복사 에러 : " + ex.Message, LogCode.Error);
                        Console.WriteLine("대상 파일이 이미 존재 합니다. 기존 파일을 삭제하는 중...");
                        new FileInfo(DestDirPath.ToString() + "\\" + file.Name).Delete();
                        Logger.WriteLog("[파일 삭제]\t" + DestDirPath.ToString() + "\\" + file.Name, LogCode.Failure);
                        Console.WriteLine("{0} 파일이 정상적으로 삭제 되었습니다.", (object)file.FullName);
                        Console.WriteLine();
                        file.CopyTo(DestDirPath.ToString() + "\\" + file.Name);
                        Logger.WriteLog(file.FullName + "\t-->\t" + DestDirPath.ToString() + "\\" + file.Name, LogCode.Success);
                        Console.WriteLine("{0} 파일이 정상적으로 복사되었습니다", (object)file.FullName);
                        Console.WriteLine();
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            Logger.WriteLog("==================================================================");
        }

        public static void CopyFile(string DaemonLogPath, string LogName, string SrcDirPath, string FileName, string DestDirPath)
        {
            FileInfo fileInfo = new FileInfo(SrcDirPath + "\\" + FileName);
            if (fileInfo.Exists)
            {
                try
                {
                    fileInfo.CopyTo(DestDirPath.ToString() + "\\" + fileInfo.Name);
                    Console.WriteLine("[Success]\t{0}->{1}", (object)fileInfo.FullName, (object)(DestDirPath.ToString() + "\\" + fileInfo.Name));
                    Logger.WriteLog(fileInfo.FullName + "\t-->\t" + DestDirPath.ToString() + "\\" + fileInfo.Name, LogCode.Success);
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Logger.WriteLog("파일복사 에러 : " + ex.Message, LogCode.Error);
                    Console.WriteLine("대상 파일이 이미 존재 합니다. 기존 파일을 삭제하는 중...");
                    new FileInfo(DestDirPath.ToString() + "\\" + fileInfo.Name).Delete();
                    Logger.WriteLog("[파일 삭제]\t" + DestDirPath.ToString() + "\\" + fileInfo.Name, LogCode.Failure);
                    Console.WriteLine("{0} 파일이 정상적으로 삭제 되었습니다.", (object)fileInfo.FullName);
                    Console.WriteLine();
                    fileInfo.CopyTo(DestDirPath.ToString() + "\\" + fileInfo.Name);
                    Logger.WriteLog(fileInfo.FullName + "\t-->\t" + DestDirPath.ToString() + "\\" + fileInfo.Name, LogCode.Success);
                    Console.WriteLine("{0} 파일이 정상적으로 복사되었습니다", (object)fileInfo.FullName);
                    Console.WriteLine();
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.ToString());
                }
            }
            Logger.WriteLog("==================================================================");
        }

        public static void MoveFile(string DaemonLogPath, string LogName, string SrcDirPath, DateTime BaseCopyDate, string DestDirPath)
        {
            IEnumerable<FileInfo> files = null;
#if Debug
            files = new DirectoryInfo(SrcDirPath).GetFiles().Take(10);

#else
            files = new DirectoryInfo(SrcDirPath).GetFiles();
#endif
            foreach (FileInfo file in files)
            {
#if !Debug
                if (file.CreationTime.ToString("d").CompareTo(BaseCopyDate.ToString("d")) < 0)
#endif
                {
                    try
                    {
#if Debug
                        file.CopyTo(DestDirPath.ToString() + "\\" + file.Name);
#else
                        file.MoveTo(DestDirPath.ToString() + "\\" + file.Name);
#endif
                        Console.WriteLine("[Success]\t{0}->{1}", (object)file.FullName, (object)(DestDirPath.ToString() + "\\" + file.Name));
                        Logger.WriteLog(file.FullName + "\t-->\t" + DestDirPath.ToString() + "\\" + file.Name, LogCode.Success);
                        Console.WriteLine();
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("파일이동 에러 : " + ex.Message, LogCode.Error);
                        Logger.WriteLog(" 파일이동 에러위치 >> : " + SrcDirPath.ToString());
                        //eragon T_cancel 때문에 년도월일 추가
                        DateTime today = DateTime.Today;
                        file.MoveTo(DestDirPath.ToString() + "\\" + file.Name + today.Year + today.Month + today.Day);
                        Logger.WriteLog("[파일 이름 변경]\t" + DestDirPath.ToString() + "\\" + file.Name + " > " + file.Name + today.Year + today.Month + today.Day, LogCode.Failure);
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            Logger.WriteLog("==================================================================");
        }

        /// <summary>
        /// compress by 7zip after delete file.
        /// </summary>
        /// <param name="destDirPath"></param>
        public static void Compress(string destDirPath)
        {
            try
            {
                //SevenZip.SevenZipCompressor.SetLibraryPath(
                //    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "7z.dll")
                //);
                SevenZip.SevenZipCompressor.SetLibraryPath(@"C:\Program Files\7-Zip\7z.dll");
                SevenZip.SevenZipCompressor cmp = new SevenZip.SevenZipCompressor();
                cmp.ArchiveFormat = SevenZip.OutArchiveFormat.SevenZip;
                cmp.CompressionLevel = SevenZip.CompressionLevel.Normal;

                IEnumerable<string> files = null;

#if Debug
                files = Directory.GetFiles(destDirPath).Where(f => Path.GetExtension(f) != ".7z").Take(10);
#else
                files = Directory.GetFiles(destDirPath).Where(f => Path.GetExtension(f) != ".7z");
#endif

                foreach (var file in files)
                {
                    var ext = Path.GetExtension(file);

                    if (ext == ".7z") continue;

                    var compressFileName = file + ".7z";
                    cmp.CompressFiles(compressFileName, file);
                    Console.WriteLine("[Success]\t{0} compress to {1}", file, compressFileName);
                    Logger.WriteLog(string.Format("파일압축 : {0} compress to {1}", file, compressFileName), LogCode.Success);
                    File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog("파일압축 에러 : " + ex.Message, LogCode.Error);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.ToString());
            }
            Logger.WriteLog("==================================================================");
        }

        public static void DeleteFile(string DaemonLogPath, string LogName, string DelDirPath, DateTime DelDate)
        {
            foreach (FileInfo file in new DirectoryInfo(DelDirPath).GetFiles())
            {
                if (file.CreationTime.ToString("d").CompareTo(DelDate.ToString("d")) < 0)
                {
                    try
                    {
                        file.Delete();
                        Console.WriteLine("{0} 파일을 삭제 하였습니다.", (object)file.FullName);
                        Logger.WriteLog("[Delete]\t" + file.FullName, LogCode.Success);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("파일 삭제 에러 : " + ex.Message, LogCode.Error);
                        Console.WriteLine("[" + (object)DateTime.Now + "]\t[Debug]\t" + ex.Message);
                    }
                }
            }
            Logger.WriteLog("==================================================================");
        }

        public static void CopyFolder(string SrcDirPath, DateTime BaseCopyDate, string DestDirPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(SrcDirPath);
            directoryInfo.GetDirectories();
            FileInfo[] files = directoryInfo.GetFiles();
            if (!directoryInfo.Exists || !Directory.Exists(DestDirPath) || directoryInfo.CreationTime.ToString("d").CompareTo(BaseCopyDate.ToString("d")) >= 0)
                return;
            foreach (FileInfo fileInfo in files)
                fileInfo.CopyTo(DestDirPath + "\\" + fileInfo.Name);
            FileDir.SubFolderCopy(SrcDirPath, DestDirPath, SrcDirPath);
        }

        public static void MoveFolder(string SrcDirPath, DateTime BaseMoveDate, string DestDirPath)
        {
            DirectoryInfo directoryInfo1 = new DirectoryInfo(SrcDirPath);
            DirectoryInfo[] directories = directoryInfo1.GetDirectories();
            FileInfo[] files = directoryInfo1.GetFiles();
            if (!directoryInfo1.Exists || !Directory.Exists(DestDirPath) || directoryInfo1.CreationTime.ToString("d").CompareTo(BaseMoveDate.ToString("d")) >= 0)
                return;
            foreach (FileInfo fileInfo in files)
            {
                fileInfo.CopyTo(DestDirPath + "\\" + fileInfo.Name);
                fileInfo.Delete();
            }
            FileDir.SubFolderCopy(SrcDirPath, DestDirPath, SrcDirPath);
            foreach (DirectoryInfo directoryInfo2 in directories)
                directoryInfo2.Delete(true);
        }

        private static void SubFolderCopy(string dir, string back_dir, string src_dir)
        {
            foreach (DirectoryInfo directory in new DirectoryInfo(dir).GetDirectories())
            {
                string str = directory.FullName.ToString().Replace(src_dir, "");
                Directory.CreateDirectory(back_dir + str);
                foreach (FileInfo file in directory.GetFiles())
                    file.CopyTo(back_dir + str + "\\" + file.Name);
                FileDir.SubFolderCopy(directory.FullName.ToString(), back_dir, src_dir);
            }
        }

        public static void DeleteFolder(string DeamonLogPath, string LogName, string DelDirPath, DateTime BaseDelDate)
        {
            foreach (DirectoryInfo directory in new DirectoryInfo(DelDirPath).GetDirectories())
            {
                if (directory.Exists)
                {
                    if (directory.CreationTime.ToString("d").CompareTo(BaseDelDate.ToString("d")) < 0)
                    {
                        try
                        {
                            directory.Delete(true);
                            Console.WriteLine("{0} 디렉토리를 삭제하였습니다.", (object)directory.FullName);
                            Logger.WriteLog("[Delete]\t" + directory.FullName, LogCode.Success);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("[" + (object)DateTime.Now + "]\t[Debug]\t" + ex.Message);
                            Logger.WriteLog("디렉토리 삭제 에러 : " + ex.Message, LogCode.Error);
                        }
                    }
                }
            }
            Logger.WriteLog("==================================================================");
        }
    }
}
