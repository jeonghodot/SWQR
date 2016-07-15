using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Fault_Localization_SE_Lab.Utility
{
    static class Logger
    {
        static string strLogFilename = string.Empty;
        static string strLogFilename2 = string.Empty;

        static StreamWriter SWrite;

        /*
        public Logger()
        {
        }
        ~Logger()
        {

        }
         */
        static public void CreateLogger(string filename)
        {
            string LogPath = Environment.CurrentDirectory + @"\Log";
            Directory.CreateDirectory(LogPath);
            string time_stamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            filename = LogPath + @"\" + time_stamp + "_" + filename + ".txt";
            strLogFilename = filename;
            
            //SWrite.WriteLine("Test");

        }

        static public void WriteLine(string msg)
        {
            SWrite = new StreamWriter(strLogFilename, true, System.Text.Encoding.UTF8);
            string time_stamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            time_stamp = "[" + time_stamp + "] ";
            SWrite.WriteLine(time_stamp + msg);
            SWrite.Close();
        }

        static public void WriteLine2(string msg)
        {

            strLogFilename2 = @"C:\SKKUFL\ISESOriginal\FaultLocalizationSELab\bin\Debug\Log\report.txt";

            SWrite = new StreamWriter(strLogFilename2, true, System.Text.Encoding.UTF8);
            //string time_stamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            //time_stamp = "[" + time_stamp + "] ";
            SWrite.WriteLine(msg);
            SWrite.Close();
        }

        static public void WriteLine(string type, string msg)
        {
            SWrite = new StreamWriter(strLogFilename, true, System.Text.Encoding.UTF8);
            string time_stamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            time_stamp = "[" + time_stamp + "] ";

            if(type.Equals("ex"))
                SWrite.WriteLine(time_stamp + "(Exception) " + msg);

            SWrite.Close();
        }


    }
}
