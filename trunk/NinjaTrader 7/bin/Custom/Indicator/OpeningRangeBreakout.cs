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
    public class OpeningRangeBreakout : Indicator
    {
        #region Variables
        // Wizard generated variables
            private int atrPeriod = 5; // Default setting for MyInput0
        // User defined variables (add any user defined variables below)
			private double longStop;
			private double shortStop;
			private double longEntry;
			private double shortEntry;
			private double longTarget1;
			private double shortTarget1;
			private double longTarget2;
			private double shortTarget2;
		
			private double atr = 0;
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			BarsRequired = AtrPeriod;
            Overlay				= true;
			
            Add(new Plot(Color.Blue, PlotStyle.Dot, "LongStop"));
            Add(new Plot(Color.Purple, PlotStyle.Dot, "ShortStop"));
            Add(new Plot(Color.Blue, PlotStyle.Cross, "LongEntry"));
            Add(new Plot(Color.Purple, PlotStyle.Cross, "ShortEntry"));
            Add(new Plot(Color.Green, PlotStyle.Dot, "LongTarget1"));
            Add(new Plot(Color.Red, PlotStyle.Dot, "ShortTarget1"));
            Add(new Plot(Color.Blue, PlotStyle.Dot, "LongTarget2"));
            Add(new Plot(Color.Purple, PlotStyle.Dot, "ShortTarget2"));
			
			Plots[0].Pen.Width = 1;
			Plots[1].Pen.Width = 1;
			
			Add(PeriodType.Day, 1);
			
			//ChartControl.BarMarginRight = 50;			
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// Checks to ensure all Bars objects contain enough bars before beginning
			if (CurrentBars[0] <= BarsRequired)
			{
				//Print(Time + " CurrentBars[0]: " + CurrentBars[0] + " CurrentBars[1]: " + CurrentBars[1] + " BarsRequired: " + BarsRequired);
	        	return;
			}

			// This should be true when the primary bar is being udpated
			if (BarsInProgress == 0)
		    {
				if (!Bars.BarsType.IsIntraday)
				{
					DrawTextFixed("error msg", "OpeningRangeBreakout only works on intraday intervals", TextPosition.BottomRight);
					return;
				}

				if (Bars.FirstBarOfSession) 
				{
					// Get the True Range of from the Daily bars
					atr = ATR(BarsArray[1], AtrPeriod)[0];
					//Print(Time + " ATR: " + atr);
					longStop = Open[0] - (atr * 0.05);
					shortStop = Open[0] + (atr * 0.05);
					longEntry = longStop + (atr * 0.236);
					shortEntry = shortStop - (atr * 0.236);
					longTarget1 = longStop + (atr * 0.5);
					shortTarget1 = shortStop - (atr * 0.5);
					longTarget2 = longStop + (atr * 0.764);
					shortTarget2 = shortStop - (atr * 0.7654); 
				}
				
				if (atr != 0)
				{
					LongStop.Set(longStop);
					ShortStop.Set(shortStop);
					LongEntry.Set(longEntry);
					ShortEntry.Set(shortEntry);
					LongTarget1.Set(longTarget1);
					ShortTarget1.Set(shortTarget1);
					LongTarget2.Set(longTarget2);
					ShortTarget2.Set(shortTarget2);
					
//					DrawLine("longTarget2L", -5, longTarget2, -50, longTarget2, Color.Blue);
//					DrawText("longTarget2T", "Exit Long Profit Target #2 ", -25, longTarget2+0.10, Color.Blue);
//				
//					DrawLine("longTarget1L", -5, longTarget1, -50, longTarget1, Color.Green);
//					DrawText("longTarget1T", "Exit Long Profit Target #1 ", -25, longTarget1+0.10, Color.Green);
//
//					DrawLine("longEntryL", -5, longEntry, -50, longEntry, Color.Blue);
//					DrawText("longEntryT", "Entry Long signal ", -25, longEntry+0.10, Color.Blue);
//				
//					DrawLine("longStopL", -5, longStop, -50, longStop, Color.Blue);
//					DrawText("longStopT", "Exit Long Stop ", -25, longStop+0.10, Color.Blue);
//				
//					DrawLine("shortStopL", -5, shortStop, -50, shortStop, Color.Purple);
//					DrawText("shortStopT", "Exit Short Stop ", -25, shortStop+0.10, Color.Purple);
//				
//					DrawLine("shortEntryL", -5, shortEntry, -50, shortEntry, Color.Purple);
//					DrawText("shortEntryT", "Entry Short signal ", -25, shortEntry+0.10, Color.Purple);
//
//					DrawLine("shortTarget1L", -5, shortTarget1, -50, shortTarget1, Color.Red);
//					DrawText("shortTarget1T", "Exit short Profit Target #1 ", -25, shortTarget1, Color.Red);
//
//					DrawLine("shortTarget2L", -5, shortTarget2, -50, shortTarget2, Color.Purple);
//					DrawText("shortTarget2T", "Exit short Profit Target #2 ", -25, shortTarget2, Color.Purple);
				}
			}
        }

        #region Properties
        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LongStop
        {
            get { return Values[0]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortStop
        {
            get { return Values[1]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LongEntry
        {
            get { return Values[2]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortEntry
        {
            get { return Values[3]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LongTarget1
        {
            get { return Values[4]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortTarget1
        {
            get { return Values[5]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries LongTarget2
        {
            get { return Values[6]; }
        }

        [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries ShortTarget2
        {
            get { return Values[7]; }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int AtrPeriod
        {
            get { return atrPeriod; }
            set { atrPeriod = Math.Max(1, value); }
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
        private OpeningRangeBreakout[] cacheOpeningRangeBreakout = null;

        private static OpeningRangeBreakout checkOpeningRangeBreakout = new OpeningRangeBreakout();

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public OpeningRangeBreakout OpeningRangeBreakout(int atrPeriod)
        {
            return OpeningRangeBreakout(Input, atrPeriod);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public OpeningRangeBreakout OpeningRangeBreakout(Data.IDataSeries input, int atrPeriod)
        {
            if (cacheOpeningRangeBreakout != null)
                for (int idx = 0; idx < cacheOpeningRangeBreakout.Length; idx++)
                    if (cacheOpeningRangeBreakout[idx].AtrPeriod == atrPeriod && cacheOpeningRangeBreakout[idx].EqualsInput(input))
                        return cacheOpeningRangeBreakout[idx];

            lock (checkOpeningRangeBreakout)
            {
                checkOpeningRangeBreakout.AtrPeriod = atrPeriod;
                atrPeriod = checkOpeningRangeBreakout.AtrPeriod;

                if (cacheOpeningRangeBreakout != null)
                    for (int idx = 0; idx < cacheOpeningRangeBreakout.Length; idx++)
                        if (cacheOpeningRangeBreakout[idx].AtrPeriod == atrPeriod && cacheOpeningRangeBreakout[idx].EqualsInput(input))
                            return cacheOpeningRangeBreakout[idx];

                OpeningRangeBreakout indicator = new OpeningRangeBreakout();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.AtrPeriod = atrPeriod;
                Indicators.Add(indicator);
                indicator.SetUp();

                OpeningRangeBreakout[] tmp = new OpeningRangeBreakout[cacheOpeningRangeBreakout == null ? 1 : cacheOpeningRangeBreakout.Length + 1];
                if (cacheOpeningRangeBreakout != null)
                    cacheOpeningRangeBreakout.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheOpeningRangeBreakout = tmp;
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
        public Indicator.OpeningRangeBreakout OpeningRangeBreakout(int atrPeriod)
        {
            return _indicator.OpeningRangeBreakout(Input, atrPeriod);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.OpeningRangeBreakout OpeningRangeBreakout(Data.IDataSeries input, int atrPeriod)
        {
            return _indicator.OpeningRangeBreakout(input, atrPeriod);
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
        public Indicator.OpeningRangeBreakout OpeningRangeBreakout(int atrPeriod)
        {
            return _indicator.OpeningRangeBreakout(Input, atrPeriod);
        }

        /// <summary>
        /// Enter the description of your new custom indicator here
        /// </summary>
        /// <returns></returns>
        public Indicator.OpeningRangeBreakout OpeningRangeBreakout(Data.IDataSeries input, int atrPeriod)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.OpeningRangeBreakout(input, atrPeriod);
        }
    }
}
#endregion
