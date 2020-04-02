﻿using System;
using System.Windows;
using System.Timers;
using System.Globalization;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace client
{
    public partial class MainWindow : Window
    {
        private void GetAndRenewAssistData(Object source, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                assistInformation = SplitAssist.Split(ParseAssist(GetAssistFromInet()));
                foreach (Subscriber s in subs)
                {
                    foreach (string a in assistInformation)
                    {
                        Subscriber s_ = s;
                        s_.assistData = a;
                        Client.SendMessage("3" + s_.Serialize());
                        string res = Client.GetMessage();
                        if (res == "0")
                            Logging(String.Format("Отправлены дополнительные сведения абоненту: IMSI = {0}, IMEI-SV = {1}", s.imsi, s.imeiSV));
                        RenewAssistDataForSub(s);
                    }
                }
            });
        }
        private void RenewAssistDataForSub(Subscriber sub)
        {
            foreach (string a in assistInformation)
            {
                Subscriber s_ = sub;
                s_.assistData = a;
                Client.SendMessage("2" + s_.Serialize());
                string res = Client.GetMessage();
                if (res == "0")
                    Logging(String.Format("Отправлены дополнительные сведения абоненту: IMSI = {0}, IMEI-SV = {1}", sub.imsi, sub.imeiSV));
            }
        }
        public void RenewAssistDataPeriodically()
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                assistTimer = new System.Timers.Timer(float.Parse(qParams.timeRefreshEphem, CultureInfo.InvariantCulture) * 1000 * 60 * 60); assistTimer = new System.Timers.Timer(float.Parse(qParams.timeRefreshEphem, CultureInfo.InvariantCulture) * 1000 * 60 * 60);
                assistTimer.Elapsed += GetAndRenewAssistData;
                assistTimer.AutoReset = true;
                assistTimer.Enabled = true;
                Logging("Запущена процедура обновления дополнительной информациии о спутниках");
            });
        }
        private string GetAssistFromInet()
        {
            try
            {
                string res;
                using (var client = new WebClient())
                {
                    string q = qParams.GetQuery();
                    res = client.DownloadString(q);
                }
                return res;
            }
            catch (Exception e)
            {
                Logging(e.Message);
                return "";
            }
        }
        public List<string> ParseAssist(string assist)
        {
            List<string> res = new List<string>();
            foreach (Match m in Regex.Matches(assist, @"apdu=([0-9A-Z]+)"))
            {
                res.Add(m.Value.Substring(5));
            }
            return res;
        }
    }
}