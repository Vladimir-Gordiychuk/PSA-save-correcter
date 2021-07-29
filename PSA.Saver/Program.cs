using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PSA.Saver
{
    class Program
    {
        static IDictionary<string, int> PlayerIdMap = new Dictionary<string, int>(PsaUnits.DefaultUnitMap);

        static void Main(string[] args)
        {
            Console.WriteLine("Pacific Storm - Allies gamesave correction unitily by VG.");
            Console.WriteLine("Special for Samurai Sprit community :)");

            try
            {
                var patch = LoadUnitMapCsvFile(new FileInfo("unit_map.csv"));
                MergeUnitMaps(PlayerIdMap, patch);

                const string path = "../../../samples/save18.xml";
                var file = new FileInfo(path);
                var doc = Load(file);
                var elements = FindPlayerIdElements(GetTacticLevelModule(doc)).ToList();

                foreach (var item in elements.Where(item => !IsValidPlayerIdElement(item)))
                {
                    var parent = item.Parent;
                    var modificationElement = parent.Elements()
                        .FirstOrDefault(child =>
                            child.Name == "string" &&
                            child.Attribute("name") != null &&
                            child.Attribute("name").Value == "Modification");
                    string modification = null;
                    if (modificationElement != null && modificationElement.Attribute("value") != null)
                    {
                        modification = modificationElement.Attribute("value").Value;
                    }
                    if (modification != null)
                    {
                        var oldValue = item.Attribute("value").Value;
                        item.Attribute("value").Value = DeterminePlayerId(modification).ToString();
                        Console.WriteLine("Fixed {0} record: {1} -> {2}", modification, oldValue, DeterminePlayerId(modification));
                    }
                }

                Save(doc, new FileInfo(file.FullName + "_"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static int DeterminePlayerId(string modification)
        {
            var model = modification.Substring(
                modification.IndexOf('.') + 1,
                modification.LastIndexOf('.') - modification.IndexOf('.') - 1);

            return PlayerIdMap[model];
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

        public static IDictionary<string, int> LoadUnitMapCsvFile(FileInfo file)
        {
            string text = null;
            using (var stream = file.Open(FileMode.Open, FileAccess.Read))
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                text = Encoding.UTF8.GetString(bytes);
            }

            var lines = text.Split(new[] { "\n\r", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

            var map = new Dictionary<string, int>();

            foreach (var line in lines)
            {
                var columns = line.Split(',');
                string model = columns[0].Substring(1, columns[0].Length - 2);
                int playerId = int.Parse(columns[1]);
                map.Add(model, playerId);
            }

            return map;
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

        
    }
}
