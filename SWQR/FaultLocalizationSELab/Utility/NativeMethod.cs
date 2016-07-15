using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Diagnostics;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;
using Excel; //ExcelReader
/* To work eith EPPlus library */
using OfficeOpenXml;
using OfficeOpenXml.Drawing;

namespace Fault_Localization_SE_Lab.Utility
{
    static class NativeMethod
    {
        static public string getFileFullName(string ext)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Multiselect = false;
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            //if you want filter only .txt file
            //dlg.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            //if you want filter all files

            string ftDesc = string.Empty;
            string strLeft = string.Empty;
            string strRight = string.Empty;
            string[] arr_split = ext.Split(';');

            foreach (string split in arr_split)
            {
                if (string.IsNullOrEmpty(strLeft))
                {
                    strLeft = split;
                }
                else
                {
                    strLeft += ", " + split;
                }

                if (string.IsNullOrEmpty(strRight))
                {
                    strRight = "*." + split; 
                }
                else
                {
                    strRight += ";" + "*." + split;
                }


            }
            ftDesc = strLeft + "|" + strRight;

            //string ftDesc = ext + @" Files (*." + ext + @") | *." + ext;
            //openFileDialog.Filter = "cs Files (*.cs) | *.cs";
            openFileDialog.Filter = ftDesc;
            Nullable<bool> result = openFileDialog.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                return openFileDialog.FileName;
            }

            return "Select file...";
        }

        static public void RunCommandLine(string cmd, string param, string workingPath)
        {
            Process myProcess = new Process();

            try
            {
                myProcess.StartInfo.UseShellExecute = false;
                // You can start any process, HelloWorld is a do-nothing example.
                myProcess.StartInfo.FileName = cmd;
                myProcess.StartInfo.Arguments = param;
                myProcess.StartInfo.WorkingDirectory = workingPath;
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();
                // This code assumes the process you are starting will terminate itself. 
                // Given that is is started without a window so you cannot terminate it 
                // on the desktop, it must terminate itself or you can do it programmatically
                // from this application using the Kill method.
                bool bRetrun = myProcess.WaitForExit(5000);

                if (!bRetrun)
                {
                    myProcess.Kill();                 
                    Logger.WriteLine(myProcess.ProcessName.ToString() + " Process was killed");
                }
            
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static public void RunCommandLineWIthVisualStudioPrompt(string cmd, string OS_ARCH_TYPE, string workingPath)
        {
            Process myProcess = new Process();

            try
            {
                myProcess.StartInfo.UseShellExecute = false;
                // You can start any process, HelloWorld is a do-nothing example.
                myProcess.StartInfo.FileName = "cmd";
                myProcess.StartInfo.Arguments = OS_ARCH_TYPE;//@"/k ""C:\Program Files\Microsoft Visual Studio 10.0\VC\vcvarsall.bat"" x86";
                myProcess.StartInfo.WorkingDirectory = workingPath;
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.StartInfo.UseShellExecute = false;

                myProcess.StartInfo.RedirectStandardError = true;
                myProcess.StartInfo.RedirectStandardInput = true;
                myProcess.StartInfo.RedirectStandardOutput = true;

                myProcess.Start();
                myProcess.StandardInput.Write(cmd + Environment.NewLine);
                myProcess.StandardInput.Close();
                // This code assumes the process you are starting will terminate itself. 
                // Given that is is started without a window so you cannot terminate it 
                // on the desktop, it must terminate itself or you can do it programmatically
                // from this application using the Kill method.

                string result = myProcess.StandardOutput.ReadToEnd();
                Logger.WriteLine(result);

                bool bRetrun = myProcess.WaitForExit(5000);
                if (!bRetrun)
                {
                    myProcess.Kill();
                    Logger.WriteLine(myProcess.ProcessName.ToString() + " Process was killed");
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static public DataTable WorksheetToDataTable(DataTable dt, ExcelWorksheet oSheet)
        {
            int totalRows = oSheet.Dimension.End.Row;
            int totalCols = oSheet.Dimension.End.Column;
            //DataTable dt = new DataTable(oSheet.Name);
            DataRow dr = null;
            for (int i = 1; i <= totalRows; i++)
            {
                if (i > 1) dr = dt.Rows.Add();
                for (int j = 1; j <= totalCols; j++)
                {
                    if (j == 650)
                        Console.WriteLine();
                    if (i == 1)
                    {
                        try
                        {
                            dt.Columns.Add(oSheet.Cells[i, j].Value.ToString(), typeof(string));
                        }
                        catch (Exception e)
                        {

                        }

                    }
                    else
                        dr[j - 1] = oSheet.Cells[i, j].Value.ToString();
                }
            }

            FileInfo workBook = null;
            try
            {
                //create FileInfo object  to read you ExcelWorkbook
                workBook = new FileInfo(@"C:\test.xlsx");
                using (ExcelPackage xlPackage = new ExcelPackage(workBook))
                {

                    ExcelWorksheet wsSourceCode = xlPackage.Workbook.Worksheets.Add("SourceCode");
                    wsSourceCode.Cells["A1"].LoadFromDataTable(dt, true);

                    xlPackage.Save();
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                //set workbook object to null
                if (workBook != null)
                    workBook = null;
            }

            return dt;
        }

        static public DataSet GetDataFromExcel(string filePath, string ver)
        {
            try
            {

                FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                //...
                //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                //...
                //3. DataSet - The result of each spreadsheet will be created in the result.Tables
                //...
                //4. DataSet - Create column names from first row
                excelReader.IsFirstRowAsColumnNames = true;
                DataSet ds = excelReader.AsDataSet();

                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        static public DataSet GetDataFromExcel(string filePath)
        {
            try
            {
                string strConn;
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filePath + ";" + "Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\"";
                //strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filePath + ";" + "Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1;TypeGuessRows=0;ImportMixedTypes=Text\"";
                
                string Sheet1;
                OleDbConnection conn = new OleDbConnection(strConn);
                {
                    conn.Open();
                    DataTable dtSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    Sheet1 = dtSchema.Rows[0].Field<string>("TABLE_NAME");
                    conn.Close();
                }

                //strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filePath + ";" + "Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1;TypeGuessRows=0;ImportMixedTypes=Text\"";
                //OleDbConnection conn = new OleDbConnection(strConn);
                //string strSQL = "SELECT * FROM [Sheet1$]";
                string strSQL = "SELECT * FROM [" + Sheet1 +"]";

                OleDbCommand cmd = new OleDbCommand(strSQL, conn);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataSet ds = new DataSet();

                da.Fill(ds);

                return ds;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        static public string GetTestResultFromTXT(string filePath)
        {
            try
            {
                StreamReader SRead = new StreamReader(filePath, System.Text.Encoding.UTF8);
                string strFileLine = string.Empty;
                if ((strFileLine = SRead.ReadToEnd()) == null)
                {
                    MessageBox.Show("filePath is null");
                }

                SRead.Close();
                return strFileLine;
            }
            catch (Exception ex)
            {
                return "";
            }
          
        }

        static public void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            string[] files = Directory.GetFiles(sourceFolder);
            string[] folders = Directory.GetDirectories(sourceFolder);

            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest,true);
            }

            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }

    }
}
