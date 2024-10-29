using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderProcessingCore;
using System.IO;
using System.Globalization;

namespace ConsoleUI
{
    class Program
    {
        private static StreamWriter logWriter = null;

        static void Main(string[] args)
        {
            string filePath = FindArgValue(args, "-data"),
                   logPath = FindArgValue(args, "-log"),
                   resultPath = FindArgValue(args, "-result"),
                   filterTime = FindArgValue(args, "-time"),
                   filterDistrict = FindArgValue(args, "-district");

            Data data = new Data();
            FileParser parser = new FileParser();
            FileWriter writer = new FileWriter();

            Log log = new Log(data, parser, writer);

            log.LogChanged += WriteLogLine;

            FileConfig.Initialize(filePath, logPath, resultPath);

            if (FileConfig.log != null)
            {
                FileConfig.log.SetLength(0);
                FileConfig.log.Flush();

                logWriter = new StreamWriter(FileConfig.log, Encoding.Unicode);
                logWriter.AutoFlush = true;
                logWriter.Write(log.log);
                log.LogChanged += WriteLogLineToFile;
            }

            if (FileConfig.data != null && FileConfig.result != null)
            {
                data.AddOrders(parser.ReadFile(FileConfig.data));

                DateTime? nTime = null;
                if (filterTime != null && DateTime.TryParse(filterTime, out DateTime time)) nTime = time;

                Order[] orders = data.GetOrders(filterDistrict, nTime);

                writer.WriteToFile(orders, FileConfig.result);

                FileConfig.Close();
            }
            else
            {
                Console.WriteLine("Не удалось открыть файл данных или файл вывода, невозможно продолжать работу");
                return;
            }
        }

        private static string FindArgValue(string[] args, string arg)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == arg && i + 1 < args.Length && args[i + 1][0] != '-') return args[i + 1];
            }

            return null;
        }

        private static void WriteLogLine(string line) => Console.WriteLine(line);
        private static void WriteLogLineToFile(string line) { if (logWriter != null) logWriter.WriteLine(line); }
    }
}
