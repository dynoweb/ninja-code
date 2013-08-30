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
    /// Holt EMA
    [Description("HoltEMA, a trend corrected EMA.")]
    public class anaHoltEMA : Indicator
    {
        #region Variables
            private int period = 89;
			private int trendPeriod = 144;
			private double alpha = 0.0; // Default setting for Alpha
            private double gamma = 0.0; // Default setting for Gamma
		  	private DataSeries trend;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.DodgerBlue, "HoltEMA"));
            Overlay				= true;
			PriceTypeSupported 	= true;
 			trend 				= new DataSeries(this);
       }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// http://www2.gsu.edu/~dscthw/8110/Chapter8.pdf
			double alpha = 2.0 /(1 + Period);
			double gamma = 2.0/(1 + TrendPeriod);
			if(CurrentBar == 0) 
			{
			  HoltEMA.Set(Input[0]);
			  trend.Set(0.0);
			  return;
			}
			double holt = alpha * Input[0] + (1-alpha)*(HoltEMA[1] + trend[1]);
			HoltEMA.Set(holt);  
			trend.Set (gamma*(holt - HoltEMA[1]) + (1-gamma)*trend[1]);
        }

        #region Properties
        [Browsable(false)]	
        [XmlIgnore()]		
        public DataSeries HoltEMA
        {
            get { return Values[0]; }
        }

        [Description("Primary Period")]
		[GridCategory("Parameters")]
        public int Period
        {
            get { return period; }
            set { period = Math.Max(1, value); }
        }

        [Description("Trend Period")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayName("Trend Period")]
        public int TrendPeriod
        {
            get { return trendPeriod; }
            set { trendPeriod = Math.Max(1, value); }
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
        private anaHoltEMA[] cacheanaHoltEMA = null;

        private static anaHoltEMA checkanaHoltEMA = new anaHoltEMA();

        /// <summary>
        /// HoltEMA, a trend corrected EMA.
        /// </summary>
        /// <returns></returns>
        public anaHoltEMA anaHoltEMA(int period, int trendPeriod)
        {
            return anaHoltEMA(Input, period, trendPeriod);
        }

        /// <summary>
        /// HoltEMA, a trend corrected EMA.
        /// </summary>
        /// <returns></returns>
        public anaHoltEMA anaHoltEMA(Data.IDataSeries input, int period, int trendPeriod)
        {
            if (cacheanaHoltEMA != null)
                for (int idx = 0; idx < cacheanaHoltEMA.Length; idx++)
                    if (cacheanaHoltEMA[idx].Period == period && cacheanaHoltEMA[idx].TrendPeriod == trendPeriod && cacheanaHoltEMA[idx].EqualsInput(input))
                        return cacheanaHoltEMA[idx];

            lock (checkanaHoltEMA)
            {
                checkanaHoltEMA.Period = period;
                period = checkanaHoltEMA.Period;
                checkanaHoltEMA.TrendPeriod = trendPeriod;
                trendPeriod = checkanaHoltEMA.TrendPeriod;

                if (cacheanaHoltEMA != null)
                    for (int idx = 0; idx < cacheanaHoltEMA.Length; idx++)
                        if (cacheanaHoltEMA[idx].Period == period && cacheanaHoltEMA[idx].TrendPeriod == trendPeriod && cacheanaHoltEMA[idx].EqualsInput(input))
                            return cacheanaHoltEMA[idx];

                anaHoltEMA indicator = new anaHoltEMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Period = period;
                indicator.TrendPeriod = trendPeriod;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaHoltEMA[] tmp = new anaHoltEMA[cacheanaHoltEMA == null ? 1 : cacheanaHoltEMA.Length + 1];
                if (cacheanaHoltEMA != null)
                    cacheanaHoltEMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaHoltEMA = tmp;
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
        /// HoltEMA, a trend corrected EMA.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaHoltEMA anaHoltEMA(int period, int trendPeriod)
        {
            return _indicator.anaHoltEMA(Input, period, trendPeriod);
        }

        /// <summary>
        /// HoltEMA, a trend corrected EMA.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaHoltEMA anaHoltEMA(Data.IDataSeries input, int period, int trendPeriod)
        {
            return _indicator.anaHoltEMA(input, period, trendPeriod);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// HoltEMA, a trend corrected EMA.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaHoltEMA anaHoltEMA(int period, int trendPeriod)
        {
            return _indicator.anaHoltEMA(Input, period, trendPeriod);
        }

        /// <summary>
        /// HoltEMA, a trend corrected EMA.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaHoltEMA anaHoltEMA(Data.IDataSeries input, int period, int trendPeriod)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaHoltEMA(input, period, trendPeriod);
        }
    }
}
#endregion
