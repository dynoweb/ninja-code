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
    /// <summary>
    /// Rick's Volume Spread Analysis
	/// I'm trying to change this to a volatility breakout indicator
    /// </summary>
    [Description("Rick's Volatility Breakout Indicator")]
    public class AAVSA : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 20; // Default setting for Period
            private double threshold = 3.5; // Default setting for Period
	        private Color _backColorTrend = Color.Transparent;
	        private Color _backColorChop = Color.Silver;

//			private int volumePeriod = 20; // Default setting for VolumePeriod
//            private int downClose = 25; // Default setting for DownClose
//            private int upClose = 70; // Default setting for UpClose
//            private int tightSpread = 70; // Default setting for TightSpread
//            private int wideSpread = 150; // Default setting for TightSpread
//            private int extremeVolume = 200; // Default setting for TightSpread
//            private int highVolume = 150; // Default setting for TightSpread
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Indigo), PlotStyle.Line, "VHFLine"));
            Add(new Plot(Color.FromKnownColor(KnownColor.BlueViolet), PlotStyle.Line, "CongestionIdx"));
			Add(new Line(System.Drawing.Color.Red,Threshold,"Threshold"));
			Plots[0].Pen = new Pen(Color.Indigo, 2); 
			Lines[0].Pen = new Pen(Color.Red, 2);

			CalculateOnBarClose	= true;
            Overlay				= false;
            PriceTypeSupported	= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar < Period + 1)
				return;

			double Numerator = MAX(Close,Period)[0] - MIN(Close,Period)[0];
			double Denominator = 0;
		
			for(int i=0; i < Period; i++)
			{
				Denominator = Denominator + Math.Abs(Close[i] - Close[i+1]);
			}

			VHFLine.Set(10 * Numerator/Denominator);

			
			//	The congestion index is calculated by taking the difference between the 
			// highest close price and the lowest close price for a specific period and 
			// then dividing the result by the lowest close price for the same period. 
			// Finally, the index value is multiplied by 100.
			double congestionIndex = 100 * (MAX(Close,Period)[0] - MIN(Close,Period)[0])/MIN(Close, Period)[0];
			CongestionIdx.Set(congestionIndex);
			
//			if (Volume[0] > SMA(Volume, volumePeriod)[0] * extremeVolume / 100)
//				DrawSquare(CurrentBar + "vol", false, 0, High[0] * 1.005, Color.Blue);
//			else if (Volume[0] > SMA(Volume, volumePeriod)[0] * highVolume / 100)
//				DrawSquare(CurrentBar + "vol", false, 0, High[0] * 1.005, Color.Cyan);

			
			if (VHFLine[0] < Threshold && CongestionIdx[0] < Threshold)
				BackColor = _backColorChop;
			else 
				BackColor = _backColorTrend;
        }

        #region Properties

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries VHFLine
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries CongestionIdx
        {
            get { return Values[1]; }
        }

        [Description("Period over which VHF is calculated")]
        [Category("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }
		
        [Description("Volatility Threshold")]
        [Category("Parameters")]
        public double Threshold
        {
            get { return threshold; }
            set { threshold = Math.Max(0, value); }
        }
		
        [XmlIgnore]
        [Description("Background color when price is trending.")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("01. Trend backcolor")]
        public Color BackColorTrend
        {
            get { return _backColorTrend; }
            set { _backColorTrend = value; }
        }

        [Browsable(false)]
        public string BackColorTrendSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(_backColorTrend); }
            set { _backColorTrend = Gui.Design.SerializableColor.FromString(value); }
        }		
		
        [XmlIgnore]
        [Description("Background color when price is oscillating")]
        [Category("Visual")]
        [Gui.Design.DisplayNameAttribute("02. Chop back color")]
        public Color BackColorChop
        {
            get { return _backColorChop; }
            set { _backColorChop = value; }
        }

        [Browsable(false)]
        public string BackColorChopSerialize
        {
            get { return Gui.Design.SerializableColor.ToString(_backColorChop); }
            set { _backColorChop = Gui.Design.SerializableColor.FromString(value); }
        }		
		
