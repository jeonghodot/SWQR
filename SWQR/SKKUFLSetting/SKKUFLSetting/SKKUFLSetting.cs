using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SKKUFLSetting
{
    class SKKUFLSetting
    {
        static void Main(string[] args)
        {
            SettingInfo.strSettingIni = Environment.CurrentDirectory + @"\Setting.ini";

            if (args.Length < 3)
            {
                MessageBox.Show(@"[SKKUFLSetting v0.2-2014-03-11]" + Environment.NewLine +
                    @"Insert args correctly. >> " + Environment.NewLine + Environment.NewLine +
                    @"SKKUFLSetting.exe AutoRun NoSourceCode NoTestSuite printtokens v2 AnswerSheet" + Environment.NewLine + Environment.NewLine +
                    @"SKKUFLSetting.exe NormalRun D:\Siemens\printtokens_2.0.tar\printtokens\versions.alt\versions.orig\v1\print_tokens.c D:\Debug\TestCase\TestCase2.xls");
            }
            else
            {
                string SettingFilepPath = string.Empty;
                SettingInfo.AutoRun = args[0];
                SettingInfo.SourceCode = args[1];
                SettingInfo.TestSuite = args[2];

                if (args.Length >= 4)
                {
                    if (!string.IsNullOrEmpty(args[3]))
                        SettingInfo.ProgramFilename = args[3];
                    if (!string.IsNullOrEmpty(args[4]))
                        SettingInfo.FaultyVersion = args[4];
                    if (!string.IsNullOrEmpty(args[5]))
                        SettingInfo.AnswerSheet = args[5];

                    try
                    {
                                           
                        if (!string.IsNullOrEmpty(args[6]))
                            SettingFilepPath = args[6];
                    }
                    catch(Exception e)
                    {

                    }

                }

                if (!string.IsNullOrEmpty(SettingFilepPath))
                {
                    SettingInfo.strSettingIni = SettingFilepPath;
                }


                if(SettingInfo.AutoRun.Equals("AutoRun"))
                    IniFile.SetIniValue("AutoRun", "AutoRun", "True", SettingInfo.strSettingIni);
                else
                    IniFile.SetIniValue("AutoRun", "AutoRun", "False", SettingInfo.strSettingIni);

                if(SettingInfo.SourceCode.Equals("NoSourceCode"))
                    IniFile.SetIniValue("AutoRun", "SourceCode", string.Empty, SettingInfo.strSettingIni);
                else
                    IniFile.SetIniValue("AutoRun", "SourceCode", SettingInfo.SourceCode, SettingInfo.strSettingIni);

                if (SettingInfo.TestSuite.Equals("NoTestSuite"))
                    IniFile.SetIniValue("AutoRun", "TestSuite", string.Empty, SettingInfo.strSettingIni);
                else
                    IniFile.SetIniValue("AutoRun", "TestSuite", SettingInfo.TestSuite, SettingInfo.strSettingIni);

                //Database
                IniFile.SetIniValue("Database", "ProgramFilename", SettingInfo.ProgramFilename, SettingInfo.strSettingIni);
                IniFile.SetIniValue("Database", "FaultyVersion", SettingInfo.FaultyVersion, SettingInfo.strSettingIni);
                IniFile.SetIniValue("Database", "AnswerSheet", SettingInfo.AnswerSheet, SettingInfo.strSettingIni);


            }
        }
    }
}
