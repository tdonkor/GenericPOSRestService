using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Windows.Forms;

namespace GenericPOSRestService.Common
{
    public static class Log
    {
        private static ConcurrentQueue<MessageInfo> messageInfos = new ConcurrentQueue<MessageInfo>();
        private static System.Timers.Timer tmrWriteMessage = null;

        static Log()
        {
            Level = LogLevel.Info;
            CleanLogsAfter = 60;

            //ORU 20141027 - required for web when iis user don't have write access into log file
            try
            {
                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // start the tmrWriteMessage timer
            tmrWriteMessage = new System.Timers.Timer(50);
            tmrWriteMessage.AutoReset = true;
            tmrWriteMessage.Elapsed += new System.Timers.ElapsedEventHandler(tmrWriteMessage_Elapsed);
            tmrWriteMessage.Start();
        }
        
        private static Object thisLock = new Object();

        public static DateTime LastDay = DateTime.Now.Date;

        public static int CleanLogsAfter { get; set; }

        private static string _path = "";

        private const string logsFolderName = "Logs";

        private static bool writting = false;

        public static string Path
        {
            get
            {
                string result = _path;

                if (string.IsNullOrEmpty(_path))
                {
                    _path = Application.StartupPath;
                }

                return System.IO.Path.Combine(_path, logsFolderName); 
            }
            set
            {
                _path = value; 
            }
        }

        public static string FileName
        {
            get;
            set;
        }

        public static LogLevel Level
        {
            get;
            set;
        }

        private static volatile List<LineLog> lineLogs = new List<LineLog>();

        public static void HeaderApp()
        {
            string executablePath = Application.ExecutablePath;

            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(executablePath); 

            Sys("");
            Sys("  ********************************************************************  ");
            Sys(string.Format("Application {0} is running", executablePath));
            Sys(string.Format("Product Name = {0}  Product Version = {1}", fvi.ProductName, fvi.ProductVersion));
            Sys("  ********************************************************************  ");

            CleanLogs();
        }

        public static void CleanLogs()
        {
            string[] filePaths = Directory.GetFiles(Path, "*.log");
            string tmpStr = string.Empty;
            DateTime tmpDate = new DateTime();
            string[] tmpStrs = null;
            foreach (var filePath in filePaths)
            {
                tmpDate = new DateTime();
                try
                {
                    tmpStr = System.IO.Path.GetFileNameWithoutExtension(filePath);
                    tmpStrs = tmpStr.Split('_');
                    tmpStr = tmpStrs[tmpStrs.Length - 1];
                    tmpDate = tmpDate.AddYears(1999 + Convert.ToInt32(tmpStr.Substring(0, 2)));
                    tmpDate = tmpDate.AddMonths(Convert.ToInt32(tmpStr.Substring(2, 2)) - 1);
                    tmpDate = tmpDate.AddDays(Convert.ToInt32(tmpStr.Substring(4, 2)) - 1);
                    if (tmpDate < DateTime.Now.AddDays(-CleanLogsAfter))
                    {
                        //LogFile it;s to old and it must be deleted
                        File.Delete(filePath);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Log.CleanLogs: ");
                    Log.Error(ex);
                }

                
            }
        }

        public static void FooterApp()
        {
            Sys("  Application Closing ... ");
            Sys("  ********************************************************************  ");
            CleanLogs();
        }

        class LineLog
        {
            public string Path { get; set; }
            public string FileName { get; set; }
            public LogLevel LogLevel { get; set; }
            public string Message { get; set; }
        }

        private static void LogMessage(string APath, string AFile, LogLevel ALogLevel, string AMessage)
        {
            WriteMessage(APath, AFile, ALogLevel, AMessage);
        }

        public static bool WriteLogInSeparateThread { get; set; }

        private static void WriteMessage(string APath, string AFile, LogLevel ALogLevel, string AMessage)
        {
            //csi 141205 - if the level is not enough to write the log then exit immediatelly
            if (ALogLevel > Level)
                return;

            string fName = string.Empty;

            DateTime currentDate = DateTime.Now.Date;
            if (String.IsNullOrEmpty(AFile))
            {
                fName = System.IO.Path.GetFileName(Application.ExecutablePath);
                fName = System.IO.Path.ChangeExtension(fName, "");
                fName = fName.Replace(".", "");
                FileName = fName;
                AFile = fName;
            }

            AFile = AFile + "_" + currentDate.ToString("yyMMdd") + ".log";

            if (currentDate != LastDay)
            {
                LastDay = currentDate;
                HeaderApp();
            }

            string msg = string.Empty;

            try
            {
                switch (ALogLevel)
                {
                    case LogLevel.System:
                        msg = DateTime.Now.ToString("HH:mm:ss:ffff") + " SYS ";
                        break;
                    case LogLevel.Windows:
                        msg = DateTime.Now.ToString("HH:mm:ss:ffff") + " WIN ";
                        break;
                    case LogLevel.Error:
                        msg = DateTime.Now.ToString("HH:mm:ss:ffff") + " ERR ";
                        break;
                    case LogLevel.Warnings:
                        msg = DateTime.Now.ToString("HH:mm:ss:ffff") + " WRN ";
                        break;
                    case LogLevel.Debug:
                        msg = DateTime.Now.ToString("HH:mm:ss:ffff") + " DBG ";
                        break;
                    case LogLevel.Info:
                        msg = DateTime.Now.ToString("HH:mm:ss:ffff") + " INF ";
                        break;
                    default:
                        msg = DateTime.Now.ToString("HH:mm:ss:ffff") + " ??? ";
                        break;
                }

                if (WriteLogInSeparateThread)
                {
                    messageInfos.Enqueue(new MessageInfo { Path = APath, File = AFile, Message = msg + AMessage });
                }
                else
                {
                    try
                    {
                        if (!Directory.Exists(APath))
                        {
                            Directory.CreateDirectory(APath);
                        }

                        using (StreamWriter fp = new StreamWriter(System.IO.Path.Combine(APath, AFile), true))
                        {
                            fp.WriteLine(msg + AMessage);
                            fp.Close();
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch //(Exception ex)
            {
            }
        }

        /// <summary>
        /// Systems the error.
        /// </summary>
        /// <param name="AMessage">The A message.</param>
        public static void Sys(string AMessage)
        {
            LogMessage(Path, FileName, LogLevel.System, AMessage);
        }

        /// <summary>
        /// Systems the error.
        /// </summary>
        /// <param name="AMessage">The A message.</param>
        /// <param name="args">The args.</param>
        public static void Sys(string AMessage, params object[] args)
        {
            LogMessage(Path, FileName, LogLevel.System, string.Format(AMessage, args));
        }

        /// <summary>
        /// Systems the error.
        /// </summary>
        /// <param name="AFile">The A file.</param>
        /// <param name="AMessage">The A message.</param>
        public static void Sys(string AFile, string AMessage)
        {
            if (string.IsNullOrWhiteSpace(AFile))
            {
                AFile = FileName;
            }

            LogMessage(Path, AFile, LogLevel.System, AMessage);
        }

        /// <summary>
        /// Systems the error.
        /// </summary>
        /// <param name="AFile">The A file.</param>
        /// <param name="AMessage">The A message.</param>
        /// <param name="args">The args.</param>
        public static void Sys(string AFile, string AMessage, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(AFile))
            {
                AFile = FileName;
            }
            
            LogMessage(Path, AFile, LogLevel.System, string.Format(AMessage, args));
        }

        /// <summary>
        /// Windowses the error.
        /// </summary>
        /// <param name="AMessage">The A message.</param>
        public static void WindowsError(string AMessage)
        {
            LogMessage(Path, FileName, LogLevel.Windows, AMessage);
        }

        /// <summary>
        /// Windowses the error.
        /// </summary>
        /// <param name="AMessage">The A message.</param>
        /// <param name="args">The args.</param>
        public static void WindowsError(string AMessage, params object[] args)
        {
            LogMessage(Path, FileName, LogLevel.Windows, string.Format(AMessage, args));
        }

        /// <summary>
        /// Windowses the error.
        /// </summary>
        /// <param name="AFile">The A file.</param>
        /// <param name="AMessage">The A message.</param>
        public static void WindowsError(string AFile, string AMessage)
        {
            if (string.IsNullOrWhiteSpace(AFile))
            {
                AFile = FileName;
            }

            LogMessage(Path, AFile, LogLevel.Windows, AMessage);
        }

        /// <summary>
        /// Windowses the error.
        /// </summary>
        /// <param name="AFile">The A file.</param>
        /// <param name="AMessage">The A message.</param>
        /// <param name="args">The args.</param>
        public static void WindowsError(string AFile, string AMessage, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(AFile))
            {
                AFile = FileName;
            }

            LogMessage(Path, AFile, LogLevel.Windows, string.Format(AMessage, args));
        }

        /// <summary>
        /// Errors the specified A message.
        /// </summary>
        /// <param name="AMessage">The A message.</param>
        public static void Error(string AMessage)
        {
            LogMessage(Path, FileName, LogLevel.Error, AMessage);
            LogMessage(Path, FileName + "_Errors", LogLevel.Error, AMessage);
        }

        /// <summary>
        /// Errors the specified A message.
        /// </summary>
        /// <param name="AMessage">The A message.</param>
        /// <param name="args">The args.</param>
        public static void Error(string AMessage, params object[] args)
        {
            LogMessage(Path, FileName, LogLevel.Error, string.Format(AMessage, args));
            LogMessage(Path, FileName + "_Errors", LogLevel.Error, string.Format(AMessage, args));
        }

        /// <summary>
        /// Errors the specified A file.
        /// </summary>
        /// <param name="AFile">The A file.</param>
        /// <param name="AMessage">The A message.</param>
        public static void Error(string AFile, string AMessage)
        {
            if (string.IsNullOrWhiteSpace(AFile))
            {
                AFile = FileName;
            }
            
            LogMessage(Path, AFile, LogLevel.Error, AMessage);
            LogMessage(Path, AFile + "_Errors", LogLevel.Error, AMessage);
        }

        /// <summary>
        /// Errors the specified A file.
        /// </summary>
        /// <param name="AFile">The A file.</param>
        /// <param name="AMessage">The A message.</param>
        /// <param name="args">The args.</param>
        public static void Error(string AFile, string AMessage, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(AFile))
            {
                AFile = FileName;
            }
            
            LogMessage(Path, AFile, LogLevel.Error, string.Format(AMessage, args));
            LogMessage(Path, AFile + "_Errors", LogLevel.Error, string.Format(AMessage, args));
        }


        /// <summary>
        /// Errors the specified A message.
        /// </summary>
        /// <param name="AMessage">The A message.</param>
        public static void Error(string aFile, Exception exception)
        {
            if (string.IsNullOrWhiteSpace(aFile))
            {
                aFile = FileName;
            }
            
            string msgErr = BuildErrorMessage(exception);

            LogMessage(Path, aFile, LogLevel.Error, msgErr);
            LogMessage(Path, aFile + "_Errors", LogLevel.Error, msgErr);
        }

        private static string BuildErrorMessage(Exception exception)
        {
            string msgErr = " Error Message: " + exception.Message + Environment.NewLine;
            if (exception.InnerException != null)
            {
                msgErr += " Inner Error Message: " + exception.InnerException.Message +
                          Environment.NewLine;
                if (exception.InnerException.InnerException != null)
                    msgErr += " Inner Error Message 2: " + exception.InnerException.InnerException.Message +
                              Environment.NewLine;
            }
            msgErr += " Stack Trace: " + exception.StackTrace;
            return msgErr;
        }


        /// <summary>
        /// Errors the specified A message.
        /// </summary>
        /// <param name="AMessage">The A message.</param>
        public static void Error(Exception exception)
        {
            string msgErr = BuildErrorMessage(exception);

            LogMessage(Path, FileName, LogLevel.Error, msgErr);
            LogMessage(Path, FileName + "_Errors", LogLevel.Error, msgErr);
        }


        /// <summary>
        /// Warningses the specified A message.
        /// </summary>
        /// <param name="AMessage">The A message.</param>
        public static void Warnings(string AMessage)
        {
            LogMessage(Path, FileName, LogLevel.Warnings, AMessage);
            LogMessage(Path, FileName + "_Errors", LogLevel.Warnings, AMessage);
        }

        /// <summary>
        /// Warningses the specified A message.
        /// </summary>
        /// <param name="AMessage">The A message.</param>
        /// <param name="args">The args.</param>
        public static void Warnings(string AMessage, params object[] args)
        {
            LogMessage(Path, FileName, LogLevel.Warnings, string.Format(AMessage, args));
            LogMessage(Path, FileName + "_Errors", LogLevel.Warnings, string.Format(AMessage, args));
        }

        /// <summary>
        /// Warningses the specified A file.
        /// </summary>
        /// <param name="AFile">The A file.</param>
        /// <param name="AMessage">The A message.</param>
        public static void Warnings(string AFile, string AMessage)
        {
            if (string.IsNullOrWhiteSpace(AFile))
            {
                AFile = FileName;
            }
            
            LogMessage(Path, AFile, LogLevel.Warnings, AMessage);
            LogMessage(Path, AFile + "_Errors", LogLevel.Warnings, AMessage);
        }

        /// <summary>
        /// Warningses the specified A file.
        /// </summary>
        /// <param name="AFile">The A file.</param>
        /// <param name="AMessage">The A message.</param>
        /// <param name="args">The args.</param>
        public static void Warnings(string AFile, string AMessage, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(AFile))
            {
                AFile = FileName;
            }

            LogMessage(Path, AFile, LogLevel.Warnings, string.Format(AMessage, args));
            LogMessage(Path, AFile + "_Errors", LogLevel.Warnings, string.Format(AMessage, args));
        }

        /// <summary>
        /// Debugs the specified A message.
        /// </summary>
        /// <param name="AMessage">The A message.</param>
        public static void Debug(string AMessage)
        {
            LogMessage(Path, FileName, LogLevel.Debug, AMessage);
        }

        /// <summary>
        /// Debugs the specified A message.
        /// </summary>
        /// <param name="AMessage">The A message.</param>
        /// <param name="args">The args.</param>
        public static void Debug(string AMessage, params object[] args)
        {
            LogMessage(Path, FileName, LogLevel.Debug, string.Format(AMessage, args));
        }

        /// <summary>
        /// Debugs the specified A file.
        /// </summary>
        /// <param name="AFile">The A file.</param>
        /// <param name="AMessage">The A message.</param>
        public static void Debug(string AFile, string AMessage)
        {
            if (string.IsNullOrWhiteSpace(AFile))
            {
                AFile = FileName;
            }

            LogMessage(Path, AFile, LogLevel.Debug, AMessage);
        }

        /// <summary>
        /// Debugs the specified A file.
        /// </summary>
        /// <param name="AFile">The A file.</param>
        /// <param name="AMessage">The A message.</param>
        /// <param name="args">The args.</param>
        public static void Debug(string AFile, string AMessage, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(AFile))
            {
                AFile = FileName;
            }

            LogMessage(Path, AFile, LogLevel.Debug, string.Format(AMessage, args));
        }

        /// <summary>
        /// Infoes the specified A message.
        /// </summary>
        /// <param name="AMessage">The A message.</param>
        public static void Info(string AMessage)
        {
            LogMessage(Path, FileName, LogLevel.Info, AMessage);
        }

        /// <summary>
        /// Infoes the specified A message.
        /// </summary>
        /// <param name="AMessage">The A message.</param>
        /// <param name="args">The args.</param>
        public static void Info(string AMessage, params object[] args)
        {
            LogMessage(Path, FileName, LogLevel.Info, string.Format(AMessage, args));
        }

        /// <summary>
        /// Infoes the specified A file.
        /// </summary>
        /// <param name="AFile">The A file.</param>
        /// <param name="AMessage">The A message.</param>
        public static void Info(string AFile, string AMessage)
        {
            if (string.IsNullOrWhiteSpace(AFile))
            {
                AFile = FileName;
            }

            LogMessage(Path, AFile, LogLevel.Info, AMessage);
        }

        /// <summary>
        /// Infoes the specified A file.
        /// </summary>
        /// <param name="AFile">The A file.</param>
        /// <param name="AMessage">The A message.</param>
        /// <param name="args">The args.</param>
        public static void Info(string AFile, string AMessage, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(AFile))
            {
                AFile = FileName;
            }

            LogMessage(Path, AFile, LogLevel.Info, string.Format(AMessage, args));
        }

