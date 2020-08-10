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
                var resultA0 = adConverter.ReadSingleInputValue(AdInput.A0);

                Console.WriteLine($"Input A0: {resultA0}");
                Thread.Sleep(1000);
            }
        }
    }
}
