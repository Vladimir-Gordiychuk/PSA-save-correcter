using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSA.Saver
{
    public static class PsaUnits
    {

        public static readonly IDictionary<string, int> DefaultUnitMap = new Dictionary<string, int>()
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

            // Neth
            { "F2A2_HL", 3 },
            { "FokkerD21", 3 },
            { "OS2U_HL", 3 },

            // Gerry
            { "Ar196", 4 },
            { "Ar234", 4 },
            { "Bf109G10", 4 },
            { "FW190A3", 4 },
            { "Ju87D", 4 },
            { "Ju88A4", 4 },

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

    };
}
