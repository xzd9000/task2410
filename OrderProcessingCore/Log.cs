using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingCore
{
    public class Log
    {
        public string log { get; private set; }

        public Log(Data data, FileParser parser, FileWriter writer)
        {
            data.Initialized += DataInitialized;
            data.OrderAdded += OrderAdded;
            data.OrdersRequested += OrdersRequested;

            parser.FileRead += FileRead;
            parser.ItemRead += ItemRead;
            parser.OrdersReadFromText += OrdersRead;

            writer.FileWriteBegan += FileWriteBegan;
            writer.FileLineWritten += FileLineWritten;
            writer.FileWriteEnded += FileWriteEnded;

            FileConfig.DataFileOpened += DataFileOpened;
            FileConfig.LogFileOpened += LogFileOpened;
            FileConfig.ResultFileOpened += ResultFileOpened;

            log = "";
        }

        private void FileWriteBegan()
        {
            string msg = "Начата запись в файл:" + Environment.NewLine;

            log += msg;
            LogChanged?.Invoke(msg);
        }
        private void FileLineWritten(string line)
        {
            string msg = line + Environment.NewLine;
            log += msg;
            LogChanged?.Invoke(msg);
        }
        private void FileWriteEnded()
        {
            string msg = "Запись окончена" + Environment.NewLine;

            log += msg;
            LogChanged?.Invoke(msg);
        }
        
        private void DataFileOpened(FileOpenResult result) => FileOpened(result, "Файл данных", FileConfig.dataPath);
        private void LogFileOpened(FileOpenResult result) => FileOpened(result, "Файл лога", FileConfig.logPath);
        private void ResultFileOpened(FileOpenResult result) => FileOpened(result, "Файл вывода", FileConfig.resultPath);

        private void FileOpened(FileOpenResult result, string file, string path)
        {
            string msg = "";

            if (result.HasFlag(FileOpenResult.noPath)) msg += file + ": не задан путь, будет создан файл " + path + " в папке программы" + Environment.NewLine;
            else msg += file + ": для использования указан файл:" + Environment.NewLine + path + Environment.NewLine;

            if (result.HasFlag(FileOpenResult.created)) msg += "Файл по заданному пути не найден, поэтому будет создан новый" + Environment.NewLine;
            else if (result.HasFlag(FileOpenResult.opened)) msg += "Файл успешно открыт" + Environment.NewLine;

            if (result == FileOpenResult.failed) msg += "Не удалось открыть файл" + Environment.NewLine;

            log += msg;
            LogChanged?.Invoke(msg);
        }


        private void DataInitialized()
        {
            string msg = "Список заказов инициализирован" + Environment.NewLine;

            log += msg;
            LogChanged?.Invoke(msg);
        }
        private void OrderAdded(Order order, AddOrderResult result)
        {
            string msg = "";

            if (result == AddOrderResult.Sucess)
            {
                msg += "Успешно добавлен заказ:" + Environment.NewLine +
                    "\tID: " + order.id + Environment.NewLine +
                    "\tВес: " + order.weight + Environment.NewLine +
                    "\tРайон: " + order.orderDistrict + Environment.NewLine +
                    "\tДата заказа: " + order.orderTime.ToString("dd-MM-yyyy HH:mm:ss") + Environment.NewLine;
            }
            else
            {
                msg += "Не удалось добавить заказ: " + Environment.NewLine;
                if (result.HasFlag(AddOrderResult.InvalidID))       msg += "\tНеверный ID" + Environment.NewLine;
                if (result.HasFlag(AddOrderResult.InvalidWeight))   msg += "\tНеверный вес" + Environment.NewLine;
                if (result.HasFlag(AddOrderResult.InvalidDistrict)) msg += "\tНеверный район" + Environment.NewLine;
                if (result.HasFlag(AddOrderResult.InvalidTime))     msg += "\tНеверное время" + Environment.NewLine;
            }

            log += msg;
            LogChanged?.Invoke(msg);
        }
        private void OrdersRequested(string districtFilter, DateTime? timeFilter, int ordersReturned)
        {
            string msg = "Запрошены заказы" + Environment.NewLine;
            if (districtFilter != null && districtFilter != "") msg += "Фильтр района: " + districtFilter + Environment.NewLine;
            if (timeFilter.HasValue) msg += "Фильтр времени: " + timeFilter.Value.ToString("dd-MM-yyyy HH:mm:ss") + Environment.NewLine;
            msg += "Возвращено заказов: " + ordersReturned + Environment.NewLine;

            log += msg;
            LogChanged?.Invoke(msg);
        }

        private void FileRead(int byteLength)
        {
            string msg = "Прочитан файл: (" + byteLength.ToString() + " байт)" + Environment.NewLine;

            log += msg;
            LogChanged?.Invoke(msg);
        }
        private void ItemRead(int line, string item, ItemReadResult result)
        {
            string msg = "";

            msg += "Строка " + line + ':' + Environment.NewLine + item + Environment.NewLine;

            msg += "Заказ считан " + (result == ItemReadResult.Success ? "успешно" : "неудачно:") + Environment.NewLine;
            
            if (result.HasFlag(ItemReadResult.FailLength)) msg += "Неверное количество значений в строке" + Environment.NewLine;
            else
            {
                if (result.HasFlag(ItemReadResult.FailID)) msg += "Неверный ID" + Environment.NewLine;
                if (result.HasFlag(ItemReadResult.FailWeight)) msg += "Неверный вес" + Environment.NewLine;
                if (result.HasFlag(ItemReadResult.FailDistrict)) msg += "Неверный район" + Environment.NewLine;
                if (result.HasFlag(ItemReadResult.FailDateTime)) msg += "Неверное время" + Environment.NewLine;
            }
            

            log += msg;
            LogChanged?.Invoke(msg);
        }
        private void OrdersRead(int count)
        {
            string msg = "Считано " + count + " заказов" + Environment.NewLine;

            log += msg;
            LogChanged?.Invoke(msg);
        }



        public event Action<string> LogChanged;
    }
}
