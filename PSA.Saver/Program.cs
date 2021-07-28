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
            // Usa Navy
            { "Iowa", 0 },
            { "Benson", 0 },
            { "Lexington", 0 },
            { "Simarron", 0 },
            { "Liberty", 0 },
            { "Gato", 0 },
            { "Baltimor", 0 },
            { "Casablanca", 0 },
            { "Clivlend", 0 },
            { "Colorado", 0 },
            { "Elco Torpedo", 0 },
            { "Essex", 0 },
            { "Evarts", 0 },
            { "Fletcher", 0 },

            // Usa Airforces
            { "A-20", 0 },
            { "B-17", 0 },
            { "B-24", 0 },
            { "B-24-T", 0 },
            { "B-25-AT", 0 },
            { "B-25-BM", 0 },
            { "B-29", 0 },
            { "F2A2", 0 },
            { "F2A3", 0 },
            { "F4F", 0 },
            { "F4U", 0 },
            { "F5U", 0 },
            { "F6F", 0 },
            { "F8F", 0 },
            { "F8F2", 0 },
            { "OS2U", 0 },
            { "P-38", 0 },
            { "P-39", 0 },
            { "P-40", 0 },
            { "P-47", 0 },
            { "P-51", 0 },
            { "P-80", 0 },
            { "PBY", 0 },
            { "PBY-T", 0 },
            { "SB2C", 0 },
            { "SBD", 0 },
            { "TBD", 0 },
            { "TBF", 0 },
            { "Ventura", 0 },

            // Japanise Airforces
            { "A5M4", 1 },
            { "A6M", 1 },
            { "A7M", 1 },
            { "B5N", 1 },
            { "B5N", 1 },
            { "B7A", 1 },
            { "D3A", 1 },
            { "D4Y", 1 },
            { "F1M", 1 },
            { "G4M", 1 },
            { "G4M-T", 1 },
            { "G8N", 1 },
            { "G10N", 1 },
            { "H8K", 1 },
            { "H8K2B", 1 },
            { "J7WJ", 1 },
            { "J7WP", 1 },
            { "Ki-21", 1 },
            { "Ki-43", 1 },
            { "Ki-61", 1 },
            { "Ki-67", 1 },
            { "Ki-84", 1 },
            { "Ki-96", 1 },
            { "Ki-100", 1 },
            { "Ki-100-1", 1 },
            { "M6A", 1 },
            { "N1K-J", 1 },
            { "P1Y", 1 },
            { "P1YS", 1 },

            // Japanise Navy
            { "Akizuki", 1 },
            { "Kagero", 1 },
            { "Myoko", 1 },
            { "Mogami", 1 },
            { "Nagato", 1 },
            { "Yamato", 1 },
            { "Agano", 1 },
            { "Akagi", 1 },
            { "Zuikaku", 1 },
            { "Kazahaia", 1 },
            { "Daihatsu", 1 },
            { "Hokoku", 1 },
            { "Shimushu", 1 },
            { "Taiho", 1 },
            { "I58", 1 },
            { "I400", 1 },
            { "Otsu-Gata", 1 },
            { "Kaiten", 1 },

            // British Navy
            { "Canonesa", 2 },
            { "Dorsetshire", 2 },
            { "Eclipse", 2 },
            { "Fiji", 2 },
            { "Hood", 2 },
            { "Hunt", 2 },
            { "Illustrious", 2 },
            { "KingGeorgV", 2 },
            { "MTB", 2 },
            { "Tempest", 2 },
            { "Tribal", 2 },
            { "Sildra", 2 },

            // British Airforces
            { "Barracuda2", 2 },
            { "Beaufighter", 2 },
            { "Blenheim4", 2 },
            { "F2A2_ENG", 2 },
            { "Huric2c", 2 },
            { "LancasterB1", 2 },
            { "Lancaster-T", 2 },
            { "PBY_ENG", 2 },
            { "PBY_ENG-T", 2 },
            { "SeaFire", 2 },
            { "SeaFuryFB11", 2 },
            { "SeaHuric2c", 2 },
            { "Spit5c", 2 },
            { "Swordfish2", 2 },
            { "TBF_ENG", 2 },
            { "VampirFB9", 2 },
            { "Ventura_ENG", 2 },
            { "Walrus", 2 },

            // Gerry
            { "Ar196", 3 },
            { "Ar234", 3 },
            { "Bf109G10", 3 },
            { "FW190A3", 3 },
            { "Ju87D", 3 },
            { "Ju88A4", 3 },

            // Neth
            { "F2A2_HL", 4 },
            { "FokkerD21", 4 },
            { "OS2U_HL", 4 },

            // USSR
            { "A-20_USSR", 5 },
            { "I16t24", 5 },
            { "Il2", 5 },
            { "Il4", 5 },
            { "La5FN", 5 },
            { "OS2U_USSR", 5 },
            { "PBY_USSR", 5 },
            { "Spit5b", 5 },
            { "Yak3", 5 },
            { "Yak9T", 5 }
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
                if (modification != null)
                {
                    item.Attribute("value").Value = DeterminePlayerId(modification).ToString();
                }
            }
            doc.Save(file.FullName + "_");
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

        public static XDocument Load(FileInfo file) {
            using (var stream = file.Open(FileMode.Open, FileAccess.Read)) {
                var reader = new StreamReader(stream);
                var text = reader.ReadToEnd();
                var builder = new StringBuilder(text);

                var vksRegex = new Regex(@"<\?\w+>");
                var matches = vksRegex.Matches(text);

                foreach (Match match in matches)
                {
                    builder[match.Index] = '#';
                    builder[match.Index + match.Length] = '#';
                }

                return XDocument.Parse(builder.ToString());
            }
        }
    }
}
