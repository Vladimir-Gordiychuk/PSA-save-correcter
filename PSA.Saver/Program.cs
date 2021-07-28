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
        static IDictionary<string, int> PlayerIdMap = new Dictionary<string, int>()
        {
            {  "Airplanes.F1M.mod1", 1 }
        };

        static void Main(string[] args)
        {
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

                item.Attribute("value").Value = DeterminePlayerId(modification).ToString();
            }
            doc.Save(file.FullName + "_");
        }

        public static int DeterminePlayerId(string modification)
        {
            return PlayerIdMap[modification];
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
            return new[] { 0, 2, 3, 4, 5, 255 }.Contains(id);
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

        public static XDocument Load(FileInfo file) {
            using (var stream = file.Open(FileMode.Open, FileAccess.Read)) {
                var reader = new StreamReader(stream);
                var text = reader.ReadToEnd();

                var vksRegex = new Regex(@"<\?\w+>");
                text = vksRegex.Replace(text, "blob");

                return XDocument.Parse(text);
            }
        }
    }
}
