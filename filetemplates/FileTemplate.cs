using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Moscrif.IDE.FileTemplates
{
    public class FileTemplate
    {
        public class Attribute
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public object Value { get; set; }
            public string DefaultValue { get; set; }
            public string ValidateExpr { get; set; }
            public override string ToString()
            {
                return String.Format("{0}:{1} = {2}", Name, Type, Value);
            }
        }

        public FileInfo FileInfo { get; private set; }
        public string Content { get; private set; }
        public string TemplateType { get; private set; }
        public string Name { get; private set; }
        public string Author { get; private set; }
        public string Description { get; private set; }
        public List<Attribute> Attributes { get; private set; }

        public FileTemplate(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
            Attributes = new List<Attribute>();

            Content = new FileTemplateReader(fileInfo).Read(
                (type, val) =>
                {
                    switch (type)
                    {
                        case FileTemplateReader.AttributeType.Type: TemplateType = val; break;
                        case FileTemplateReader.AttributeType.Name: Name = val; break;
                        case FileTemplateReader.AttributeType.Author: Author = val; break;
                        case FileTemplateReader.AttributeType.Description: Description = val; break;
                    }
                },
                (name, type, defVal, validate) =>
                {
                    Attributes.Add(new Attribute
                    {
                        Name = name,
                        Type = type,
                        Value = defVal,
                        DefaultValue = defVal,
                        ValidateExpr = validate
                    });
                }
            );
        }

        public Attribute this[string key]
        {
            get { return this.Attributes.Find(x => String.Compare(x.Name, key, true) == 0); }
        }

        public Dictionary<string, object> GetAttributesAsDictionary()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            //Attributes.ForEach(x => dict[x.Name] = x.Value);
            Attributes.ForEach(x => dict[x.Name] = x.Type != "bool" ? x.Value : (x.Value.ToString().ToLower() == "false" ? String.Empty : x.Value.ToString().ToLower()));
            return dict;
        }

        public override string ToString()
        {
            return Name;
        }
    }

}
