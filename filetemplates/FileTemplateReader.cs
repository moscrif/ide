using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Moscrif.IDE.FileTemplates
{
    internal class FileTemplateReader
    {
        private const string SEPARATOR = "#####";
        private const string KEY_TYPE = "type";
        private const string KEY_NAME = "name";
        private const string KEY_AUTHOR = "author";
        private const string KEY_DESCRIPTION = "description";
        private const string KEY_PARAM = "param";

        private readonly FileInfo fileInfo;

        public enum AttributeType
        {
            Type, 
            Name,
            Author,
            Description
        }

        public FileTemplateReader(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
        }

        public string Read(Action<AttributeType, string> attribute, Action<string, string, string, string> param)
        {
            using (StreamReader sr = fileInfo.OpenText())
            {
                bool headDone = false;
                string line;
                StringBuilder content = new StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    if (!headDone)
                    {
                        if (line.Trim().StartsWith(SEPARATOR))
                        {
                            headDone = true;
                            continue;
                        }
                        int pos = line.IndexOf(':');
                        if (pos < 0) continue;
                        string key = line.Substring(0, pos).Trim().ToLower();
                        string val = line.Substring(pos + 1).Trim();
                        if (key == KEY_TYPE && attribute != null) attribute(AttributeType.Type, val);
                        if (key == KEY_NAME && attribute != null) attribute(AttributeType.Name, val);
                        if (key == KEY_AUTHOR && attribute != null) attribute(AttributeType.Author, val);
                        if (key == KEY_DESCRIPTION && attribute != null) attribute(AttributeType.Description, val);
                        if (key == KEY_PARAM && param != null)
                        {
                            // name type defVal validate
                            string[] x = val.Trim().Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            string name = x.Length > 0 ? x[0] : String.Empty;
                            string type = x.Length > 1 ? x[1] : String.Empty;
                            string defVal = x.Length > 2 ? x[2] : String.Empty;
                            if (defVal == "<empty>") defVal = String.Empty;
                            string validate = x.Length > 3 ? x[3] : String.Empty;
                            param(name, type, defVal, validate);
                        }
                    }
                    else
                    {
                        content.AppendLine(line);
                    }
                }
                return content.ToString();
            }
        }

    }
}
