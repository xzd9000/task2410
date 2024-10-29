using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace OrderProcessingCore
{
    public class FileWriter
    {
        public void WriteToFile(IEnumerable<Order> orders, FileStream file)
        {
            StreamWriter writer = new StreamWriter(file);

            writer.BaseStream.SetLength(0);
            writer.BaseStream.Flush();

            FileWriteBegan?.Invoke();
            writer.AutoFlush = true;

            foreach (Order order in orders)
            {
                NumberFormatInfo format = ((CultureInfo)CultureInfo.CurrentUICulture.Clone()).NumberFormat;
                format.NumberDecimalSeparator = ".";

                string line = "";
                line += order.id.ToString() + ',';
                line += order.weight.ToString(format) + ',';
                line += ((order.orderDistrict != null && order.orderDistrict != "") ? order.orderDistrict : " ") + ',';
                line += order.orderTime.ToString("dd-MM-yyyy HH:mm:ss");
                writer.WriteLine(line);

                FileLineWritten?.Invoke(line);
            }

            FileWriteEnded?.Invoke();
        }

        public event Action         FileWriteBegan;
        public event Action<string> FileLineWritten;
        public event Action         FileWriteEnded;
    }
}
