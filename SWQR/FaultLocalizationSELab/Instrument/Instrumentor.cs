using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Windows.Forms;
using System.Data;
using Fault_Localization_SE_Lab.Test;

namespace Fault_Localization_SE_Lab.Instrument
{
    class Instrumentor
    {
        public string GenInstrumentedCode(string filename, DataSet dsSourceCode, string fileExtension)
        {
            FileInfo f = new FileInfo(filename);
            string dirName = f.DirectoryName;
            string strInstrumentedFile = dirName + @"\" + Path.GetFileNameWithoutExtension(filename) + "_inst" + fileExtension; //_inst.cs
            StreamWriter SWrite = new StreamWriter(strInstrumentedFile, false, System.Text.Encoding.UTF8);

            StreamReader SRead = new StreamReader(filename, System.Text.Encoding.UTF8);
            string strFileLine = string.Empty;
            string strAppendFileLine = string.Empty;

            string inst_header = @"#include ""InstrumentEditor.h""";
            string inst_str_line = string.Empty;
            if (TestInfo.SOURCE_CODE_TYPE.ToLower().Equals(".cs"))
            {
                inst_str_line = @"InstrumentEditor.InstrumentEditor.WriteActualValue";
            }
            if (TestInfo.SOURCE_CODE_TYPE.ToLower().Equals(".c") || TestInfo.SOURCE_CODE_TYPE.ToLower().Equals(".cpp"))
            {
                inst_str_line = @"WriteActualValue";
            }
            int nRowIndex=0;
            int FnRowIndex = 0;
            int PnRowIndex = 0;

            DataRow dr;
            DataRow Fdr;
            DataRow Pdr;

            dr = dsSourceCode.Tables["SourceCode"].NewRow();
            dr[0] = (nRowIndex).ToString();
            dsSourceCode.Tables["SourceCode"].Rows.Add(dr); // add first row(reserved)


           // Fdr = dsFSourceCode.Tables["FSourceCode"].NewRow();
           // Fdr[0] = (FnRowIndex).ToString();
           // dsFSourceCode.Tables["FSourceCode"].Rows.Add(Fdr);


           //Pdr = dsPSourceCode.Tables["PSourceCode"].NewRow();
           // Pdr[0] = (PnRowIndex).ToString();
           // dsPSourceCode.Tables["PSourceCode"].Rows.Add(Pdr);

            /* insert InstrumentEditor.h" 
            if (TestInfo.SOURCE_CODE_TYPE.ToLower().Equals(".c") || TestInfo.SOURCE_CODE_TYPE.ToLower().Equals(".cpp"))
            {
                SWrite.WriteLine(inst_header);
                dr = dsSourceCode.Tables["SourceCode"].NewRow();
                dr[0] = (++nRowIndex).ToString();
                dr[1] = inst_header;
                dsSourceCode.Tables["SourceCode"].Rows.Add(dr);
            }
            */

            while ((strFileLine = SRead.ReadLine()) != null)
            {
                strAppendFileLine = string.Empty;

                dr = dsSourceCode.Tables["SourceCode"].NewRow();
                //Fdr = dsFSourceCode.Tables["FSourceCode"].NewRow();
                //Pdr = dsPSourceCode.Tables["PSourceCode"].NewRow();


                if (strFileLine.Contains(@"//@INSTRUMENT_ACTUALVALUE"))
                {
                    strAppendFileLine = string.Empty;
                    strAppendFileLine = strFileLine.Replace(@"//@INSTRUMENT_ACTUALVALUE", inst_str_line);
                    SWrite.WriteLine(strAppendFileLine);

                    dr[0] = (++nRowIndex).ToString();
                    dr[1] = strAppendFileLine;
                    dsSourceCode.Tables["SourceCode"].Rows.Add(dr);


                    //Fdr[0] = (++FnRowIndex).ToString();
                    //Fdr[1] = strAppendFileLine;
                    //dsFSourceCode.Tables["FSourceCode"].Rows.Add(Fdr);

                    //Pdr[0] = (++PnRowIndex).ToString();
                    //Pdr[1] = strAppendFileLine;
                    //dsPSourceCode.Tables["PSourceCode"].Rows.Add(Pdr);

                }
                else
                {
                    SWrite.WriteLine(strFileLine);

                    dr[0] = (++nRowIndex).ToString();
                    dr[1] = strFileLine;
                    dsSourceCode.Tables["SourceCode"].Rows.Add(dr);

                    //Fdr[0] = (++FnRowIndex).ToString();
                    //Fdr[1] = strFileLine;
                    //dsFSourceCode.Tables["FSourceCode"].Rows.Add(Fdr);

                    //Pdr[0] = (++PnRowIndex).ToString();
                    //Pdr[1] = strFileLine;
                    //dsPSourceCode.Tables["PSourceCode"].Rows.Add(Pdr);
                    continue;
                }
            }
            SRead.Close();
            SWrite.Close();

            //Instrument InstrumentEditor's namespace,too.
            dr = dsSourceCode.Tables["SourceCode"].NewRow();
            //Fdr = dsFSourceCode.Tables["FSourceCode"].NewRow();
            //Pdr = dsPSourceCode.Tables["PSourceCode"].NewRow();

            dr[0] = (++nRowIndex).ToString();
            dr[1] = "Pass/Fail Status";
            dsSourceCode.Tables["SourceCode"].Rows.Add(dr); // add last row(reserved)


            //Fdr = dsFSourceCode.Tables["FSourceCode"].NewRow();
            //Fdr[0] = (++nRowIndex).ToString();
            //Fdr[1] = "Pass/Fail Status";
            //dsFSourceCode.Tables["FSourceCode"].Rows.Add(Fdr);

            //Pdr = dsPSourceCode.Tables["PSourceCode"].NewRow();
            //Pdr[0] = (++nRowIndex).ToString();
            //Pdr[1] = "Pass/Fail Status";
            //dsPSourceCode.Tables["PSourceCode"].Rows.Add(Pdr);

            return strInstrumentedFile;
        }

