using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
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
        /// Read value from input. It read a single shot of the analog volatage.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal ushort ReadSingleInputValue(AdsInput input)
        {
            // disable the comparator
            // Single-shot mode (default)
            // Operational status/single-shot conversion start
            // - Begin a single conversion
            ushort config = 0x8103;

            config |= (ushort)input;

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
