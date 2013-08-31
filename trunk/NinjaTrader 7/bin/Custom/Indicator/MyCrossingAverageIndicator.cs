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
    /// </summary>
    [Description("Enter the description of your new custom indicator here")]
    public class MyCrossingAverageIndicator : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int fastPeriod = 50; // Default setting for FastPeriod
            private int slowPeriod = 200; // Default setting for SlowPeriod
        // User defined variables (add any user defined variables below)
			private DataSeries myDataSeries;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Line, "Uptrend"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Red), PlotStyle.Line, "Downtrend"));
            Add(new Line(Color.FromKnownColor(KnownColor.DarkOliveGreen), 0, "Oscillator1"));
            Add(new Line(Color.FromKnownColor(KnownColor.Khaki), 1, "Oscillator2"));
			CalculateOnBarClose = true;
            Overlay				= false;
			
			Plots[0].Min = 0;
			Plots[1].Max = 0;
			
			//myDataSeries = new DataSeries(this);
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			double y = SMA(fastPeriod)[0] - SMA(slowPeriod)[0];
			
			if (CrossAbove(SMA(fastPeriod), SMA(slowPeriod), 10))
			{
				DrawArrowUp(CurrentBar + "UP", true, 0, Low[0], Color.Blue);
				Uptrend.Set(1);
				//myDataSeries.Set(1);
			} 
				else if (CrossBelow(SMA(fastPeriod), SMA(slowPeriod), 10))
			{
				DrawArrowUp(CurrentBar + "DOWN", true, 0, High[0], Color.Red);
				//myDataSeries.Set(-1);
				Uptrend.Set(-1);
			}
				else
			{
				Uptrend.Set(0);
				//myDataSeries.Set(0);
			}
			
			
			//Uptrend.Set(y);
			
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            //Uptrend.Set(High[0]);
            //Downtrend.Set(Low[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Uptrend
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Downtrend
        {
            get { return Values[1]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int FastPeriod
        {
            get { return fastPeriod; }
            set { fastPeriod = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int SlowPeriod
        {
            get { return slowPeriod; }
            set { slowPeriod = Math.Max(6, value); }
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
        private MyCrossingAverageIndicator[] cacheMyCrossingAverageIndicator = null;

        private static MyCrossingAverageIndicator checkMyCrossingAverageIndicator = new MyCrossingAverageIndicator();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public MyCrossingAverageIndicator MyCrossingAverageIndicator(int fastPeriod, int slowPeriod)
        {
            return MyCrossingAverageIndicator(Input, fastPeriod, slowPeriod);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public MyCrossingAverageIndicator MyCrossingAverageIndicator(Data.IDataSeries input, int fastPeriod, int slowPeriod)
        {
            if (cacheMyCrossingAverageIndicator != null)
                for (int idx = 0; idx < cacheMyCrossingAverageIndicator.Length; idx++)
                    if (cacheMyCrossingAverageIndicator[idx].FastPeriod == fastPeriod && cacheMyCrossingAverageIndicator[idx].SlowPeriod == slowPeriod && cacheMyCrossingAverageIndicator[idx].EqualsInput(input))
                        return cacheMyCrossingAverageIndicator[idx];

            lock (checkMyCrossingAverageIndicator)
            {
                checkMyCrossingAverageIndicator.FastPeriod = fastPeriod;
                fastPeriod = checkMyCrossingAverageIndicator.FastPeriod;
                checkMyCrossingAverageIndicator.SlowPeriod = slowPeriod;
                slowPeriod = checkMyCrossingAverageIndicator.SlowPeriod;

                if (cacheMyCrossingAverageIndicator != null)
                    for (int idx = 0; idx < cacheMyCrossingAverageIndicator.Length; idx++)
                        if (cacheMyCrossingAverageIndicator[idx].FastPeriod == fastPeriod && cacheMyCrossingAverageIndicator[idx].SlowPeriod == slowPeriod && cacheMyCrossingAverageIndicator[idx].EqualsInput(input))
                            return cacheMyCrossingAverageIndicator[idx];

                MyCrossingAverageIndicator indicator = new MyCrossingAverageIndicator();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.FastPeriod = fastPeriod;
                indicator.SlowPeriod = slowPeriod;
                Indicators.Add(indicator);
                indicator.SetUp();

                MyCrossingAverageIndicator[] tmp = new MyCrossingAverageIndicator[cacheMyCrossingAverageIndicator == null ? 1 : cacheMyCrossingAverageIndicator.Length + 1];
                if (cacheMyCrossingAverageIndicator != null)
                    cacheMyCrossingAverageIndicator.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheMyCrossingAverageIndicator = tmp;
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
        public Indicator.MyCrossingAverageIndicator MyCrossingAverageIndicator(int fastPeriod, int slowPeriod)
        {
            return _indicator.MyCrossingAverageIndicator(Input, fastPeriod, slowPeriod);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.MyCrossingAverageIndicator MyCrossingAverageIndicator(Data.IDataSeries input, int fastPeriod, int slowPeriod)
        {
            return _indicator.MyCrossingAverageIndicator(input, fastPeriod, slowPeriod);
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
        public Indicator.MyCrossingAverageIndicator MyCrossingAverageIndicator(int fastPeriod, int slowPeriod)
        {
            return _indicator.MyCrossingAverageIndicator(Input, fastPeriod, slowPeriod);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.MyCrossingAverageIndicator MyCrossingAverageIndicator(Data.IDataSeries input, int fastPeriod, int slowPeriod)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.MyCrossingAverageIndicator(input, fastPeriod, slowPeriod);
        }
    }
}
#endregion
