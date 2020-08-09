using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System;
using System.Threading;

namespace ExampleAds1115
{
    /// <summary>
    /// Data and procedures I have explored via the Datasheet PDF 
    /// and the sources of an existing library for Arduino (https://github.com/adafruit/Adafruit_ADS1X15).
    /// NOTE: The class only implements the method for the single measurement mode.
    /// </summary>
    internal class Ads1115 : App<F7Micro, Ads1115>
    {
        private const byte ADDRESS = 0x48;

        #region Pointer REGISTER

        private const byte ADS1015_REG_POINTER_CONFIG = 0x01;
        private const byte ADS1015_REG_POINTER_CONVERT = 0x00;

        #endregion

        #region CONFIG REGISTER

        /// <summary>
        /// +/- 6.144V range (limited to VDD +0.3V max!)
        /// </summary>
        private const ushort ADS1015_REG_CONFIG_PGA_6_144V = 0x0000;

        private const ushort ADS1015_REG_CONFIG_CQUE_NONE = 0x0003;
        private const ushort ADS1015_REG_CONFIG_OS_SINGLE = 0x8000;

        private const ushort ADS1015_REG_CONFIG_CLAT_NONLAT = 0x0000;
        private const ushort ADS1015_REG_CONFIG_CPOL_ACTVLOW = 0x0000;
        private const ushort ADS1015_REG_CONFIG_CMODE_TRAD = 0x0000;
        private const ushort ADS1015_REG_CONFIG_DR_1600SPS = 0x0080;
        private const ushort ADS1015_REG_CONFIG_MODE_SINGLE = 0x0100;

        #endregion



        #region Single-ended Analog Input

        private const int ADS1015_REG_CONFIG_MUX_SINGLE_0 = 0x4000;
        private const int ADS1015_REG_CONFIG_MUX_SINGLE_1 = 0x5000;
        private const int ADS1015_REG_CONFIG_MUX_SINGLE_2 = 0x6000;
        private const int ADS1015_REG_CONFIG_MUX_SINGLE_3 = 0x7000;

        #endregion

        private readonly II2cPeripheral _i2CPeripheral;

        /// <summary>
        /// The setting for the expected voltage level at the analog inputs.
        /// </summary>
        private ushort _gain = 0;

        public Ads1115()
        {
            var i2CBus = Device.CreateI2cBus();
            this._i2CPeripheral = new I2cPeripheral(i2CBus, ADDRESS);
            this._gain = ADS1015_REG_CONFIG_PGA_6_144V;
        }

        /// <summary>
        /// Reads the signal from the selected channel.
        /// </summary>
        /// <param name="input">Set the input channel.</param>
        /// <returns>Return the result of the input voltage</returns>
        internal ushort ReadSingleInputValue(AdInput input)
        {
            ushort config = ADS1015_REG_CONFIG_CQUE_NONE |  // disable the comparator
                ADS1015_REG_CONFIG_CLAT_NONLAT |            // Non-latching (default val)
                ADS1015_REG_CONFIG_CPOL_ACTVLOW |           // Alert/Rdy active low   (default val)
                ADS1015_REG_CONFIG_CMODE_TRAD |             // Traditional comparator (default val)
                ADS1015_REG_CONFIG_DR_1600SPS |             // 1600 samples per second (default)
                ADS1015_REG_CONFIG_MODE_SINGLE;             // Single-shot mode (default)

            // set PGA/voltage range
            config |= this._gain;
            config |= GetConfigAnalogInput(input);

            config |= ADS1015_REG_CONFIG_OS_SINGLE;

            // ADS1015_REG_POINTER_CONFIG
            byte[] data = new byte[] { (byte)(config >> 8), (byte)(config & 0xff) };
            this._i2CPeripheral.WriteRegisters(ADS1015_REG_POINTER_CONFIG, data);

            // Conversion delay
            Thread.Sleep(9);

            return this._i2CPeripheral.ReadRegister(ADS1015_REG_POINTER_CONVERT);
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
                case AdInput.A0:
                    return ADS1015_REG_CONFIG_MUX_SINGLE_0;
                case AdInput.A1:
                    return ADS1015_REG_CONFIG_MUX_SINGLE_1;
                case AdInput.A2:
                    return ADS1015_REG_CONFIG_MUX_SINGLE_2;
                case AdInput.A3:
                    return ADS1015_REG_CONFIG_MUX_SINGLE_3;
                default:
                    throw new ArgumentOutOfRangeException(nameof(AdInput), $"Check parameter: {input}");
            }
        }
    }
}