//        [Description("Volume Period")]
//        [GridCategory("Parameters")]
//        public int VolumePeriod
//        {
//            get { return volumePeriod; }
//            set { volumePeriod = Math.Max(1, value); }
//        }
//
//        [Description("Down Close(%)")]
//        [GridCategory("Parameters")]
//        public int DownClose
//        {
//            get { return downClose; }
//            set { downClose = Math.Max(1, value); }
//        }
//
//        [Description("Up Close(%)")]
//        [GridCategory("Parameters")]
//        public int UpClose
//        {
//            get { return upClose; }
//            set { upClose = Math.Max(1, value); }
//        }
//
//        [Description("Tight Spread")]
//        [GridCategory("Parameters")]
//        public int TightSpread
//        {
//            get { return tightSpread; }
//            set { tightSpread = Math.Max(1, value); }
//        }
//
//        [Description("Wide Spread")]
//        [GridCategory("Parameters")]
//        public int WideSpread
//        {
//            get { return wideSpread; }
//            set { wideSpread = Math.Max(1, value); }
//        }
//
//        [Description("Extreme Volume")]
//        [GridCategory("Parameters")]
//        public int ExtremeVolume
//        {
//            get { return extremeVolume; }
//            set { extremeVolume = Math.Max(1, value); }
//        }
//
//        [Description("High Volume")]
//        [GridCategory("Parameters")]
//        public int HighVolume
//        {
//            get { return highVolume; }
//            set { highVolume = Math.Max(1, value); }
//        }
        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private AAVSA[] cacheAAVSA = null;

        private static AAVSA checkAAVSA = new AAVSA();

        /// <summary>
        /// Rick's Volatility Breakout Indicator
        /// </summary>
        /// <returns></returns>
        public AAVSA AAVSA(int period, double threshold)
        {
            return AAVSA(Input, period, threshold);
        }

        /// <summary>
        /// Rick's Volatility Breakout Indicator
        /// </summary>
        /// <returns></returns>
        public AAVSA AAVSA(Data.IDataSeries input, int period, double threshold)
        {
            if (cacheAAVSA != null)
                for (int idx = 0; idx < cacheAAVSA.Length; idx++)
                    if (cacheAAVSA[idx].Period == period && Math.Abs(cacheAAVSA[idx].Threshold - threshold) <= double.Epsilon && cacheAAVSA[idx].EqualsInput(input))
                        return cacheAAVSA[idx];

            lock (checkAAVSA)
            {
                checkAAVSA.Period = period;
                period = checkAAVSA.Period;
                checkAAVSA.Threshold = threshold;
                threshold = checkAAVSA.Threshold;

                if (cacheAAVSA != null)
                    for (int idx = 0; idx < cacheAAVSA.Length; idx++)
                        if (cacheAAVSA[idx].Period == period && Math.Abs(cacheAAVSA[idx].Threshold - threshold) <= double.Epsilon && cacheAAVSA[idx].EqualsInput(input))
                            return cacheAAVSA[idx];

                AAVSA indicator = new AAVSA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                indicator.Threshold = threshold;
                Indicators.Add(indicator);
                indicator.SetUp();

                AAVSA[] tmp = new AAVSA[cacheAAVSA == null ? 1 : cacheAAVSA.Length + 1];
                if (cacheAAVSA != null)
                    cacheAAVSA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheAAVSA = tmp;
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
        /// Rick's Volatility Breakout Indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AAVSA AAVSA(int period, double threshold)
        {
            return _indicator.AAVSA(Input, period, threshold);
        }

        /// <summary>
        /// Rick's Volatility Breakout Indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.AAVSA AAVSA(Data.IDataSeries input, int period, double threshold)
        {
            return _indicator.AAVSA(input, period, threshold);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Rick's Volatility Breakout Indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.AAVSA AAVSA(int period, double threshold)
        {
            return _indicator.AAVSA(Input, period, threshold);
        }

        /// <summary>
        /// Rick's Volatility Breakout Indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.AAVSA AAVSA(Data.IDataSeries input, int period, double threshold)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.AAVSA(input, period, threshold);
        }
    }
}
#endregion
