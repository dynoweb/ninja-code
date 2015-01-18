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
    /// Added markers to indicate 5 bar pattern resistance support
    /// </summary>
    [Description("Added markers to indicate 5 bar pattern")]
    public class FiveBarSample : Indicator
    {
        #region Variables
//			private DataSeries lower;
//			private DataSeries upper;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// Written by Rick Cromer
        /// </summary>
        protected override void Initialize()
        {
            Add(new Plot(Color.Blue, PlotStyle.Dot, "Upper Five Bar"));
            Add(new Plot(Color.Red, PlotStyle.Dot, "Lower Five Bar"));
            
//			lower = new DataSeries(this);
//			upper = new DataSeries(this);
			
			this.AutoScale = false;
			this.PaintPriceMarkers = false;
			
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
			if (
				HighestBar(High, 7) == 2
			 || HighestBar(High, 6) == 2
			 || HighestBar(High, 5) == 2
				)
			{
				Values[0][2] = High[2] + Math.Max(1 * TickSize, halfStdDevValue);
				//upper.Set(0, High[2] + halfStdDevValue);
			}

//			if (CurrentBar % 5 == 0)
//				Values[1][2] = CurrentBar;
			
//			if (CurrentBar % 5 == 0)
//				lower.Set(2, CurrentBar);	// intentionally setting it 2 barsAgo back
//			else
//				lower.Set(2, Time[0].DayOfYear);
			
			if (
				LowestBar(Low, 7) == 2
			 ||	LowestBar(Low, 6) == 2
			 ||	LowestBar(Low, 5) == 2
				)
			{
				Values[1][2] = Low[2] - Math.Max(1 * TickSize, halfStdDevValue);
//				lower.Set(2, Low[2] - halfStdDevValue);
			}
        }
		

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries FiveBarUpper
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries FiveBarLower
        {
            get { return Values[1]; }
        }

//		/// <summary>
//		/// Gets the lower value.
//		/// </summary>
//		[Browsable(false)]
//		[XmlIgnore()]
//		public DataSeries FiveBarLower
//		{
//			get { return lower; }
//		}
//		
//		/// <summary>
//		/// Get the Upper value.
//		/// </summary>
//		[Browsable(false)]
//		[XmlIgnore()]
//		public DataSeries FiveBarUpper
//		{
//			get { return upper; }
//		}
		
        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private FiveBarSample[] cacheFiveBarSample = null;

        private static FiveBarSample checkFiveBarSample = new FiveBarSample();

        /// <summary>
        /// Added markers to indicate 5 bar pattern
        /// </summary>
        /// <returns></returns>
        public FiveBarSample FiveBarSample()
        {
            return FiveBarSample(Input);
        }

        /// <summary>
        /// Added markers to indicate 5 bar pattern
        /// </summary>
        /// <returns></returns>
        public FiveBarSample FiveBarSample(Data.IDataSeries input)
        {
            if (cacheFiveBarSample != null)
                for (int idx = 0; idx < cacheFiveBarSample.Length; idx++)
                    if (cacheFiveBarSample[idx].EqualsInput(input))
                        return cacheFiveBarSample[idx];

            lock (checkFiveBarSample)
            {
                if (cacheFiveBarSample != null)
                    for (int idx = 0; idx < cacheFiveBarSample.Length; idx++)
                        if (cacheFiveBarSample[idx].EqualsInput(input))
                            return cacheFiveBarSample[idx];

                FiveBarSample indicator = new FiveBarSample();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                FiveBarSample[] tmp = new FiveBarSample[cacheFiveBarSample == null ? 1 : cacheFiveBarSample.Length + 1];
                if (cacheFiveBarSample != null)
                    cacheFiveBarSample.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheFiveBarSample = tmp;
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
        public Indicator.FiveBarSample FiveBarSample()
        {
            return _indicator.FiveBarSample(Input);
        }

        /// <summary>
        /// Added markers to indicate 5 bar pattern
        /// </summary>
        /// <returns></returns>
        public Indicator.FiveBarSample FiveBarSample(Data.IDataSeries input)
        {
            return _indicator.FiveBarSample(input);
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
        public Indicator.FiveBarSample FiveBarSample()
        {
            return _indicator.FiveBarSample(Input);
        }

        /// <summary>
        /// Added markers to indicate 5 bar pattern
        /// </summary>
        /// <returns></returns>
        public Indicator.FiveBarSample FiveBarSample(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.FiveBarSample(input);
        }
    }
}
#endregion
