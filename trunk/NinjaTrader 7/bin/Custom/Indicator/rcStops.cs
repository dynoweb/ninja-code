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
    public class rcStops : Indicator
    {
        #region Variables
        // Wizard generated variables
        // User defined variables (add any user defined variables below)
			private int lookbackPeriod = 5;
			private DataSeries longCrossedAgo;
			private DataSeries shortCrossedAgo;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Firebrick), PlotStyle.Line, "LongStopLoss"));
            Add(new Plot(Color.FromKnownColor(KnownColor.DarkViolet), PlotStyle.Line, "ShortStopLoss"));
			//BarsRequired = 2;
			
			shortCrossedAgo = new DataSeries(this);
			longCrossedAgo = new DataSeries(this);
			
            Overlay				= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			if (CurrentBar < 3)
				return;
			
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
			double stop = (MIN(Low, lookbackPeriod)[0]) - ATR(Low, lookbackPeriod)[0];
			if (stop < LongStopLoss[1] && Low[0] > LongStopLoss[1])
			{
				stop = LongStopLoss[1];
			}
			LongStopLoss.Set(stop);
			
			// Process stops for short trades
			stop = (MAX(High, lookbackPeriod)[0]) + ATR(High, lookbackPeriod)[0];
			if (stop > ShortStopLoss[1] && High[0] < ShortStopLoss[1])
			{
				stop = ShortStopLoss[1];
			}
            ShortStopLoss.Set(stop);

			// Set number of bars ago the long stop (low) was broken
			LongCrossedAgo.Set(int.MaxValue);	// initialize in case we don't intentionally set it
			if (Low[0] < LongStopLoss[1])
			{
				LongCrossedAgo.Set(0);
			} 
			else
			{
				LongCrossedAgo.Set(LongCrossedAgo[1] + 1);
			}
			//Print(Time + " long crossed ago: " + LongCrossedAgo[0]);
						
			// Set number of bars ago the short stop (high) was broken
			ShortCrossedAgo.Set(int.MaxValue);	// initialize in case we don't intentionally set it
			if (High[0] > ShortStopLoss[1])
			{
				ShortCrossedAgo.Set(0);
			} 
			else
			{
				ShortCrossedAgo.Set(ShortCrossedAgo[1] + 1);
			}
			//Print(Time + " short crossed ago: " + ShortCrossedAgo[0]);
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LongStopLoss
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortStopLoss
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LongCrossedAgo
        {
            get { return longCrossedAgo; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortCrossedAgo
        {
            get { return shortCrossedAgo; }
        }

        [Description("Lookback Period")]
        [GridCategory("Parameters")]
        public int LookbackPeriod
        {
            get { return lookbackPeriod; }
            set { lookbackPeriod = Math.Max(1, value); }
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
        private rcStops[] cachercStops = null;

        private static rcStops checkrcStops = new rcStops();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public rcStops rcStops(int lookbackPeriod)
        {
            return rcStops(Input, lookbackPeriod);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public rcStops rcStops(Data.IDataSeries input, int lookbackPeriod)
        {
            if (cachercStops != null)
                for (int idx = 0; idx < cachercStops.Length; idx++)
                    if (cachercStops[idx].LookbackPeriod == lookbackPeriod && cachercStops[idx].EqualsInput(input))
                        return cachercStops[idx];

            lock (checkrcStops)
            {
                checkrcStops.LookbackPeriod = lookbackPeriod;
                lookbackPeriod = checkrcStops.LookbackPeriod;

                if (cachercStops != null)
                    for (int idx = 0; idx < cachercStops.Length; idx++)
                        if (cachercStops[idx].LookbackPeriod == lookbackPeriod && cachercStops[idx].EqualsInput(input))
                            return cachercStops[idx];

                rcStops indicator = new rcStops();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.LookbackPeriod = lookbackPeriod;
                Indicators.Add(indicator);
                indicator.SetUp();

                rcStops[] tmp = new rcStops[cachercStops == null ? 1 : cachercStops.Length + 1];
                if (cachercStops != null)
                    cachercStops.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachercStops = tmp;
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
        public Indicator.rcStops rcStops(int lookbackPeriod)
        {
            return _indicator.rcStops(Input, lookbackPeriod);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.rcStops rcStops(Data.IDataSeries input, int lookbackPeriod)
        {
            return _indicator.rcStops(input, lookbackPeriod);
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
        public Indicator.rcStops rcStops(int lookbackPeriod)
        {
            return _indicator.rcStops(Input, lookbackPeriod);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.rcStops rcStops(Data.IDataSeries input, int lookbackPeriod)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.rcStops(input, lookbackPeriod);
        }
    }
}
#endregion
