using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System;
using System.Threading;

namespace ExampleAds1115
{
    /// <summary>
    /// This class is a simple and shortes version to read a single value from the analog digital-inputs. The default setup for input is +/- 6.144V range (limited to VDD +0.3V max!).
    /// </summary>
    internal class SimpleAds1115 : App<F7Micro, SimpleAds1115>
    {
        private readonly II2cPeripheral _i2CPeripheral;

        public SimpleAds1115()
        {
            var i2CBus = Device.CreateI2cBus();
            this._i2CPeripheral = new I2cPeripheral(i2CBus, 0x48);
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal ushort ReadSingleInputValue(AdInput input)
        {
            ushort config = 0x0003 |    // disable the comparator
               0x0100;                  // Single-shot mode (default)
            //0x0080 |                 // 128 samples per second (default)

            config |= GetConfigAnalogInput(input);

            // Operational status/single-shot conversion start
            // - Begin a single conversion
            config |= 0x8000;

            byte[] data = new byte[] { (byte)(config >> 8), (byte)(config & 0xff) };
            // Write to configuratioin register
            this._i2CPeripheral.WriteRegisters(0x01, data);

            // Conversion delay
            Thread.Sleep(9);

            // read from conversion register
            var result = this._i2CPeripheral.ReadRegisters(0x00, 2);

            return (ushort)((result[0] << 8) | result[1]);
        }

        /// <summary>
        /// Mapped config bytes for mapped input setup.
        /// </summary>
        /// <param name="input">Set the input port.</param>
        /// <returns>Return the bit setup.</returns>
        private static ushort GetConfigAnalogInput(AdInput input)
        {
            switch (input)
            {
                case AdInput.A0: return 0x4000;
                case AdInput.A1: return 0x5000;
                case AdInput.A2: return 0x6000;
                case AdInput.A3: return 0x7000;
                default:
                    throw new ArgumentOutOfRangeException(nameof(AdInput), $"Check parameter: {input}");
            }
        }
    }
}
