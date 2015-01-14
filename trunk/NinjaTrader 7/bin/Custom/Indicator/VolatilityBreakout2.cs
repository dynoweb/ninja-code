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
    /// Determine the periods of extremes of low volatility which usually followed by big moves. Indicator does also shows  direction of the trade based on Momentum indicator
    /// </summary>
    [Description("Determine the periods of extremes of low volatility which usually followed by big moves. Indicator does also shows  direction of the trade based on Momentum indicator")]
    public class VolatilityBreakout2 : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int period = 20; // Default setting for Period
            private double kCDeviation = 1.500; // Default setting for KCDeviation
            private double bBDeviation = 2.000; // Default setting for BBDeviation
            private int momentumPeriod = 12; // Default setting for MomentumPeriod
			private int barSensitivity=3;
        // User defined variables (add any user defined variables below)
			public bool blnFlag = false;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.OrangeRed), PlotStyle.Bar, "Plot0"));
            CalculateOnBarClose	= true;
            Overlay				= false;
            PriceTypeSupported	= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            //Plot0.Set(Close[0]);
			
			/* The squeeze takes advantage of quiet periods in the market when the volatility 
			has decreased significantly and the market is building up energy for its next major 
			move higher or lower. Period of low volatility are identified as the times when the bands 
			"move closer together". How do we know that the current narrowness is really narrow enough 
			to qualify as low volatility? By adding Keltner Channels and momentum index oscillator 
			as per mentioned in John Carter's book Mastering the Trade.
			
			While Bolling Bands expand and contract as the markets alter between periods of high and 
			low volatility, the Keltner Channels stay in more of a steady range. The momentum index 
			oscillator is used to estimate the direction.
			
			How does this Setup work?
			The quite period is identified whent he Bollinger Bands narrow in width to the point that 
			they are actually trading inside of the Keltner Channels. This marks a period of reduced 
			volatility and signals that the market is taking a significant breather, building up steam
			for its next move. The trade signal occurs when the Bollinger Bands then move back outside 
			the Keltner Channels. Use 12 period momentum index oscillator to determine whether to go 
			long or short. If the oscillator is above 0 when this happens, GO LONG; if it id below 0 then
			GO SHORT.
			
			Usually the moves are explosive when the BB Width is lowest over past 6 months which comes to 
			across 126 days and hence we need more than 126 price data bars.
			
			*/
			
			if (CurrentBar > 127) 
			{
				
				/* if (bBandWidth < 0)
				{
					bBandWidth=0;
				}*/
				
				if (KeltnerChannel(kCDeviation, Period).Upper[0] >  Bollinger(bBDeviation, Period).Upper[0] &&
					KeltnerChannel(kCDeviation, Period).Lower[0] <  Bollinger(bBDeviation, Period).Lower[0] &&
					(BollingerBandWidth2(bBDeviation,Period)[0] == MIN(BollingerBandWidth2(bBDeviation,Period), 126)[0]) ) //BollingerBandWidth(bBdeviation,Period)[0] > bollingerWidth)
				{
					Plot0.Set(1); //This is just a warning signal to be ready
					blnFlag = true;
				}
				else if (blnFlag && 
					KeltnerChannel(kCDeviation, Period).Upper[0] <=  Bollinger(bBDeviation, Period).Upper[0] &&
					KeltnerChannel(kCDeviation, Period).Lower[0] >= Bollinger(bBDeviation, Period).Lower[0])
				{
					Plot0.Set(Momentum(momentumPeriod)[0] >0? 2:-2); // +ve buy & -ve short
					blnFlag = false;
						
				}
				else
				{
					Plot0.Set(0);
				}
			}
			else
			{		
				Plot0.Set(0);
			}

			
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
        {
            get { return Values[0]; }
        }

        [Description("Used to calculate band/channel")]
        [Category("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }

        [Description("Used to calculate Keltner Channel")]
        [Category("Parameters")]
        public double KCDeviation
        {
            get { return kCDeviation; }
            set { kCDeviation = Math.Max(1, value); }
        }		

        [Description("Used to calculate Bollinger Bands")]
        [Category("Parameters")]
        public double BBDeviation
        {
            get { return bBDeviation; }
            set { bBDeviation = Math.Max(1, value); }
        }
		
        [Description("Used to calculate Momentum")]
        [Category("Parameters")]
        public int MomentumPeriod
        {
            get { return momentumPeriod; }
            set { momentumPeriod = Math.Max(1, value); }
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
        private VolatilityBreakout2[] cacheVolatilityBreakout2 = null;

        private static VolatilityBreakout2 checkVolatilityBreakout2 = new VolatilityBreakout2();

        /// <summary>
        /// Determine the periods of extremes of low volatility which usually followed by big moves. Indicator does also shows  direction of the trade based on Momentum indicator
        /// </summary>
        /// <returns></returns>
        public VolatilityBreakout2 VolatilityBreakout2(double bBDeviation, double kCDeviation, int momentumPeriod, int period)
        {
            return VolatilityBreakout2(Input, bBDeviation, kCDeviation, momentumPeriod, period);
        }

        /// <summary>
        /// Determine the periods of extremes of low volatility which usually followed by big moves. Indicator does also shows  direction of the trade based on Momentum indicator
        /// </summary>
        /// <returns></returns>
        public VolatilityBreakout2 VolatilityBreakout2(Data.IDataSeries input, double bBDeviation, double kCDeviation, int momentumPeriod, int period)
        {
            if (cacheVolatilityBreakout2 != null)
                for (int idx = 0; idx < cacheVolatilityBreakout2.Length; idx++)
                    if (Math.Abs(cacheVolatilityBreakout2[idx].BBDeviation - bBDeviation) <= double.Epsilon && Math.Abs(cacheVolatilityBreakout2[idx].KCDeviation - kCDeviation) <= double.Epsilon && cacheVolatilityBreakout2[idx].MomentumPeriod == momentumPeriod && cacheVolatilityBreakout2[idx].Period == period && cacheVolatilityBreakout2[idx].EqualsInput(input))
                        return cacheVolatilityBreakout2[idx];

            lock (checkVolatilityBreakout2)
            {
                checkVolatilityBreakout2.BBDeviation = bBDeviation;
                bBDeviation = checkVolatilityBreakout2.BBDeviation;
                checkVolatilityBreakout2.KCDeviation = kCDeviation;
                kCDeviation = checkVolatilityBreakout2.KCDeviation;
                checkVolatilityBreakout2.MomentumPeriod = momentumPeriod;
                momentumPeriod = checkVolatilityBreakout2.MomentumPeriod;
                checkVolatilityBreakout2.Period = period;
                period = checkVolatilityBreakout2.Period;

                if (cacheVolatilityBreakout2 != null)
                    for (int idx = 0; idx < cacheVolatilityBreakout2.Length; idx++)
                        if (Math.Abs(cacheVolatilityBreakout2[idx].BBDeviation - bBDeviation) <= double.Epsilon && Math.Abs(cacheVolatilityBreakout2[idx].KCDeviation - kCDeviation) <= double.Epsilon && cacheVolatilityBreakout2[idx].MomentumPeriod == momentumPeriod && cacheVolatilityBreakout2[idx].Period == period && cacheVolatilityBreakout2[idx].EqualsInput(input))
                            return cacheVolatilityBreakout2[idx];

                VolatilityBreakout2 indicator = new VolatilityBreakout2();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BBDeviation = bBDeviation;
                indicator.KCDeviation = kCDeviation;
                indicator.MomentumPeriod = momentumPeriod;
                indicator.Period = period;
                Indicators.Add(indicator);
                indicator.SetUp();

                VolatilityBreakout2[] tmp = new VolatilityBreakout2[cacheVolatilityBreakout2 == null ? 1 : cacheVolatilityBreakout2.Length + 1];
                if (cacheVolatilityBreakout2 != null)
                    cacheVolatilityBreakout2.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheVolatilityBreakout2 = tmp;
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
        /// Determine the periods of extremes of low volatility which usually followed by big moves. Indicator does also shows  direction of the trade based on Momentum indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolatilityBreakout2 VolatilityBreakout2(double bBDeviation, double kCDeviation, int momentumPeriod, int period)
        {
            return _indicator.VolatilityBreakout2(Input, bBDeviation, kCDeviation, momentumPeriod, period);
        }

        /// <summary>
        /// Determine the periods of extremes of low volatility which usually followed by big moves. Indicator does also shows  direction of the trade based on Momentum indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.VolatilityBreakout2 VolatilityBreakout2(Data.IDataSeries input, double bBDeviation, double kCDeviation, int momentumPeriod, int period)
        {
            return _indicator.VolatilityBreakout2(input, bBDeviation, kCDeviation, momentumPeriod, period);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Determine the periods of extremes of low volatility which usually followed by big moves. Indicator does also shows  direction of the trade based on Momentum indicator
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.VolatilityBreakout2 VolatilityBreakout2(double bBDeviation, double kCDeviation, int momentumPeriod, int period)
        {
            return _indicator.VolatilityBreakout2(Input, bBDeviation, kCDeviation, momentumPeriod, period);
        }

        /// <summary>
        /// Determine the periods of extremes of low volatility which usually followed by big moves. Indicator does also shows  direction of the trade based on Momentum indicator
        /// </summary>
        /// <returns></returns>
        public Indicator.VolatilityBreakout2 VolatilityBreakout2(Data.IDataSeries input, double bBDeviation, double kCDeviation, int momentumPeriod, int period)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.VolatilityBreakout2(input, bBDeviation, kCDeviation, momentumPeriod, period);
        }
    }
}
#endregion
