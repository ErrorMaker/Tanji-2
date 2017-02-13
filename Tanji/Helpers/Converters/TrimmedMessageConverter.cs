using System;
using System.Globalization;

using Sulakore.Protocol;

namespace Tanji.Helpers.Converters
{
    public class TrimmedMessageConverter : SingleConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var message = (value as HMessage);
            if (message != null)
            {
                if (message.Length > 1000)
                {
                    var trimmed = new byte[1000];
                    byte[] data = message.ToBytes();
                    Buffer.BlockCopy(data, 0, trimmed, 0, 1000);

                    return (HMessage.ToString(trimmed) + " ... | Data Too Large");
                }
                else return message;
            }
            return value;
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}