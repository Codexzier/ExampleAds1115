using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
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
        /// <summary>
        /// Pointer Register. The four register are accessed by writing register byte. (Bit 1 und Bit 0)
        /// </summary>
        public enum RegisterAddress
        {
            ConversionRegister = 0x00,
            ConfigRegister = 0x01,
            LoThreshRegister = 0x02,
            HiThreshRegister = 0x03
        }

        /// <summary>
        /// OS: Operational status / single-shot conversion start.
        /// This option can only written when in power-down mode. (Bits[15])
        /// </summary>
        public enum OperationalStatus
        {
            /// <summary>
            /// Write: No effect. 
            /// Read: Device is currently performing a conversion
            /// </summary>
            Status = 0x0000,
            /// <summary>
            /// Write: Begin a single conversion (when in power-down mode). 
            /// Read: Device is not currently performing a conversion
            /// </summary>
            SingleShot = 0x8000
        }

        /// <summary>
        /// MUX: Input multiplexer configuration (Bits[14:12])
        /// </summary>
        public enum InputMultiplexerConfiguration
        {
            /// <summary>
            /// 000: AINp = AIN0 and AINn = AIN1 (default)
            /// </summary>
            AINpAIN0_AINnAIN1 = 0x0000,
            /// <summary>
            /// 001: AINP = AIN0 and AINN = AIN3
            /// </summary>
            AINpAIN0_AINnAIN3 = 0x0,
            /// <summary>
            /// 010: AINP = AIN1 and AINN = AIN3
            /// </summary>
            AINpAIN1_AINnAIN3,
            /// <summary>
            /// 011: AINP = AIN2 and AINN = AIN3
            /// </summary>
            AINpAIN2_AINnAIN3,
            /// <summary>
            /// 100: AINP = AIN0 and AINN = GND
            /// </summary>
            AINpAIN0_AINnGND,
            /// <summary>
            /// 101: AINP = AIN1 and AINN = GND
            /// </summary>
            AINpAIN1_AINnGND,
            /// <summary>
            /// 110: AINP = AIN2 and AINN = GND
            /// </summary>
            AINpAIN2_AINnGND,
            /// <summary>
            /// 111: AINP = AIN3 and AINN = GND
            /// </summary>
            AINpAIN3_AINnGND
        }

        /// <summary>
        /// PGA: Programmable gain amplifier configuration (Bits[11:9])
        /// </summary>
        public enum ProgrammableGainAmplifier
        {
            /// <summary>
            /// Full scall range of ADC scaling. In no event should more than VDD + 0.3V be applied to this device.
            /// </summary>
            FS_6_144V = 0x0000,
            /// <summary>
            /// In no event should more than VDD + 0.3V be applied to this device.
            /// </summary>
            FS_4_096V = 0x0200,
            FS_2_048V = 0x0400,
            FS_1_024V = 0x0600,
            FS_0_512V = 0x0800,
            FS_0_256V = 0x0A00
        }

        /// <summary>
        /// MODE: Device operating mode for continuous or power-down single-shot (Bits[8])
        /// </summary>
        public enum DeviceOperatingMode
        {
            ContinuousConversion = 0x0000,
            PowerDownSingleShot = 0x0100
        }

        /// <summary>
        /// DR: Date rate setting (Bits[7:5]).
        /// </summary>
        public enum DataRate
        {
            SPS8 = 0x0000,
            SPS16 = 0x0020,
            SPS32 = 0x0040,
            SPS64 = 0x0060,
            /// <summary>
            /// Default rate
            /// </summary>
            SPS128 = 0x0080,
            SPS250 = 0x00A0,
            SPS475 = 0x00C0,
            SPS860 = 0x00E0
        }

        /// <summary>
        /// COMP_MODE: Comparator mode (Bits[4])
        /// </summary>
        public enum ComparatorMode
        {
            /// <summary>
            /// Traditional comparator with hysteresis (default)
            /// </summary>
            Traditional = 0x0000,
            /// <summary>
            /// Window comparator.
            /// </summary>
            Window = 0x0010
        }

        /// <summary>
        /// COMP_POL: Comparator polarity (Bits[3])
        /// </summary>
        public enum ComparatorPolarity
        {
            ActiveLow = 0x0000,
            ActiveHigh = 0x0008
        }

        /// <summary>
        /// COMP_LAT: Latching comparator (Bits[2])
        /// </summary>
        public enum LatchingComparator
        {
            /// <summary>
            /// default
            /// </summary>
            NonLatching = 0x0000,
            Latching = 0x0002
        }

        /// <summary>
        /// COMP_QUE: Comparator queue and disable (Bits[1:0])
        /// </summary>
        public enum ComparatorQueueAndDisable
        {
            AssertAfterOneConversion = 0x0000,
            AssertAfterTwoConversion = 0x0001,
            AssertAfterFourConversion = 0x0002,
            /// <summary>
            /// Default
            /// </summary>
            Disable = 0x0003,
        }

      
        private readonly II2cPeripheral _i2CPeripheral;

        public Ads1115(AdsAddress address = AdsAddress.Default)
        {
            var i2CBus = Device.CreateI2cBus();
            this._i2CPeripheral = new I2cPeripheral(i2CBus, (byte)address);
        }

        /// <summary>
        /// Reads the signal from the selected channel.
        /// </summary>
        /// <param name="input">Set the input channel.</param>
        /// <returns>Return the result of the input voltage</returns>
        internal ushort ReadInputValue(AdsInput input)
        {
            ushort config = (ushort)ComparatorQueueAndDisable.Disable;
            config |= (ushort)LatchingComparator.NonLatching;
            config |= (ushort)ComparatorPolarity.ActiveLow;
            config |= (ushort)ComparatorMode.Traditional;
            config |= (ushort)DataRate.SPS128;
            config |= (ushort)DeviceOperatingMode.PowerDownSingleShot;             
            config |= (ushort)ProgrammableGainAmplifier.FS_6_144V;
            config |= (ushort)input;
            config |= (ushort)OperationalStatus.SingleShot;

            // ADS1015_REG_POINTER_CONFIG
            byte[] data = new byte[] { (byte)(config >> 8), (byte)(config & 0xff) };
            this._i2CPeripheral.WriteRegisters((byte)RegisterAddress.ConfigRegister, data);

            // Conversion delay
            Thread.Sleep(9);

            // read from conversion register
            var result = this._i2CPeripheral.ReadRegisters((byte)RegisterAddress.ConversionRegister, 2);

            return (ushort)((result[0] << 8) | result[1]);
        }
    }
}