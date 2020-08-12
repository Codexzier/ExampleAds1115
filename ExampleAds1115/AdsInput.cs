namespace ExampleAds1115
{
    /// <summary>
    /// Input multiplexer configuration for ADS1115 (Bit 14 bis 12)
    /// </summary>
    public enum AdsInput
    {
        ///// <summary>
        ///// AINp = AIN0 and AINn = AIN1 (Default)
        ///// </summary>
        //AINp_AIN0_AND_AINn_AIN1_DEFAULT = 0x0000,
        //AINp_AIN0_AND_AINn_AIN3 = 0x1000,
        //AINp_AIN1_AND_AINn_AIN3 = 0x2000,
        //AINp_AIN2_AND_AINn_AIN3 = 0x3000,
        /// <summary>
        /// AINp = AIN0 and AINn = GND
        /// </summary>
        A0 = 0x4000,
        /// <summary>
        /// AINp = AIN1 and AINn = GND
        /// </summary>
        A1 = 0x5000,
        /// <summary>
        /// AINp = AIN2 and AINn = GND
        /// </summary>
        A2 = 0x6000,
        /// <summary>
        /// AINp = AIN3 and AINn = GND
        /// </summary>
        A3 = 0x7000
    }
}