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
    /// This indicator will use pivot point to draw a upper and lower trend
	/// to use to detemine when the trend changes.
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class TrendBreaks : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int maxLookBack = 50; // Default setting for MaxLookBack
            private int minLookBack = 10; // Default setting for MinLookBack
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
			if (CurrentBar < minLookBack)
				return;
			
			// look for upper trend line
			
        }

        #region Properties

        [Description("Maximum number of bars to look for second pivot")]
        [GridCategory("Parameters")]
        public int MaxLookBack
        {
            get { return maxLookBack; }
            set { maxLookBack = Math.Max(5, value); }
        }
		
        [Description("Minimum number of bars to look for second pivot")]
        [GridCategory("Parameters")]
        public int MinLookBack
        {
            get { return minLookBack; }
            set { minLookBack = Math.Max(5, value); }
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
        private TrendBreaks[] cacheTrendBreaks = null;

        private static TrendBreaks checkTrendBreaks = new TrendBreaks();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public TrendBreaks TrendBreaks(int maxLookBack, int minLookBack)
        {
            return TrendBreaks(Input, maxLookBack, minLookBack);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public TrendBreaks TrendBreaks(Data.IDataSeries input, int maxLookBack, int minLookBack)
        {
            if (cacheTrendBreaks != null)
                for (int idx = 0; idx < cacheTrendBreaks.Length; idx++)
                    if (cacheTrendBreaks[idx].MaxLookBack == maxLookBack && cacheTrendBreaks[idx].MinLookBack == minLookBack && cacheTrendBreaks[idx].EqualsInput(input))
                        return cacheTrendBreaks[idx];

            lock (checkTrendBreaks)
            {
                checkTrendBreaks.MaxLookBack = maxLookBack;
                maxLookBack = checkTrendBreaks.MaxLookBack;
                checkTrendBreaks.MinLookBack = minLookBack;
                minLookBack = checkTrendBreaks.MinLookBack;

                if (cacheTrendBreaks != null)
                    for (int idx = 0; idx < cacheTrendBreaks.Length; idx++)
                        if (cacheTrendBreaks[idx].MaxLookBack == maxLookBack && cacheTrendBreaks[idx].MinLookBack == minLookBack && cacheTrendBreaks[idx].EqualsInput(input))
                            return cacheTrendBreaks[idx];

                TrendBreaks indicator = new TrendBreaks();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.MaxLookBack = maxLookBack;
                indicator.MinLookBack = minLookBack;
                Indicators.Add(indicator);
                indicator.SetUp();

                TrendBreaks[] tmp = new TrendBreaks[cacheTrendBreaks == null ? 1 : cacheTrendBreaks.Length + 1];
                if (cacheTrendBreaks != null)
                    cacheTrendBreaks.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTrendBreaks = tmp;
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
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TrendBreaks TrendBreaks(int maxLookBack, int minLookBack)
        {
            return _indicator.TrendBreaks(Input, maxLookBack, minLookBack);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.TrendBreaks TrendBreaks(Data.IDataSeries input, int maxLookBack, int minLookBack)
        {
            return _indicator.TrendBreaks(input, maxLookBack, minLookBack);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TrendBreaks TrendBreaks(int maxLookBack, int minLookBack)
        {
            return _indicator.TrendBreaks(Input, maxLookBack, minLookBack);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.TrendBreaks TrendBreaks(Data.IDataSeries input, int maxLookBack, int minLookBack)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TrendBreaks(input, maxLookBack, minLookBack);
        }
    }
}
#endregion
