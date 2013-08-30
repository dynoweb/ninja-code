#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{	
    #region Revisions
	/*
	Revisor:	Rev date:	Zip version:		Changes:		
	[Ducman]	21Feb11		MFISMA2				First release of MFISMA2
	[Ducman]	27Feb11		MFISMA2_1			Selection of moving average type added
	*/
	#endregion
	
    /// <summary>
    /// February 2011, by Ducman.
	/// This 'MFISMA2' is my version of the Tick Volume Oscillator from Bill Blau. It's a moving average of the 'MFI(14)'.
	/// It plots different types of averages (SMA, EMA, TEMA, VMA) of which you can select one in your strategy.
    /// </summary>
    [Description("Moving average of MFI")]
    public class MFISMA2 : Indicator
    {
        #region Variables
        // Wizard generated variables
      		private int mFIperiod 		= 6; // Default setting for MFIperiod    
			private int sMAperiod 		= 3; // Default setting for SMAperiod
            private int eMAperiod 		= 3; // Default setting for EMAperiod
            private int tEMAperiod 		= 6; // Default setting for TEMAperiod
            private int vMAperiod 		= 3; // Default setting for VMAperiod
			private int vMAvolatility 	= 6; // Default setting for VMAvolatility
			private int	mFIaverageType	= 3; // Type of Average for MFISMA2. 1=SMA, 2=EMA 3=TEMA, 4=VMA
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {  
			Add(new Plot(Color.FromKnownColor(KnownColor.Orange), PlotStyle.Line, "PlotSMA"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "PlotEMA"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkViolet), PlotStyle.Line, "PlotTEMA"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Firebrick), PlotStyle.Line, "PlotVMA"));
            CalculateOnBarClose	= false;
            Overlay				= false;
            PriceTypeSupported	= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            switch (mFIaverageType) // (1)SMA, (2)EMA, (3)TEMA, (4)VMA
				{
				case (1):
				PlotSMA.Set(SMA(MFI(mFIperiod), sMAperiod)[0]);
				break;
				case (2):
				PlotEMA.Set(EMA(MFI(mFIperiod), eMAperiod)[0]);
				break;
				case (3):
				PlotTEMA.Set(TEMA(MFI(mFIperiod), tEMAperiod)[0]);
				break;
				case (4):
				PlotVMA.Set(VMA(MFI(mFIperiod), vMAperiod, vMAvolatility)[0]);
				break;
				}           			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PlotSMA
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PlotEMA
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PlotTEMA
        {
            get { return Values[2]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries PlotVMA
        {
            get { return Values[3]; }
        }
		
		[Description("Type of Moving Average for MFISMA2. 1=SMA, 2=EMA 3=TEMA, 4=VMA")]
        [Category("Parameters")]
		public int MFIaverageType
        {
            get { return mFIaverageType; }
            set { mFIaverageType = Math.Max(1, value); }
        }

        [Description("MFI period")]
        [Category("Parameters")]
        public int MFIperiod
        {
            get { return mFIperiod; }
            set { mFIperiod = Math.Max(1, value); }
        }
		
		[Description("Simple moving average")]
        [Category("Parameters")]
        public int SMAperiod
        {
            get { return sMAperiod; }
            set { sMAperiod = Math.Max(1, value); }
        }

        [Description("Exponential moving average")]
        [Category("Parameters")]
        public int EMAperiod
        {
            get { return eMAperiod; }
            set { eMAperiod = Math.Max(1, value); }
        }

        [Description("Triple exponential moving average")]
        [Category("Parameters")]
        public int TEMAperiod
        {
            get { return tEMAperiod; }
            set { tEMAperiod = Math.Max(1, value); }
        }

        [Description("Volatilty dependend moving average")]
        [Category("Parameters")]
        public int VMAperiod
        {
            get { return vMAperiod; }
            set { vMAperiod = Math.Max(1, value); }
        }
		
		[Description("VMA Volatilty")]
        [Category("Parameters")]
        public int VMAvolatility
        {
            get { return vMAvolatility; }
            set { vMAvolatility = Math.Max(1, value); }
        }
        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private MFISMA2[] cacheMFISMA2 = null;

        private static MFISMA2 checkMFISMA2 = new MFISMA2();

        /// <summary>
        /// Moving average of MFI
        /// </summary>
        /// <returns></returns>
        public MFISMA2 MFISMA2(int eMAperiod, int mFIaverageType, int mFIperiod, int sMAperiod, int tEMAperiod, int vMAperiod, int vMAvolatility)
        {
            return MFISMA2(Input, eMAperiod, mFIaverageType, mFIperiod, sMAperiod, tEMAperiod, vMAperiod, vMAvolatility);
        }

        /// <summary>
        /// Moving average of MFI
        /// </summary>
        /// <returns></returns>
        public MFISMA2 MFISMA2(Data.IDataSeries input, int eMAperiod, int mFIaverageType, int mFIperiod, int sMAperiod, int tEMAperiod, int vMAperiod, int vMAvolatility)
        {
            if (cacheMFISMA2 != null)
                for (int idx = 0; idx < cacheMFISMA2.Length; idx++)
                    if (cacheMFISMA2[idx].EMAperiod == eMAperiod && cacheMFISMA2[idx].MFIaverageType == mFIaverageType && cacheMFISMA2[idx].MFIperiod == mFIperiod && cacheMFISMA2[idx].SMAperiod == sMAperiod && cacheMFISMA2[idx].TEMAperiod == tEMAperiod && cacheMFISMA2[idx].VMAperiod == vMAperiod && cacheMFISMA2[idx].VMAvolatility == vMAvolatility && cacheMFISMA2[idx].EqualsInput(input))
                        return cacheMFISMA2[idx];

            lock (checkMFISMA2)
            {
                checkMFISMA2.EMAperiod = eMAperiod;
                eMAperiod = checkMFISMA2.EMAperiod;
                checkMFISMA2.MFIaverageType = mFIaverageType;
                mFIaverageType = checkMFISMA2.MFIaverageType;
                checkMFISMA2.MFIperiod = mFIperiod;
                mFIperiod = checkMFISMA2.MFIperiod;
                checkMFISMA2.SMAperiod = sMAperiod;
                sMAperiod = checkMFISMA2.SMAperiod;
                checkMFISMA2.TEMAperiod = tEMAperiod;
                tEMAperiod = checkMFISMA2.TEMAperiod;
                checkMFISMA2.VMAperiod = vMAperiod;
                vMAperiod = checkMFISMA2.VMAperiod;
                checkMFISMA2.VMAvolatility = vMAvolatility;
                vMAvolatility = checkMFISMA2.VMAvolatility;

                if (cacheMFISMA2 != null)
                    for (int idx = 0; idx < cacheMFISMA2.Length; idx++)
                        if (cacheMFISMA2[idx].EMAperiod == eMAperiod && cacheMFISMA2[idx].MFIaverageType == mFIaverageType && cacheMFISMA2[idx].MFIperiod == mFIperiod && cacheMFISMA2[idx].SMAperiod == sMAperiod && cacheMFISMA2[idx].TEMAperiod == tEMAperiod && cacheMFISMA2[idx].VMAperiod == vMAperiod && cacheMFISMA2[idx].VMAvolatility == vMAvolatility && cacheMFISMA2[idx].EqualsInput(input))
                            return cacheMFISMA2[idx];

                MFISMA2 indicator = new MFISMA2();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.EMAperiod = eMAperiod;
                indicator.MFIaverageType = mFIaverageType;
                indicator.MFIperiod = mFIperiod;
                indicator.SMAperiod = sMAperiod;
                indicator.TEMAperiod = tEMAperiod;
                indicator.VMAperiod = vMAperiod;
                indicator.VMAvolatility = vMAvolatility;
                Indicators.Add(indicator);
                indicator.SetUp();

                MFISMA2[] tmp = new MFISMA2[cacheMFISMA2 == null ? 1 : cacheMFISMA2.Length + 1];
                if (cacheMFISMA2 != null)
                    cacheMFISMA2.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheMFISMA2 = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// Moving average of MFI
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MFISMA2 MFISMA2(int eMAperiod, int mFIaverageType, int mFIperiod, int sMAperiod, int tEMAperiod, int vMAperiod, int vMAvolatility)
        {
            return _indicator.MFISMA2(Input, eMAperiod, mFIaverageType, mFIperiod, sMAperiod, tEMAperiod, vMAperiod, vMAvolatility);
        }

        /// <summary>
        /// Moving average of MFI
        /// </summary>
        /// <returns></returns>
        public Indicator.MFISMA2 MFISMA2(Data.IDataSeries input, int eMAperiod, int mFIaverageType, int mFIperiod, int sMAperiod, int tEMAperiod, int vMAperiod, int vMAvolatility)
        {
            return _indicator.MFISMA2(input, eMAperiod, mFIaverageType, mFIperiod, sMAperiod, tEMAperiod, vMAperiod, vMAvolatility);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Moving average of MFI
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MFISMA2 MFISMA2(int eMAperiod, int mFIaverageType, int mFIperiod, int sMAperiod, int tEMAperiod, int vMAperiod, int vMAvolatility)
        {
            return _indicator.MFISMA2(Input, eMAperiod, mFIaverageType, mFIperiod, sMAperiod, tEMAperiod, vMAperiod, vMAvolatility);
        }

        /// <summary>
        /// Moving average of MFI
        /// </summary>
        /// <returns></returns>
        public Indicator.MFISMA2 MFISMA2(Data.IDataSeries input, int eMAperiod, int mFIaverageType, int mFIperiod, int sMAperiod, int tEMAperiod, int vMAperiod, int vMAvolatility)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.MFISMA2(input, eMAperiod, mFIaverageType, mFIperiod, sMAperiod, tEMAperiod, vMAperiod, vMAvolatility);
        }
    }
}
#endregion
