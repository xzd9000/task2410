using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace OrderProcessingCore
{
    [Flags] public enum ItemReadResult
    {
        Success = 0,
        FailID = 0b0_0001,
        FailWeight = 0b0_0010,
        FailDistrict = 0b0_0100,
        FailDateTime = 0b0_1000,
        FailLength = 0b1_0000,
    }

    public class FileParser
    {
        public List<Order> ReadFile(FileStream file)
        {
            StreamReader reader = new StreamReader(file);
            reader.BaseStream.Position = 0;
            string data = reader.ReadToEnd();

            FileRead?.Invoke(data.Length * sizeof(char));

            return ReadText(data);
        }

        public List<Order> ReadText(string textData)
        {
            List<Order> ret = new List<Order>();

            string[] lines = textData.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                Order add;
                ItemReadResult result = TryReadItem(lines[i], out add);
                if (result == ItemReadResult.Success) ret.Add(add);               
                ItemRead?.Invoke(i, lines[i], result);

            }

            OrdersReadFromText?.Invoke(ret.Count);

            return ret;
        }

        public ItemReadResult TryReadItem(string item, out Order order)
        {
            int ret = 0;
            Order result = new Order();

            string[] values = item.Split(',');

            if (values.Length == 4)
            {
                if (!int.TryParse(values[0], out result.id)) ret |= (int)ItemReadResult.FailID;

                NumberFormatInfo format = ((CultureInfo)CultureInfo.CurrentUICulture.Clone()).NumberFormat;
                format.NumberDecimalSeparator = ".";

                if (!float.TryParse(values[1], NumberStyles.Number, format, out result.weight)) ret |= (int)ItemReadResult.FailWeight;

                result.orderDistrict = values[2].TrimStart(' ');
                if (result.orderDistrict == "") ret |= (int)ItemReadResult.FailDistrict;

                if (!DateTime.TryParse(values[3], out result.orderTime))
                    ret |= (int)ItemReadResult.FailDateTime;
            }
            else ret |= (int)ItemReadResult.FailLength;

            order = result;
            return (ItemReadResult)ret;
        }

        public event Action<int> FileRead;
        public event Action<int> OrdersReadFromText;
        public event Action<int, string, ItemReadResult> ItemRead;
    }
}
