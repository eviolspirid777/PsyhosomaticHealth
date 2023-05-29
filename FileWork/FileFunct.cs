using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Disciplines;

namespace FileWorking
{
    public class FileFunct
    {
        private static string pathTo = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "disciplinesFile.json");

        static JsonSerializerOptions Options()
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true
            };
            return options;
        }

        static public void ReadData(out List<DisciplinesTypes> disciplines)
        {
            string flag = File.ReadAllText(pathTo).Trim();
            if (File.ReadAllText(pathTo).Length > 0 && flag != "")
                disciplines = JsonSerializer.Deserialize<List<DisciplinesTypes>>(File.ReadAllText(pathTo));
            else
                disciplines = new List<DisciplinesTypes>();
        }

        static public void WriteData(List<DisciplinesTypes> Discplines)
        {
            string jsonString = JsonSerializer.Serialize(Discplines, Options());
            File.WriteAllText(pathTo, jsonString);
        }

        static public StreamWriter CreateFile()
        {
            return File.CreateText(pathTo);
        }
    }
}