        public void AdjustNamespaceOfInstrumentEditor()
        {

        }
        public bool CompileCode(string sourceFile, string exeFile)
        {
            CodeDomProvider provider = null;
     
            Console.WriteLine("Enter the source language for Hello World (cs, vb, etc):");

            if (CodeDomProvider.IsDefinedLanguage("cs"))
            {
                provider = CodeDomProvider.CreateProvider("cs");
            }

            if (provider == null)
            {
                Console.WriteLine("There is no CodeDomProvider for the input language.");
            }

              CompilerParameters cp = new CompilerParameters();

            // Generate an executable instead of 
            // a class library.
            cp.GenerateExecutable = true;

            // Set the assembly file name to generate.
            cp.OutputAssembly = exeFile;

            // Generate debug information.
            cp.IncludeDebugInformation = true;

            // Add an assembly reference.
            cp.ReferencedAssemblies.Add( "System.dll" );

            // Save the assembly as a physical file.
            cp.GenerateInMemory = false;

            // Set the level at which the compiler 
            // should start displaying warnings.
            cp.WarningLevel = 3;

            // Set whether to treat all warnings as errors.
            cp.TreatWarningsAsErrors = false;

            // Set compiler argument to optimize output.
            cp.CompilerOptions = "/optimize";

            // Set a temporary files collection.
            // The TempFileCollection stores the temporary files
            // generated during a build in the current directory,
            // and does not delete them after compilation.
            cp.TempFiles = new TempFileCollection(".", true);

            if (provider.Supports(GeneratorSupport.EntryPointMethod))
            {
                // Specify the class that contains 
                // the main method of the executable.
                cp.MainClass = "ConsoleApplication1.Program";
            }

            if (Directory.Exists("Resources"))
            {
                if (provider.Supports(GeneratorSupport.Resources))
                {
                    // Set the embedded resource file of the assembly.
                    // This is useful for culture-neutral resources,
                    // or default (fallback) resources.
                    cp.EmbeddedResources.Add("Resources\\Default.resources");

                    // Set the linked resource reference files of the assembly.
                    // These resources are included in separate assembly files,
                    // typically localized for a specific language and culture.
                    cp.LinkedResources.Add("Resources\\nb-no.resources");
                }
            }

            // Invoke compilation.
            CompilerResults cr = provider.CompileAssemblyFromFile(cp, sourceFile);

            if(cr.Errors.Count > 0)
            {
                // Display compilation errors.
                Console.WriteLine("Errors building {0} into {1}",  
                    sourceFile, cr.PathToAssembly);
                foreach(CompilerError ce in cr.Errors)
                {
                    Console.WriteLine("  {0}", ce.ToString());
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Source {0} built into {1} successfully.",
                    sourceFile, cr.PathToAssembly);
                Console.WriteLine("{0} temporary files created during the compilation.",
                    cp.TempFiles.Count.ToString());

            }

            // Return the results of compilation.
            if (cr.Errors.Count > 0)
            {
                return false;
            }
            else 
            {
                return true;
            }

        }

