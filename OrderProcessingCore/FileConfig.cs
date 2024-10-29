using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OrderProcessingCore
{
    [Flags] public enum FileOpenResult
    {
        failed = 0,
        opened = 0b001,
        created = 0b010,
        noPath = 0b100
    }

    public static class FileConfig
    {
        private static FileStream _data;
        private static FileStream _log;
        private static FileStream _result;

        public static string dataPath { get; private set; }
        public static string logPath { get; private set; }
        public static string resultPath { get; private set; }

        public static FileStream data => _data;
        public static FileStream log => _log;
        public static FileStream result => _result;

        public static void Initialize(string dataPath = null, string logPath = null, string resultPath = null)
        {
            FileConfig.dataPath = dataPath;
            FileConfig.logPath = logPath;
            FileConfig.resultPath = resultPath;

            OpenFile(dataPath, "data", out _data, DataFileOpened);
            OpenFile(logPath, "log.txt", out _log, LogFileOpened);
            OpenFile(resultPath, "result.txt", out _result, ResultFileOpened);
        }

        public static void Close()
        {
            if (_data != null) _data.Close();
            if (_log != null) _log.Close();
            if (_result != null) _result.Close();
        }

        private static FileOpenResult OpenFile(string path, string default_, out FileStream file, Action<FileOpenResult> event_)
        {
            string finalPath = path;
            int ret = (int)FileOpenResult.failed;
            file = null;

            if (path == null || path == "")
            {
                finalPath = default_;
                ret |= (int)FileOpenResult.noPath;
            }

            int flag = File.Exists(finalPath) ? (int)FileOpenResult.opened : (int)FileOpenResult.created;

            try
            {
                file = File.Open(finalPath, FileMode.OpenOrCreate);
                ret |= flag;
            }
            catch { }

            event_?.Invoke((FileOpenResult)ret);

            return (FileOpenResult)ret;
        }

        public static event Action<FileOpenResult> DataFileOpened;
        public static event Action<FileOpenResult> LogFileOpened;
        public static event Action<FileOpenResult> ResultFileOpened;
    }
}
