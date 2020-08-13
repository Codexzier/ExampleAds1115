using System;
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

            var adConverter = new SimpleAds1115();

            Console.WriteLine("Converter was initialize");

            while (true)
            {
                var resultA0 = adConverter.ReadSingleInputValue(AdsInput.A0);

                Console.Write($"Input A0: {resultA0}, ");
                Console.WriteLine($"Voltage: {GetVoltage(resultA0)}V");

                Thread.Sleep(1000);
            }
        }

        private static float GetVoltage(ushort value)
        {
            if(value <= 0)
            {
                return 0;
            }

            return (float)(value / 22610f) * 5.0f;
        }
    }
}
