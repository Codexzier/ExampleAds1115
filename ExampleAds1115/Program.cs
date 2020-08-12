using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ExampleAds1115
{
    class Program
    {
        public static void Main(string[] args)     
        {
            if (args.Length > 0 && args[0] == "--exitOnDebug")
            {
                // Wrong way. Application cannot be debugged in this mode. 
                // Right-click on the project and click Deploy on the context menu.
                return;
            }

            var adConverter = new Ads1115();

            Console.WriteLine("Converter was initialize");

            while (true)
            {
                var resultA0 = adConverter.ReadInputValue(AdsInput.A0);

                Console.Write($"Input A0: {resultA0}, ");

                if(resultA0 <= 0)
                {
                    Console.WriteLine($"Voltage: 0V");
                }
                else
                {
                    // 5v ca. 22610
                    var voltage = (float)(resultA0 / 22610f) * 5.0f;
                    Console.WriteLine($"Voltage: {voltage}V");
                }
                
                Thread.Sleep(1000);
            }
        }
    }
}
