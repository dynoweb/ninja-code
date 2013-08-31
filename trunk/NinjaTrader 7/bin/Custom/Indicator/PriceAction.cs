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
    /// Enter the description of your new custom indicator here
	/// 
	/// Started on 2/5/13 - not useful yet.
	/// 
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class PriceAction : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int bars = 4; // Default setting for Bars
            private int minPullback = 6; // Default setting for MinPullback
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Block, "ShortColor"));
            Add(new Plot(Color.FromKnownColor(KnownColor.HotPink), PlotStyle.Bar, "ShortColorBar"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkViolet), PlotStyle.Block, "LongColor"));
            Add(new Plot(Color.FromKnownColor(KnownColor.LightSeaGreen), PlotStyle.Bar, "LongColorBar"));
			Add(new Plot(new Pen(Color.Red, 5), PlotStyle.Bar, "Diff"));
			Add(new Plot(new Pen(Color.Navy, 5), PlotStyle.Bar, "DiffPos"));
            //Add(new Line(Color.FromKnownColor(KnownColor.DarkOliveGreen), 0, "TrendStrengthOscillator"));
			Add(new Line(Color.DarkGray, 0, "Zero line"));
            Overlay				= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
			int low = LowestBar(Low, Bars);
			int high = HighestBar(High, Bars);
			
            //ShortColor.Set(-1);
            //ShortColorBar.Set(-1);
            //LongColor.Set(1);
            //LongColorBar.Set(1);
			Diff.Set(-low);
			DiffPos.Set(high);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortColor
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortColorBar
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LongColor
        {
            get { return Values[2]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LongColorBar
        {
            get { return Values[3]; }
        }

        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Diff
		{
			get { return Values[4]; }
		}
		
        /// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries DiffPos
		{
			get { return Values[5]; }
		}
		
        [Description("Number of bars to be considered a new zero")]
        [GridCategory("Parameters")]
        public int Bars
        {
            get { return bars; }
            set { bars = Math.Max(1, value); }
        }

        [Description("Number of ticks back to count as a pullback")]
        [GridCategory("Parameters")]
        public int MinPullback
        {
            get { return minPullback; }
            set { minPullback = Math.Max(1, value); }
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
        private PriceAction[] cachePriceAction = null;

        private static PriceAction checkPriceAction = new PriceAction();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public PriceAction PriceAction(int bars, int minPullback)
        {
            return PriceAction(Input, bars, minPullback);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public PriceAction PriceAction(Data.IDataSeries input, int bars, int minPullback)
        {
            if (cachePriceAction != null)
                for (int idx = 0; idx < cachePriceAction.Length; idx++)
                    if (cachePriceAction[idx].Bars == bars && cachePriceAction[idx].MinPullback == minPullback && cachePriceAction[idx].EqualsInput(input))
                        return cachePriceAction[idx];

            lock (checkPriceAction)
            {
                checkPriceAction.Bars = bars;
                bars = checkPriceAction.Bars;
                checkPriceAction.MinPullback = minPullback;
                minPullback = checkPriceAction.MinPullback;

                if (cachePriceAction != null)
                    for (int idx = 0; idx < cachePriceAction.Length; idx++)
                        if (cachePriceAction[idx].Bars == bars && cachePriceAction[idx].MinPullback == minPullback && cachePriceAction[idx].EqualsInput(input))
                            return cachePriceAction[idx];

                PriceAction indicator = new PriceAction();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Bars = bars;
                indicator.MinPullback = minPullback;
                Indicators.Add(indicator);
                indicator.SetUp();

                PriceAction[] tmp = new PriceAction[cachePriceAction == null ? 1 : cachePriceAction.Length + 1];
                if (cachePriceAction != null)
                    cachePriceAction.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachePriceAction = tmp;
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
        public Indicator.PriceAction PriceAction(int bars, int minPullback)
        {
            return _indicator.PriceAction(Input, bars, minPullback);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.PriceAction PriceAction(Data.IDataSeries input, int bars, int minPullback)
        {
            return _indicator.PriceAction(input, bars, minPullback);
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
        public Indicator.PriceAction PriceAction(int bars, int minPullback)
        {
            return _indicator.PriceAction(Input, bars, minPullback);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.PriceAction PriceAction(Data.IDataSeries input, int bars, int minPullback)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.PriceAction(input, bars, minPullback);
        }
    }
}
#endregion
