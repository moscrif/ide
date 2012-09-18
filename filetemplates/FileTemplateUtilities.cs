using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using Antlr3.ST;
using System.Collections;

namespace Moscrif.IDE.FileTemplates
{
    internal static class FileTemplateUtilities
    {
        public static string Apply(string template, Dictionary<string, object> @params )//= null
        {
            StringTemplate st = new StringTemplate(template);
            AddDefaultAttributes(st);
            if (@params != null) AddAttributes(st, @params);
            return st.ToString();
        }

        public static void AddDefaultAttributes(StringTemplate st)
        {
            st.SetAttribute("username", Environment.UserName);
            st.SetAttribute("computername", Environment.MachineName);
            st.SetAttribute("now", DateTime.Now);
        }

        public static void AddAttributes(StringTemplate st, Dictionary<string, object> @params)
        {
            foreach(string key in @params.Keys) {
                object val = @params[key];
                if (val != null) {
                    if (val is string && String.IsNullOrEmpty(val as string)) 
                        continue;
                    /*
                   if (val is ArrayList)
                    {
                        //st.SetAttribute(key, (val as ArrayList).ToArray());
                        ArrayList al = val as ArrayList;
                        foreach (object o in al)
                            st.SetAttribute(key, o);
                    }
                    else
                    */
                        st.SetAttribute(key, val);
                }
            }
        }
    }
}