        private static void tmrWriteMessage_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (writting || (messageInfos.Count == 0))
            {
                return;
            }

            writting = true;
            tmrWriteMessage.Stop();

            try
            {
                MessageInfo msgInfo;
                StreamWriter fp;

                while (messageInfos.Count > 0)
                {
                    if (messageInfos.TryDequeue(out msgInfo))
                    {
                        try
                        {
                            if (!Directory.Exists(msgInfo.Path))
                            {
                                Directory.CreateDirectory(msgInfo.Path);
                            }

                            using (fp = new StreamWriter(System.IO.Path.Combine(msgInfo.Path, msgInfo.File), true))
                            {
                                fp.WriteLine(msgInfo.Message);
                                fp.Close();
                            }
                        }
                        catch
                        { 
                        }
                    }
                }
            }
            catch
            { 
            }
            finally
            {
                writting = false;
                tmrWriteMessage.Start();
            }
        }
    }

    /// <summary>
    /// LogLevel access
    /// </summary>
    public enum LogLevel
    {
        System = 0,
        Windows = 5,
        Error = 10,
        Warnings = 15,
        Debug = 20,
        Info = 25
    }

    public struct MessageInfo
    { 
        public string Path { get; set; }
        public string File { get; set; }
        public string Message { get; set; }
    }
}
