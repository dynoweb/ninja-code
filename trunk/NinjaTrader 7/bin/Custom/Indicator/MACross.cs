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

#region Global Enums

public enum TimeframeType {Short, Medium, Long}

#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// To be used in the market analyzer to indicate number of periods since the fast crossed the slow MA. Negative reflects periods since the fast crossed below the slow MA.
    /// </summary>
    [Description("To be used in the market analyzer to indicate number of periods since the fast crossed the slow MA. Negative reflects periods since the fast crossed below the slow MA.")]
    public class MACross : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int mASlowShort = 5; // Default setting for MASlow
            private int mAFastShort = 2; // Default setting for MAFast
            private int mASlowMedium = 10; // Default setting for MASlow
            private int mAFastMedium = 4; // Default setting for MAFast
            private int mASlowLong = 15; // Default setting for MASlow
            private int mAFastLong = 6; // Default setting for MAFast
        // User defined variables (add any user defined variables below)
			private TimeframeType timeframe	= TimeframeType.Medium;
		    private int slowPeriod;
			private int fastPeriod;
			private int direction = 1;  // 1=up, -1=down
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.FromKnownColor(KnownColor.Blue), PlotStyle.Line, "FastPlot"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Black), PlotStyle.Line, "SlowPlot"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Black), PlotStyle.Line, "CrossedAgo"));
            Add(new Plot(Color.FromKnownColor(KnownColor.Black), PlotStyle.Line, "CrossedDirection"));

			Plots[0].Pen.Width = 2;
			Plots[1].Pen.Width = 2;

			PaintPriceMarkers = false;
			
			if (timeframe == TimeframeType.Short)
			{
				slowPeriod = mASlowShort; 
				fastPeriod = mAFastShort;
			}
			if (timeframe == TimeframeType.Medium)
			{
				slowPeriod = mASlowMedium; 
				fastPeriod = mAFastMedium;
			}
			if (timeframe == TimeframeType.Long)
			{
				slowPeriod = mASlowLong; 
				fastPeriod = mAFastLong;
			}

			CalculateOnBarClose = false;
			AutoScale = false;
            Overlay	= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			double smaValue    = SMA(slowPeriod)[0];
			double emaValue    = EMA(fastPeriod)[0];
			
            // Use this method for calculating your indicator values. Assign a value to each
            // plot below by replacing 'Close[0]' with your own formula.
            FastPlot.Set(emaValue);
            SlowPlot.Set(smaValue);
			CrossedAgo.Set(calcLastCross(EMA(fastPeriod), SMA(slowPeriod)));
			CrossedDirection.Set(direction);
        }
		
		private int calcLastCross(IDataSeries series1, IDataSeries series2)
		{
			int crossedAbove = 0;
			for (crossedAbove = 0; crossedAbove < FastPlot.Count; crossedAbove++)
			{
				if (CrossAbove(series1, series2, crossedAbove))
				{
					break;
				}
			}
			
			int crossedBelow = 0;
			for (crossedBelow = 0; crossedBelow < FastPlot.Count; crossedBelow++)
			{
				if (CrossBelow(series1, series2, crossedBelow))
				{
					break;
				}
			}
			
			int lookBackPeriod = Math.Min(crossedAbove, crossedBelow);
			
			direction = -1;  // init direction, crossed below
			if (lookBackPeriod == crossedAbove)
				direction = 1;		// crossed above
			
			return lookBackPeriod;
		}

		#region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries FastPlot
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries SlowPlot
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries CrossedAgo
        {
            get { return Values[2]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries CrossedDirection
        {
            get { return Values[3]; }
        }
		
		/// <summary>
		/// </summary>
		[Description("Trend Timeframe")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Timeframe for trading")]
		public TimeframeType Timeframe
		{
			get { return timeframe; }
			set { timeframe = value; }
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
        private MACross[] cacheMACross = null;

        private static MACross checkMACross = new MACross();

        /// <summary>
        /// To be used in the market analyzer to indicate number of periods since the fast crossed the slow MA. Negative reflects periods since the fast crossed below the slow MA.
        /// </summary>
        /// <returns></returns>
        public MACross MACross(TimeframeType timeframe)
        {
            return MACross(Input, timeframe);
        }

        /// <summary>
        /// To be used in the market analyzer to indicate number of periods since the fast crossed the slow MA. Negative reflects periods since the fast crossed below the slow MA.
        /// </summary>
        /// <returns></returns>
        public MACross MACross(Data.IDataSeries input, TimeframeType timeframe)
        {
            if (cacheMACross != null)
                for (int idx = 0; idx < cacheMACross.Length; idx++)
                    if (cacheMACross[idx].Timeframe == timeframe && cacheMACross[idx].EqualsInput(input))
                        return cacheMACross[idx];

            lock (checkMACross)
            {
                checkMACross.Timeframe = timeframe;
                timeframe = checkMACross.Timeframe;

                if (cacheMACross != null)
                    for (int idx = 0; idx < cacheMACross.Length; idx++)
                        if (cacheMACross[idx].Timeframe == timeframe && cacheMACross[idx].EqualsInput(input))
                            return cacheMACross[idx];

                MACross indicator = new MACross();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Timeframe = timeframe;
                Indicators.Add(indicator);
                indicator.SetUp();

                MACross[] tmp = new MACross[cacheMACross == null ? 1 : cacheMACross.Length + 1];
                if (cacheMACross != null)
                    cacheMACross.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheMACross = tmp;
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
        /// To be used in the market analyzer to indicate number of periods since the fast crossed the slow MA. Negative reflects periods since the fast crossed below the slow MA.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MACross MACross(TimeframeType timeframe)
        {
            return _indicator.MACross(Input, timeframe);
        }

        /// <summary>
        /// To be used in the market analyzer to indicate number of periods since the fast crossed the slow MA. Negative reflects periods since the fast crossed below the slow MA.
        /// </summary>
        /// <returns></returns>
        public Indicator.MACross MACross(Data.IDataSeries input, TimeframeType timeframe)
        {
            return _indicator.MACross(input, timeframe);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// To be used in the market analyzer to indicate number of periods since the fast crossed the slow MA. Negative reflects periods since the fast crossed below the slow MA.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.MACross MACross(TimeframeType timeframe)
        {
            return _indicator.MACross(Input, timeframe);
        }

        /// <summary>
        /// To be used in the market analyzer to indicate number of periods since the fast crossed the slow MA. Negative reflects periods since the fast crossed below the slow MA.
        /// </summary>
        /// <returns></returns>
        public Indicator.MACross MACross(Data.IDataSeries input, TimeframeType timeframe)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.MACross(input, timeframe);
        }
    }
}
#endregion
