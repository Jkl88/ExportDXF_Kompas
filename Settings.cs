using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExportDXF_Kompas
{
    public class Settings
    {
        public bool BreakLink { get; set; } = true;
        public bool CenterLinesVisible { get; set; } = true;
        public bool BendsLinesVisible { get; set; } = true;
        public bool BreakLinesVisible { get; set; } = false;
        public bool Disignation { get; set; } = false;
        public bool CreateViewElements { get; set; } = false;
        public bool RemoveOuterDiameter { get; set; } = false;

        public string Separator { get; set; } = "_";
        public string Sample { get; set; } = "{ИмяФайлаОриг}";

        // 🧩 список шаблонов
        public List<FileNameTemplate> Templates { get; set; } = new();

        // 🧩 имя шаблона по умолчанию
        public string DefaultTemplateName { get; set; } = "";

    }

    public class FileNameTemplate
    {
        public string Name { get; set; } = "";
        public string Pattern { get; set; } = "";
        public string Separator { get; set; } = "_";
        public string ReplaceIn { get; set; } = "";
        public string ReplaceOut { get; set; } = "";
        public bool IsDefault { get; set; } = false; // 👈 новый флаг
    }

}
