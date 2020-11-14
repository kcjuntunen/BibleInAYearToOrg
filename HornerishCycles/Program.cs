using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;

namespace HornerishCycles
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileInPath = args[0];
            var cycles = GetCyclesFromCsv(fileInPath);

            var fileOutPath = args[1];
            var startDate = args.Length > 2 ? DateTime.Parse(args[2]) : DateTime.Now;
            var range = args.Length > 3 ? Convert.ToInt32(args[3]) : 365;
            WriteOrgFile(cycles, fileOutPath, startDate, range);
        }

        private static void WriteOrgFile(Dictionary<int, List<string>> cycles, string fileOutPath, DateTime startDate, int range = 365)
        {
            using (var sw = new StreamWriter(fileOutPath))
            {
                sw.WriteLine($"#+title: Horner Reading Plan, start date [{startDate.ToString("yyyy-MM-dd ddd")}]");
                sw.WriteLine("#+columns: %Item %IntendedDate %ListNumber %Closed");
                for (int i = 0; i < range; i++)
                {
                    var date = startDate.AddDays(i);
                    var dayNumber = $"Day {i + 1}";
                    sw.WriteLine($"* {dayNumber}");
                    sw.WriteLine("    :PROPERTIES:");
                    sw.WriteLine($"    :CUSTOM_ID: {Guid.NewGuid().ToString()}");
                    sw.WriteLine($"    :IntendedDate: [{date.ToString("yyyy-MM-dd ddd")}]");
                    sw.WriteLine("    :END:");
                    foreach (var key in cycles.Keys)
                    {
                        var listNumber = (key / 2) + 1;
                        sw.WriteLine($"** {cycles[key][i % (cycles[key].Count)]}");
                        sw.WriteLine("    :PROPERTIES:");
                        sw.WriteLine($"    :CUSTOM_ID: {Guid.NewGuid().ToString()}");
                        sw.WriteLine($"    :ListNumber: {listNumber}");
                        sw.WriteLine("    :END:");
                    }
                }
            }
        }

        private static Dictionary<int, List<string>>  GetCyclesFromCsv(string filePath)
        {
            var data = new Dictionary<int, List<string>>();
            using (var sr = File.Open(filePath, FileMode.Open))
            {
                using (var tfp = new TextFieldParser(sr)
                {
                    Delimiters = new[] { "," },
                    HasFieldsEnclosedInQuotes = true,
                    TrimWhiteSpace = true,
                    TextFieldType = FieldType.Delimited
                })
                {
                    var lineCount = 0;
                    while (!tfp.EndOfData)
                    {
                        var datum = tfp.ReadFields();

                        if (++lineCount < 2) continue;


                        for (int i = 0; i < datum.Length; i++)
                        {
                            var book = i % 2 == 0;

                            if (book)
                            {
                                if (!data.ContainsKey(i))
                                    data.Add(i, new List<string>());

                                var bookName = datum[i].Trim();
                                var chapter = datum[i + 1].Trim();

                                if (string.IsNullOrWhiteSpace(bookName)) continue;

                                data[i].Add($"{bookName} {chapter}");
                            }
                        }
                    }
                }
            }
            return data;
        }
    }
}
