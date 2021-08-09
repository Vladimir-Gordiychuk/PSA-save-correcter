using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;

namespace PSA.Saver
{
    public class PsaSaver
    {
        IDictionary<string, int> PlayerIdMap = new Dictionary<string, int>(PsaUnits.DefaultUnitMap);

        public TextWriter Out { get; set; }

        public FileInfo AttrConv { get; set; }

        public void FixSave(FileInfo file)
        {
            if (IsBinarySaveFile(file))
            {
                Out.WriteLine("Save file has a binary (packed) format.");
                if (AttrConv != null && AttrConv.Exists)
                {
                    Out.WriteLine("Trying to unpack it with AttrConv. Administrator priviliges are required.");

                    // Wait for file system to make this file available.
                    Thread.Sleep(300);
                    UnpackSaveFile(file, AttrConv);
                    // And again, wait for file system to make this file available.
                    Thread.Sleep(300);
                }
            }

            var doc = Load(file);

            var tacticModule = GetTacticLevelModule(doc);
            if (tacticModule == null)
            {
                Out.WriteLine("Save file does not contain a TacticModule (probably, there were no battles yet.");
            }

            var elements = FindPlayerIdElements(GetTacticLevelModule(doc))
                .Where(item => !IsValidPlayerIdElement(item) && GetModification(item) != null)
                .ToList();

            var models = elements
                .Select(item => SelectModelFromModification(GetModification(item)))
                .Distinct()
                .ToList();

            if (models.Any(model => !PlayerIdMap.ContainsKey(model)))
            {
                Out.WriteLine("Unit map missing some models found in the save-file.");
                Out.WriteLine("Add the following models to your 'unit_map.csv' file:");
                foreach (var model in models)
                {
                    Out.WriteLine(model);
                }
                return;
            }

            foreach (var item in elements)
            {
                string modification = GetModification(item);

                if (modification != null)
                {
                    var correctedPlayerId = DeterminePlayerId(modification, PlayerIdMap);

                    var oldValue = item.Attribute("value").Value;

                    if (correctedPlayerId.HasValue)
                    {
                        item.Attribute("value").Value = correctedPlayerId.Value.ToString();
                    }
                    else
                    {
                        Out.WriteLine("Invalid PlayerId found but no match for '{0}' found in unit map.", modification);
                    }
                }
            }

            var modified = new FileInfo(Path.Combine(file.Directory.FullName,
                Path.GetFileNameWithoutExtension(file.Name) + "_" + file.Extension));

            Save(doc, modified);

            modified.Replace(file.FullName, file.FullName + ".backup");

            Out.WriteLine("Modifed file : {0}", file.FullName);
            Out.WriteLine("Backup  file : {0}", file.FullName + ".backup");
            Out.WriteLine("Update page in your file manager to see changes (Explorer & TotalCmd could be a bit slow on this one).");
        }

        private static string GetModification(XElement item)
        {
            var parent = item.Parent;
            var modificationElement = parent.Elements()
                .FirstOrDefault(child =>
                    child.Name == "string" &&
                    child.Attribute("name") != null &&
                    child.Attribute("name").Value == "Modification");

            if (modificationElement != null && modificationElement.Attribute("value") != null)
            {
                return modificationElement.Attribute("value").Value;
            }

            return null;
        }

        public static int? DeterminePlayerId(string modification, IDictionary<string, int> map)
        {
            string model = SelectModelFromModification(modification);

            if (map.ContainsKey(model))
            {
                return map[model];
            }
            else
            {
                return null;
            }
        }

        private static string SelectModelFromModification(string modification)
        {
            return modification.Substring(
                modification.IndexOf('.') + 1,
                modification.LastIndexOf('.') - modification.IndexOf('.') - 1);
        }

        public static XElement GetTacticLevelModule(XDocument document)
        {
            return document
                .Root
                .Element("object")
                .Elements()
                .FirstOrDefault(element => IsTacticLevelModule(element));
        }

