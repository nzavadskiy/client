using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace client
{
    public static class SplitAssist
    {
        const int headLength = 23;
        const string satNum = "0000";
        const int satNumLength = 4;
        const int satAssistLenght = 68 * 8 + 9;
        const string moreDataFlagYes = "1";
        const string moreDataFlagNo = "0";
        public static string BinaryStringToHexString(string binary)
        {
            StringBuilder result = new StringBuilder(binary.Length / 8 + 1);
            int mod4Len = binary.Length % 8;
            if (mod4Len != 0)
            {
                // pad to length multiple of 8
                binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
            }
            for (int i = 0; i < binary.Length; i += 8)
            {
                string eightBits = binary.Substring(i, 8);
                result.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
            }
            return result.ToString();
        }
        public static string HexStringToBinaryString(string hex)
        {
            return String.Join(String.Empty, hex.Select(c =>
                Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
        }
        public static List<string> Split(List<string> assist)
        {
            List<string> newAssist = new List<string>();
            foreach (string s in assist)
            {
                if (s.Length == 78)
                    newAssist.Add(s);
                else if (s.Length == 422)
                {
                    string binAssist;
                    binAssist = HexStringToBinaryString(s);
                    string headAssist = binAssist.Substring(0, 23);
                    for (int i = 0; i < 3; i++)
                    {
                        string satAssist = binAssist.Substring(headLength + satNumLength + satAssistLenght * i, satAssistLenght);
                        string result = headAssist + satNum + satAssist + moreDataFlagYes;
                        int zerosToAdd = 8 - result.Length % 8;
                        for (int j = 0; j < zerosToAdd; j++)
                            result += '0';
                        newAssist.Add(BinaryStringToHexString(result));
                    }
                }
            }
            return newAssist;
        }
        public static Geolocation GetGeolocation(Subscriber sub)
        {
            string assist = sub.assistData;
            assist = assist.Substring(14);
            assist = HexStringToBinaryString(assist);
            assist = assist.Substring(6);
            assist = assist.Substring(0, assist.Length - (assist.Length % 8));
            assist = BinaryStringToHexString(assist);
            string lat = assist.Substring(2, 6);
            string lon = assist.Substring(8, 6);
            int decLat = Convert.ToInt32(lat, 16);
            int decLon = Convert.ToInt32(lon, 16);
            double _decLat = (double)decLat * 90 / 8388608;
            double _decLon = (double)decLon * 360 / 16777216;
            return new Geolocation(sub.imsi, sub.imeiSV, _decLat, _decLon, DateTime.Now.ToLongTimeString());
        }
    }    
}