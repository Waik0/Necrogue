using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterScriptTemplate
{
    public static string Template =
        @"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
[MasterPath(""%1"")]
public class %2 : IMasterRecord
    {
        public int Id { get => id; }

        %3
    }
";
    public static string FieldTemplate = "public %1 %2";
    public class Config
    {
        public string ClassName = "";
        public string MasterPath = "";
        public List<(string def, string name)> Colums;
    }

    public static string MasterTemplate(Config config)
    {
        if (string.IsNullOrEmpty(config.ClassName) ||
           string.IsNullOrEmpty(config.MasterPath) ||
           config.Colums == null)
        {
            return "";
        }
        var fields = "";
        foreach(var col in config.Colums)
        {
            var c = FieldTemplate;
            c.Replace("%1", col.def);
            c.Replace("%1", col.name);
            fields += c + "\n";
        }
        
        var t = Template;
        t.Replace("%1", config.MasterPath);
        t.Replace("%2", config.ClassName);
        t.Replace("%3", fields);
        return t;
    }
}

