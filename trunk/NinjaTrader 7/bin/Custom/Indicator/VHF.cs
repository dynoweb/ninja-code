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
    /// The VHF indicator can be a useful tool as it attempts to measure the “trendiness” of a market. It
	/// reveals the strength of a trend, not the trend direction. This filter compares the sum of one-day
	/// price changes within a specific time period to the range between the high and low prices over the
	/// specified period. The VHF values help identify whether prices are in a trending phase as well as
	/// the degree or strength of a trend, where the investor should use trend-following indicators, or if
	/// the prices are in a trading range market, a case where congestion-phase indicators (such as
	/// oscillators which work well in trading range markets) should be used.
	///
	/// In theory, the VHF indicator can be used in the following three ways:
	/// 
	/// 1. In determining whether a particular market condition is trending or is in a congestion/non-
	/// trending phase: A rising VHF line indicates a developing trend; a falling VHF line indicates
	/// prices may be entering a trading range market.
	/// 
	/// 2. To determine the degree or strength of a trend: The higher the VHF line, the higher the
	/// degree of trending. A falling line indicates a trend is weak.
	/// 
	/// 3. The use of the VHF as a contrarian indicator: Relatively low VHF values can indicate that prices
	/// can be expected to break out of the congestion phase and start trending. On the other hand,
	/// relatively high VHF values indicate that a trending phase may be coming to a close and that the
	/// market is getting ready to enter a congestion phase.
	/// 
	/// Version 1.0: Author: gregid; Date: 06/12/2009; Compatibility: NT 6.5 & NT 7.0
	/// 
    /// </summary>

    [Description("Vertical Horizontal Filter - trendiness indicator")]
    public class VHF : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 14; // Default setting for Period
            private double treshold = 0.35; // Default setting for Period
	        private Color _backColorTrend = Color.Transparent;
	        private Color _backColorChop = Color.Silver;

        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Indigo), PlotStyle.Line, "VHFLine"));
			Add(new Line(System.Drawing.Color.Red,Treshold,"Treshold"));
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

			VHFLine.Set(Numerator/Denominator);

			if (VHFLine[0] < Treshold)
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

        [Description("Period over which VHF is calculated")]
        [Category("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }
		
        [Description("Period over which VHF is calculated")]
        [Category("Parameters")]
        public double Treshold
        {
            get { return treshold; }
            set { treshold = Math.Max(0, value); }
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
        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private VHF[] cacheVHF = null;

        private static VHF checkVHF = new VHF();

        /// <summary>
        /// Vertical Horizontal Filter - trendiness indicator
        /// </summary>
        /// <returns></returns>
        public VHF VHF(int period, double treshold)
        {
            return VHF(Input, period, treshold);
        }

        /// <summary>
        /// Vertical Horizontal Filter - trendiness indicator
        /// </summary>
        /// <returns></returns>
        public VHF VHF(Data.IDataSeries input, int period, double treshold)
        {
            if (cacheVHF != null)
                for (int idx = 0; idx < cacheVHF.Length; idx++)
                    if (cacheVHF[idx].Period == period && Math.Abs(cacheVHF[idx].Treshold - treshold) <= double.Epsilon && cacheVHF[idx].EqualsInput(input))
                        return cacheVHF[idx];

            lock (checkVHF)
            {
                checkVHF.Period = period;
                period = checkVHF.Period;
                checkVHF.Treshold = treshold;
                treshold = checkVHF.Treshold;

                if (cacheVHF != null)
                    for (int idx = 0; idx < cacheVHF.Length; idx++)
                        if (cacheVHF[idx].Period == period && Math.Abs(cacheVHF[idx].Treshold - treshold) <= double.Epsilon && cacheVHF[idx].EqualsInput(input))
                            return cacheVHF[idx];

                VHF indicator = new VHF();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                indicator.Treshold = treshold;
                Indicators.Add(indicator);
                indicator.SetUp();

                VHF[] tmp = new VHF[cacheVHF == null ? 1 : cacheVHF.Length + 1];
                if (cacheVHF != null)
                    cacheVHF.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVHF = tmp;
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
        /// Vertical Horizontal Filter - trendiness indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VHF VHF(int period, double treshold)
        {
            return _indicator.VHF(Input, period, treshold);
        }

        /// <summary>
        /// Vertical Horizontal Filter - trendiness indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.VHF VHF(Data.IDataSeries input, int period, double treshold)
        {
            return _indicator.VHF(input, period, treshold);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Vertical Horizontal Filter - trendiness indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VHF VHF(int period, double treshold)
        {
            return _indicator.VHF(Input, period, treshold);
        }

        /// <summary>
        /// Vertical Horizontal Filter - trendiness indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.VHF VHF(Data.IDataSeries input, int period, double treshold)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VHF(input, period, treshold);
        }
    }
}
#endregion