        public void Compile(string sourcefile,string exefile)
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            ICodeCompiler icc = codeProvider.CreateCompiler();
            string Output = exefile;

            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            //Make sure we generate an EXE, not a DLL
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = Output;
            //parameters.ReferencedAssemblies.Add("System.dll");
            //parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            CompilerResults results = icc.CompileAssemblyFromSource(parameters, sourcefile);

            if (results.Errors.Count > 0)
            {
                foreach (CompilerError CompErr in results.Errors)
                {
                    MessageBox.Show(
                                "Line number " + CompErr.Line +
                                ", Error Number: " + CompErr.ErrorNumber +
                                ", '" + CompErr.ErrorText + ";" +
                                Environment.NewLine + Environment.NewLine);
                }
            }
            else
            {

            }

        }

        public void GetReflectionInfo(string filename) //for get namespace
        {
            FileInfo f = new FileInfo(filename);
            string dirName = f.DirectoryName;
            List<string> sources = new List<string>();

            //foreach (string file in Directory.GetFiles(dirName, "*.cs"))
            {
                //sources.Add(File.ReadAllText(file));
                sources.Add(File.ReadAllText(filename));
            }

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true;
            //parameters.GenerateExecutable = true;
            parameters.GenerateInMemory = true;

            //parameters.ReferencedAssemblies.Add("mscorlib.dll");
            //parameters.ReferencedAssemblies.Add("System.dll");
            // etc

            var provider = new CSharpCodeProvider();
            var results = provider.CompileAssemblyFromSource(parameters, sources.ToArray());
            //CompilerResults cr = provider.CompileAssemblyFromFile(cp, sources.);

            if (results.Errors.HasErrors)
            {
                // display results.Errors
                foreach (CompilerError error in results.Errors)
                {
                    Console.WriteLine("COMPILER ERROR: " + error.ErrorText);
                }
            }

            var assembly = results.CompiledAssembly;
            var types = assembly.GetTypes();

            foreach (Type type in types)
            {
                string strNameSpace = type.Namespace;
                string name = type.Name;

                TestInfo.strNameSpace = strNameSpace;
                TestInfo.strClassName = name;

                var properties = type.GetProperties(); // public properties
                PropertyInfo[] pinfos = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                foreach (PropertyInfo p in pinfos)
                {
                    Console.WriteLine(p.Name);
                }
                // etc
                BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
                Console.WriteLine("\n함수 목록");
                MethodInfo[] mList = type.GetMethods(bindingAttr);
                foreach (MethodInfo m in mList)
                    Console.WriteLine(m);

                Console.WriteLine("\n변수 목록");
                FieldInfo[] fList = type.GetFields();
                foreach (FieldInfo fi in fList)
                    Console.WriteLine(f);

    

            }
        }
    }
}
