using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleInAYearToOrg
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = args[0];
            var startDate = DateTime.Parse(args[1]);

            var outputFile = CreateOrgFile(filePath, startDate);
            Console.WriteLine(outputFile);
            Console.ReadLine();
        }

        private static string CreateOrgFile(string filePath, DateTime startDate)
        {
            var fn = $"biay-{DateTime.Now.ToString("yyyyMMdd-HHmmssfff")}.org";

            using (var sr = new StreamReader(filePath))
            {
                var o = JsonConvert.DeserializeObject<Biay>(sr.ReadToEnd());

                using (var sw = new StreamWriter(fn))
                {
                    var daysFromStart = 0;
                    sw.WriteLine($"#+title: {o.info}, start date [{startDate.ToString("yyyy-MM-dd ddd")}]");
                    sw.WriteLine("#+columns: %Item %IntendedDate %Closed");
                    sw.WriteLine();
                    foreach (var datum in o.data2)
                    {
                        var thisDate = startDate.AddDays(daysFromStart++);
                        foreach (var item in datum)
                        {
                            sw.WriteLine($"* {item}");
                            sw.WriteLine("  :PROPERTIES:");
                            sw.WriteLine($"  :CUSTOM_ID: {Guid.NewGuid().ToString()}");
                            sw.WriteLine($"  :IntendedDate: [{thisDate.ToString("yyyy-MM-dd ddd")}]");
                            sw.WriteLine("  :END:");
                        }
                    }
                }
            }

            return fn;
        }
    }
}
