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
    /// Added markers to indicate 5 bar pattern
    /// </summary>
    [Description("Added markers to indicate 5 bar pattern")]
    public class FiveBarPattern : Indicator
    {
        #region Variables
			private DataSeries fiveBarLower;
			private DataSeries fiveBarUpper;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// Written by Rick Cromer
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Blue, PlotStyle.Dot, "Upper Five Bar"));
            Add(new Plot(Color.Red, PlotStyle.Dot, "Lower Five Bar"));
            
			// Will contain the bar numbers that had the low or high
			fiveBarLower = new DataSeries(this);
			fiveBarUpper = new DataSeries(this);
			
			this.AutoScale = false;
			this.PaintPriceMarkers = false;
			
			// Store all series values instead of only the last 256 values
			MaximumBarsLookBack = MaximumBarsLookBack.Infinite;
			
			Overlay				= true;			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
    		if (CurrentBar < 7)
        		return;

			// to scale Dot for Instrument
			double halfStdDevValue = Instrument.MasterInstrument.Round2TickSize((StdDev(5)[2])/2);
			
			// Checking 7 bars instead of 5 in case there's are other bars that hit the high
			if (HighestBar(High, 7) == 2)
			{
				Values[0][2] = High[2] + halfStdDevValue;
				//fiveBarUpper.Set(2, CurrentBar - 2);
			}
			
			if (LowestBar(Low, 7) == 2)
			{
				Values[1][2] = Low[2] - halfStdDevValue;
				//fiveBarLower.Set(2, CurrentBar - 2);
			}
        }
		

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot0
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Plot1
        {
            get { return Values[1]; }
        }

		/// <summary>
		/// Gets the lower value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries FiveBarLower
		{
			get { Update(); return fiveBarLower; }
		}
		
		/// <summary>
		/// Get the Upper value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries FiveBarUpper
		{
			get { Update(); return fiveBarUpper; }
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
        private FiveBarPattern[] cacheFiveBarPattern = null;

        private static FiveBarPattern checkFiveBarPattern = new FiveBarPattern();

        /// <summary>
        /// Added markers to indicate 5 bar pattern
        /// </summary>
        /// <returns></returns>
        public FiveBarPattern FiveBarPattern()
        {
            return FiveBarPattern(Input);
        }

        /// <summary>
        /// Added markers to indicate 5 bar pattern
        /// </summary>
        /// <returns></returns>
        public FiveBarPattern FiveBarPattern(Data.IDataSeries input)
        {
            if (cacheFiveBarPattern != null)
                for (int idx = 0; idx < cacheFiveBarPattern.Length; idx++)
                    if (cacheFiveBarPattern[idx].EqualsInput(input))
                        return cacheFiveBarPattern[idx];

            lock (checkFiveBarPattern)
            {
                if (cacheFiveBarPattern != null)
                    for (int idx = 0; idx < cacheFiveBarPattern.Length; idx++)
                        if (cacheFiveBarPattern[idx].EqualsInput(input))
                            return cacheFiveBarPattern[idx];

                FiveBarPattern indicator = new FiveBarPattern();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                FiveBarPattern[] tmp = new FiveBarPattern[cacheFiveBarPattern == null ? 1 : cacheFiveBarPattern.Length + 1];
                if (cacheFiveBarPattern != null)
                    cacheFiveBarPattern.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheFiveBarPattern = tmp;
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
        /// Added markers to indicate 5 bar pattern
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.FiveBarPattern FiveBarPattern()
        {
            return _indicator.FiveBarPattern(Input);
        }

        /// <summary>
        /// Added markers to indicate 5 bar pattern
        /// </summary>
        /// <returns></returns>
        public Indicator.FiveBarPattern FiveBarPattern(Data.IDataSeries input)
        {
            return _indicator.FiveBarPattern(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Added markers to indicate 5 bar pattern
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.FiveBarPattern FiveBarPattern()
        {
            return _indicator.FiveBarPattern(Input);
        }

        /// <summary>
        /// Added markers to indicate 5 bar pattern
        /// </summary>
        /// <returns></returns>
        public Indicator.FiveBarPattern FiveBarPattern(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.FiveBarPattern(input);
        }
    }
}
#endregion
