using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Fault_Localization_SE_Lab.Utility
{
   public class Node

   {

       public Node(string n) { Title = n; }

       public string Title { get; set; }

       public bool IsSelected { get; set; }

   }

   public class ObservableNodeList : ObservableCollection<Node>
   {

       public ObservableNodeList()
       {

       }

       public override string ToString()
       {

           StringBuilder outString = new StringBuilder();

           foreach (Node s in this.Items)
           {

               if (s.IsSelected == true)
               {

                   outString.Append(s.Title);

                   outString.Append(',');

               }

           }

           return outString.ToString().TrimEnd(new char[] { ',' });

       }
   }
}
