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

        /// <summary>
        /// Input multiplexer configuration for ADS1115 (Bit 14 bis 12)
        /// </summary>
        public enum AdsInput
        {
            /// <summary>
            /// AINp = AIN0 and AINn = GND
            /// </summary>
            AIN_0 = 0x4000,
            /// <summary>
            /// AINp = AIN1 and AINn = GND
            /// </summary>
            AIN_1 = 0x5000,
            /// <summary>
            /// AINp = AIN2 and AINn = GND
            /// </summary>
            AIN_2 = 0x6000,
            /// <summary>
            /// AINp = AIN3 and AINn = GND
            /// </summary>
            AIN_3 = 0x7000
        }

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
        internal ushort ReadSingleInputValue(AdsInput input)
        {
            ushort config = 0x0003 |    // disable the comparator
                            0x0100;     // Single-shot mode (default)

            config |= (ushort)input;

            // Operational status/single-shot conversion start
            // - Begin a single conversion
            config |= 0x8000;

            // prepare configuration to send on I²C
            byte[] data = new byte[] { (byte)(config >> 8), (byte)(config & 0xff) };

            // Write to configuratioin register
            this._i2CPeripheral.WriteRegisters(0x01, data);

            // Conversion delay
            Thread.Sleep(9);

            // read from conversion register
            var result = this._i2CPeripheral.ReadRegisters(0x00, 2);

            return (ushort)((result[0] << 8) | result[1]);
        }
    }
}
