using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;

namespace LinkBankAcc
{
    public class Common
    {
        StreamWriter VietFileText;
        public void Log_event(string s_function, string sEvent)
        {
            try
            {
                string sSource = null;
                string sLog = null;
                string sMachine = null;
                sSource = "WebDBConnect - " + s_function;
                sLog = "Application";
                sMachine = ".";
                EventLog ELog = new EventLog(sLog, sMachine, sSource);
                ELog.WriteEntry(sEvent, EventLogEntryType.Error, 234, Convert.ToInt16(3));
            }
            catch (Exception ex)
            {
                string sSource = null;
                string sLog = null;
                string sMachine = null;
                sSource = "WebDBConnect - " + s_function;
                sLog = "Application";
                sMachine = ".";
                EventLog ELog = new EventLog(sLog, sMachine, sSource);
                ELog.WriteEntry(ex.Message, EventLogEntryType.Error, 234);
            }
        }

        public string So_chuan(double so)
        {
            try
            {
                string so1 = "";
                if (so < 10)
                {
                    so1 = "0" + so.ToString();
                }
                else
                {
                    so1 = so.ToString();
                }
                return so1.ToString();
            }
            catch (Exception ex)
            {
                this.Log_event("So_chuan",ex.Message);
                return "";
            }

        }

        public string Bon_So_chuan(string so)
        {
            string kq = "";
            try
            {
                int l = so.Length;
                switch (l)
                {
                    case 1:
                        kq = "000" + so.ToString();
                        break;
                    case 2:
                        kq = "00" + so.ToString();
                        break;
                    case 3:
                        kq = "0" + so.ToString();
                        break;
                    case 4:
                        kq = so.ToString();
                        break;
                }
                return kq.ToString();
            }
            catch (Exception ex)
            {
                Log_event("Bon_So_chuan", ex.Message);
                return "";
            }
        }

        public string Current_DATE(string Date_type)
        {
            DateTime dt = System.DateTime.Now;
            string F_Time = dt.ToShortDateString();
            try
            {
                switch (Date_type.ToLower())
                {
                    case "yyyy_mm_dd":
                        F_Time = So_chuan(dt.Year).ToString() + "_" + So_chuan(dt.Month).ToString() + "_" + So_chuan(dt.Day).ToString();
                        break;
                    case "yyyymmdd":
                        F_Time = So_chuan(dt.Year).ToString() + So_chuan(dt.Month).ToString() + So_chuan(dt.Day).ToString();
                        break;
                    case "dd/mm/yyyy hh:pp:ss":
                        F_Time = So_chuan(dt.Day).ToString() + "/" + So_chuan(dt.Month).ToString() + "/" + So_chuan(dt.Year).ToString() + " " + So_chuan(dt.Hour).ToString() + ":" + So_chuan(dt.Minute).ToString() + ":" + So_chuan(dt.Second).ToString();
                        break;
                    case "hh:mm:ss:ms":
                        F_Time = So_chuan(dt.Hour).ToString() + ":" + So_chuan(dt.Minute).ToString() + ":" + So_chuan(dt.Second).ToString() + ":" + Bon_So_chuan(dt.Millisecond.ToString()).ToString();
                        break;
                    //case "HH:mm:ss":
                    case "hh:mm:ss":
                        F_Time = So_chuan(dt.Hour).ToString() + ":" + So_chuan(dt.Minute).ToString() + ":" + So_chuan(dt.Second).ToString();
                        break;
                    //case "dd/MM/yyyy":
                    case "dd/mm/yyyy":
                        F_Time = So_chuan(dt.Day).ToString() + "/" + So_chuan(dt.Month).ToString() + "/" + So_chuan(dt.Year).ToString();
                        break;
                }
                return F_Time;
            }
            catch (Exception ex)
            {
                Log_event("Current_DATE", ex.Message);
                return "";
            }
        }

        public string Convert_String2DateTime(string data, string Date_type)
        {
            DateTime dt = Convert.ToDateTime(data);
            string F_Time = dt.ToShortDateString();
            try
            {
                switch (Date_type.ToLower())
                {
                    case "yyyy_mm_dd":
                        F_Time = So_chuan(dt.Year).ToString() + "_" + So_chuan(dt.Month).ToString() + "_" + So_chuan(dt.Day).ToString();
                        break;
                    case "yyyymmdd":
                        F_Time = So_chuan(dt.Year).ToString() + So_chuan(dt.Month).ToString() + So_chuan(dt.Day).ToString();
                        break;
                    case "dd/mm/yyyy hh:pp:ss":
                        F_Time = So_chuan(dt.Day).ToString() + "/" + So_chuan(dt.Month).ToString() + "/" + So_chuan(dt.Year).ToString() + " " + So_chuan(dt.Hour).ToString() + ":" + So_chuan(dt.Minute).ToString() + ":" + So_chuan(dt.Second).ToString();
                        break;
                    case "hh:mm:ss:ms":
                        F_Time = So_chuan(dt.Hour).ToString() + ":" + So_chuan(dt.Minute).ToString() + ":" + So_chuan(dt.Second).ToString() + ":" + Bon_So_chuan(dt.Millisecond.ToString()).ToString();
                        break;
                    //case "HH:mm:ss":
                    case "hh:mm:ss":
                        F_Time = So_chuan(dt.Hour).ToString() + ":" + So_chuan(dt.Minute).ToString() + ":" + So_chuan(dt.Second).ToString();
                        break;
                    //case "dd/MM/yyyy":
                    case "dd/mm/yyyy":
                        F_Time = So_chuan(dt.Day).ToString() + "/" + So_chuan(dt.Month).ToString() + "/" + So_chuan(dt.Year).ToString();
                        break;
                }
                return F_Time;
            }
            catch (Exception ex)
            {
                Log_event("Current_DATE", ex.Message);
                return "";
            }
        }

        public void CREATE_FILE_LOG(string FILE_N)
        {
            try
            {
                string log_folder = System.Environment.CurrentDirectory + "/logs";
                string file_path = log_folder + "/" + FILE_N;
                if (File.Exists(file_path) == false)
                {
                    if (!Directory.Exists(log_folder))
                    {
                        Directory.CreateDirectory(log_folder);
                    }
                    VietFileText = File.CreateText(file_path);
                    VietFileText.Close();
                }
            }
            catch (Exception ex)
            {
                Log_event("CREATE_FILE_LOG", ex.Message);
            }
        }


        public void UPDATE_FILE_LOG(string FILE_NAME, string p_Sukien, string p_hamgoi, string Err_str, string logtype, string V_TLID)
        {
            try
            {
                if (File.Exists(FILE_NAME) == false)
                {
                    // MessageBox.Show("Không tìm thấy file " + FILE_NAME, "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
                    return;
                }
                else
                {
                    VietFileText = File.AppendText(FILE_NAME);
                    VietFileText.WriteLine("====" + V_TLID + "===" + Current_DATE("dd/mm/yyyy hh:pp:ss") + "=============================");
                    VietFileText.WriteLine("  Sự kiện : " + p_Sukien);
                    VietFileText.WriteLine("  Hàm gọi : " + p_hamgoi);
                    VietFileText.WriteLine("  Logtype : " + logtype);
                    VietFileText.WriteLine("  ex.tostring : " + Err_str);
                    VietFileText.WriteLine("-------------------------------------------");
                    VietFileText.WriteLine("  ");
                    VietFileText.Close();
                }

            }
            catch (Exception ex)
            {
                Log_event("UPDATE_FILE_LOG", ex.Message);
            }
        }

    }
}