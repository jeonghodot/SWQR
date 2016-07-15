using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;

namespace Fault_Localization_SE_Lab.Utility
{
    class XmlParser
    {
        XmlDocument xmlDoc = new XmlDocument();

        public XmlParser(string filename)
        {
            xmlDoc.Load(filename);
        }

        public void SetCountInformation(DataSet dsSourceCode,string TC_ID)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("x", "http://schemas.atlassian.com/clover3/report");

            //XmlNodeList xnList = xmlDoc.SelectNodes("//x:coverage",nsmgr);
            XmlNodeList xnList = xmlDoc.SelectNodes("//x:coverage/x:project/x:package", nsmgr);
            string name, line_num, line_count,r_value;
            string col = TC_ID;

            foreach (XmlNode xn in xnList)
            {

                name = xn.Attributes["name"].Value;

                if (name.Equals("InstrumentEditor"))
                    continue;

                foreach (XmlNode chldNode in xn.ChildNodes)
                {
                        //Read the attribute Name**
                    if (chldNode.Name == "file")
                    {                    
                        if (chldNode.HasChildNodes)
                        {
                            foreach (XmlNode item in chldNode.ChildNodes)
                            {
                                if (item.Name == "line")
                                {
                                    line_num=item.Attributes["num"].Value;
                                    line_count = item.Attributes["count"].Value;

                                    if (!line_count.Equals("0"))
                                        r_value = @"1"; //●
                                    else
                                        r_value = "0";
                                    dsSourceCode.Tables["SourceCode"].Rows[int.Parse(line_num)][col] = r_value;
                                }
                            }
                        }
                    }
                }
             
              
            }
        }

        public void SetCountInformation_CodeCoverage(DataSet dsSourceCode,  string TC_ID)
        {
            XmlNodeList xnList = xmlDoc.SelectNodes("//results/modules/module/functions/function/ranges/range");
            string line_num, covered, r_value;
            string col = TC_ID;

            foreach (XmlNode xn in xnList)
            {
                    //Read the attribute Name**

                if (xn.Name == "range")
                    {
                        line_num = xn.Attributes["start_line"].Value;
                        covered = xn.Attributes["covered"].Value;

                        if (covered.Equals("no"))
                            r_value = @"0"; //
                        else
                            r_value = "1"; //● yes, partial
                        dsSourceCode.Tables["SourceCode"].Rows[int.Parse(line_num)][col] = r_value;

                    //dsFSourceCode.Tables["FSourceCode"].Rows[int.Parse(line_num)][col] = r_value;
                    //dsPSourceCode.Tables["PSourceCode"].Rows[int.Parse(line_num)][col] = r_value;
                }
      


            }
        }



    }
}
