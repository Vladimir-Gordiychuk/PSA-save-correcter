using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;

namespace PSA.Saver
{
    class Program
    {
        static PsaConfigElement Config
        {
            get
            {
                return (ConfigurationManager.GetSection("PsaConfigSection") as PsaConfigSection).Config;
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Pacific Storm - Allies gamesave correction unitily by VG.");

            if (args.Length == 0)
            {
                PrintHelpAndInfo();
                return;
            }

            FileInfo file = LocateSaveFile(args[0]);

            if (file == null || !file.Exists)
            {
                Console.WriteLine("Specified file not found : {0}.", file.FullName);
                return;
            }

            var saver = new PsaSaver()
            {
                Out = Console.Out
            };

            var unitMapFile = new FileInfo("unit_map.csv");
            if (unitMapFile.Exists)
            {
                var patch = LoadUnitMapCsvFile(new FileInfo("unit_map.csv"));
                saver.MergeUnitMap(patch);
            }
            else
            {
                Console.WriteLine("Unit map file ({0}) not found.", unitMapFile.FullName);
                Console.WriteLine("Default PSA unit map will be used.");
            }

            try
            {
                saver.FixSave(file);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured. Error information will be presented below.");
                Console.WriteLine("Contact the author if you need help.");
                Console.WriteLine(e);
            }
        }

        private static FileInfo LocateSaveFile(string filePath)
        {
            FileInfo file = null;
            try
            {
                file = new FileInfo(filePath);
            }
            catch
            {
                Console.WriteLine("File path is invalid or not accessable.");
            }

            if (!file.Exists && filePath.Length == file.Name.Length)
            {
                // If specified file not found & no directory was specifed - check save directory specied in condig file.
                var saveDirectory = Config.SaveDirectory;
                if (saveDirectory != null && saveDirectory.GetFiles(file.Name, SearchOption.TopDirectoryOnly).Length > 0)
                {
                    file = saveDirectory.GetFiles(file.Name, SearchOption.TopDirectoryOnly)[0];
                    Console.WriteLine("Specifed file found in save directory: {0}.", file.FullName);
                }
            }

            return file;
        }

        private static void PrintHelpAndInfo()
        {
            Console.WriteLine("In order to use this utility specify file path for your Pacific Storm gamesave.");
            Console.WriteLine("Example: {0} \"{1}\".", Assembly.GetExecutingAssembly().GetName().Name, @"D:\Games\PSA\saves\save1.xml");
            Console.WriteLine("Please note that the file path should be wrapped in quotation marks");
            Console.WriteLine("(unless file path does not contain any white spaces).");
            Console.WriteLine();
            Console.WriteLine("Version : {0}", Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine("Author  : Vladimir Gordiychuk ( https://vk.com/vladimir.gordiychuk )");
            Console.WriteLine("Special thanks to Samurai Sprit community :)");
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

    }
}