        public static bool IsTacticLevelModule(XElement element)
        {
            return element.Attribute("name") != null && element.Attribute("name").Value == "TacticLevelModule";
        }

        public static bool IsValidPlayerIdElement(XElement element)
        {
            var valueAttribute = element.Attribute("value");
            if (valueAttribute == null)
            {
                return false;
            }
            int id;
            if (int.TryParse(valueAttribute.Value, out id))
            {
                return IsValidPlayerId(id);
            }
            return false;
        }

        public static bool IsValidPlayerId(int id)
        {
            return new[] { 0, 1, 2, 3, 4, 5, 255 }.Contains(id);
        }

        public static IEnumerable<XElement> FindPlayerIdElements(XElement element)
        {
            foreach (var child in element.Elements())
            {
                if (child.Name == "integer" && child.Attribute("name") != null && child.Attribute("name").Value == "PlayerId")
                {
                    yield return child;
                }
                else
                {
                    foreach (var item in FindPlayerIdElements(child))
                    {
                        yield return item;
                    }
                }
            }
        }

        public void MergeUnitMap(IDictionary<string, int> patch)
        {
            MergeUnitMaps(PlayerIdMap, patch);
        }

        public static void MergeUnitMaps(IDictionary<string, int> master, IDictionary<string, int> patch)
        {
            foreach (var item in patch)
            {
                if (master.ContainsKey(item.Key))
                {
                    master[item.Key] = item.Value;
                }
                else
                {
                    master.Add(item.Key, item.Value);
                }
            }
        }

        private static string ReplaceNonXmlElements(string text)
        {
            var builder = new StringBuilder(text);

            var vksRegex = new Regex(@"<\?\w+>");
            var matches = vksRegex.Matches(text);

            foreach (Match match in matches)
            {
                builder[match.Index] = '#';
                builder[match.Index + match.Length - 1] = '#';
            }

            text = builder.ToString();
            return text;
        }

        private static string RestoreNonXmlElements(string text)
        {
            var builder = new StringBuilder(text);

            var vksRegex = new Regex(@"#\?\w+#");
            var matches = vksRegex.Matches(text);

            foreach (Match match in matches)
            {
                builder[match.Index] = '<';
                builder[match.Index + match.Length - 1] = '>';
            }

            text = builder.ToString();
            return text;
        }

        public static bool IsBinarySaveFile(Stream file)
        {
            file.Seek(0, SeekOrigin.Begin);
            var header = new byte[4];
            file.Read(header, 0, 4);
            return header.SequenceEqual(new byte[] { 31, 139, 8, 0 });
        }

        public static void UnpackSaveFile(FileInfo file, FileInfo attrConv = null)
        {
            var processStartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = attrConv.FullName,
                Arguments = "\"" + file.FullName + "\""
            };
            var process = new Process() { StartInfo = processStartInfo };
            process.Start();
            process.WaitForExit();
        }

        public static bool IsBinarySaveFile(FileInfo file)
        {
            using (var stream = file.Open(FileMode.Open, FileAccess.Read))
            {
                return IsBinarySaveFile(stream);
            }
        }

        public static XDocument Load(FileInfo file)
        {
            using (var stream = file.Open(FileMode.Open, FileAccess.Read))
            {
                var reader = new StreamReader(stream);
                var text = reader.ReadToEnd();
                text = ReplaceNonXmlElements(text);

                return XDocument.Parse(text);
            }
        }

        public static void Save(XDocument document, FileInfo file)
        {
            var encoding = new UTF8Encoding(false, true);
            string text = null;

            using (var stream = new MemoryStream())
            {
                document.Save(stream, SaveOptions.None);
                text = encoding.GetString(stream.ToArray());
            }

            text = RestoreNonXmlElements(text);

            var bytes = encoding.GetBytes(text);

            using (var stream = file.Open(FileMode.Create, FileAccess.Write))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

    };
}
